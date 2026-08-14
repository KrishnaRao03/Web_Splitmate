using System.Diagnostics;
using Final_Project.Models;
using Final_Project.Services;
using Microsoft.AspNetCore.Mvc;

namespace Final_Project.Controllers;

public class HomeController : Controller
{
    private readonly SplitmateStore _store;

    public HomeController(SplitmateStore store)
    {
        _store = store;
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

    public IActionResult NotesTasks()
    {
        return View(_store.GetDashboard());
    }

    public IActionResult Groups()
    {
        return View(_store.GetDashboard());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateGroup(CreateGroupFormModel form)
    {
        TrySave(() => _store.CreateGroup(form), "Group created and selected.");
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
        return RedirectToAction(nameof(NotesTasks));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddTask(TaskFormModel form)
    {
        TrySave(() => _store.AddTask(form), "Task added to the group board.");
        return RedirectToAction(nameof(NotesTasks));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CompleteTask(int id)
    {
        _store.CompleteTask(id);
        TempData["StatusMessage"] = "Task status updated.";
        return RedirectToAction(nameof(NotesTasks));
    }

    [HttpPost]
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
