using System.Linq;
using Slysoft.RestResource.Extensions;

namespace Slysoft.RestResource.Client.Tests.Common; 

public static class TestData {
    public static Resource CreateUserListResource(params string[] names) {
        var userResourceList = names.Select(name => new Resource().Data("name", name)).ToList();
        return new Resource().Embedded("users", userResourceList);
    }
}