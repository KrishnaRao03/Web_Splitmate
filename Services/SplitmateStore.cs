using System.Globalization;
using Final_Project.Models;

namespace Final_Project.Services;

public class SplitmateStore
{
    private readonly object _gate = new();
    private readonly SplitmateGroup _group = new();
    private readonly List<Expense> _expenses = new();
    private readonly List<Payment> _payments = new();
    private readonly List<GroupNote> _notes = new();
    private readonly List<GroupTask> _tasks = new();
    private readonly string[] _splitMethods = { "Equal", "Exact", "Percentage" };
    private int _nextExpenseId = 1;
    private int _nextPaymentId = 1;
    private int _nextNoteId = 1;
    private int _nextTaskId = 1;

    public SplitmateStore()
    {
        SeedDemoData();
    }

    public SplitmateDashboardViewModel GetDashboard()
    {
        lock (_gate)
        {
            return new SplitmateDashboardViewModel
            {
                Group = _group,
                Expenses = _expenses.ToList(),
                Payments = _payments.ToList(),
                Notes = _notes.ToList(),
                Tasks = _tasks.ToList(),
                Balances = BuildBalances(),
                Settlements = BuildSettlements(),
                SplitMethods = _splitMethods.ToList()
            };
        }
    }

    public void AddExpense(ExpenseFormModel form)
    {
        lock (_gate)
        {
            if (string.IsNullOrWhiteSpace(form.Description))
            {
                throw new InvalidOperationException("Expense description is required.");
            }

            if (form.Amount <= 0)
            {
                throw new InvalidOperationException("Expense amount must be greater than zero.");
            }

            var paidBy = NormalizeMember(form.PaidBy);
            var method = NormalizeSplitMethod(form.SplitMethod);
            var amount = Money(form.Amount);

            _expenses.Insert(0, new Expense
            {
                Id = _nextExpenseId++,
                Description = form.Description.Trim(),
                Amount = amount,
                PaidBy = paidBy,
                SplitMethod = method,
                CreatedOn = DateTime.Now,
                Shares = BuildShares(amount, method, form.ShareInput)
            });
        }
    }

    public void AddPayment(PaymentFormModel form)
    {
        lock (_gate)
        {
            var fromMember = NormalizeMember(form.FromMember);
            var toMember = NormalizeMember(form.ToMember);

            if (fromMember == toMember)
            {
                throw new InvalidOperationException("Choose two different members for a payment.");
            }

            if (form.Amount <= 0)
            {
                throw new InvalidOperationException("Payment amount must be greater than zero.");
            }

            _payments.Insert(0, new Payment
            {
                Id = _nextPaymentId++,
                FromMember = fromMember,
                ToMember = toMember,
                Amount = Money(form.Amount),
                Note = form.Note?.Trim() ?? string.Empty,
                PaidOn = DateTime.Now
            });
        }
    }

    public void AddNote(NoteFormModel form)
    {
        lock (_gate)
        {
            if (string.IsNullOrWhiteSpace(form.Title))
            {
                throw new InvalidOperationException("Note title is required.");
            }

            _notes.Insert(0, new GroupNote
            {
                Id = _nextNoteId++,
                Title = form.Title.Trim(),
                Body = form.Body?.Trim() ?? string.Empty,
                CreatedBy = NormalizeMember(form.CreatedBy),
                IsPinned = form.IsPinned,
                CreatedOn = DateTime.Now
            });
        }
    }

    public void AddTask(TaskFormModel form)
    {
        lock (_gate)
        {
            if (string.IsNullOrWhiteSpace(form.Title))
            {
                throw new InvalidOperationException("Task title is required.");
            }

            _tasks.Insert(0, new GroupTask
            {
                Id = _nextTaskId++,
                Title = form.Title.Trim(),
                AssignedTo = NormalizeMember(form.AssignedTo),
                DueDate = form.DueDate == default ? DateTime.Today.AddDays(3) : form.DueDate,
                IsComplete = false
            });
        }
    }

    public void CompleteTask(int id)
    {
        lock (_gate)
        {
            var task = _tasks.FirstOrDefault(item => item.Id == id);

            if (task is not null)
            {
                task.IsComplete = !task.IsComplete;
            }
        }
    }

    public void ResetDemoData()
    {
        lock (_gate)
        {
            _expenses.Clear();
            _payments.Clear();
            _notes.Clear();
            _tasks.Clear();
            _nextExpenseId = 1;
            _nextPaymentId = 1;
            _nextNoteId = 1;
            _nextTaskId = 1;
            SeedDemoData();
        }
    }

    private void SeedDemoData()
    {
        _group.Id = 1;
        _group.Name = "Splitmate Lakeshore House";
        _group.Description = "A shared living group used for final-project expense splitting, payments, notes, and tasks.";
        _group.CreatedOn = DateTime.Today.AddDays(-35);
        _group.Members = new List<Member>
        {
            new() { Id = 1, Name = "Krishna", Email = "krishna@example.com", Role = "Group owner" },
            new() { Id = 2, Name = "Aanya", Email = "aanya@example.com", Role = "Roommate" },
            new() { Id = 3, Name = "Mateo", Email = "mateo@example.com", Role = "Roommate" },
            new() { Id = 4, Name = "Priya", Email = "priya@example.com", Role = "Roommate" }
        };

        AddExpense(new ExpenseFormModel
        {
            Description = "Weekly groceries",
            Amount = 128.40m,
            PaidBy = "Krishna",
            SplitMethod = "Equal"
        });

        AddExpense(new ExpenseFormModel
        {
            Description = "Internet bill",
            Amount = 72.00m,
            PaidBy = "Aanya",
            SplitMethod = "Percentage",
            ShareInput = "Krishna=25, Aanya=25, Mateo=25, Priya=25"
        });

        AddExpense(new ExpenseFormModel
        {
            Description = "Cleaning supplies",
            Amount = 46.75m,
            PaidBy = "Priya",
            SplitMethod = "Exact",
            ShareInput = "Krishna=12.00, Aanya=12.00, Mateo=10.75, Priya=12.00"
        });

        AddPayment(new PaymentFormModel
        {
            FromMember = "Mateo",
            ToMember = "Krishna",
            Amount = 20.00m,
            Note = "Partial grocery settlement"
        });

        AddNote(new NoteFormModel
        {
            Title = "Receipt rule",
            Body = "Upload or describe the receipt when adding shared expenses.",
            CreatedBy = "Krishna",
            IsPinned = true
        });

        AddTask(new TaskFormModel
        {
            Title = "Confirm hydro bill before Friday",
            AssignedTo = "Aanya",
            DueDate = DateTime.Today.AddDays(2)
        });

        AddTask(new TaskFormModel
        {
            Title = "Buy dish soap",
            AssignedTo = "Mateo",
            DueDate = DateTime.Today.AddDays(5)
        });
    }

    private List<SplitShare> BuildShares(decimal amount, string method, string shareInput)
    {
        return method switch
        {
            "Equal" => BuildEqualShares(amount),
            "Exact" => BuildExactShares(amount, shareInput),
            "Percentage" => BuildPercentageShares(amount, shareInput),
            _ => throw new InvalidOperationException("Unsupported split method.")
        };
    }

    private List<SplitShare> BuildEqualShares(decimal amount)
    {
        var shares = new List<SplitShare>();
        var baseShare = Money(amount / _group.Members.Count);
        var assigned = 0m;

        for (var i = 0; i < _group.Members.Count; i++)
        {
            var share = i == _group.Members.Count - 1 ? Money(amount - assigned) : baseShare;
            assigned += share;
            shares.Add(new SplitShare { MemberName = _group.Members[i].Name, Amount = share });
        }

        return shares;
    }

    private List<SplitShare> BuildExactShares(decimal amount, string shareInput)
    {
        var assignments = ParseAssignments(shareInput);
        var shares = _group.Members
            .Select(member => new SplitShare
            {
                MemberName = member.Name,
                Amount = assignments.TryGetValue(member.Name, out var share) ? Money(share) : 0m
            })
            .ToList();

        var total = shares.Sum(share => share.Amount);
        if (Math.Abs(total - amount) > 0.01m)
        {
            throw new InvalidOperationException($"Exact shares must total ${amount:N2}. Current total is ${total:N2}.");
        }

        return shares;
    }

    private List<SplitShare> BuildPercentageShares(decimal amount, string shareInput)
    {
        var percentages = ParseAssignments(shareInput);
        var percentTotal = percentages.Values.Sum();

        if (Math.Abs(percentTotal - 100m) > 0.01m)
        {
            throw new InvalidOperationException($"Percentage shares must total 100%. Current total is {percentTotal:N2}%.");
        }

        var assignedMembers = _group.Members.Where(member => percentages.ContainsKey(member.Name)).ToList();
        var shares = _group.Members.Select(member => new SplitShare { MemberName = member.Name, Amount = 0m }).ToList();
        var assignedAmount = 0m;

        for (var i = 0; i < assignedMembers.Count; i++)
        {
            var member = assignedMembers[i];
            var share = i == assignedMembers.Count - 1
                ? Money(amount - assignedAmount)
                : Money(amount * percentages[member.Name] / 100m);

            assignedAmount += share;
            shares.First(item => item.MemberName == member.Name).Amount = share;
        }

        return shares;
    }

    private Dictionary<string, decimal> ParseAssignments(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new InvalidOperationException("Enter member shares such as Krishna=25, Aanya=25.");
        }

        var assignments = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
        var tokens = input.Split(new[] { ',', ';', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var token in tokens)
        {
            var parts = token.Split(new[] { '=', ':' }, 2, StringSplitOptions.TrimEntries);

            if (parts.Length != 2)
            {
                throw new InvalidOperationException("Use the format Member=Value for exact and percentage splits.");
            }

            var memberName = NormalizeMember(parts[0]);

            if (!decimal.TryParse(parts[1], NumberStyles.Number, CultureInfo.InvariantCulture, out var value) || value < 0)
            {
                throw new InvalidOperationException($"'{parts[1]}' is not a valid share value.");
            }

            assignments[memberName] = value;
        }

        return assignments;
    }

    private List<MemberBalance> BuildBalances()
    {
        return _group.Members
            .Select(member =>
            {
                var paid = _expenses.Where(expense => expense.PaidBy == member.Name).Sum(expense => expense.Amount);
                var share = _expenses.SelectMany(expense => expense.Shares).Where(split => split.MemberName == member.Name).Sum(split => split.Amount);
                var paymentImpact = _payments.Where(payment => payment.FromMember == member.Name).Sum(payment => payment.Amount)
                    - _payments.Where(payment => payment.ToMember == member.Name).Sum(payment => payment.Amount);

                return new MemberBalance
                {
                    MemberName = member.Name,
                    TotalPaid = paid,
                    TotalShare = share,
                    PaymentImpact = paymentImpact,
                    NetBalance = Money(paid - share + paymentImpact)
                };
            })
            .OrderBy(balance => balance.MemberName)
            .ToList();
    }

    private List<SettlementSuggestion> BuildSettlements()
    {
        var balances = BuildBalances();
        var debtors = balances
            .Where(balance => balance.NetBalance < -0.01m)
            .Select(balance => new MemberBalance { MemberName = balance.MemberName, NetBalance = Math.Abs(balance.NetBalance) })
            .OrderByDescending(balance => balance.NetBalance)
            .ToList();
        var creditors = balances
            .Where(balance => balance.NetBalance > 0.01m)
            .Select(balance => new MemberBalance { MemberName = balance.MemberName, NetBalance = balance.NetBalance })
            .OrderByDescending(balance => balance.NetBalance)
            .ToList();
        var settlements = new List<SettlementSuggestion>();
        var debtorIndex = 0;
        var creditorIndex = 0;

        while (debtorIndex < debtors.Count && creditorIndex < creditors.Count)
        {
            var amount = Money(Math.Min(debtors[debtorIndex].NetBalance, creditors[creditorIndex].NetBalance));

            if (amount > 0)
            {
                settlements.Add(new SettlementSuggestion
                {
                    FromMember = debtors[debtorIndex].MemberName,
                    ToMember = creditors[creditorIndex].MemberName,
                    Amount = amount
                });
            }

            debtors[debtorIndex].NetBalance = Money(debtors[debtorIndex].NetBalance - amount);
            creditors[creditorIndex].NetBalance = Money(creditors[creditorIndex].NetBalance - amount);

            if (debtors[debtorIndex].NetBalance <= 0.01m)
            {
                debtorIndex++;
            }

            if (creditors[creditorIndex].NetBalance <= 0.01m)
            {
                creditorIndex++;
            }
        }

        return settlements;
    }

    private string NormalizeMember(string value)
    {
        var member = _group.Members.FirstOrDefault(item =>
            string.Equals(item.Name, value?.Trim(), StringComparison.OrdinalIgnoreCase)
            || string.Equals(item.Email, value?.Trim(), StringComparison.OrdinalIgnoreCase));

        if (member is null)
        {
            throw new InvalidOperationException($"'{value}' is not a member of this group.");
        }

        return member.Name;
    }

    private string NormalizeSplitMethod(string value)
    {
        var method = _splitMethods.FirstOrDefault(item => string.Equals(item, value?.Trim(), StringComparison.OrdinalIgnoreCase));

        if (method is null)
        {
            throw new InvalidOperationException("Choose Equal, Exact, or Percentage split.");
        }

        return method;
    }

    private static decimal Money(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);
}
