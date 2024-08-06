using FinancialPlannerAPI.Data;
using FinancialPlannerAPI.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
// Add services to the container
builder.Services.AddDbContext<FinancialContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
object value = builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

// Minimal API Endpoints
app.MapGet("/api/expenses", async (FinancialContext db) =>
    await db.Expenses.ToListAsync());

app.MapPost("/api/expenses", async (FinancialContext db, Expense expense) =>
{
    db.Expenses.Add(expense);
    await db.SaveChangesAsync();
    return Results.Created($"/api/expenses/{expense.Id}", expense);
});

app.MapGet("/api/incomes", async (FinancialContext db) =>
    await db.Incomes.ToListAsync());

app.MapPost("/api/incomes", async (FinancialContext db, Income income) =>
{
    db.Incomes.Add(income);
    await db.SaveChangesAsync();
    return Results.Created($"/api/incomes/{income.Id}", income);
});

// Seed database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<FinancialContext>();
    SeedData.Initialize(context);
}

app.Run();

public static class SeedData
{
    public static void Initialize(FinancialContext context)
    {

        // Configure Npgsql to handle local DateTime values
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        context.Database.Migrate();

        // Check if the database has been seeded already
        if (!context.Expenses.Any() && !context.Incomes.Any())
        {
            var expenses = new[]
                    {
                        new Expense { Description = "Compra de Supermercado", Amount = 150.75m, Category = "Alimentação", Date = DateTime.UtcNow },
                        new Expense { Description = "Conta de Luz", Amount = 90.00m, Category = "Utilidades", Date = DateTime.UtcNow },
                        new Expense { Description = "Gasolina", Amount = 50.25m, Category = "Transporte", Date = DateTime.UtcNow }
                    };

                                var incomes = new[]
                                {
                        new Income { Description = "Salário", Amount = 3000.00m, Source = "Empresa XYZ", Date = DateTime.UtcNow },
                        new Income { Description = "Freelance", Amount = 500.00m, Source = "Cliente ABC", Date = DateTime.UtcNow }
                    };


            context.Expenses.AddRange(expenses);
            context.Incomes.AddRange(incomes);
            context.SaveChanges();
        }
    }
}
