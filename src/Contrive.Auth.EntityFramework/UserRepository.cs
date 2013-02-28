using Contrive.Common;
using Contrive.Common.Extensions;

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

        public void Insert(IUser user)
        {
            _repository.Insert(user);
            SaveChanges();
        }

        public void Update(IUser user)
        {
            _repository.Update(user);
            SaveChanges();
        }

        public void Delete(IUser user)
        {
            _repository.Delete(user);
            SaveChanges();
        }

        void SaveChanges()
        {
            _repository.SaveChanges();
        }
    }
}