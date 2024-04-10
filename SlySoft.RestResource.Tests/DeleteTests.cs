using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SlySoft.RestResource.Tests;

[TestClass]
public class DeleteTests {
    [TestMethod]
    public void DeleteMustAddLink() {
        //arrange
        const string uri = "/api/use/34";

        //act
        var resource = new Resource()
            .Delete("deleteUser", uri);

        //assert
        var link = resource.GetLink("deleteUser");
        Assert.IsNotNull(link);
        Assert.AreEqual("deleteUser", link.Name);
        Assert.AreEqual(uri, link.Href);
        Assert.AreEqual("DELETE", link.Verb);
    }
}