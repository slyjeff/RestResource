using System.Collections.Generic;
using System.Threading.Tasks;

namespace Slysoft.RestResource.Client.Tests.Common; 

public interface ILinkTest {
    IUserList GetAllUsers();
    IUserList GetAllUsersTemplated(int id1, int id2);
    IUserList GetAllUsersTemplated(object ids);
    IUserList SearchUsers(string lastName, string firstName);
    IUserList SearchUsers(object searchParameters);
}

public interface ILinkTestAsync {
    Task<IUserList> GetAllUsers();
    Task<IUserList> GetAllUsersTemplated(int id1, int id2);
    Task<IUserList> GetAllUsersTemplated(object ids);
    Task<IUserList> SearchUsers(string lastName, string firstName);
    Task<IUserList> SearchUsers(object searchParameters);
}

public interface IUserList {
    IList<IUser> Users { get; }
}

public interface IUser {
    public string Name { get; }
}