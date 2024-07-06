using System.ComponentModel.DataAnnotations;

namespace MinApiEssential.Users;

public record UserRequest
{
    /// <summary>
    ///     Application User.
    /// </summary>
    /// <param name="Name">Name of the user.</param>
    /// <param name="Email">Email of the user.</param>
    public UserRequest(string Name, string Email)
    {
        this.Name = Name;
        this.Email = Email;
    }

    /// Name of the user.
    public string Name { get; }

    /// Email of the user.
    [EmailAddress]
    public string Email { get; }
}