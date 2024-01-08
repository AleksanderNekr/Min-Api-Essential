namespace MinApiEssential.Users;

using System.Net.Mail;
/// <summary>
///     Application User.
/// </summary>
public record UserResponse
{
    private readonly string _email = null!;
    /// <summary>
    ///     Application User.
    /// </summary>
    /// <param name="Id">ID of the user.</param>
    /// <param name="Name">Name of the user.</param>
    /// <param name="Email">Email of the user.</param>
    public UserResponse(Guid Id, string Name, string Email)
    {
        this.Id = Id;
        this.Name = Name;
        this.Email = Email;
    }

    /// ID of the user.
    public Guid Id { get; }

    /// Name of the user.
    public string Name { get; }
    
    /// Email of the user.
    public string Email
    {
        get
        {
            return _email;
        }
        private init
        {
            if (!MailAddress.TryCreate(value, out MailAddress? address))
            {
                throw new EmailIncorrectException(value);
            }

            _email = address.Address;
        }
    }
}
