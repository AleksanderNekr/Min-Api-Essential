using System.ComponentModel.DataAnnotations;

namespace MinApiEssential.Users.Api;

/// <summary>
///     Application User.
/// </summary>
/// <param name="Name">Name of the user</param>
/// <param name="Email">Email of the user</param>
public record UserRequest(string Name, [property: EmailAddress] string Email);