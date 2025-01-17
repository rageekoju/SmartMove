namespace SmartMove.Model
{
    // Represents a user in the system with their credentials and preferences
    public class User
    {
        // Unique identifier for the user (using a GUID to ensure it's globally unique)
        public Guid Id { get; set; }

        // The user's username (used for login identification)
        public string UserName { get; set; }

        // The user's password (used for login authentication)
        public string Password { get; set; }

        // The preferred currency of the user (e.g., NPR, USD, EUR)
        // It is stored as an enum to make it easier to handle different currency types
        public Currency Currency { get; set; }
    }
}
