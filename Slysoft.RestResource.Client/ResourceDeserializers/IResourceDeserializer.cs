using System.Net.Http;

namespace Slysoft.RestResource.Client.ResourceDeserializers; 

/// <summary>
/// User to provide a custom deserializer for converting an HttpResponseMessage into a resource
/// </summary>
public interface IResourceDeserializer {
    /// <summary>
    /// Whether this deserializer can deserialize the message
    /// </summary>
    /// <param name="response">Message to deserialize</param>
    /// <returns>True if this deserializer can deserialize the message</returns>
    bool CanDeserialize(HttpResponseMessage response);

    /// <summary>
    /// Generate a message from the HttpResponseMessage
    /// </summary>
    /// <param name="response">Message to deserialize</param>
    /// <returns>Deserialized Resource</returns>
    Resource Deserialize(HttpResponseMessage response);
}