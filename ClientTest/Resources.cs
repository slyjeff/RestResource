namespace ClientTest; 

public interface IApplicationResource {
    string Information { get; }
    Task<ITestsResource> GetTests();
}

public interface ITestsResource {
    string Description { get; }
    Task<string> NotFound();
    Task<string> Text();
    Task<IQueryResultResource> Query(string parameter1, string parameter2);
    Task<IPostResultResource> Post(string parameter1, int parameter2);
}

public interface IQueryResultResource {
    string Parameter1 { get; }
    string Parameter2 { get; }
}

public interface IPostResultResource {
    string Parameter1 { get; }
    int Parameter2 { get; }
}
