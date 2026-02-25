using System.ComponentModel.DataAnnotations;

namespace LeSheet.Models;

public class User
{
  public int Id { get; set; }
  public string Name { get; set; } = "";
  [Required(ErrorMessage = "Password Obligatoire")]
  [StringLength(100, MinimumLength = 8, ErrorMessage = "Le password doit comporter entre 8 et 20 caracteres")]
  public string Password { get; set; } = "";
}


public record DepenseCreateDto
{
  [Required(ErrorMessage = "La description est obligatoire")]
  [StringLength(100, MinimumLength = 3, ErrorMessage = "La déscription doit comporter en 3 et 100 caracteres")]
  public required string  Description { get; set; }
  
  [Required]
  [Range(0, 2500, ErrorMessage = "Le montant doit etre > à 0 et < à 2500")]
  public required decimal Amount { get; set; }
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
  [Required]
  [Range(0, 2500, ErrorMessage = "Le montant du remboursement doit etre > à 0 et < à 2500")]
  public required decimal Amount { get; set; }
  [Required]
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