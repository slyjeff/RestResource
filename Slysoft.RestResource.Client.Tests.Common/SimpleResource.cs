using System;
using System.Collections.Generic;
using TestUtils;

namespace Slysoft.RestResource.Client.Tests.Common;

public enum OptionEnum { Option1, Option2 };

public interface ISimpleResource {
    string Message { get; }
    int Number { get; }
    OptionEnum Option { get; }
    bool? IsOptional { get; }
    DateTime Date { get;  }
    IList<string> Strings { get; }
    IList<int> Numbers { get; }
    ChildResource Child { get; }
    IChildResource ChildInterface { get; }
    IList<ChildResource> Children { get; }
    IList<IChildResource> ChildInterfaces { get; }
}

public sealed class SimpleResource : ISimpleResource {
    public string Message { get; set; } = GenerateRandom.String();
    public int Number { get; set; } = GenerateRandom.Int();
    public OptionEnum Option { get; set; } = OptionEnum.Option2;
    public bool? IsOptional { get; set; } = true;
    public DateTime Date { get; set; } = DateTime.Now;
    public IList<string> Strings { get; } = new List<string> { GenerateRandom.String(), GenerateRandom.String(), GenerateRandom.String() };
    public IList<int> Numbers { get; } = new List<int> { GenerateRandom.Int(), GenerateRandom.Int(), GenerateRandom.Int() };
    public ChildResource Child { get; } = new();
    public IChildResource ChildInterface { get; } = new ChildResource();
    public IList<ChildResource> Children { get; } = new List<ChildResource> { new(), new(), new() };
    public IList<IChildResource> ChildInterfaces { get; } = new List<IChildResource> { new ChildResource(), new ChildResource(), new ChildResource() };
}

public interface IChildResource {
    string ChildMessage { get; }
    int ChildNumber { get; }
}

public sealed class ChildResource : IChildResource {
    public string ChildMessage { get; set; } = GenerateRandom.String();
    public int ChildNumber { get; set; } = GenerateRandom.Int();
}
