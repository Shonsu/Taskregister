namespace Taskregister.Server.Shared;

public sealed record Error(string Code, String Description)
{
    public static readonly Error None = new Error(string.Empty, string.Empty);
}
