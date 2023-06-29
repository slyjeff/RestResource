using Microsoft.VisualStudio.TestTools.UnitTesting;
using Slysoft.RestResource.Client.Tests.Common;
using Slysoft.RestResource.Extensions;

namespace Slysoft.RestResource.Client.Tests.NetFramework;

[TestClass]
public sealed class AccessPropertiesTests {
    private IResourceAccessorFactory _factory = null!;

    [TestInitialize]
    public void SetUp() {
        _factory = new ResourceAccessorFactory();
    }

    [TestMethod]
    public void MustBeAbleToAccessString() {
        //arrange
        var source = new SimpleResource();
        var resource = new Resource()
            .MapDataFrom(source)
                .Map(x => x.Message)
            .EndMap();

        //act
        var destination = _factory.CreateAccessor<ISimpleResource>(resource);

        //assert
        Assert.AreEqual(source.Message, destination.Message);
    }
}