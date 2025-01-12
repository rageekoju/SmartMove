using SmartMove.Model;

namespace SmartMove.Components.Pages
{
    public partial class Login
    {

        private User user { get; set; } = new();

        private string ErrorMessage { get; set; } = string.Empty;

        private void HandleLogin()
        {
            if (UserInterface.Login(user))
            {
                Nav.NavigateTo("/dashboard");
            }
            else
            {
                ErrorMessage = "Invalid Username or Password Credentials";
            }
        }
    }
}