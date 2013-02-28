using System;

namespace Contrive.Auth.Membership
{
  public interface IProfile
  {
    string UserName { get; }

    DateTime LastActivityDate { get; }

    DateTime LastUpdatedDate { get; }

    bool IsAnonymous { get; }

    int Size { get; }
  }
}