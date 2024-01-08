namespace MinApiEssential.Users;

using System.Net.Mail;
public record UserRequest
{
    private readonly string _email = null!;
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