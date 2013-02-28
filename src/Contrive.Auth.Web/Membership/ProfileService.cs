using System;
using System.Collections.Generic;
using System.Configuration;
using Contrive.Auth.Membership;

namespace Contrive.Auth.Web.Membership
{
  public class ProfileService : IProfileService
  {
    public string ApplicationName { get; set; }

    public int DeleteInactiveProfiles(ProfileAuthenticationType authenticationType, DateTime userInactiveSinceDate)
    {
      return 0;
    }

    public int DeleteProfiles(string[] usernames)
    {
      return 0;
    }

    public int DeleteProfiles(IEnumerable<IProfile> profiles)
    {
      return 0;
    }

    public IEnumerable<IProfile> FindInactiveProfilesByUserName(ProfileAuthenticationType authenticationType,
                                                                string usernameToMatch,
                                                                DateTime userInactiveSinceDate,
                                                                int pageIndex,
                                                                int pageSize,
                                                                out int totalRecords)
    {
      totalRecords = 0;
      return new IProfile[0];
    }

    public IEnumerable<IProfile> FindProfilesByUserName(ProfileAuthenticationType authenticationType,
                                                        string usernameToMatch,
                                                        int pageIndex,
                                                        int pageSize,
                                                        out int totalRecords)
    {
      totalRecords = 0;
      return new IProfile[0];
    }

    public IEnumerable<IProfile> GetAllInactiveProfiles(ProfileAuthenticationType authenticationType,
                                                        DateTime userInactiveSinceDate,
                                                        int pageIndex,
                                                        int pageSize,
                                                        out int totalRecords)
    {
      totalRecords = 0;
      return new IProfile[0];
    }

    public IEnumerable<IProfile> GetAllProfiles(ProfileAuthenticationType authenticationType,
                                                int pageIndex,
                                                int pageSize,
                                                out int totalRecords)
    {
      totalRecords = 0;
      return new IProfile[0];
    }

    public int GetNumberOfInactiveProfiles(ProfileAuthenticationType authenticationType, DateTime userInactiveSinceDate)
    {
      return 0;
    }

    public SettingsPropertyValueCollection GetPropertyValues(SettingsContext context,
                                                             SettingsPropertyCollection collection)
    {
      return null;
    }

    public void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection) {}
  }
}