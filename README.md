# Splitmate Final Project

This is an ASP.NET Core MVC final project based on the feature set from the original Python/Flask Splitmate app. It focuses on the functions that are easiest to demonstrate for a Web Programming submission: group expense splitting, balances, payment settlement, group notes, and task tracking.

## Main Features

- Login with admin and member demo accounts
- Dashboard with total shared spend, group count, open tasks, outstanding balances, and recent activity
- Admin-only group create, edit, delete, and reset controls
- Admin-only member add, edit, and delete controls inside each group
- Member access for viewing and switching active groups
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
- Cookie authentication
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
      Login.cshtml
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
2. Show the Login page and explain the admin/member demo accounts.
3. Log in as admin and show Dashboard summary cards.
4. Open the code and explain Models, Services, Controllers, Views, and cookie authentication.
5. Create a new group from the Groups page and show that it becomes active.
6. Add, edit, and delete a group member as admin.
7. Edit and delete a group as admin.
8. Log out, log in as member, and show that group and member management controls are hidden.
9. Add an expense with Equal split, then add Exact or Percentage split.
10. Open Balances and show recalculated balances plus settlement suggestions.
11. Record a payment, add a note, add a task, and complete a task.

## Demo Login Accounts

```text
Admin:  admin@splitmate.com  / admin123
Member: member@splitmate.com / member123
```

## Screenshot Checklist

Save screenshots in the `screenshots` folder before uploading them to Blackboard.

- Login page with demo accounts
- Dashboard summary
- Groups page as admin with group and member management controls
- Groups page as member with read-only group management
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
