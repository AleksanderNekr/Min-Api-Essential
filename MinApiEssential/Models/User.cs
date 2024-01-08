namespace MinApiEssential.Models;

/// <summary>
/// Application User.
/// </summary>
/// <param name="Id">ID of the user.</param>
/// <param name="Name">Name of the user.</param>
/// <param name="Email">Email of the user.</param>
public record User(Guid Id, string Name, string Email);
