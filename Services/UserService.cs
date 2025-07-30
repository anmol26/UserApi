using System.Text.Json;
using UserApi.Models;

namespace UserApi.Services;

public class UserService
{
    private readonly string _filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "users.json");
    private List<User> _users;

    public UserService()
    {
        if (File.Exists(_filePath))
        {
            try
            {
                string json = File.ReadAllText(_filePath);
                _users = JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading users.json: {ex.Message}");
                _users = new List<User>();
            }
        }
        else
        {
            Console.WriteLine("users.json not found. Fetching from API...");
            _users = FetchInitialUsers().Result;
            SaveChanges();
        }
    }

    public List<User> GetAllUsers()
    {
        return _users;
    }

    public User? GetUserById(int id)
    {
        return _users.FirstOrDefault(u => u.Id == id);
    }
    
    public User AddUser(User user)
    {
        int newId = _users.Count == 0 ? 1 : _users.Max(u => u.Id) + 1;
        user.Id = newId;
        _users.Add(user);
        SaveChanges();
        return user;
    }

    public bool UpdateUser(int id, User updatedUser)
    {
        var existing = GetUserById(id);
        if (existing == null) return false;

        existing.Name = updatedUser.Name;
        existing.Username = updatedUser.Username;
        existing.Email = updatedUser.Email;
        existing.Address = updatedUser.Address;
        existing.Phone = updatedUser.Phone;
        existing.Website = updatedUser.Website;
        existing.Company = updatedUser.Company;

        SaveChanges();
        return true;
    }

    public bool DeleteUser(int id)
    {
        var user = GetUserById(id);
        if (user == null) return false;

        _users.Remove(user);
        SaveChanges();
        return true;
    }

    private void SaveChanges()
    {
        try
        {
            if (!Directory.Exists("Data"))
                Directory.CreateDirectory("Data");

            string json = JsonSerializer.Serialize(_users, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving users.json: {ex.Message}");
        }
    }

    private async Task<List<User>> FetchInitialUsers()
    {
        try
        {
            using var client = new HttpClient() { Timeout = TimeSpan.FromSeconds(5) };
            string url = "https://jsonplaceholder.typicode.com/users";
            string json = await client.GetStringAsync(url);
            return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching initial users: {ex.Message}");
            return new List<User>();
        }
    }
}
