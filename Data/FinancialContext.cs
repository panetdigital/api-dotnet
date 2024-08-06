using Microsoft.EntityFrameworkCore;
using FinancialPlannerAPI.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace FinancialPlannerAPI.Data
{
    public class FinancialContext : IdentityDbContext
    {
        public FinancialContext(DbContextOptions<FinancialContext> options) : base(options) 
        {
            
        }

        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Income> Incomes { get; set; }
    }
}

