using WebTest.Entities;
// ReSharper disable StringLiteralTypo

namespace WebTest.Stores; 

public static class UserStore {
    private static readonly IList<User> Users = new List<User> {
        new() { FirstName = "Joe", LastName = "Smith", Username = "jsmith", Roles = { UserRole.Basic } },
        new() { FirstName = "Angela", LastName = "Harris", Username = "aharris", Roles = { UserRole.Basic } },
        new() { FirstName = "Roger", LastName = "Hartman", Username = "rhartman", Roles = { UserRole.Basic } },
        new() { FirstName = "John", LastName = "Sutton", Username = "jsutton", Roles = { UserRole.Basic, UserRole.Admin } },
        new() { FirstName = "Susan", LastName = "Harrington", Username = "sharrington", Roles = { UserRole.Basic } },
        new() { FirstName = "Mary", LastName = "Goodwin", Username = "mgoodwin", Roles = { UserRole.Basic, UserRole.Admin } },
    };

    public static IEnumerable<User> GetAll() {
        return Users;
    }

    public static User? GetByUserName(string userName) {
        return Users.FirstOrDefault(x => x.Username.Equals(userName, StringComparison.CurrentCultureIgnoreCase));
    }
}