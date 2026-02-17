namespace HMSMini.API.Models.DTOs.ExpenseHead;

public class ExpenseHeadDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? DefaultAccountId { get; set; }
    public string? DefaultAccountName { get; set; }
    public bool IsActive { get; set; }
}

public class CreateExpenseHeadDto
{
    public string Name { get; set; } = string.Empty;
    public int? DefaultAccountId { get; set; }
}

public class UpdateExpenseHeadDto
{
    public string Name { get; set; } = string.Empty;
    public int? DefaultAccountId { get; set; }
    public bool IsActive { get; set; }
}
