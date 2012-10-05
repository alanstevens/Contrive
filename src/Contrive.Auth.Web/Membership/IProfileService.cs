using System;
using System.Configuration;
using System.Web.Profile;

namespace Contrive.Auth.Web.Membership
{
  public interface IProfileService
  {
    string ApplicationName { get; set; }

    int DeleteInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate);

    int DeleteProfiles(string[] usernames);

    int DeleteProfiles(ProfileInfoCollection profiles);

    ProfileInfoCollection FindInactiveProfilesByUserName(ProfileAuthenticationOption authenticationOption,
                                                         string usernameToMatch,
                                                         DateTime userInactiveSinceDate,
                                                         int pageIndex,
                                                         int pageSize,
                                                         out int totalRecords);

    ProfileInfoCollection FindProfilesByUserName(ProfileAuthenticationOption authenticationOption,
                                                 string usernameToMatch,
                                                 int pageIndex,
                                                 int pageSize,
                                                 out int totalRecords);

    ProfileInfoCollection GetAllInactiveProfiles(ProfileAuthenticationOption authenticationOption,
                                                 DateTime userInactiveSinceDate,
                                                 int pageIndex,
                                                 int pageSize,
                                                 out int totalRecords);

    ProfileInfoCollection GetAllProfiles(ProfileAuthenticationOption authenticationOption,
                                         int pageIndex,
                                         int pageSize,
                                         out int totalRecords);

    int GetNumberOfInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate);

    SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection collection);

    void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection);
  }
}