using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SIGD.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIGD.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Each of models must be set here to have sql context
        /// </summary>
        public DbSet<ActivationAccount> ActivationAccount { get; set; }
    }
}
