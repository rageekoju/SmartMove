﻿using SmartMove.Model;
using System.Text.Json;

namespace SmartMove.Common
{
    public abstract class DbInitialize
    {
        protected static readonly string FilePath = Path.Combine(FileSystem.AppDataDirectory, "users.json");

        // Method to load user data from the users.json file
        protected List<User> LoadUsers()
        {
            // If the file does not exist, return an empty list to indicate no users are saved
            if (!File.Exists(FilePath)) return new List<User>();

            // Read the JSON content from the file
            var json = File.ReadAllText(FilePath);

            // Deserialize the JSON content into a list of User objects
            // If deserialization fails (null result), return an empty list
            return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
        }

        // Method to save user data to the users.json file
        protected void SaveUsers(List<User> users)
        {
            // Serialize the list of User objects into a JSON string
            var json = JsonSerializer.Serialize(users);

            // Write the JSON string to the users.json file
            File.WriteAllText(FilePath, json);
        }
    }
}
