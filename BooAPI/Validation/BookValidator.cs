using BooAPI.Models;
using FluentValidation;

namespace BookAPI.Validation
{
    public class BookValidator:AbstractValidator<Book>
    {
        public BookValidator()
        {
            RuleFor(book => book.BookName).NotEmpty()
            .WithMessage("Please Set a Name For the Book");
            RuleFor(book => book.AuthorId).NotEmpty()
                    .WithMessage("Please Set an Name For the Author");
        }
    }
}
