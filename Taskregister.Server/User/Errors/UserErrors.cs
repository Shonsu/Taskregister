using Taskregister.Server.Shared;

namespace Taskregister.Server.User.Errors
{
    public static class UserErrors
    {
        public static Error NotFoundByEmail(string userEmail) => new("Users.NotFoundByEmail", $"The user with Email='{userEmail}' was not found.");
    }
}
