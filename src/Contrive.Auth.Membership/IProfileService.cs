using System;
using System.Collections.Generic;
using System.Configuration;

namespace Contrive.Auth.Membership
{
    public interface IProfileService
    {
        string ApplicationName { get; set; }

        int DeleteInactiveProfiles(ProfileAuthenticationType authenticationType, DateTime userInactiveSinceDate);

        int DeleteProfiles(string[] usernames);

        int DeleteProfiles(IEnumerable<IProfile> profiles);

        IEnumerable<IProfile> FindInactiveProfilesByUserName(ProfileAuthenticationType authenticationType,
                                                             string usernameToMatch,
                                                             DateTime userInactiveSinceDate,
                                                             int pageIndex,
                                                             int pageSize,
                                                             out int totalRecords);

        IEnumerable<IProfile> FindProfilesByUserName(ProfileAuthenticationType authenticationType,
                                                     string usernameToMatch,
                                                     int pageIndex,
                                                     int pageSize,
                                                     out int totalRecords);

        IEnumerable<IProfile> GetAllInactiveProfiles(ProfileAuthenticationType authenticationType,
                                                     DateTime userInactiveSinceDate,
                                                     int pageIndex,
                                                     int pageSize,
                                                     out int totalRecords);

        IEnumerable<IProfile> GetAllProfiles(ProfileAuthenticationType authenticationType,
                                             int pageIndex,
                                             int pageSize,
                                             out int totalRecords);

        int GetNumberOfInactiveProfiles(ProfileAuthenticationType authenticationType, DateTime userInactiveSinceDate);

        SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection collection);

        void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection);
    }
}