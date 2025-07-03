using Personal_Finance_Manager.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Personal_Finance_Manager.Services
{
    public class FileDataService : IDataService
    {
        private readonly string filePath = "users.json";

        public void SaveUsers(List<User> users)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    // Preserve enums as strings for readability
                    Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
                };

                string json = JsonSerializer.Serialize(users, options);
                File.WriteAllText(filePath, json);
                Console.WriteLine("✅ Data saved to file.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error saving data: {ex.Message}");
            }
        }

        public List<User> LoadUsers()
        {
            try
            {
                if (!File.Exists(filePath))
                    return new List<User>();

                string json = File.ReadAllText(filePath);

                var options = new JsonSerializerOptions
                {
                    Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
                };

                var users = JsonSerializer.Deserialize<List<User>>(json, options);
                return users ?? new List<User>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error loading data: {ex.Message}");
                return new List<User>();
            }
        }
    }
}
