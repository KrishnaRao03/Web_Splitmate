# Splitmate Final Project

This is an ASP.NET Core MVC final project based on the feature set from the original Python/Flask Splitmate app. It focuses on the functions that are easiest to demonstrate for a Web Programming submission: group expense splitting, balances, payment settlement, group notes, and task tracking.

## Main Features

- Dashboard with total shared spend, group count, open tasks, outstanding balances, and recent activity
- Create groups with custom members and switch the active group
- Add expenses using Equal, Exact, or Percentage split logic
- Automatic member share validation and balance recalculation
- Balance table showing amount paid, amount owed, payment impact, and net result
- Settlement suggestions that show who should pay whom
- Manual payment ledger
- Group notes with pinned-note support
- Group task board with due dates and completion status

## Technology Used

- C#
- ASP.NET Core MVC
- Razor views
- Singleton in-memory service for demo data
- HTML, CSS, JavaScript
- Bootstrap assets from the MVC template

## Project Structure

```text
Final_Project/
  Controllers/
    HomeController.cs
  Models/
    SplitmateModels.cs
  Services/
    SplitmateStore.cs
  Views/
    Home/
      Index.cshtml
      Groups.cshtml
      Expenses.cshtml
      Balances.cshtml
      NotesTasks.cshtml
  wwwroot/
    css/site.css
    js/site.js
    images/
  screenshots/
```

## How to Run

```bash
cd "C:\Users\krish\Desktop\Krishna\Study\Sem6\Web Programming\Final_Project"
dotnet restore
dotnet run
```

Open the local URL shown in the terminal. On this machine the launch profile uses:

```text
http://localhost:5129
```

## Suggested Video Demo

Keep the video between 8 and 10 minutes.

1. Introduce the project and explain that it ports Splitmate features into ASP.NET Core MVC.
2. Show the Dashboard and describe the summary cards.
3. Open the code and explain Models, Services, Controllers, and Views.
4. Run the app.
5. Create a new group from the Groups page and show that it becomes active.
6. Add an expense with Equal split.
7. Add an expense with Exact or Percentage split.
8. Open Balances and show recalculated balances plus settlement suggestions.
9. Record a payment and show the payment ledger.
10. Add a note, add a task, and complete a task.

## Screenshot Checklist

Save screenshots in the `screenshots` folder before uploading them to Blackboard.

- Dashboard summary
- Groups page with create-group form and saved group cards
- Expense form and expense ledger
- Balance table and settlement suggestions
- Payment ledger after recording a payment
- Notes and tasks board
- GitHub repository page

## GitHub Upload

Create a GitHub repository, then run:

```bash
git init
git add .
git commit -m "Add Splitmate final project"
git branch -M main
git remote add origin https://github.com/YOUR-USERNAME/splitmate-final-project.git
git push -u origin main
```

Submit the GitHub repository link and upload the screenshots to Blackboard.
