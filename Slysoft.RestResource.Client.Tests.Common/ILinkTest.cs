using System.Collections.Generic;
using System.Threading.Tasks;

namespace Slysoft.RestResource.Client.Tests.Common; 

public interface ILinkTest {
    IUserList GetAllUsers();
    IUserList GetAllUsersTemplated(int id1, int id2);
    IUserList GetAllUsersTemplated(object ids);
}

public interface ILinkTestAsync {
    Task<IUserList> GetAllUsers();
}

public interface IUserList {
    IList<IUser> Users { get; }
}

public interface IUser {
    public string Name { get; }
}