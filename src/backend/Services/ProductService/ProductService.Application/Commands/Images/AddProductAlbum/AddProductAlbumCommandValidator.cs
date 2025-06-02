using FluentValidation;

namespace ProductService.Application.Commands.Images.AddProductAlbum
{
    public class AddProductAlbumCommandValidator : AbstractValidator<AddProductAlbumCommand>
    {
        public AddProductAlbumCommandValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("Product id is empty.");

            RuleFor(x => x.Files)
                .NotEmpty().WithMessage("Thumbnail is empty.")
                .ChildRules(x => x.RuleForEach(f => f.ToList())
                    .NotNull().Must(f => f.Equals("image/jpeg")).WithMessage("Unsupported file type. Only jpeg supported."));
        }
    }
}
