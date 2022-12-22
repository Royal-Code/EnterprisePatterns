
using System.Text.RegularExpressions;

namespace RoyalCode.DESGenerator.Tests.Models.Users;

public class EMail
{
    // single email regex
    private const string EmailRegex = @"^(([^<>()[\]\\.,;:\s@\""]+"
        + @"(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@"
        + @"((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|"
        + @"(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$";
    
    public EMail(string address)
    {
        Address = address;
        // validate address
        IsValid = Regex.IsMatch(address, EmailRegex);
    }

    public string Address { get; }

    public bool IsValid { get; }
    
    public bool IsEmpty => string.IsNullOrEmpty(Address);

    public bool IsNotEmpty => !IsEmpty;
    
    public override string ToString() => Address;

    public override bool Equals(object? obj)
    {
        if (obj is EMail other)
            return Address == other.Address;
        return false;
    }

    public override int GetHashCode() => Address.GetHashCode();

    public static bool operator ==(EMail left, EMail right) => left.Equals(right);

    public static bool operator !=(EMail left, EMail right) => !(left == right);

    public static implicit operator string(EMail email) => email.Address;

    public static implicit operator EMail(string address) => new EMail(address);

    private static readonly EMail empty = new (string.Empty);
    public static EMail Empty => empty;
}
