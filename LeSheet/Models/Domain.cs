using System.ComponentModel.DataAnnotations;

namespace LeSheet.Models;

public class User
{
  public int Id { get; set; }
  public string Name { get; set; } = "";
}


public record DepenseCreateDto
{
  [Required(ErrorMessage = "La description est obligatoire")]
  [MinLength(3, ErrorMessage = "La description doit faire au moins 3 caractères")]
  public required string  Description { get; set; }
  
  [Range(0.01, 10000, ErrorMessage = "Le montant doit etre superieur à 0")]
  public required decimal Amount { get; set; }
  public required int PaidByUserId { get; set; }
};

public class Depense
{
  public int Id { get; set; }
  public required string Description { get; set; }
  public required decimal Amount { get; set; }
  public DateTime Date { get; set; } = DateTime.UtcNow;
  public required int PaidByUserId { get; set; }
}

public record RemboursementCreateDto
{
  public required decimal Amount { get; set; }
  public required int FromUserId { get; set; }
  public required int ToUserId { get; set; }
};
public class Remboursement
{
  public int Id { get; set; }
  public required decimal Amount { get; set; }
  public required int FromUserId { get; set; }
  public required int ToUserId { get; set; }
  public DateTime Date { get; set; } = DateTime.UtcNow;
}

public record BalanceResponseDto
(
 string User1Name,
 string User2Name,
 decimal TotalPayeParUser1,
 decimal TotalPayeParUser2,
 decimal CurrentBalance,  
 string Recommendation
);