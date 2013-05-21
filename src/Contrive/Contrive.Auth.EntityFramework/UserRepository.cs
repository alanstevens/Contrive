using System.Collections.Generic;
using System.Linq;
using Contrive.Common;

namespace Contrive.Auth.EntityFramework
{
    public class UserRepository : IUserRepository
    {
        public UserRepository(IRepository<IUser> repository)
        {
            _repository = repository;
        }

        readonly IRepository<IUser> _repository;

        public IUser GetUserByUserName(string userName)
        {
            return _repository.FirstOrDefault(u => u.UserName == userName);
        }

        public IUser GetUserByEmailAddress(string emailAddress)
        {
            return _repository.FirstOrDefault(u => u.Email == emailAddress);
        }

        public IEnumerable<IUser> GetUsersForUserNames(IEnumerable<string> userNames)
        {
            return _repository.Where(u => userNames.Contains(u.UserName));
        }

        public void Insert(IUser user)
        {
            _repository.Insert(user);
            _repository.SaveChanges();
        }

        public void Update(IUser user)
        {
            _repository.Update(user);
            _repository.SaveChanges();
        }

        public void Delete(IUser user)
        {
            _repository.Delete(user);
            _repository.SaveChanges();
        }
    }
}