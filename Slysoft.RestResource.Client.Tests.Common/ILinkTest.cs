using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlySoft.RestResource.Client.Tests.Common;

public interface ILinkTestCommon {
    [LinkCheck]
    bool CanGetAllUsers { get; }

    [LinkCheck(nameof(ILinkTest.GetAllUsersTemplated))]
    bool LinkCheckGetTemplated { get; }

    [LinkCheck]
    bool CanLinkThatDoesNotExist { get; }

    [ParameterInfo(nameof(ILinkTest.SearchUsers), "lastName")]
    IParameterInfo SearchLastNameInfo { get; }
}

public interface ILinkTest : ILinkTestCommon {
    string FirstName { get; set; }
    string LastName { get; set; }

    IUserList GetAllUsers();
    IUserList GetAllUsersWithTimeout();
    IUserList GetAllUsersTemplated(int id1, int id2);
    IUserList GetAllUsersTemplated(object ids);
    IUserList SearchUsers();
    IUserList SearchUsers(string lastName, string firstName);
    IUserList SearchUsers(object searchParameters);
    IUserList LinkThatDoesNotExist();
    IUser CreateUser();
    IUser CreateUser(string lastName, string firstName);
    IUser CreateUser(object createParameters);
    IUser CreateUserWithTimeout(string lastName, string firstName);
    IUser UpdateUser();
}

public interface ILinkTestAsync : ILinkTestCommon {
    Task<IUserList> GetAllUsers();
    Task<IUserList> GetAllUsersWithTimeout();
    Task<IUserList> GetAllUsersTemplated(int id1, int id2);
    Task<IUserList> GetAllUsersTemplated(object ids);
    Task<IUserList> SearchUsers();
    Task<IUserList> SearchUsers(string lastName, string firstName);
    Task<IUserList> SearchUsers(object searchParameters);
    Task<IUserList> LinkThatDoesNotExist();
    Task<IUser> CreateUser();
    Task<IUser> CreateUser(string lastName, string firstName);
    Task<IUser> CreateUser(object createParameters);
    Task<IUser> CreateUserWithTimeout(string lastName, string firstName);
    Task<IUser> UpdateUser();
}

public interface IUserList {
    IList<IUser> Users { get; }
}

public interface IUser {
    public string FirstName { get; }
    public string LastName { get; }
}