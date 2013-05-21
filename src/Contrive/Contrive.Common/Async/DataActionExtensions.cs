using System;
using Contrive.Common.Extensions;

namespace Contrive.Common.Async
{
    public static class DataActionExtensions
    {
        public static Action<Action, Action, bool> Executor = (action, uponCompletion, indicateBusy) =>
                                                              {
                                                                  try
                                                                  {
                                                                      action();
                                                                  }
                                                                  finally
                                                                  {
                                                                      if (uponCompletion.IsNotNull()) uponCompletion();
                                                                  }
                                                              };

        public static void InvokeDataProcess(this Action action, Action uponCompletion = null, bool indicateBusy = true)
        {
            Executor.Invoke(action, uponCompletion, indicateBusy);
        }
    }

    public class DataActionBusyEvent {}

    public class DataActionCompleteEvent {}
}