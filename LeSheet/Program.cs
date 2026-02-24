using LeSheet.Models;
using Microsoft.EntityFrameworkCore;
using LeSheet.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var connectionStrings = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDataContext>(options =>
  options.UseSqlServer(connectionStrings));

//recup allowedOrigin
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();
builder.Services.AddCors(option =>
{
  option.AddPolicy("AllowAngular", policy =>
  {
    policy.WithOrigins(allowedOrigins ?? []);
    policy.AllowAnyHeader();
    policy.AllowAnyMethod();
  });
});

builder.Services.AddOpenApi();

var app = builder.Build();
app.UseCors("AllowAngular");

app.MapPost("/api/setup", async (AppDataContext db) =>
{
  if (await db.Users.AnyAsync())
    return Results.BadRequest("Les utilisateurs existent deja");

  var users = new List<User>
  {
    new User { Name = "Mathieu" },
    new User { Name = "Caroline" }
  };

  db.Users.AddRange(users);
  await db.SaveChangesAsync();

  return Results.Ok("profil ok");
});

app.MapPost("/api/depenses", async (DepenseCreateDto dto, AppDataContext db) =>
{
  if (dto.Amount <= 0)
  {
    return Results.BadRequest("Le montant doit etre superieur à 0");
  }
  var userExists = await db.Users.AnyAsync(u => u.Id == dto.PaidByUserId);

  if (!userExists)
  {
    return Results.BadRequest($"L'utilisateur avec l'ID {dto.PaidByUserId} n'existe pas");
  }

  var nouvelleDepense = new Depense
  {
    Description = dto.Description,
    Amount = dto.Amount,
    PaidByUserId = dto.PaidByUserId
  };

  db.Depenses.Add(nouvelleDepense);
  await db.SaveChangesAsync();
  return Results.Created($"/api/depenses/{nouvelleDepense.Id}", nouvelleDepense);
});

app.MapPost("/api/remboursement", async (RemboursementCreateDto dto, AppDataContext db) =>
{
  if (dto.Amount <= 0)
  {
    return Results.BadRequest("Un remboursement doit avoir un montant positif");
  }
  var nouveauRemboursement = new Remboursement
  {
    Amount = dto.Amount,
    FromUserId = dto.FromUserId,
    ToUserId = dto.ToUserId,
    Date = DateTime.UtcNow
  };

  db.Remboursements.Add(nouveauRemboursement);
  await db.SaveChangesAsync();
  return Results.Ok("Remboursement enregistré avec succès");
});

app.MapGet("/api/depenses", async (AppDataContext db) =>
{
  var depenses = await db.Depenses
    .Select(d => new HistoryItemDto(d.Id, d.Description, d.Amount, d.PaidByUserId, d.Date, false))
    .ToListAsync();


  var remboursements = await db.Remboursements
    .Select(r => new HistoryItemDto(r.Id, "Remboursement ✅", r.Amount, r.FromUserId, r.Date, true))
    .ToListAsync();


  var historique = depenses
    .Concat(remboursements)
    .OrderByDescending(x => x.Date)
    .ToList();

  return Results.Ok(historique);
});

app.MapDelete("/api/depenses/{id:int}", async (int id, bool isRemboursement, AppDataContext context) =>
{
  if (isRemboursement)
  {
    var remb = await context.Remboursements.FindAsync(id);
    if (remb == null) return Results.NotFound();
    context.Remboursements.Remove(remb);
  }
  else
  {
    var depense = await context.Depenses.FindAsync(id);
    if (depense == null) return Results.NotFound();
    context.Depenses.Remove(depense);
  }

  await context.SaveChangesAsync();
  return Results.NoContent();
});

app.MapGet("/api/Balance", async (AppDataContext db) =>
{
  var users = await db.Users.ToListAsync();
  if (users.Count < 2) return Results.BadRequest("Il faut entrer les 2 utilisateurs pour pouvoir calculer.");

  var u1 = users[0];
  var u2 = users[1];

  var totalPayeParUser1 = await db.Depenses
    .Where(d => d.PaidByUserId == u1.Id)
    .SumAsync(d => d.Amount);

  var totalPayeParUser2 = await db.Depenses
    .Where(d => d.PaidByUserId == u2.Id)
    .SumAsync(d => d.Amount);

  var remboursementUser2VersUser1 = await db.Remboursements
    .Where(r => r.FromUserId == u2.Id && r.ToUserId == u1.Id)
    .SumAsync(r => r.Amount);

  var remboursementUser1VersUser2 = await db.Remboursements
    .Where(r => r.FromUserId == u1.Id && r.ToUserId == u2.Id)
    .SumAsync(r => r.Amount);

  decimal detteBrute = (totalPayeParUser1 / 2m) - (totalPayeParUser2 / 2m);
  decimal balanceUser1 = detteBrute - remboursementUser2VersUser1 + remboursementUser1VersUser2;

  string message = balanceUser1 > 0 ? $"{u2.Name} doit {balanceUser1:f2}€ à {u1.Name}" : $"{u1.Name} doit : {Math.Abs(balanceUser1):F2}€ à {u2.Name}";

  var response = new BalanceResponseDto(
    u1.Name,
    u2.Name,
    totalPayeParUser1,
    totalPayeParUser2,
    balanceUser1,
    message
  );


  return Results.Ok(response);
});

if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
  app.MapScalarApiReference();
}

app.Run();

public record HistoryItemDto(int Id, string Description, decimal Amount, int PaidByUserId, DateTime Date, bool IsRemboursement);