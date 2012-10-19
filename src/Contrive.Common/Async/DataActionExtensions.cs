using System;
using System.Threading;
using Contrive.Common.Extensions;
using Microsoft.Practices.ServiceLocation;

namespace Contrive.Common.Async
{
  public static class DataActionExtensions
  {
    public static Action<Action, Action, SynchronizationContext, bool> Executor =
      (action, uponCompletion, context, indicateBusy) =>
      {
        action();
        if (uponCompletion.IsNotNull()) uponCompletion();
      };

    public static void InvokeDataProcess(this Action action,
                                         Action uponCompletion = null,
                                         SynchronizationContext syncContext = null,
                                         bool indicateBusy = true)
    {
      Executor.Invoke(action, uponCompletion, syncContext, indicateBusy);
    }
  }

  public class DataActionExtensionsStartupTask : IStartupTask
  {
    public DataActionExtensionsStartupTask(IEventAggregator eventAggregator)
    {
      _eventAggregator = eventAggregator;
    }

    readonly IEventAggregator _eventAggregator;

    public void OnStartup()
    {
      DataActionExtensions.Executor = (dataAction, uponCompletion, syncContext, indicateBusy) =>
                                      {
                                        Action work = () =>
                                                      {
                                                        try
                                                        {
                                                          // TODO: HAS 10/04/2012 What happens if we have multiple calls using this UnitOfWork?
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

                                        Action continueWith = () =>
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
                                                                  if (indicateBusy)
                                                                    _eventAggregator.Publish(new DataActionCompleteEvent());
                                                                }
                                                              };

                                        if (indicateBusy) _eventAggregator.Publish(new DataActionBusyEvent());

                                        work.RunAsync(continueWith);
                                      };
    }
  }

  public class DataActionBusyEvent {}

  public class DataActionCompleteEvent {}
}