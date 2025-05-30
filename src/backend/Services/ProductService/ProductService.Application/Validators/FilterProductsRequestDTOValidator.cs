using FluentValidation;
using ProductService.Application.DTOs.Request;

namespace ProductService.Application.Validators
{
    public class FilterProductsRequestDTOValidator : AbstractValidator<FilterProductsRequestDTO>
    {
        public FilterProductsRequestDTOValidator()
        {
            RuleFor(q => q.SearchTerm).MaximumLength(256);
        }
    }
}
