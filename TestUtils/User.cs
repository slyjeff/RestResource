namespace TestUtils; 

public sealed class User {
    public string LastName { get; set; } = GenerateRandom.String();
    public string FirstName { get; set; } = GenerateRandom.String();
    public string Position { get; set; } = "Standard";
    public int YearsEmployed { get; set; } = GenerateRandom.Int(1, 15);
    public bool IsRegistered { get; set; }
}