using Microsoft.AspNetCore.Identity;

namespace MinApiEssential.Resources;

public class ErrorDescriber : IdentityErrorDescriber
{
    /// <summary>
    /// Returns the default <see cref="IdentityError"/>.
    /// </summary>
    /// <returns>The default <see cref="IdentityError"/>.</returns>
    public override IdentityError DefaultError()
    {
        return new IdentityError
        {
            Code = nameof(DefaultError),
            Description = Resources.DefaultError
        };
    }

    /// <summary>
    /// Returns an <see cref="IdentityError"/> indicating a concurrency failure.
    /// </summary>
    /// <returns>An <see cref="IdentityError"/> indicating a concurrency failure.</returns>
    public override IdentityError ConcurrencyFailure()
    {
        return new IdentityError
        {
            Code = nameof(ConcurrencyFailure),
            Description = Resources.ConcurrencyFailure
        };
    }

    /// <summary>
    /// Returns an <see cref="IdentityError"/> indicating a password mismatch.
    /// </summary>
    /// <returns>An <see cref="IdentityError"/> indicating a password mismatch.</returns>
    public override IdentityError PasswordMismatch()
    {
        return new IdentityError
        {
            Code = nameof(PasswordMismatch),
            Description = Resources.PasswordMismatch
        };
    }

    /// <summary>
    /// Returns an <see cref="IdentityError"/> indicating an invalid token.
    /// </summary>
    /// <returns>An <see cref="IdentityError"/> indicating an invalid token.</returns>
    public override IdentityError InvalidToken()
    {
        return new IdentityError
        {
            Code = nameof(InvalidToken),
            Description = Resources.InvalidToken
        };
    }

    /// <summary>
    /// Returns an <see cref="IdentityError"/> indicating a recovery code was not redeemed.
    /// </summary>
    /// <returns>An <see cref="IdentityError"/> indicating a recovery code was not redeemed.</returns>
    public override IdentityError RecoveryCodeRedemptionFailed()
    {
        return new IdentityError
        {
            Code = nameof(RecoveryCodeRedemptionFailed),
            Description = Resources.RecoveryCodeRedemptionFailed
        };
    }

    /// <summary>
    /// Returns an <see cref="IdentityError"/> indicating an external login is already associated with an account.
    /// </summary>
    /// <returns>An <see cref="IdentityError"/> indicating an external login is already associated with an account.</returns>
    public override IdentityError LoginAlreadyAssociated()
    {
        return new IdentityError
        {
            Code = nameof(LoginAlreadyAssociated),
            Description = Resources.LoginAlreadyAssociated
        };
    }

    /// <summary>
    /// Returns an <see cref="IdentityError"/> indicating the specified user <paramref name="userName"/> is invalid.
    /// </summary>
    /// <param name="userName">The username that is invalid.</param>
    /// <returns>An <see cref="IdentityError"/> indicating the specified user <paramref name="userName"/> is invalid.</returns>
    public override IdentityError InvalidUserName(string? userName)
    {
        return new IdentityError
        {
            Code = nameof(InvalidUserName),
            Description = FormatInvalidUserName(userName)
        };
    }

    /// <summary>
    /// Returns an <see cref="IdentityError"/> indicating the specified <paramref name="email"/> is invalid.
    /// </summary>
    /// <param name="email">The email that is invalid.</param>
    /// <returns>An <see cref="IdentityError"/> indicating the specified <paramref name="email"/> is invalid.</returns>
    public override IdentityError InvalidEmail(string? email)
    {
        return new IdentityError
        {
            Code = nameof(InvalidEmail),
            Description = FormatInvalidEmail(email)
        };
    }

    /// <summary>
    /// Returns an <see cref="IdentityError"/> indicating the specified <paramref name="userName"/> already exists.
    /// </summary>
    /// <param name="userName">The user name that already exists.</param>
    /// <returns>An <see cref="IdentityError"/> indicating the specified <paramref name="userName"/> already exists.</returns>
    public override IdentityError DuplicateUserName(string userName)
    {
        return new IdentityError
        {
            Code = nameof(DuplicateUserName),
            Description = FormatDuplicateUserName(userName)
        };
    }

    /// <summary>
    /// Returns an <see cref="IdentityError"/> indicating the specified <paramref name="email"/> is already associated with an account.
    /// </summary>
    /// <param name="email">The email that is already associated with an account.</param>
    /// <returns>An <see cref="IdentityError"/> indicating the specified <paramref name="email"/> is already associated with an account.</returns>
    public override IdentityError DuplicateEmail(string email)
    {
        return new IdentityError
        {
            Code = nameof(DuplicateEmail),
            Description = FormatDuplicateEmail(email)
        };
    }

    /// <summary>
    /// Returns an <see cref="IdentityError"/> indicating the specified <paramref name="role"/> name is invalid.
    /// </summary>
    /// <param name="role">The invalid role.</param>
    /// <returns>An <see cref="IdentityError"/> indicating the specific role <paramref name="role"/> name is invalid.</returns>
    public override IdentityError InvalidRoleName(string? role)
    {
        return new IdentityError
        {
            Code = nameof(InvalidRoleName),
            Description = FormatInvalidRoleName(role)
        };
    }

    /// <summary>
    /// Returns an <see cref="IdentityError"/> indicating the specified <paramref name="role"/> name already exists.
    /// </summary>
    /// <param name="role">The duplicate role.</param>
    /// <returns>An <see cref="IdentityError"/> indicating the specific role <paramref name="role"/> name already exists.</returns>
    public override IdentityError DuplicateRoleName(string role)
    {
        return new IdentityError
        {
            Code = nameof(DuplicateRoleName),
            Description = FormatDuplicateRoleName(role)
        };
    }

    /// <summary>
    /// Returns an <see cref="IdentityError"/> indicating a user already has a password.
    /// </summary>
    /// <returns>An <see cref="IdentityError"/> indicating a user already has a password.</returns>
    public override IdentityError UserAlreadyHasPassword()
    {
        return new IdentityError
        {
            Code = nameof(UserAlreadyHasPassword),
            Description = Resources.UserAlreadyHasPassword
        };
    }

    /// <summary>
    /// Returns an <see cref="IdentityError"/> indicating user lockout is not enabled.
    /// </summary>
    /// <returns>An <see cref="IdentityError"/> indicating user lockout is not enabled.</returns>
    public override IdentityError UserLockoutNotEnabled()
    {
        return new IdentityError
        {
            Code = nameof(UserLockoutNotEnabled),
            Description = Resources.UserLockoutNotEnabled
        };
    }

    /// <summary>
    /// Returns an <see cref="IdentityError"/> indicating a user is already in the specified <paramref name="role"/>.
    /// </summary>
    /// <param name="role">The duplicate role.</param>
    /// <returns>An <see cref="IdentityError"/> indicating a user is already in the specified <paramref name="role"/>.</returns>
    public override IdentityError UserAlreadyInRole(string role)
    {
        return new IdentityError
        {
            Code = nameof(UserAlreadyInRole),
            Description = FormatUserAlreadyInRole(role)
        };
    }

    /// <summary>
    /// Returns an <see cref="IdentityError"/> indicating a user is not in the specified <paramref name="role"/>.
    /// </summary>
    /// <param name="role">The duplicate role.</param>
    /// <returns>An <see cref="IdentityError"/> indicating a user is not in the specified <paramref name="role"/>.</returns>
    public override IdentityError UserNotInRole(string role)
    {
        return new IdentityError
        {
            Code = nameof(UserNotInRole),
            Description = FormatUserNotInRole(role)
        };
    }

    /// <summary>
    /// Returns an <see cref="IdentityError"/> indicating a password of the specified <paramref name="length"/> does not meet the minimum length requirements.
    /// </summary>
    /// <param name="length">The length that is not long enough.</param>
    /// <returns>An <see cref="IdentityError"/> indicating a password of the specified <paramref name="length"/> does not meet the minimum length requirements.</returns>
    public override IdentityError PasswordTooShort(int length)
    {
        return new IdentityError
        {
            Code = nameof(PasswordTooShort),
            Description = FormatPasswordTooShort(length)
        };
    }

    /// <summary>
    /// Returns an <see cref="IdentityError"/> indicating a password does not meet the minimum number <paramref name="uniqueChars"/> of unique chars.
    /// </summary>
    /// <param name="uniqueChars">The number of different chars that must be used.</param>
    /// <returns>An <see cref="IdentityError"/> indicating a password does not meet the minimum number <paramref name="uniqueChars"/> of unique chars.</returns>
    public override IdentityError PasswordRequiresUniqueChars(int uniqueChars)
    {
        return new IdentityError
        {
            Code = nameof(PasswordRequiresUniqueChars),
            Description = FormatPasswordRequiresUniqueChars(uniqueChars)
        };
    }

    /// <summary>
    /// Returns an <see cref="IdentityError"/> indicating a password entered does not contain a non-alphanumeric character, which is required by the password policy.
    /// </summary>
    /// <returns>An <see cref="IdentityError"/> indicating a password entered does not contain a non-alphanumeric character.</returns>
    public override IdentityError PasswordRequiresNonAlphanumeric()
    {
        return new IdentityError
        {
            Code = nameof(PasswordRequiresNonAlphanumeric),
            Description = Resources.PasswordRequiresNonAlphanumeric
        };
    }

    /// <summary>
    /// Returns an <see cref="IdentityError"/> indicating a password entered does not contain a numeric character, which is required by the password policy.
    /// </summary>
    /// <returns>An <see cref="IdentityError"/> indicating a password entered does not contain a numeric character.</returns>
    public override IdentityError PasswordRequiresDigit()
    {
        return new IdentityError
        {
            Code = nameof(PasswordRequiresDigit),
            Description = Resources.PasswordRequiresDigit
        };
    }

    /// <summary>
    /// Returns an <see cref="IdentityError"/> indicating a password entered does not contain a lower case letter, which is required by the password policy.
    /// </summary>
    /// <returns>An <see cref="IdentityError"/> indicating a password entered does not contain a lower case letter.</returns>
    public override IdentityError PasswordRequiresLower()
    {
        return new IdentityError
        {
            Code = nameof(PasswordRequiresLower),
            Description = Resources.PasswordRequiresLower
        };
    }

    /// <summary>
    /// Returns an <see cref="IdentityError"/> indicating a password entered does not contain an upper case letter, which is required by the password policy.
    /// </summary>
    /// <returns>An <see cref="IdentityError"/> indicating a password entered does not contain an upper case letter.</returns>
    public override IdentityError PasswordRequiresUpper()
    {
        return new IdentityError
        {
            Code = nameof(PasswordRequiresUpper),
            Description = Resources.PasswordRequiresUpper
        };
    }

    private static string FormatDuplicateEmail<T>(T p0)
    {
        return string.Format(Resources.Culture, Resources.DuplicateEmail, p0);
    }

    private static string FormatDuplicateRoleName<T>(T p0)
    {
        return string.Format(Resources.Culture, Resources.DuplicateRoleName, p0);
    }

    private static string FormatDuplicateUserName<T>(T p0)
    {
        return string.Format(Resources.Culture, Resources.DuplicateUserName, p0);
    }

    internal static string FormatInvalidEmail<T>(T p0)
    {
        return string.Format(Resources.Culture, Resources.InvalidEmail, p0);
    }

    private static string FormatInvalidManagerType<T>(T p0, T p1, T p2)
    {
        return string.Format(Resources.Culture, Resources.InvalidManagerType, p0, p1, p2);
    }

    private static string FormatInvalidRoleName<T>(T p0)
    {
        return string.Format(Resources.Culture, Resources.InvalidRoleName, p0);
    }

    private static string FormatInvalidUserName<T>(T p0)
    {
        return string.Format(Resources.Culture, Resources.InvalidUserName, p0);
    }

    private static string FormatNoTokenProvider<T>(T p0, T p1)
    {
        return string.Format(Resources.Culture, Resources.NoTokenProvider, p0, p1);
    }

    private static string FormatPasswordTooShort<T>(T p0)
    {
        return string.Format(Resources.Culture, Resources.PasswordTooShort, p0);
    }

    private static string FormatRoleNotFound<T>(T p0)
    {
        return string.Format(Resources.Culture, Resources.RoleNotFound, p0);
    }

    private static string FormatUserAlreadyInRole<T>(T p0)
    {
        return string.Format(Resources.Culture, Resources.UserAlreadyInRole, p0);
    }

    private static string FormatUserNameNotFound<T>(T p0)
    {
        return string.Format(Resources.Culture, Resources.UserNameNotFound, p0);
    }

    private static string FormatUserNotInRole<T>(T p0)
    {
        return string.Format(Resources.Culture, Resources.UserNotInRole, p0);
    }

    private static string FormatPasswordRequiresUniqueChars<T>(T p0)
    {
        return string.Format(Resources.Culture, Resources.PasswordRequiresUniqueChars, p0);
    }
}