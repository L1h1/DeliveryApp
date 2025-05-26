using FluentValidation;

namespace ProductService.Application.Commands.Manufacturer.DeleteManufacturer
{
    public class DeleteManufacturerCommandValidator : AbstractValidator<DeleteManufacturerCommand>
    {
        public DeleteManufacturerCommandValidator()
        {
            RuleFor(m => m.Id)
                .NotEmpty().WithMessage("Manufacturer id is empty.");
        }
    }
}
