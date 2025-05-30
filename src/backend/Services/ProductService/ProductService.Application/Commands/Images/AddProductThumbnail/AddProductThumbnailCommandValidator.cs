using FluentValidation;

namespace ProductService.Application.Commands.Images.AddProductThumbnail
{
    public class AddProductThumbnailCommandValidator : AbstractValidator<AddProductThumbnailCommand>
    {
        public AddProductThumbnailCommandValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("Product id is empty.");

            RuleFor(x => x.File)
                .NotEmpty().WithMessage("Thumbnail is empty.")
                .ChildRules(x => x.RuleFor(f => f.ContentType)
                    .NotNull().Must(f => f.Equals("image/jpeg")).WithMessage("Unsupported file type. Only jpeg supported."));
        }
    }
}
