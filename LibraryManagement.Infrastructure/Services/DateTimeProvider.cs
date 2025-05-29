// src/LibraryManagementSystem.Infrastructure/Services/DateTimeProvider.cs
using LibraryManagement.Application.Contracts.Infrastructure;
using System;

namespace LibraryManagement.Infrastructure.Services
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}