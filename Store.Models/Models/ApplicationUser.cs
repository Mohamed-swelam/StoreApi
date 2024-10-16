﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Store.Models.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Address {  get; set; }
        public ICollection<Order> orders { get; set; }
        public ICollection<Review> reviews { get; set; }
        public List<RefreshToken>? refreshTokens { get; set; }
    }
}
