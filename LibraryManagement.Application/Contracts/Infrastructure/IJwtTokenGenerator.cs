﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Application.Contracts.Infrastructure
{
    public interface IJwtTokenGenerator
    {
        Task<string> GenerateTokenAsync(string userId);
    }
}
