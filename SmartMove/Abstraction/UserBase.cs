using System.Text.Json; // Provides functionality for JSON serialization and deserialization
using SmartMove.Model;

namespace SmartMove.Abstraction;

public abstract class UserBase
{
    // This path is resolved dynamically to ensure the file is saved in an appropriate directory
    protected static readonly string FilePath = Path.Combine(FileSystem.AppDataDirectory, "users.json");

    // If the file doesn't exist or is empty, it returns an empty list.
    protected List<User> LoadUsers()
    {
        // Check if the file exists. If it doesn't, return an empty list indicating no users are saved
        if (!File.Exists(FilePath)) return new List<User>();

        // Read the raw JSON content from the file
        var json = File.ReadAllText(FilePath);

        // Deserialize the JSON content into a List of User objects.
        return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
    }

    protected void SaveUsers(List<User> users)
    {
        // Serialize the list of User objects into a JSON string
        var json = JsonSerializer.Serialize(users);

        // Write the JSON string to the users.json file, saving the user data
        File.WriteAllText(FilePath, json);
    }
}
