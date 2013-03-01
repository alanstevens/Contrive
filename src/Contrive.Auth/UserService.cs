using System;
using System.Configuration.Provider;
using System.Text.RegularExpressions;
using Contrive.Common;
using Contrive.Common.Extensions;

namespace Contrive.Auth
{
    public class UserService : IUserService
    {
        public UserService(IUserRepository userRepository,
                           ICryptographer cryptographer,
                           IUserServiceSettings settings)
        {
            _userRepository = userRepository;
            _cryptographer = cryptographer;
            _settings = settings;
        }

        readonly ICryptographer _cryptographer;
        readonly IUserServiceSettings _settings;
        readonly IUserRepository _userRepository;

        public bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            Verify.NotEmpty(userName, "userName");
            Verify.NotEmpty(oldPassword, "oldPassword");
            Verify.NotEmpty(newPassword, "newPassword");

            var user = VerifyUserExists(userName);

            var verificationSucceeded = VerifyPassword(user, oldPassword);

            if (verificationSucceeded) {}

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

        public UserCreateStatus CreateUser(string userName,
                                           string password,
                                           string emailAddress,
                                           string firstName = null,
                                           string lastName = null)
        {
            Verify.NotEmpty(userName, "userName");
            Verify.NotEmpty(password, "password");
            Verify.NotEmpty(emailAddress, "emailAddress");

            if (!IsValidPassword(password)) return UserCreateStatus.InvalidPassword;

            if (_settings.RequiresUniqueEmail)
            {
                var emailUser = GetUserByEmailAddress(emailAddress);
                if (emailUser != null) return UserCreateStatus.DuplicateEmail;
            }

            var existingUser = GetUserByUserName(userName);

            if (existingUser != null) return UserCreateStatus.DuplicateUserName;

            var newUser = NewUser();
            var passwordSalt = _cryptographer.GenerateSalt();

            newUser.Id = Guid.NewGuid();
            newUser.UserName = userName;
            newUser.FirstName = firstName;
            newUser.FirstName = lastName;
            newUser.PasswordSalt = passwordSalt;
            newUser.Email = emailAddress;
            try
            {
                newUser.Password = EncodePassword(password, passwordSalt);
            }
            catch (ArgumentException e)
            {
                return UserCreateStatus.PasswordTooLong;
            }
            _userRepository.Insert(newUser);

            return UserCreateStatus.Success;
        }

        protected virtual IUser NewUser()
        {
            return new User();
        }

        IUser VerifyUserExists(string userName)
        {
            var user = GetUserByUserName(userName);

            if (user == null)
                throw new InvalidOperationException(string.Format("User not found: {0}", userName));

            return user;
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

            var verified = VerifyPassword(user, password);

            _userRepository.Update(user);

            return verified;
        }

        string EncodePassword(string password, string passwordSalt)
        {
            if (!IsValidPassword(password)) throw new ArgumentException("Password is invalid");

            string encodedPassword;

            switch (_settings.PasswordFormat)
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

            if (encodedPassword.Length > _settings.MaxHashedPasswordLength)
                throw new ArgumentException("Password too long");

            return encodedPassword;
        }

        bool IsValidPassword(string password)
        {
            Verify.NotEmpty(password, "password");
            if (password.Length < _settings.MinRequiredPasswordLength) return false;

            if (_settings.MinRequiredNonAlphanumericCharacters > 0)
            {
                var nonAlpahNumericCharsCount = Regex.Matches(password, "[^a-zA-Z0-9]").Count;
                if (nonAlpahNumericCharsCount < _settings.MinRequiredNonAlphanumericCharacters)
                    return false;
            }

            if (!_settings.PasswordStrengthRegularExpression.IsEmpty())
            {
                if (!Regex.IsMatch(password, _settings.PasswordStrengthRegularExpression))
                    return false;
            }

            return true;
        }
    }
}