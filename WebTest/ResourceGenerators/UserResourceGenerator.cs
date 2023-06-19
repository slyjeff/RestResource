using Slysoft.RestResource;
using Slysoft.RestResource.Extensions;
using WebTest.Entities;

namespace WebTest.ResourceGenerators; 

public static class UserResourceGenerator {
    public static Resource ToListItemResource(this User user) {
        return new Resource()
            .Uri($"/user/{user.Id}")
            .MapAllDataFrom(user as IUserData);
    }

    public static Resource ToResource(this User user) {
        var baseHref = $"/user/{user.Id}";

        var resource = user.ToListItemResource();

        foreach (var role in Enum.GetValues<UserRole>()) {
            var roleHref = $"{baseHref}/role/{role}";
            if (user.Roles.Contains(role)) {
                resource.Delete($"removeRole{role}", roleHref);
            } else {
                resource.Put($"addRole{role}", roleHref);
            }
        }

        resource.Delete("deleteUser", baseHref);

        return resource;
    }
}