using FluentValidation;
using NotificationService.Domain.Entities;

namespace NotificationService.Infrastructure.Validation;

public class NotificationValidation : AbstractValidator<NotificationDTOs>
{
    public NotificationValidation()
    {
        RuleFor(x => x.Title).NotEmpty()
            .WithMessage("Title is required")
            .Length(1, 50);
        RuleFor(x => x.Message)
            .NotEmpty()
            .WithMessage("Message is required")
            .Length(1, 500);
        RuleFor(x => x.RecipientEmail)
            .NotEmpty()
            .WithMessage("RecipientEmail is required")
            .EmailAddress()
            .WithMessage("Recipient email must be a valid email address");
    }
}