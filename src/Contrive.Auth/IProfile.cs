using System;
using System.Runtime;

namespace Contrive.Auth
{
  public interface IProfile {
    string UserName { [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get; }
    DateTime LastActivityDate { get; }
    DateTime LastUpdatedDate { get; }
    bool IsAnonymous { [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get; }
    int Size { [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get; }
  }
}