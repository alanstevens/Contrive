using System;
using System.Configuration.Provider;
using System.Text.RegularExpressions;
using Contrive.Auth.Membership;
using Contrive.Common;
using Contrive.Common.Extensions;
using Microsoft.Practices.ServiceLocation;

namespace Contrive.Auth
{
    public class UserService:IUserService
    {
        public UserService(IUserRepository userRepository,
                           ICryptographer cryptographer,
                           IUserServiceSettings settings)
        {
            _userRepository = userRepository;
            _cryptographer = cryptographer;
            Settings = settings;
        }

        const int MAX_HASHED_PASSWORD_LENGTH = 128;

        readonly ICryptographer _cryptographer;
        readonly IUserRepository _userRepository;

        public IUserServiceSettings Settings { get; private set; }

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

        public IUser GetUserByUserName(string userName)
        {
            Verify.NotEmpty(userName, "userName");

            return _userRepository.GetUserByUserName(userName);
        }

        public bool ValidateUser(string userName, string password)
        {
            Verify.NotEmpty(userName, "userName");
            Verify.NotEmpty(password, "password");

            return VerifyUser(GetUserByUserName(userName), password);
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

        public bool DeleteAccount(string userName)
        {
            Verify.NotEmpty(userName, "userName");

            var user = VerifyUserExists(userName);

            _userRepository.Delete(user);

            return true;
        }

        public IUser GetUserByEmailAddress(string emailAddress)
        {
            Verify.NotEmpty(emailAddress, "emailAddress");

            return _userRepository.GetUserByEmailAddress(emailAddress);
        }

        public void UpdateUser(IUser user)
        {
            Verify.NotNull(user, "user");
            _userRepository.Update(user);
        }

        IUser VerifyUserExists(string userName)
        {
            var user = GetUserByUserName(userName);

            if (user == null)
                throw new InvalidOperationException(string.Format("User not found: {0}", userName));

            return user;
        }

        protected virtual void SetPasswordFor(IUser user, string password)
        {
            if (!IsValidPassword(password)) throw new ArgumentException("Password is invalid");

            var newEncodedPassword = EncodePassword(password, user.PasswordSalt);

            if (newEncodedPassword.Length > MAX_HASHED_PASSWORD_LENGTH)
                throw new ArgumentException("Password too long");

            user.Password = newEncodedPassword;
        }

        bool VerifyPassword(IUser user, string password)
        {
            Verify.NotNull(user, "user");
            if (user.Password.IsEmpty()) return false;

            var encodedPassword = EncodePassword(password, user.PasswordSalt);

            var verified = encodedPassword.Equals(user.Password);

            return verified;
        }

        bool VerifyUser(IUser user, string password)
        {
            Verify.NotNull(user, "user");
            Verify.NotEmpty(password, "password");

            if (user == null) return false;

            var userex = user.As<IUserExtended>();

            if (!userex.IsConfirmed) return false;

            var verified = VerifyPassword(userex, password);

            _userRepository.Update(userex);

            return verified;
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
    }
}