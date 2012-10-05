using System;
using System.Runtime;

namespace Contrive.Auth.Web.Membership
{
  [Serializable]
  public class Profile : IProfile
  {
    public Profile(string username, bool isAnonymous, DateTime lastActivityDate, DateTime lastUpdatedDate, int size)
    {
      if (username != null) username = username.Trim();
      _UserName = username;
      if (lastActivityDate.Kind == DateTimeKind.Local) lastActivityDate = lastActivityDate.ToUniversalTime();
      _LastActivityDate = lastActivityDate;
      if (lastUpdatedDate.Kind == DateTimeKind.Local) lastUpdatedDate = lastUpdatedDate.ToUniversalTime();
      _LastUpdatedDate = lastUpdatedDate;
      _IsAnonymous = isAnonymous;
      _Size = size;
    }

    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    protected Profile() {}

    readonly bool _IsAnonymous;
    readonly int _Size;

    readonly string _UserName;
    DateTime _LastActivityDate;
    DateTime _LastUpdatedDate;

    public virtual string UserName
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get { return _UserName; }
    }

    public virtual DateTime LastActivityDate { get { return _LastActivityDate.ToLocalTime(); } }

    public virtual DateTime LastUpdatedDate { get { return _LastUpdatedDate.ToLocalTime(); } }

    public virtual bool IsAnonymous
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get { return _IsAnonymous; }
    }

    public virtual int Size
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get { return _Size; }
    }
  }
}