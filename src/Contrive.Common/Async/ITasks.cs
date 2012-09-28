using System;

namespace Contrive.Common.Async
{
  public interface ITasks
  {
    ITasks Enqueue(Action task);

    ITasks Run();
  }
}