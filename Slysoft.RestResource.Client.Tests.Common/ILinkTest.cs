using System.Collections.Generic;
using System.Threading.Tasks;

namespace Slysoft.RestResource.Client.Tests.Common; 

public interface ILinkTest {
    IUserList GetAllUsers();
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