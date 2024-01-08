namespace MinApiEssential.Users;

internal class EmailIncorrectException(string mail, string? message = null) : Exception(message)
{
    public string Mail { get; } = mail;
}
