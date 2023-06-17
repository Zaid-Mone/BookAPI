using BooAPI.Models;
using FluentValidation;

namespace BookAPI.Validation
{
    public class AuthorValidator : AbstractValidator<Author>
    {
        public AuthorValidator()
        {
            RuleFor(auhtor => auhtor.AuthorName).NotEmpty()
                .WithMessage("Please Set a Name For the Author");
        }
    }
}
