namespace ClientTest; 

public interface IApplicationResource {
    string Information { get; }
    Task<ITestsResource> GetTests();
}

public interface ITestsResource {
    public string Description { get; }
}

