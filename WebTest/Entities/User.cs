namespace WebTest.Entities; 

public enum UserRole {Basic, Admin}

public sealed class User {
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public IList<UserRole> Roles { get; } = new List<UserRole>();
}