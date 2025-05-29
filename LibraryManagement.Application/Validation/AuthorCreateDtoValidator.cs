using FluentValidation;
using LibraryManagement.Application.Contracts.Infrastructure;
using LibraryManagement.Application.DTOs.AuthorDtos;
using System;

namespace LibraryManagement.Application.Validation
{
    public class AuthorCreateDtoValidator : AbstractValidator<AuthorCreateDto>
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        public AuthorCreateDtoValidator(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(150).WithMessage("Full name cannot exceed 150 characters.");

            RuleFor(x => x.BirthDate)
                .NotEmpty().WithMessage("Birth date is required.")
                .LessThan(DateTime.Now.AddYears(-18).AddDays(1)) 
                    .WithMessage("Author must be at least 18 years old.")
                .Must(date => date < _dateTimeProvider.UtcNow) 
                    .WithMessage("Birth date cannot be in the future.")
                .GreaterThan(DateTime.Now.AddYears(-150)) 
                    .WithMessage("Birth date is unrealistic (too old).");
        }

        public AuthorCreateDtoValidator() : this(new StaticDateTimeProviderValidator()) { }

        private class StaticDateTimeProviderValidator : IDateTimeProvider { public DateTime UtcNow => DateTime.UtcNow; }
    }
}