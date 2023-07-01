﻿using System.Threading.Tasks;

namespace Slysoft.RestResource.Client.Tests.Common; 

public interface ILinkTest {
    IUser GetAllUsers();
}

public interface ILinkTestAsync {
    Task<IUser> GetAllUsers();
}

public interface IUser {
    public string Name { get; set; }
}