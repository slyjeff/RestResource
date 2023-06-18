using Slysoft.RestResource;
using Slysoft.RestResource.Extensions;
using WebTest.Entities;

namespace WebTest.ResourceGenerators; 

public static class UserResourceGenerator {
    public static Resource ToResource(this User user) {
        return new Resource()
            .Uri($"/user/{user.Id}")
            .MapAllDataFrom(user as IUserData);
    }
}