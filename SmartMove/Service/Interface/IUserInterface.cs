using SmartMove.Model;

namespace SmartMove.Service.Interface
{
    public interface IUserInterface
    {
        // Method declaration for logging in a user.
        bool Login(User user);
    }
}
