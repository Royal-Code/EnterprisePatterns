
namespace RoyalCode.DomainEvents.Attributes;

/// <summary>
/// Attribute to define the name of the event.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class EventNameAttribute : Attribute
{
    /// <summary>
    /// The name of the event.
    /// </summary>
    public string Name;

    /// <summary>
    /// Creates a new attribute.
    /// </summary>
    /// <param name="name">The name of the event.</param>
    public EventNameAttribute(string name)
    {
        Name = name;
    }
}
