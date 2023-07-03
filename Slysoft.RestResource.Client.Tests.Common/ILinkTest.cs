using System.Collections.Generic;
using System.Threading.Tasks;

namespace Slysoft.RestResource.Client.Tests.Common;

public interface ILinkTestCommon {
    [LinkCheck]
    bool CanGetAllUsers { get; }

    [LinkCheck(nameof(ILinkTest.GetAllUsersTemplated))]
    bool LinkCheckGetTemplated { get; }

    [LinkCheck]
    bool CanLinkThatDoesNotExist { get; }
}

public interface ILinkTest : ILinkTestCommon {
    IUserList GetAllUsers();
    IUserList GetAllUsersTemplated(int id1, int id2);
    IUserList GetAllUsersTemplated(object ids);
    IUserList SearchUsers();
    IUserList SearchUsers(string lastName, string firstName);
    IUserList SearchUsers(object searchParameters);
    IUserList LinkThatDoesNotExist();
}

public interface ILinkTestAsync : ILinkTestCommon {
    Task<IUserList> GetAllUsers();
    Task<IUserList> GetAllUsersTemplated(int id1, int id2);
    Task<IUserList> GetAllUsersTemplated(object ids);
    Task<IUserList> SearchUsers();
    Task<IUserList> SearchUsers(string lastName, string firstName);
    Task<IUserList> SearchUsers(object searchParameters);
    Task<IUserList> LinkThatDoesNotExist();
}

public interface IUserList {
    IList<IUser> Users { get; }
}

public interface IUser {
    public string Name { get; }
}