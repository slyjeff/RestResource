using WebTest.Entities;
// ReSharper disable StringLiteralTypo

namespace WebTest.Stores; 

public static class UserStore {
    private static readonly IList<User> Users = new List<User> {
        new() { Id = 1, FirstName = "Joe", LastName = "Smith", Username = "jsmith", Roles = { UserRole.Basic } },
        new() { Id = 2, FirstName = "Angela", LastName = "Harris", Username = "aharris", Roles = { UserRole.Basic } },
        new() { Id = 3, FirstName = "Roger", LastName = "Hartman", Username = "rhartman", Roles = { UserRole.Basic } },
        new() { Id = 4, FirstName = "John", LastName = "Sutton", Username = "jsutton", Roles = { UserRole.Basic, UserRole.Admin } },
        new() { Id = 5, FirstName = "Susan", LastName = "Harrington", Username = "sharrington", Roles = { UserRole.Basic } },
        new() { Id = 6, FirstName = "Mary", LastName = "Goodwin", Username = "mgoodwin", Roles = { UserRole.Basic, UserRole.Admin } },
    };

    public static bool Add(User user) {
        if (Users.Any(x => x.Username.Equals(user.Username, StringComparison.CurrentCultureIgnoreCase))) {
            return false;
        }

        user.Id = Users.Max(x => x.Id) + 1;
        Users.Add(user);
        return true;
    }

    public static IEnumerable<User> GetAll() {
        return Users.OrderBy(x => x.Username);
    }

    public static User? Get(int id) {
        return Users.FirstOrDefault(x => x.Id == id);
    }

    public static void Delete(int id) {
        var userToDelete = Get(id);
        if (userToDelete == null) {
            return;
        }

        Users.Remove(userToDelete);
    }
}