using SmartMove.Model;

namespace SmartMove.Components.Pages
{
    // The Login component handles the login functionality for users
    public partial class Login
    {
        // The 'user' object holds the login details entered by the user (Username, Password, and Currency)
        private User user { get; set; } = new();

        // Holds the error message that will be shown if login fails
        private string ErrorMessage { get; set; } = string.Empty;

        // Method that handles the login process when the user submits the login form
        private void HandleLogin()
        {
            // Attempt to log in using the 'UserInterface.Login' method, passing the user object
            if (UserInterface.Login(user))
            {
                // If login is successful, navigate to the dashboard
                Nav.NavigateTo("/dashboard");
            }
            else
            {
                // If login fails, set an error message to inform the user
                ErrorMessage = "Invalid Username or Password Credentials";
            }
        }
    }
}
