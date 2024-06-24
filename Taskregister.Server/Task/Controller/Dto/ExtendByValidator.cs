using FluentValidation;
using Newtonsoft.Json.Linq;

namespace Taskregister.Server.Task.Controller.Dto;

public class ExtendByValidator : AbstractValidator<ExtendBy>
{
    private int[] allowedExtendNumbers = [7, 14, 30];

    public ExtendByValidator()
    {
        RuleFor(e => e.days)
            .Must(value => allowedExtendNumbers.Contains(value))
            .When(e => e is not null);

        When(e => allowedExtendNumbers.Contains(e.days), () => RuleFor(e => e.extendByDayRationale).NotNull().NotEmpty());
    }
}
