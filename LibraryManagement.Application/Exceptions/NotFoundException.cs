// src/LibraryManagementSystem.Application/Exceptions/NotFoundException.cs
using System;

namespace LibraryManagement.Application.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string name, object key)
            : base($"Entity \"{name}\" ({key}) was not found.")
        {
        }
    }
}