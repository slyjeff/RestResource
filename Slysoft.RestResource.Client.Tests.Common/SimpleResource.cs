using System;
using TestUtils;

namespace Slysoft.RestResource.Client.Tests.Common;

public enum OptionEnum { Option1, Option2 };

public interface ISimpleResource {
    string Message { get; }
    int Number { get; }
    OptionEnum Option { get; }
    bool? IsOptional { get; }
    DateTime Date { get;  }
}

public sealed class SimpleResource : ISimpleResource {
    public string Message { get; set; } = GenerateRandom.String();
    public int Number { get; set; } = GenerateRandom.Int();
    public OptionEnum Option { get; set; } = OptionEnum.Option2;
    public bool? IsOptional { get; set; } = true;
    public DateTime Date { get; set; } = DateTime.Now;
}