using SmartMove.Abstraction;
using SmartMove.Common;
using SmartMove.Model;
using SmartMove.Service.Interface;

namespace SmartMove.Service
{
    public class UserService : UserBase, IUserInterface
    {
        private List<User> _User;

        private const string SeedUser = "Ragee";
        private const string SeedPassword = "RageeKoju";
        private const int SeedCurrency = 1;

        public UserService()
        {
            _User = LoadUsers();

            if (!_User.Any())
            {
                _User.Add(new User { Id = new Guid(), UserName = SeedUser, Password = SeedPassword, Currency = (Currency)SeedCurrency });
                SaveUsers(_User);
            }
        }
        public bool Login(User user)
        {
            if (string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.Password))
            {
                return false;
            }

            return _User.Any(x => x.UserName == user.UserName && x.Password == user.Password);
        }
    }
}
