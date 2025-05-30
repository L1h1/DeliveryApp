using FluentValidation;
using ProductService.Application.DTOs.Request;

namespace ProductService.Application.Validators
{
    public class ManufacturerRequestDTOValidator : AbstractValidator<ManufacturerRequestDTO>
    {
        public ManufacturerRequestDTOValidator()
        {
            RuleFor(m => m.Name)
                .NotEmpty().WithMessage("Manufacturer name is empty.")
                .MaximumLength(128).WithMessage("Manufacturer name can't be longer than 128 characters.");

            RuleFor(m => m.Country)
                .NotEmpty().WithMessage("Manufacturer country is empty.")
                .MaximumLength(64).WithMessage("Manufacturer country can't be longer than 64 characters.");

            RuleFor(m => m.Address)
                .NotEmpty().WithMessage("Manufacturer address is empty.")
                .MaximumLength(256).WithMessage("Manufacturer's address can't be longer than 256 characters.");
        }
    }
}
