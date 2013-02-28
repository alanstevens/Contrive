using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Text.RegularExpressions;
using Contrive.Common;
using Contrive.Common.Extensions;
using Microsoft.Practices.ServiceLocation;

namespace Contrive.Auth.Membership
{
    public class UserServiceExtended : IUserServiceExtended
    {
        public UserServiceExtended(IUserExtendedRepository userRepository,
                           ICryptographer cryptographer,
                           IUserServiceSettings settings)
        {
            _userRepository = userRepository;
            _cryptographer = cryptographer;
            Settings = settings;
        }

        const int MAX_HASHED_PASSWORD_LENGTH = 128;
        const int DEFAULT_NUMBER_OF_PASSWORD_FAILURES = 0;

        readonly ICryptographer _cryptographer;
        readonly IUserExtendedRepository _userRepository;

        public IUserServiceSettings Settings { get; private set; }

        public bool ValidateUser(string userName, string password)
        {
            Verify.NotEmpty(userName, "userName");
            Verify.NotEmpty(password, "password");

            return VerifyUser(GetUserByUserName(userName), password);
        }

        public bool VerifyUser(IUserExtended user, string password)
        {
            Verify.NotNull(user, "user");
            Verify.NotEmpty(password, "password");

            if (user == null) return false;

            if (!user.IsConfirmed) return false;

            var verified = VerifyPassword(user, password);

            _userRepository.Update(user);

            return verified;
        }

        public UserCreateStatus CreateUser(string userName,
                                           string password,
                                           string emailAddress,
                                           bool isApproved = true)
        {
            Verify.NotEmpty(userName, "userName");
            Verify.NotEmpty(password, "password");
            Verify.NotEmpty(emailAddress, "emailAddress");

            if (Settings.RequiresQuestionAndAnswer)
            {
                throw new NotSupportedException(
                  "Contrive: RequiresQuestionAndAnswer not supported.");
            }

            if (!IsValidPassword(password)) return UserCreateStatus.InvalidPassword;

            if (Settings.RequiresUniqueEmail)
            {
                var emailUser = GetUserByEmailAddress(emailAddress);
                if (emailUser != null) return UserCreateStatus.DuplicateEmail;
            }

            var existingUser = GetUserByUserName(userName);

            if (existingUser != null) return UserCreateStatus.DuplicateUserName;

            var passwordSalt = _cryptographer.GenerateSalt();

            var newUser = ServiceLocator.Current.GetInstance<IUserExtended>();

            newUser.Id = Guid.NewGuid();
            newUser.UserName = userName;
            newUser.PasswordSalt = passwordSalt;
            newUser.Email = emailAddress;
            newUser.IsApproved = isApproved;
            // TODO: HAS 10/23/2011 Read setting for require confirmation.
            newUser.IsConfirmed = true;
            newUser.DateCreated = DateTime.UtcNow;
            newUser.IsLockedOut = false;
            newUser.LastLockedOutDate = DateTime.MinValue;

            SetPasswordFor(newUser, password);

            _userRepository.Insert(newUser);

            return UserCreateStatus.Success;
        }

        public string CreateAccount(string userName,
                                    string password,
                                    string emailAddress,
                                    bool requireConfirmationToken = false)
        {
            Verify.NotEmpty(userName, "userName");
            Verify.NotEmpty(password, "password");
            Verify.NotEmpty(emailAddress, "emailAddress");

            string token = null;

            if (requireConfirmationToken) token = _cryptographer.GenerateToken();

            var status = CreateUser(userName, password, emailAddress);

            if (status != UserCreateStatus.Success) throw new CreateUserException(status);

            var newUser = GetUserByUserName(userName);

            newUser.IsConfirmed = !requireConfirmationToken;
            newUser.ConfirmationToken = token;

            return token;
        }

        public bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            Verify.NotEmpty(userName, "userName");
            Verify.NotEmpty(oldPassword, "oldPassword");
            Verify.NotEmpty(newPassword, "newPassword");

            var user = VerifyUserExists(userName);

            var verificationSucceeded = VerifyPassword(user, oldPassword);

            if (verificationSucceeded) SetPasswordFor(user, newPassword);

            return verificationSucceeded;
        }

        public IUserExtended GetUserByUserName(string userName)
        {
            Verify.NotEmpty(userName, "userName");

            return _userRepository.GetUserByUserName(userName);
        }

        public bool ConfirmAccount(string token)
        {
            Verify.NotEmpty(token, "token");

            var user = _userRepository.GetUserByConfirmationToken(token);

            if (user != null)
            {
                user.IsConfirmed = true;
                return true;
            }
            return false;
        }

        public bool DeleteAccount(string userName)
        {
            Verify.NotEmpty(userName, "userName");

            var user = VerifyUserExists(userName);

            _userRepository.Delete(user);

            return true;
        }

        public bool IsConfirmed(string userName)
        {
            Verify.NotEmpty(userName, "userName");

            var user = VerifyUserExists(userName);

            return user.IsConfirmed;
        }

        public string GeneratePasswordResetToken(string userName,
                                                 int tokenExpirationInMinutesFromNow)
        {
            Verify.NotEmpty(userName, "userName");
            Verify.NotEmpty(tokenExpirationInMinutesFromNow, "tokenExpirationInMinutesFromNow");

            var user = VerifyUserExists(userName);

            if (!user.IsConfirmed)
                throw new InvalidOperationException(String.Format("User not found: {0}", userName));

            if (user.PasswordVerificationTokenExpirationDate <= DateTime.UtcNow)
            {
                user.PasswordVerificationToken = _cryptographer.GenerateToken();
                user.PasswordVerificationTokenExpirationDate =
                  DateTime.UtcNow.AddMinutes(tokenExpirationInMinutesFromNow);
            }

            return user.PasswordVerificationToken;
        }

        public bool ResetPasswordWithToken(string token, string newPassword)
        {
            Verify.NotEmpty(token, "token");
            Verify.NotEmpty(newPassword, "newPassword");

            if (!IsValidPassword(newPassword))
                throw new CreateUserException(UserCreateStatus.InvalidPassword);

            var user = _userRepository.GetUserByPasswordVerificationToken(token);

            var validUser = user != null;

            if (validUser) SetPasswordFor(user, newPassword);

            return validUser;
        }

        public string ValidateUserExtended(string userNameOrEmailAddress, string password)
        {
            Verify.NotEmpty(userNameOrEmailAddress, "userNameOrEmail");
            Verify.NotEmpty(password, "password");

            var user = GetUserByUserNameOrEmailAddress(userNameOrEmailAddress);

            if (user == null) return "";

            return VerifyUser(user, password) ? user.UserName : String.Empty;
        }

        public IUserExtended GetUserByUserNameOrEmailAddress(string userNameOrEmailAddress)
        {
            Verify.NotEmpty(userNameOrEmailAddress, "userNameOrEmailAddress");
            return GetUserByUserName(userNameOrEmailAddress) ??
                   GetUserByEmailAddress(userNameOrEmailAddress);
        }

        public IEnumerable<IUserExtended> FindUsersForUserName(string searchTerm)
        {
            Verify.NotEmpty(searchTerm, "searchTerm");
            return _userRepository.FindUsersForUserName(searchTerm);
        }

        public IEnumerable<IUserExtended> FindUsersForEmailAddress(string searchTerm)
        {
            Verify.NotEmpty(searchTerm, "searchTerm");
            return _userRepository.FindUsersForEmailAddress(searchTerm);
        }

        public DateTime GetPasswordChangedDate(string userName)
        {
            Verify.NotEmpty(userName, "userName");

            var user = VerifyUserExists(userName);

            return user.PasswordChangedDate.GetValueOrDefault();
        }

        public DateTime GetCreateDate(string userName)
        {
            Verify.NotEmpty(userName, "userName");

            var user = VerifyUserExists(userName);

            return user.DateCreated.GetValueOrDefault();
        }

        public int GetPasswordFailuresSinceLastSuccess(string userName)
        {
            Verify.NotEmpty(userName, "userName");

            var user = VerifyUserExists(userName);

            return user.FailedPasswordAttemptCount;
        }

        public IUserExtended GetUserFromPasswordResetToken(string token)
        {
            Verify.NotEmpty(token, "token");

            return _userRepository.GetUserByPasswordVerificationToken(token);
        }

        public DateTime GetLastPasswordFailureDate(string userName)
        {
            Verify.NotEmpty(userName, "userName");

            var user = VerifyUserExists(userName);

            return user.LastPasswordFailureDate.GetValueOrDefault();
        }

        public void UpdateUser(IUserExtended user)
        {
            Verify.NotNull(user, "user");
            _userRepository.Update(user);
        }

        public IUserExtended GetUserByEmailAddress(string emailAddress)
        {
            Verify.NotEmpty(emailAddress, "emailAddress");

            return _userRepository.GetUserByEmailAddress(emailAddress);
        }

        bool IsValidPassword(string password)
        {
            Verify.NotEmpty(password, "password");
            if (password.Length < Settings.MinRequiredPasswordLength) return false;

            if (Settings.MinRequiredNonAlphanumericCharacters > 0)
            {
                var nonAlpahNumericCharsCount = Regex.Matches(password, "[^a-zA-Z0-9]").Count;
                if (nonAlpahNumericCharsCount < Settings.MinRequiredNonAlphanumericCharacters)
                    return false;
            }

            if (!Settings.PasswordStrengthRegularExpression.IsEmpty())
            {
                if (!Regex.IsMatch(password, Settings.PasswordStrengthRegularExpression))
                    return false;
            }

            return true;
        }

        string GetAuthDigest(string userName, string password)
        {
            var format = "{0}:{1}:{2}".FormatWith(userName, Settings.Realm, password);

            return format.CalculateMd5Hash().Base64ToHex();
        }

        string EncodePassword(string password, string passwordSalt)
        {
            string encodedPassword;

            switch (Settings.PasswordFormat)
            {
                case UserPasswordFormat.Clear:
                    throw new ArgumentException("Clear password storage is not allowed.");
                case UserPasswordFormat.Encrypted:
                    encodedPassword = _cryptographer.Encrypt(password);
                    break;
                case UserPasswordFormat.Hashed:
                    encodedPassword = _cryptographer.CalculatePasswordHash(password, passwordSalt);
                    break;
                default:
                    throw new ProviderException("Unsupported password format.");
            }

            return encodedPassword;
        }

        bool VerifyPassword(IUserExtended user, string password)
        {
            Verify.NotNull(user, "user");
            if (user.Password.IsEmpty()) return false;

            var encodedPassword = EncodePassword(password, user.PasswordSalt);

            var verified = encodedPassword.Equals(user.Password);

            var currentDate = DateTime.UtcNow;
            if (verified)
            {
                user.FailedPasswordAttemptCount = DEFAULT_NUMBER_OF_PASSWORD_FAILURES;
                user.LastActivityDate = currentDate;
                user.LastPasswordFailureDate = DateTime.MinValue;
            }
            else
            {
                var failures = user.FailedPasswordAttemptCount;

                if (failures != -1)
                {
                    user.FailedPasswordAttemptCount += 1;
                    user.LastPasswordFailureDate = currentDate;
                }
            }

            return verified;
        }

        void SetPasswordFor(IUserExtended user, string password)
        {
            if (!IsValidPassword(password)) throw new ArgumentException("Password is invalid");

            var newEncodedPassword = EncodePassword(password, user.PasswordSalt);

            if (newEncodedPassword.Length > MAX_HASHED_PASSWORD_LENGTH)
                throw new ArgumentException("Password too long");

            user.Password = newEncodedPassword;
            user.AuthDigest = GetAuthDigest(user.UserName, password);
            user.PasswordVerificationToken = null;
            user.PasswordVerificationTokenExpirationDate = DateTime.MinValue;

            var currentDate = DateTime.UtcNow;

            user.LastActivityDate = currentDate;
            user.PasswordChangedDate = currentDate;
        }

        IUserExtended VerifyUserExists(string userName)
        {
            var user = GetUserByUserName(userName);

            if (user == null)
                throw new InvalidOperationException(string.Format("User not found: {0}", userName));

            return user;
        }
    }
}