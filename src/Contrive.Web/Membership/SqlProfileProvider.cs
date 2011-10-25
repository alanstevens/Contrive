using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Profile;

namespace Contrive.Web.Membership
{
  public class SqlProfileProvider : ProfileProvider
  {
    string _AppName;
    int _CommandTimeout;
    int _SchemaVersionCheck;
    string _sqlConnectionString;

    public override string ApplicationName
    {
      get { return _AppName; }
      set
      {
        if (value.Length > 256)
          throw new ProviderException(SR.GetString(SR.Provider_application_name_too_long));
        _AppName = value;
      }
    }

    int CommandTimeout
    {
      get { return _CommandTimeout; }
    }

    public override void Initialize(string name, NameValueCollection config)
    {
      // Remove CAS in sample: HttpRuntime.CheckAspNetHostingPermission(AspNetHostingPermissionLevel.Low, SR.Feature_not_supported_at_this_level);
      if (config == null)
        throw new ArgumentNullException("config");
      if (name == null || name.Length < 1)
        name = "SqlProfileProvider";
      if (string.IsNullOrEmpty(config["description"]))
      {
        config.Remove("description");
        config.Add("description", SR.GetString(SR.ProfileSqlProvider_description));
      }
      base.Initialize(name, config);

      _SchemaVersionCheck = 0;

      string temp = config["connectionStringName"];
      if (temp == null || temp.Length < 1)
        throw new ProviderException(SR.GetString(SR.Connection_name_not_specified));
      //_sqlConnectionString = SqlConnectionHelper.GetConnectionString(temp, true, true);
      if (_sqlConnectionString == null || _sqlConnectionString.Length < 1)
        throw new ProviderException(SR.GetString(SR.Connection_string_not_found, temp));

      _AppName = config["applicationName"];
      if (string.IsNullOrEmpty(_AppName))
        _AppName = SecUtility.GetDefaultAppName();

      if (_AppName.Length > 256)
        throw new ProviderException(SR.GetString(SR.Provider_application_name_too_long));

      _CommandTimeout = SecUtility.GetIntValue(config, "commandTimeout", 30, true, 0);

      config.Remove("commandTimeout");
      config.Remove("connectionStringName");
      config.Remove("applicationName");
      if (config.Count > 0)
      {
        string attribUnrecognized = config.GetKey(0);
        if (!String.IsNullOrEmpty(attribUnrecognized))
          throw new ProviderException(SR.GetString(SR.Provider_unrecognized_attribute, attribUnrecognized));
      }
    }

    void CheckSchemaVersion(SqlConnection connection)
    {
      string[] features = {"Profile"};
      string version = "1";

      SecUtility.CheckSchemaVersion(this,
                                    connection,
                                    features,
                                    version,
                                    ref _SchemaVersionCheck);
    }

    public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext sc,
                                                                      SettingsPropertyCollection properties)
    {
      SettingsPropertyValueCollection svc = new SettingsPropertyValueCollection();

      if (properties.Count < 1)
        return svc;

      string username = (string) sc["UserName"];

      foreach (SettingsProperty prop in properties)
      {
        if (prop.SerializeAs == SettingsSerializeAs.ProviderSpecific)
        {
          if (prop.PropertyType.IsPrimitive || prop.PropertyType == typeof (string))
            prop.SerializeAs = SettingsSerializeAs.String;
          else
            prop.SerializeAs = SettingsSerializeAs.Xml;
        }

        svc.Add(new SettingsPropertyValue(prop));
      }
      if (!String.IsNullOrEmpty(username))
        GetPropertyValuesFromDatabase(username, svc);
      return svc;
    }

    void GetPropertyValuesFromDatabase(string userName, SettingsPropertyValueCollection svc)
    {
      // Comment out events in sample: if (HostingEnvironment.IsHosted && EtwTrace.IsTraceEnabled(EtwTraceLevel.Information, EtwTraceFlags.AppSvc)) EtwTrace.Trace(EtwTraceType.ETW_TYPE_PROFILE_BEGIN, HttpContext.Current.WorkerRequest);

      HttpContext context = HttpContext.Current;
      string[] names = null;
      string values = null;
      byte[] buf = null;
      string sName = null;

      if (context != null)
        sName = (context.Request.IsAuthenticated ? context.User.Identity.Name : context.Request.AnonymousID);

      try
      {
                               
        SqlDataReader reader = null;
        //try
        //{
        //  //holder = SqlConnectionHelper.GetConnection(_sqlConnectionString, true);
        //  //CheckSchemaVersion(holder.Connection);
        //  //SqlCommand cmd = new SqlCommand("dbo.aspnet_Profile_GetProperties", holder.Connection);

        //  cmd.CommandTimeout = CommandTimeout;
        //  cmd.CommandType = CommandType.StoredProcedure;
        //  cmd.Parameters.Add(CreateInputParam("@ApplicationName", SqlDbType.NVarChar, ApplicationName));
        //  cmd.Parameters.Add(CreateInputParam("@UserName", SqlDbType.NVarChar, userName));
        //  cmd.Parameters.Add(CreateInputParam("@CurrentTimeUtc", SqlDbType.DateTime, DateTime.UtcNow));
        //  reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
        //  if (reader.Read())
        //  {
        //    names = reader.GetString(0).Split(':');
        //    values = reader.GetString(1);

        //    int size = (int) reader.GetBytes(2, 0, null, 0, 0);

        //    buf = new byte[size];
        //    reader.GetBytes(2, 0, buf, 0, size);
        //  }
        //}
        //finally
        //{
        //  if (holder != null)
        //  {
        //    holder.Close();
        //    holder = null;
        //  }

        //  if (reader != null)
        //    reader.Close();
        //}

        ParseDataFromDB(names, values, buf, svc);

        // Comment out events in sample: if (HostingEnvironment.IsHosted && EtwTrace.IsTraceEnabled(EtwTraceLevel.Information, EtwTraceFlags.AppSvc)) EtwTrace.Trace(EtwTraceType.ETW_TYPE_PROFILE_END, HttpContext.Current.WorkerRequest, userName);
      }
      catch
      {
        throw;
      }
    }

    public override void SetPropertyValues(SettingsContext sc, SettingsPropertyValueCollection properties)
    {
      string username = (string) sc["UserName"];
      bool userIsAuthenticated = (bool) sc["IsAuthenticated"];

      if (username == null || username.Length < 1 || properties.Count < 1)
        return;

      string names = String.Empty;
      string values = String.Empty;
      byte[] buf = null;

      PrepareDataForSaving(ref names, ref values, ref buf, true, properties, userIsAuthenticated);
      if (names.Length == 0)
        return;

      //try
      //{
      //  SqlConnectionHolder holder = null;
      //  try
      //  {
      //    holder = SqlConnectionHelper.GetConnection(_sqlConnectionString, true);
      //    CheckSchemaVersion(holder.Connection);

      //    SqlCommand cmd = new SqlCommand("dbo.aspnet_Profile_SetProperties", holder.Connection);

      //    cmd.CommandTimeout = CommandTimeout;
      //    cmd.CommandType = CommandType.StoredProcedure;
      //    cmd.Parameters.Add(CreateInputParam("@ApplicationName", SqlDbType.NVarChar, ApplicationName));
      //    cmd.Parameters.Add(CreateInputParam("@UserName", SqlDbType.NVarChar, username));
      //    cmd.Parameters.Add(CreateInputParam("@PropertyNames", SqlDbType.NText, names));
      //    cmd.Parameters.Add(CreateInputParam("@PropertyValuesString", SqlDbType.NText, values));
      //    cmd.Parameters.Add(CreateInputParam("@PropertyValuesBinary", SqlDbType.Image, buf));
      //    cmd.Parameters.Add(CreateInputParam("@IsUserAnonymous", SqlDbType.Bit, !userIsAuthenticated));
      //    cmd.Parameters.Add(CreateInputParam("@CurrentTimeUtc", SqlDbType.DateTime, DateTime.UtcNow));
      //    cmd.ExecuteNonQuery();
      //  }
      //  finally
      //  {
      //    if (holder != null)
      //    {
      //      holder.Close();
      //      holder = null;
      //    }
      //  }
      //}
      //catch
      //{
      //  throw;
      //}
    }

    SqlParameter CreateInputParam(string paramName, SqlDbType dbType, object objValue)
    {
      SqlParameter param = new SqlParameter(paramName, dbType);
      if (objValue == null)
        objValue = String.Empty;
      param.Value = objValue;
      return param;
    }

    public override int DeleteProfiles(ProfileInfoCollection profiles)
    {
      if (profiles == null)
        throw new ArgumentNullException("profiles");

      if (profiles.Count < 1)
      {
        throw new ArgumentException(
          SR.GetString(
            SR.Parameter_collection_empty,
            "profiles"),
          "profiles");
      }

      string[] usernames = new string[profiles.Count];

      int iter = 0;
      foreach (ProfileInfo profile in profiles)
      {
        usernames[iter++] = profile.UserName;
      }

      return DeleteProfiles(usernames);
    }

    public override int DeleteProfiles(string[] usernames)
    {
      SecUtility.CheckArrayParameter(ref usernames,
                                     true,
                                     true,
                                     true,
                                     256,
                                     "usernames");

      int numProfilesDeleted = 0;
      bool beginTranCalled = false;
      //try
      //{
      //  SqlConnectionHolder holder = null;
      //  try
      //  {
      //    holder = SqlConnectionHelper.GetConnection(_sqlConnectionString, true);
      //    CheckSchemaVersion(holder.Connection);

      //    SqlCommand cmd;

      //    int numUsersRemaing = usernames.Length;
      //    while (numUsersRemaing > 0)
      //    {
      //      string allUsers = usernames[usernames.Length - numUsersRemaing];
      //      numUsersRemaing--;
      //      for (int iter = usernames.Length - numUsersRemaing; iter < usernames.Length; iter++)
      //      {
      //        if (allUsers.Length + usernames[iter].Length + 1 >= 4000)
      //          break;
      //        allUsers += "," + usernames[iter];
      //        numUsersRemaing--;
      //      }

      //      // We don't need to start a transaction if we can finish this in one sql command
      //      //
      //      // Note:  ADO.NET 2.0 introduced the TransactionScope class - in your own code you should use TransactionScope
      //      //            rather than explicitly managing transactions with the TSQL BEGIN/COMMIT/ROLLBACK statements.
      //      //
      //      if (!beginTranCalled && numUsersRemaing > 0)
      //      {
      //        cmd = new SqlCommand("BEGIN TRANSACTION", holder.Connection);
      //        cmd.ExecuteNonQuery();
      //        beginTranCalled = true;
      //      }

      //      cmd = new SqlCommand("dbo.aspnet_Profile_DeleteProfiles", holder.Connection);

      //      cmd.CommandTimeout = CommandTimeout;
      //      cmd.CommandType = CommandType.StoredProcedure;
      //      cmd.Parameters.Add(CreateInputParam("@ApplicationName", SqlDbType.NVarChar, ApplicationName));
      //      cmd.Parameters.Add(CreateInputParam("@UserNames", SqlDbType.NVarChar, allUsers));
      //      object o = cmd.ExecuteScalar();
      //      if (o != null && o is int)
      //        numProfilesDeleted += (int) o;
      //    }

      //    if (beginTranCalled)
      //    {
      //      cmd = new SqlCommand("COMMIT TRANSACTION", holder.Connection);
      //      cmd.ExecuteNonQuery();
      //      beginTranCalled = false;
      //    }
      //  }
      //  catch
      //  {
      //    if (beginTranCalled)
      //    {
      //      SqlCommand cmd = new SqlCommand("ROLLBACK TRANSACTION", holder.Connection);
      //      cmd.ExecuteNonQuery();
      //      beginTranCalled = false;
      //    }
      //    throw;
      //  }
      //  finally
      //  {
      //    if (holder != null)
      //    {
      //      holder.Close();
      //      holder = null;
      //    }
      //  }
      //}
      //catch
      //{
      //  throw;
      //}
      return numProfilesDeleted;
    }

    public override int DeleteInactiveProfiles(ProfileAuthenticationOption authenticationOption,
                                               DateTime userInactiveSinceDate)
    {
      //try
      //{
      //  SqlConnectionHolder holder = null;
      //  try
      //  {
      //    holder = SqlConnectionHelper.GetConnection(_sqlConnectionString, true);
      //    CheckSchemaVersion(holder.Connection);

      //    SqlCommand cmd = new SqlCommand("dbo.aspnet_Profile_DeleteInactiveProfiles", holder.Connection);

      //    cmd.CommandTimeout = CommandTimeout;
      //    cmd.CommandType = CommandType.StoredProcedure;
      //    cmd.Parameters.Add(CreateInputParam("@ApplicationName", SqlDbType.NVarChar, ApplicationName));
      //    cmd.Parameters.Add(CreateInputParam("@ProfileAuthOptions", SqlDbType.Int, (int) authenticationOption));
      //    cmd.Parameters.Add(CreateInputParam("@InactiveSinceDate", SqlDbType.DateTime,
      //                                        userInactiveSinceDate.ToUniversalTime()));
      //    object o = cmd.ExecuteScalar();
      //    if (o == null || !(o is int))
      //      return 0;
      //    return (int) o;
      //  }
      //  finally
      //  {
      //    if (holder != null)
      //    {
      //      holder.Close();
      //      holder = null;
      //    }
      //  }
      //}
      //catch
      //{
      //  throw;
      //}
      return 0;
    }

    public override int GetNumberOfInactiveProfiles(ProfileAuthenticationOption authenticationOption,
                                                    DateTime userInactiveSinceDate)
    {
      //try
      //{
      //  SqlConnectionHolder holder = null;
      //  try
      //  {
      //    holder = SqlConnectionHelper.GetConnection(_sqlConnectionString, true);
      //    CheckSchemaVersion(holder.Connection);

      //    SqlCommand cmd = new SqlCommand("dbo.aspnet_Profile_GetNumberOfInactiveProfiles", holder.Connection);

      //    cmd.CommandTimeout = CommandTimeout;
      //    cmd.CommandType = CommandType.StoredProcedure;
      //    cmd.Parameters.Add(CreateInputParam("@ApplicationName", SqlDbType.NVarChar, ApplicationName));
      //    cmd.Parameters.Add(CreateInputParam("@ProfileAuthOptions", SqlDbType.Int, (int) authenticationOption));
      //    cmd.Parameters.Add(CreateInputParam("@InactiveSinceDate", SqlDbType.DateTime,
      //                                        userInactiveSinceDate.ToUniversalTime()));
      //    object o = cmd.ExecuteScalar();
      //    if (o == null || !(o is int))
      //      return 0;
      //    return (int) o;
      //  }
      //  finally
      //  {
      //    if (holder != null)
      //    {
      //      holder.Close();
      //      holder = null;
      //    }
      //  }
      //}
      //catch
      //{
      //  throw;
      //}
      return 0;
    }

    public override ProfileInfoCollection GetAllProfiles(ProfileAuthenticationOption authenticationOption, int pageIndex,
                                                         int pageSize, out int totalRecords)
    {
      return GetProfilesForQuery(new SqlParameter[0], authenticationOption, pageIndex, pageSize, out totalRecords);
    }

    public override ProfileInfoCollection GetAllInactiveProfiles(ProfileAuthenticationOption authenticationOption,
                                                                 DateTime userInactiveSinceDate, int pageIndex,
                                                                 int pageSize, out int totalRecords)
    {
      SqlParameter[] args = new SqlParameter[1];
      args[0] = CreateInputParam("@InactiveSinceDate", SqlDbType.DateTime, userInactiveSinceDate.ToUniversalTime());
      return GetProfilesForQuery(args, authenticationOption, pageIndex, pageSize, out totalRecords);
    }

    public override ProfileInfoCollection FindProfilesByUserName(ProfileAuthenticationOption authenticationOption,
                                                                 string usernameToMatch, int pageIndex, int pageSize,
                                                                 out int totalRecords)
    {
      SecUtility.CheckParameter(ref usernameToMatch, true, true, false, 256, "username");
      SqlParameter[] args = new SqlParameter[1];
      args[0] = CreateInputParam("@UserNameToMatch", SqlDbType.NVarChar, usernameToMatch);
      return GetProfilesForQuery(args, authenticationOption, pageIndex, pageSize, out totalRecords);
    }

    public override ProfileInfoCollection FindInactiveProfilesByUserName(
      ProfileAuthenticationOption authenticationOption, string usernameToMatch, DateTime userInactiveSinceDate,
      int pageIndex, int pageSize, out int totalRecords)
    {
      SecUtility.CheckParameter(ref usernameToMatch, true, true, false, 256, "username");
      SqlParameter[] args = new SqlParameter[2];
      args[0] = CreateInputParam("@UserNameToMatch", SqlDbType.NVarChar, usernameToMatch);
      args[1] = CreateInputParam("@InactiveSinceDate", SqlDbType.DateTime, userInactiveSinceDate.ToUniversalTime());
      return GetProfilesForQuery(args, authenticationOption, pageIndex, pageSize, out totalRecords);
    }

    static void ParseDataFromDB(string[] names, string values, byte[] buf, SettingsPropertyValueCollection properties)
    {
      if (names == null || values == null || buf == null || properties == null)
        return;
      try
      {
        for (int iter = 0; iter < names.Length/4; iter++)
        {
          string name = names[iter*4];
          SettingsPropertyValue pp = properties[name];

          if (pp == null) // property not found
            continue;

          int startPos = Int32.Parse(names[iter*4 + 2], CultureInfo.InvariantCulture);
          int length = Int32.Parse(names[iter*4 + 3], CultureInfo.InvariantCulture);

          if (length == -1 && !pp.Property.PropertyType.IsValueType) // Null Value
          {
            pp.PropertyValue = null;
            pp.IsDirty = false;
            pp.Deserialized = true;
          }
          if (names[iter*4 + 1] == "S" && startPos >= 0 && length > 0 && values.Length >= startPos + length)
            pp.SerializedValue = values.Substring(startPos, length);

          if (names[iter*4 + 1] == "B" && startPos >= 0 && length > 0 && buf.Length >= startPos + length)
          {
            byte[] buf2 = new byte[length];

            Buffer.BlockCopy(buf, startPos, buf2, 0, length);
            pp.SerializedValue = buf2;
          }
        }
      }
      catch
      {
        // Eat exceptions
      }
    }

    static void PrepareDataForSaving(ref string allNames, ref string allValues, ref byte[] buf, bool binarySupported,
                                     SettingsPropertyValueCollection properties, bool userIsAuthenticated)
    {
      StringBuilder names = new StringBuilder();
      StringBuilder values = new StringBuilder();

      MemoryStream ms = (binarySupported ? new MemoryStream() : null);
      try
      {
        try
        {
          bool anyItemsToSave = false;

          foreach (SettingsPropertyValue pp in properties)
          {
            if (pp.IsDirty)
            {
              if (!userIsAuthenticated)
              {
                bool allowAnonymous = (bool) pp.Property.Attributes["AllowAnonymous"];
                if (!allowAnonymous)
                  continue;
              }
              anyItemsToSave = true;
              break;
            }
          }

          if (!anyItemsToSave)
            return;

          foreach (SettingsPropertyValue pp in properties)
          {
            if (!userIsAuthenticated)
            {
              bool allowAnonymous = (bool) pp.Property.Attributes["AllowAnonymous"];
              if (!allowAnonymous)
                continue;
            }

            if (!pp.IsDirty && pp.UsingDefaultValue) // Not fetched from DB and not written to
              continue;

            int len = 0, startPos = 0;
            string propValue = null;

            if (pp.Deserialized && pp.PropertyValue == null) // is value null?
              len = -1;
            else
            {
              object sVal = pp.SerializedValue;

              if (sVal == null) len = -1;
              else
              {
                if (!(sVal is string) && !binarySupported) sVal = Convert.ToBase64String((byte[]) sVal);

                if (sVal is string)
                {
                  propValue = (string) sVal;
                  len = propValue.Length;
                  startPos = values.Length;
                }
                else
                {
                  byte[] b2 = (byte[]) sVal;
                  startPos = (int) ms.Position;
                  ms.Write(b2, 0, b2.Length);
                  ms.Position = startPos + b2.Length;
                  len = b2.Length;
                }
              }
            }

            names.Append(pp.Name + ":" + ((propValue != null) ? "S" : "B") +
                         ":" + startPos.ToString(CultureInfo.InvariantCulture) + ":" +
                         len.ToString(CultureInfo.InvariantCulture) + ":");
            if (propValue != null)
              values.Append(propValue);
          }

          if (binarySupported) buf = ms.ToArray();
        }
        finally
        {
          if (ms != null)
            ms.Close();
        }
      }
      catch
      {
        throw;
      }
      allNames = names.ToString();
      allValues = values.ToString();
    }

    ProfileInfoCollection GetProfilesForQuery(SqlParameter[] args, ProfileAuthenticationOption authenticationOption,
                                              int pageIndex, int pageSize, out int totalRecords)
    {
      if (pageIndex < 0)
        throw new ArgumentException(SR.GetString(SR.PageIndex_bad), "pageIndex");
      if (pageSize < 1)
        throw new ArgumentException(SR.GetString(SR.PageSize_bad), "pageSize");

      long upperBound = (long) pageIndex*pageSize + pageSize - 1;
      if (upperBound > Int32.MaxValue)
        throw new ArgumentException(SR.GetString(SR.PageIndex_PageSize_bad), "pageIndex and pageSize");

      //try
      //{
      //  SqlConnectionHolder holder = null;
      //  SqlDataReader reader = null;
      //  try
      //  {
      //    holder = SqlConnectionHelper.GetConnection(_sqlConnectionString, true);
      //    CheckSchemaVersion(holder.Connection);

      //    SqlCommand cmd = new SqlCommand("dbo.aspnet_Profile_GetProfiles", holder.Connection);

      //    cmd.CommandTimeout = CommandTimeout;
      //    cmd.CommandType = CommandType.StoredProcedure;
      //    cmd.Parameters.Add(CreateInputParam("@ApplicationName", SqlDbType.NVarChar, ApplicationName));
      //    cmd.Parameters.Add(CreateInputParam("@ProfileAuthOptions", SqlDbType.Int, (int) authenticationOption));
      //    cmd.Parameters.Add(CreateInputParam("@PageIndex", SqlDbType.Int, pageIndex));
      //    cmd.Parameters.Add(CreateInputParam("@PageSize", SqlDbType.Int, pageSize));
      //    foreach (SqlParameter arg in args)
      //      cmd.Parameters.Add(arg);
      //    reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess);
      //    ProfileInfoCollection profiles = new ProfileInfoCollection();
      //    while (reader.Read())
      //    {
      //      string username;
      //      DateTime dtLastActivity, dtLastUpdated;
      //      bool isAnon;

      //      username = reader.GetString(0);
      //      isAnon = reader.GetBoolean(1);
      //      dtLastActivity = DateTime.SpecifyKind(reader.GetDateTime(2), DateTimeKind.Utc);
      //      dtLastUpdated = DateTime.SpecifyKind(reader.GetDateTime(3), DateTimeKind.Utc);
      //      int size = reader.GetInt32(4);
      //      profiles.Add(new ProfileInfo(username, isAnon, dtLastActivity, dtLastUpdated, size));
      //    }
      //    totalRecords = profiles.Count;
      //    if (reader.NextResult())
      //    {
      //      if (reader.Read())
      //        totalRecords = reader.GetInt32(0);
      //    }
      //    return profiles;
      //  }
      //  finally
      //  {
      //    if (reader != null)
      //      reader.Close();

      //    if (holder != null)
      //    {
      //      holder.Close();
      //      holder = null;
      //    }
      //  }
      //}
      //catch
      //{
      //  throw;
      //}
      totalRecords = 0;
      return null;
    }
  }
}