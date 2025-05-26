using FluentValidation;

namespace ProductService.Application.Commands.Manufacturer.UpdateManufacturer
{
    public class UpdateManufacturerCommandValidator : AbstractValidator<UpdateManufacturerCommand>
    {
        public UpdateManufacturerCommandValidator()
        {
            RuleFor(x => x.id)
                .NotEmpty().WithMessage("Manufacturer id is empty.");
        }
    }
}
