using FluentValidation;

namespace ProductService.Application.Commands.Manufacturer.DeleteManufacturer
{
    public class DeleteManufacturerCommandValidator : AbstractValidator<DeleteManufacturerCommand>
    {
        public DeleteManufacturerCommandValidator()
        {
            RuleFor(m => m.id)
                .NotEmpty().WithMessage("Manufacturer id is empty.")
                .Must(m => Guid.TryParse(m, out _));
        }
    }
}
