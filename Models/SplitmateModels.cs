using System.ComponentModel.DataAnnotations;

namespace Final_Project.Models;

public class SplitmateGroup
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
    public List<Member> Members { get; set; } = new();
}

public class Member
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public class Expense
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string PaidBy { get; set; } = string.Empty;
    public string SplitMethod { get; set; } = "Equal";
    public DateTime CreatedOn { get; set; }
    public List<SplitShare> Shares { get; set; } = new();
}

public class SplitShare
{
    public string MemberName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

public class Payment
{
    public int Id { get; set; }
    public string FromMember { get; set; } = string.Empty;
    public string ToMember { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Note { get; set; } = string.Empty;
    public DateTime PaidOn { get; set; }
}

public class GroupNote
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public bool IsPinned { get; set; }
    public DateTime CreatedOn { get; set; }
}

public class GroupTask
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string AssignedTo { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public bool IsComplete { get; set; }
}

public class MemberBalance
{
    public string MemberName { get; set; } = string.Empty;
    public decimal TotalPaid { get; set; }
    public decimal TotalShare { get; set; }
    public decimal PaymentImpact { get; set; }
    public decimal NetBalance { get; set; }
    public string Status => NetBalance >= 0 ? "Gets back" : "Owes";
}

public class SettlementSuggestion
{
    public string FromMember { get; set; } = string.Empty;
    public string ToMember { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

public class ExpenseFormModel
{
    [Required]
    public string Description { get; set; } = string.Empty;

    [Range(0.01, 100000)]
    public decimal Amount { get; set; }

    [Required]
    public string PaidBy { get; set; } = string.Empty;

    [Required]
    public string SplitMethod { get; set; } = "Equal";

    public string ShareInput { get; set; } = string.Empty;
}

public class PaymentFormModel
{
    [Required]
    public string FromMember { get; set; } = string.Empty;

    [Required]
    public string ToMember { get; set; } = string.Empty;

    [Range(0.01, 100000)]
    public decimal Amount { get; set; }

    public string Note { get; set; } = string.Empty;
}

public class NoteFormModel
{
    [Required]
    public string Title { get; set; } = string.Empty;

    public string Body { get; set; } = string.Empty;

    [Required]
    public string CreatedBy { get; set; } = string.Empty;

    public bool IsPinned { get; set; }
}

public class TaskFormModel
{
    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string AssignedTo { get; set; } = string.Empty;

    public DateTime DueDate { get; set; } = DateTime.Today.AddDays(3);
}

public class CreateGroupFormModel
{
    [Required]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [Required]
    public string MembersInput { get; set; } = string.Empty;
}

public class EditGroupFormModel
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}

public class AddMemberFormModel
{
    public int GroupId { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public string Role { get; set; } = "Roommate";
}

public class LoginFormModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }
    public string ReturnUrl { get; set; } = "/";
}

public class UserAccount
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public class SplitmateDashboardViewModel
{
    public SplitmateGroup Group { get; set; } = new();
    public IReadOnlyList<SplitmateGroup> Groups { get; set; } = Array.Empty<SplitmateGroup>();
    public IReadOnlyList<Expense> Expenses { get; set; } = Array.Empty<Expense>();
    public IReadOnlyList<Payment> Payments { get; set; } = Array.Empty<Payment>();
    public IReadOnlyList<GroupNote> Notes { get; set; } = Array.Empty<GroupNote>();
    public IReadOnlyList<GroupTask> Tasks { get; set; } = Array.Empty<GroupTask>();
    public IReadOnlyList<MemberBalance> Balances { get; set; } = Array.Empty<MemberBalance>();
    public IReadOnlyList<SettlementSuggestion> Settlements { get; set; } = Array.Empty<SettlementSuggestion>();
    public IReadOnlyList<string> SplitMethods { get; set; } = Array.Empty<string>();
    public int ActiveGroupId { get; set; }

    public decimal TotalExpenses => Expenses.Sum(expense => expense.Amount);
    public decimal OutstandingTotal => Balances.Where(balance => balance.NetBalance < 0).Sum(balance => Math.Abs(balance.NetBalance));
    public int OpenTaskCount => Tasks.Count(task => !task.IsComplete);
    public int CompletedTaskCount => Tasks.Count(task => task.IsComplete);
    public int ProgressPercent => Tasks.Count == 0 ? 0 : (int)Math.Round(CompletedTaskCount * 100m / Tasks.Count);
}
