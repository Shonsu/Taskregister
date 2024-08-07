using FluentValidation;

namespace Taskregister.Server.Todos.Controller.Dto;

public class ExtendByValidator : AbstractValidator<ExtendBy>
{
    private int[] allowedExtendNumbers = [7, 14, 30];

    public ExtendByValidator()
    {
        RuleFor(e => e.days)
            .Must(value => allowedExtendNumbers.Contains(value))
            .When(e => e is not null)
            .WithMessage(e => string.Format("{0} must be digit one of {1}", nameof(e.days), string.Join(", ", allowedExtendNumbers)));

        When(e => allowedExtendNumbers.Contains(e.days), () => RuleFor(e => e.extendByDayRationale).NotNull().NotEmpty());
    }
}
