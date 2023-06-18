namespace WebTest.Entities; 

public enum UserRole {Basic, Admin}

public interface IUserData {
    string Username { get; }
    string LastName { get; }
    string FirstName { get; }
    IList<UserRole> Roles { get; }
}

public sealed class User : IUserData {
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public IList<UserRole> Roles { get; } = new List<UserRole>();
}