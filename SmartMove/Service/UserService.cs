using SmartMove.Abstraction;
using SmartMove.Model;
using SmartMove.Service.Interface;

namespace SmartMove.Service
{
    // UserService handles user-related operations such as login and user data management.
    public class UserService : UserBase, IUserInterface
    {
        // List to hold users in memory
        private List<User> _User;

        // Default values for seed user, password, and currency. These are used if no users exist in the data store.
        private const string SeedUser = "Ragee";
        private const string SeedPassword = "RageeKoju";
        private const int SeedCurrency = 1; // Represents currency type (using enum value)

        // Constructor initializes the UserService
        public UserService()
        {
            // Load existing users from the data store
            _User = LoadUsers();

            // If no users exist, create a default user and save them to the data store
            if (!_User.Any())
            {
                _User.Add(new User
                {
                    Id = new Guid(), // Generate a new unique GUID for the user
                    UserName = SeedUser, // Set the default username
                    Password = SeedPassword, // Set the default password
                    Currency = (Currency)SeedCurrency // Convert the seed currency value to the Currency enum
                });

                // Save the newly created user(s) to the data store
                SaveUsers(_User);
            }
        }

        // Validates if the provided user credentials (username and password) match any user in the list
        public bool Login(User user)
        {
            // Check if username or password is empty
            if (string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.Password))
            {
                return false; // Return false if login fields are empty
            }

            // Return true if a user exists with the provided username and password
            return _User.Any(x => x.UserName == user.UserName && x.Password == user.Password);
        }
    }
}
