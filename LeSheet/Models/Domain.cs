namespace LeSheet.Models;

public class User
{
  public int Id { get; set; }
  public string Name { get; set; } = "";
}


public record DepenseCreateDto
{
  public string Description { get; set; }
  public decimal Amount { get; set; }
  public int PaidByUserId { get; set; }
};

public class Depense
{
  public int Id { get; set; }
  public string Description { get; set; } = "";
  public decimal Amount { get; set; }
  public DateTime Date { get; set; } = DateTime.UtcNow;
  public int PaidByUserId { get; set; }
}

public record RemboursementCreateDto
{
  public decimal Amount { get; set; }
  public int FromUserId { get; set; }
  public int ToUserId { get; set; }
};
public class Remboursement
{
  public int Id { get; set; }
  public decimal Amount { get; set; }
  public int FromUserId { get; set; }
  public int ToUserId { get; set; }
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