using System.ComponentModel.DataAnnotations;

namespace MinApiEssential.Users.Api;

/// <summary>
///     Application User.
/// </summary>
/// <param name="Id">ID of the user.</param>
/// <param name="Name">Name of the user.</param>
/// <param name="Email">Email of the user.</param>
public record UserResponse(Guid Id, string Name, [property: EmailAddress] string Email);