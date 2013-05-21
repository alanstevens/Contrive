using System;
using Contrive.Common.Async;
using Contrive.Common.Extensions;

namespace Contrive.Common.Data
{
    public class AsyncDataStartupTask : IStartupTask
    {
        public AsyncDataStartupTask(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        readonly IEventAggregator _eventAggregator;

        public void OnStartup()
        {
            DataActionExtensions.Executor = (dataAction, uponCompletion,  indicateBusy) =>
                                            {
                                                Action work = () =>
                                                              {
                                                                  try
                                                                  {
                                                                      // TODO: HAS 10/04/2012 What happens if we have multiple call to InvokeDataProcess using this UnitOfWork?
                                                                      using (UnitOfWork.Current) dataAction();
                                                                  }
                                                                  catch (Exception ex)
                                                                  {
                                                                      this.LogException(ex);
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
                                                                      };

                                                try
                                                {
                                                    if (indicateBusy) _eventAggregator.Publish(new DataActionBusyEvent());

                                                    work.RunAsync(continueWith);
                                                }
                                                finally
                                                {
                                                    if (indicateBusy) _eventAggregator.Publish(new DataActionCompleteEvent());
                                                }
                                            };
        }
    }
}