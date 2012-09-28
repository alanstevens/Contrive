using System;
using Contrive.Common.Extensions;
using Microsoft.Practices.ServiceLocation;

namespace Contrive.Common.Async
{
  public static class DataActionExtensions
  {
    public static Action<Action, Action, bool> Executor = (action, uponCompletion, indicateBusy) =>
                                                          {
                                                            action();
                                                            uponCompletion();
                                                          };

    public static void InvokeDataProcess(this Action action, Action uponCompletion = null, bool indicateBusy = true)
    {
      Executor.Invoke(action, uponCompletion, indicateBusy);
    }
  }

  public class DataActionExtensionsStartupTask: IStartupTask
  {
    readonly IEventAggregator _eventAggregator;
    readonly ITasks _taskQueue;

    public DataActionExtensionsStartupTask(IEventAggregator eventAggregator, ITasks taskQueue)
    {
      _eventAggregator = eventAggregator;
      _taskQueue = taskQueue;
    }

    public void OnStartup()
    {
      DataActionExtensions.Executor = (dataAction, uponCompletion, indicateBusy) =>
                                      {
                                        Action work = () =>
                                                      {
                                                        try
                                                        {
                                                          using (ServiceLocator.Current.GetInstance<IUnitOfWork>())
                                                          {
                                                            dataAction();
                                                          }
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                          this.LogException(ex);
                                                          if (indicateBusy) _eventAggregator.Publish(new DataActionCompleteEvent());
                                                          throw;
                                                        }
                                                      };

                                        Action complete = () =>
                                                      {
                                                        try
                                                        {
                                                          if (uponCompletion.IsNotNull()) uponCompletion();
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                          this.LogException(ex);
                                                          throw;
                                                        }
                                                        finally
                                                        {
                                                          if (indicateBusy) _eventAggregator.Publish(new DataActionCompleteEvent());
                                                        }
                                                      };

                                        if (indicateBusy) _eventAggregator.Publish(new DataActionBusyEvent());

                                        Action task = () =>
                                                            {
                                                              work();
                                                              Execute.OnMainThread(complete);
                                                            };
                                        _taskQueue.Enqueue(task).Run();
                                      };
    }
  }

  public class DataActionBusyEvent {}

  public class DataActionCompleteEvent {}
}