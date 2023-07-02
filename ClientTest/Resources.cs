namespace ClientTest; 

public interface IApplicationResource {
    string Information { get; }
    Task<ITestsResource> GetTests();
}

public interface ITestsResource {
    string Description { get; }
    Task<string> NotFound();
    Task<string> Text();
}

