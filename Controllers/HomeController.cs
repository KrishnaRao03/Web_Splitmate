using System.Diagnostics;
using System.Security.Claims;
using Final_Project.Models;
using Final_Project.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Final_Project.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly SplitmateStore _store;
    private readonly UserStore _users;

    public HomeController(SplitmateStore store, UserStore users)
    {
        _store = store;
        _users = users;
    }

    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction(nameof(Index));
        }

        ViewBag.DemoUsers = _users.DemoUsers;
        return View(new LoginFormModel { ReturnUrl = string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginFormModel form)
    {
        ViewBag.DemoUsers = _users.DemoUsers;

        var user = _users.Validate(form.Email, form.Password);
        if (user is null)
        {
            TempData["ErrorMessage"] = "Invalid email or password.";
            return View(form);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role)
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties { IsPersistent = form.RememberMe });

        if (!string.IsNullOrWhiteSpace(form.ReturnUrl) && Url.IsLocalUrl(form.ReturnUrl))
        {
            return Redirect(form.ReturnUrl);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }

    public IActionResult AccessDenied()
    {
        return View();
    }

    public IActionResult Index()
    {
        return View(_store.GetDashboard());
    }

    public IActionResult Expenses()
    {
        return View(_store.GetDashboard());
    }

    public IActionResult Balances()
    {
        return View(_store.GetDashboard());
    }

    public IActionResult Notes()
    {
        return View(_store.GetDashboard());
    }

    public IActionResult Tasks()
    {
        return View(_store.GetDashboard());
    }

    public IActionResult Groups()
    {
        return View(_store.GetDashboard());
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public IActionResult CreateGroup(CreateGroupFormModel form)
    {
        TrySave(() => _store.CreateGroup(form), "Group created and selected.");
        return RedirectToAction(nameof(Groups));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public IActionResult EditGroup(EditGroupFormModel form)
    {
        TrySave(() => _store.UpdateGroup(form), "Group details updated.");
        return RedirectToAction(nameof(Groups));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteGroup(int id)
    {
        TrySave(() => _store.DeleteGroup(id), "Group deleted.");
        return RedirectToAction(nameof(Groups));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public IActionResult AddMember(AddMemberFormModel form)
    {
        TrySave(() => _store.AddMember(form), "Member added to the group.");
        return RedirectToAction(nameof(Groups));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteMember(int groupId, int memberId)
    {
        TrySave(() => _store.DeleteMember(groupId, memberId), "Member deleted from the group.");
        return RedirectToAction(nameof(Groups));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SelectGroup(int id)
    {
        TrySave(() => _store.SelectGroup(id), "Active group changed.");
        return RedirectToAction(nameof(Groups));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddExpense(ExpenseFormModel form)
    {
        TrySave(() => _store.AddExpense(form), "Expense added and balances recalculated.");
        return RedirectToAction(nameof(Expenses));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RecordPayment(PaymentFormModel form)
    {
        TrySave(() => _store.AddPayment(form), "Payment recorded in the ledger.");
        return RedirectToAction(nameof(Balances));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddNote(NoteFormModel form)
    {
        TrySave(() => _store.AddNote(form), "Group note added.");
        return RedirectToAction(nameof(Notes));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddTask(TaskFormModel form)
    {
        TrySave(() => _store.AddTask(form), "Task added to the group board.");
        return RedirectToAction(nameof(Tasks));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CompleteTask(int id)
    {
        _store.CompleteTask(id);
        TempData["StatusMessage"] = "Task status updated.";
        return RedirectToAction(nameof(Tasks));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public IActionResult ResetDemo()
    {
        _store.ResetDemoData();
        TempData["StatusMessage"] = "Demo data has been reset.";
        return RedirectToAction(nameof(Index));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private void TrySave(Action action, string successMessage)
    {
        try
        {
            action();
            TempData["StatusMessage"] = successMessage;
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
    }
}
