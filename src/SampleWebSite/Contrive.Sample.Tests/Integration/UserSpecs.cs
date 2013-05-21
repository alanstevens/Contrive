using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using Contrive.Auth;
using Contrive.Common;
using Contrive.Common.Data;
using Contrive.Common.Data.Sql;
using Contrive.Common.Extensions;
using Contrive.Common.Web;
using Contrive.Sample.Data;
using FluentAssertions;
using SubSpec;

namespace Contrive.Sample.Tests.Integration
{
    public class Using_UserService
    {
        IEnumerable<IUser> _results;
        IUser _user;
        WebCryptographer _cryptographer = new WebCryptographer();

        [Specification]
        public void When_retrieving_all_users()
        {
            "Given ".Context(() => Setup());
            "When ".Do(() =>
                       {
                           using (UnitOfWork.Current)
                           {
                               _results = new UserRepository().GetAll();
                               _results.Each(u => Console.WriteLine("{0} - {1} {2}".FormatWith(u.Id, u.FirstName, u.LastName)));
                           }
                       });
            "It should ".Assert(() => _results.Any()
                                              .Should()
                                              .BeTrue());
        }

        [Specification]
        public void When_retrieving_user_by_username()
        {
            "Given ".Context(() => Setup());
            "When ".Do(() =>
                       {
                           using (UnitOfWork.Current)
                           {
                               _user = new UserRepository().GetUserByUserName("JodanJacobson");
                               Console.WriteLine("{0} - {1} {2}, {3}".FormatWith(_user.Id, _user.FirstName, _user.LastName, _user.Email));
                           }
                       });
            "It should ".Assert(() => _user.Should()
                                           .NotBeNull());
        }

        [Specification]
        public void When_verifying_a_username_and_password()
        {
            string encodedPassword = null;
            "Given ".Context(() => Setup());
            "When ".Do(() =>
                       {
                           using (UnitOfWork.Current)
                           {
                               _user = new UserRepository().GetUserByUserName("JodanJacobson");

                               encodedPassword = _cryptographer.CalculatePasswordHash("pass@word1", _user.PasswordSalt);
                               Console.WriteLine("{0} - {1} {2}, {3}".FormatWith(_user.Id, _user.FirstName, _user.LastName, _user.Email));
                           }
                       });
            "It should ".Assert(() => _user.PasswordHash.Should()
                                           .BeEquivalentTo(encodedPassword));
        }

        [Specification]
        public void When_retrieving_user_by_email()
        {
            "Given ".Context(() => Setup());
            "When ".Do(() =>
                       {
                           using (UnitOfWork.Current)
                           {
                               _user = new UserRepository().GetUserByEmailAddress("stefan0@adventure-works.com");
                               Console.WriteLine("{0} - {1} {2}, {3}".FormatWith(_user.Id, _user.FirstName, _user.LastName, _user.Email));
                           }
                       });
            "It should ".Assert(() => _user.Should()
                                           .NotBeNull());
        }

        [Specification]
        public void When_generating_a_user_password()
        {
            bool verified = false;
            "Given an existing user".Context(() => Setup());
            "When generating a password".Do(() =>
                       {
                           using (UnitOfWork.Current)
                           {
                               var repository = new UserRepository();
                               var u = repository.GetUserByUserName("ChrisAshton");
                               var cryptographer = new WebCryptographer();
                               //u.PasswordSalt = cryptographer.GenerateSalt();
                               var passwordHash1 = cryptographer.CalculatePasswordHash("pass@word1", u.PasswordSalt);
                               var passwordHash2 = cryptographer.CalculatePasswordHash("pass@word1", u.PasswordSalt);
                               verified = u.PasswordHash == passwordHash1;
                               Console.WriteLine("{0} - {1} {2}, {3}".FormatWith(u.Id, u.FirstName, u.LastName, u.Email));
                               Console.WriteLine("Existing hash:    {0}".FormatWith(u.PasswordHash));
                               Console.WriteLine("Generated hash 2: {0}".FormatWith(passwordHash1));
                               Console.WriteLine("Generated hash 1: {0}".FormatWith(passwordHash2));
                           }
                       });
            "It should match the existing password hash".Assert(() => verified.Should().BeTrue());
        }

        //[Specification]
        public void When_updating_all_users()
        {
            "Given ".Context(() => Setup());
            "When ".Do(() =>
                       {
                           using (UnitOfWork.Current)
                           {
                               var repository = new UserRepository();
                               var users = repository.GetAll();
                               users.Each(u =>
                                          {
                                              //u.PasswordSalt = cryptographer.GenerateSalt();
                                              u.PasswordHash = _cryptographer.CalculatePasswordHash("pass@word1", u.PasswordSalt);
                                              repository.Update(u);
                                              Console.WriteLine("{0} - {1} {2}, {3}".FormatWith(u.Id, u.FirstName, u.LastName, u.Email));
                                          });
                           }
                       });
            "It should ".Assert(() => { });
        }

        static void Setup()
        {
            new DefaultConnectionStringStartupTask(new ConfigurationProvider()).OnStartup();
            new ConnectionProviderStartupTask().OnStartup();
            new DefaultConnectionStringStartupTask(new WebConfigurationProvider()).OnStartup();
            new SqlParameterStartupTask().OnStartup();
        }
    }
}