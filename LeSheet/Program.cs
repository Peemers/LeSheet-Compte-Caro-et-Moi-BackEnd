using LeSheet.Models;
using Microsoft.EntityFrameworkCore;
using LeSheet.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var connectionStrings = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDataContext>(options =>
  options.UseSqlServer(connectionStrings));

builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

builder.Services.AddOpenApi();

var app = builder.Build();
app.UseCors();

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
  var nouveauReboursement = new Remboursement
  {
    Amount = dto.Amount,
    FromUserId = dto.FromUserId,
    ToUserId = dto.ToUserId,
    Date = DateTime.UtcNow
  };

  db.Remboursements.Add(nouveauReboursement);
  await db.SaveChangesAsync();
  return Results.Ok("Montant du remboursement calculé");
});

app.MapGet("api/Balance", async (AppDataContext db) =>
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

  decimal balanceUser1 = (totalPayeParUser1 / 2) - (totalPayeParUser2 / 2) + remboursementUser2VersUser1 - remboursementUser1VersUser2;

  string message = balanceUser1 > 0 ? $"{u2.Name} doit {balanceUser1:f2}€ à {u1.Name}" : $"{Math.Abs(balanceUser1):F2}€ à {u2.Name}";

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