using System;

namespace Slysoft.RestResource.Client; 

public abstract class RestResourceClientException : Exception {
    protected RestResourceClientException(string message) : base(message) {
    }
    protected RestResourceClientException(string message, Exception innerException) : base(message, innerException) {
    }
}


/// <summary>
/// An error thrown when creating an accessor fails
/// </summary>
public sealed class CreateAccessorException : RestResourceClientException {
    internal CreateAccessorException(string message) : base(message) {
    }

    internal CreateAccessorException(string message, Exception innerException) : base(message, innerException) {
    }
}