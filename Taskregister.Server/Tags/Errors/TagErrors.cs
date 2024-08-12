using Taskregister.Server.Shared;

namespace Taskregister.Server.Tags.Errors;

public class TagErrors
{
    public static Error NotFoundTagWithId(int tagId) => new Error("Tag.NotFoundTagWithId", $"Not found tag with id='{tagId}'.");
    public static Error TagNameAlreadyExist(string tagName) => new Error("Tag.TagNameAlreadyExist", $"Tag with name='{tagName}' already exists.");
    public static Error AlreadyAssignedToTodo(int tagId) => new Error("Tag.AlreadyAssignedToTodo", $"Tag with id {tagId} is assigned to Todo.");

}