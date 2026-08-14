using Final_Project.Models;

namespace Final_Project.Services;

public class UserStore
{
    private readonly List<UserAccount> _users = new()
    {
        new()
        {
            Name = "Krishna Admin",
            Email = "admin@splitmate.com",
            Password = "admin123",
            Role = "Admin"
        },
        new()
        {
            Name = "Splitmate Member",
            Email = "member@splitmate.com",
            Password = "member123",
            Role = "Member"
        }
    };

    public UserAccount? Validate(string email, string password)
    {
        return _users.FirstOrDefault(user =>
            string.Equals(user.Email, email?.Trim(), StringComparison.OrdinalIgnoreCase)
            && user.Password == password);
    }

    public IReadOnlyList<UserAccount> DemoUsers => _users;
}
