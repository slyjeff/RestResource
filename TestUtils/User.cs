namespace TestUtils; 

public enum UserPosition { Standard, Admin }

public sealed class User {
    public string LastName { get; set; } = GenerateRandom.String();
    public string FirstName { get; set; } = GenerateRandom.String();
    public UserPosition Position { get; set; } = UserPosition.Standard;
    public UserPosition? PositionNullable { get; set; } = UserPosition.Standard;
    public int YearsEmployed { get; set; } = GenerateRandom.Int(1, 15);
    public bool IsRegistered { get; set; }
    public bool? IsRegisteredNullable { get; set; }
}