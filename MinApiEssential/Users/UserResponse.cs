using System.ComponentModel.DataAnnotations;

namespace MinApiEssential.Users;

using System.Net.Mail;
/// <summary>
///     Application User.
/// </summary>
public record UserResponse
{
    /// <summary>
    ///     Application User.
    /// </summary>
    /// <param name="Id">ID of the user.</param>
    /// <param name="Name">Name of the user.</param>
    /// <param name="Email">Email of the user.</param>
    public UserResponse(Guid Id, string Name, MailAddress Email)
    {
        this.Id = Id;
        this.Name = Name;
        this.Email = Email.Address;
    }

    /// ID of the user.
    public Guid Id { get; }

    /// Name of the user.
    public string Name { get; }
    
    /// Email of the user.
    [EmailAddress]
    public string Email { get; }
}
