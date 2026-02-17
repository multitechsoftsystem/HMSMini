using HMSMini.API.Models.Enums;

namespace HMSMini.API.Models.DTOs.ChartOfAccount;

public class ChartOfAccountDto
{
    public int Id { get; set; }
    public string AccountCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public AccountType AccountType { get; set; }
    public string AccountTypeName => AccountType.ToString();
    public int? ParentAccountId { get; set; }
    public string? ParentAccountName { get; set; }
    public bool IsSystemAccount { get; set; }
    public bool IsActive { get; set; }
}

public class CreateChartOfAccountDto
{
    public string AccountCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public AccountType AccountType { get; set; }
    public int? ParentAccountId { get; set; }
}

public class UpdateChartOfAccountDto
{
    public string AccountName { get; set; } = string.Empty;
    public int? ParentAccountId { get; set; }
    public bool IsActive { get; set; }
}

public class AccountDropdownDto
{
    public int Id { get; set; }
    public string AccountCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public AccountType AccountType { get; set; }
    public string DisplayName => $"{AccountCode} - {AccountName}";
}
