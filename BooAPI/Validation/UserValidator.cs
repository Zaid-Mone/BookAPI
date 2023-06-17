using BooAPI.Models;
using FluentValidation;

namespace BookAPI.Validation
{
    public class UserValidator:AbstractValidator<AppUser>
    {
        public UserValidator()
        {
            RuleFor(x => x.FirstName).EmailAddress()
                .NotEmpty()
                .WithMessage("Please Enter Your First Name");

            RuleFor(x => x.LastName).EmailAddress()
                .NotEmpty()
                .WithMessage("Please Enter Your Last Name");

            RuleFor(x => x.Email).EmailAddress()
                .NotEmpty()
                .WithMessage("Please Enter Your Email");

            RuleFor(x => x.PhoneNumber).EmailAddress()
                .NotEmpty()
                .WithMessage("Please Enter Your Phone Number");

        }
    }
}
