// src/LibraryManagementSystem.Application/Validation/AuthorCreateDtoValidator.cs
using FluentValidation;
using LibraryManagement.Application.Contracts.Infrastructure;
using LibraryManagement.Application.DTOs.AuthorDtos;
using System;

namespace LibraryManagement.Application.Validation
{
    public class AuthorCreateDtoValidator : AbstractValidator<AuthorCreateDto>
    {
        // IDateTimeProvider can be injected if this validator is resolved by DI.
        // For .NET Framework, DI for FluentValidation validators might require more setup.
        // Using a static provider or passing it in if not using DI for validators.
        private readonly IDateTimeProvider _dateTimeProvider;

        public AuthorCreateDtoValidator(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(150).WithMessage("Full name cannot exceed 150 characters.");

            RuleFor(x => x.BirthDate)
                .NotEmpty().WithMessage("Birth date is required.")
                .LessThan(DateTime.Now.AddYears(-18).AddDays(1)) // Ensure they are at least 18
                    .WithMessage("Author must be at least 18 years old.")
                .Must(date => date < _dateTimeProvider.UtcNow) // Cannot be a future date
                    .WithMessage("Birth date cannot be in the future.")
                .GreaterThan(DateTime.Now.AddYears(-150)) // Example: not older than 150 years
                    .WithMessage("Birth date is unrealistic (too old).");
        }

        // Parameterless constructor for cases where DI isn't straightforward for validators
        // or if IDateTimeProvider is not available for injection into the validator.
        public AuthorCreateDtoValidator() : this(new StaticDateTimeProviderValidator()) { }

        private class StaticDateTimeProviderValidator : IDateTimeProvider { public DateTime UtcNow => DateTime.UtcNow; }
    }
}