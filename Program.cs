using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Personal_Finance_Manager.Models;
using Personal_Finance_Manager.Services;

namespace Personal_Finance_Manager
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var dataService = new FileDataService();
            List<User> users = dataService.LoadUsers();
            var financeService = new FinanceManagerService(users);
            financeService.ExpenseLimitExceeded += (user, totalExpense) =>
            {
                Console.WriteLine($"\n🚨 ALERT: You have exceeded your monthly expense limit\a!");
                Console.WriteLine($"Your total expense this month is {totalExpense} BDT.");
            };

            User currentUser = null;

            while (true)
            {
                Console.WriteLine("==== Personal Finance Manager ====");
                Console.WriteLine("1. Register");
                Console.WriteLine("2. Login");
                Console.WriteLine("3. Exit");

                Console.Write("Choose option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Write("Enter username: ");
                        string newUsername = Console.ReadLine();

                        Console.Write("Enter password: ");
                        string newPassword = Console.ReadLine();

                        bool registered = financeService.RegisterUser(newUsername, newPassword);

                        if (registered)
                            Console.WriteLine("✅ Registration successful!");
                        else
                            Console.WriteLine("❌ Username already exists.");
                        break;

                    case "2":
                        Console.Write("Enter username: ");
                        string loginUsername = Console.ReadLine();

                        Console.Write("Enter password: ");
                        string loginPassword = Console.ReadLine();

                        currentUser = financeService.LoginUser(loginUsername, loginPassword);

                        if (currentUser != null)
                        {
                            bool inSession = true;
                            while (inSession)
                            {
                                Console.WriteLine($"\n=== Welcome, {currentUser.Username} ===");
                                Console.WriteLine("1. Add Transaction");
                                Console.WriteLine("2. View Transactions");
                                Console.WriteLine("3. Check Balance");
                                Console.WriteLine("4. Generate Monthly Report");
                                Console.WriteLine("5. Export Monthly Report to CSV");
                                Console.WriteLine("6. Logout");

                                Console.Write("Choose option: ");
                                string userChoice = Console.ReadLine();

                                switch (userChoice)
                                {
                                    case "1":
                                        Console.Write("Enter amount: ");
                                        decimal amount = decimal.Parse(Console.ReadLine());

                                        Console.Write("Enter description: ");
                                        string desc = Console.ReadLine();

                                        Console.Write("Enter category name (e.g. Food, Salary): ");
                                        string categoryName = Console.ReadLine();

                                        Console.Write("Type (1 = Income, 2 = Expense): ");
                                        string typeInput = Console.ReadLine();
                                        TransactionType type = typeInput == "1" ? TransactionType.Income : TransactionType.Expense;

                                        financeService.AddTransaction(currentUser, amount, desc, categoryName, type);
                                        financeService.CheckMonthlyExpenses(currentUser, DateTime.Now.Month, DateTime.Now.Year);
                                        break;

                                    case "2":
                                        financeService.ViewTransactions(currentUser);
                                        break;

                                    case "3":
                                        decimal balance = financeService.GetBalance(currentUser);
                                        Console.WriteLine($"💰 Your current balance is: {balance} BDT");
                                        break;

                                    case "4":
                                        Console.Write("Enter month (1-12): ");
                                        int month = int.Parse(Console.ReadLine());

                                        Console.Write("Enter year (e.g. 2025): ");
                                        int year = int.Parse(Console.ReadLine());

                                        financeService.GenerateMonthlyReport(currentUser, month, year);
                                        break;

                                    case "5":
                                        Console.Write("Enter month (1-12): ");
                                        int m = int.Parse(Console.ReadLine());

                                        Console.Write("Enter year (e.g. 2025): ");
                                        int y = int.Parse(Console.ReadLine());

                                        financeService.ExportMonthlyReportToCsv(currentUser, m, y);
                                        break;

                                    case "6":
                                        Console.WriteLine("👋 Logged out!");
                                        inSession = false;
                                        currentUser = null;
                                        break;

                                    default:
                                        Console.WriteLine("Invalid choice.");
                                        break;
                                }
                            }

                        }
                        else
                        {
                            Console.WriteLine("❌ Invalid username or password.");
                        }
                        break;

                    case "3":
                        Console.WriteLine("Goodbye!");
                        foreach (var user in users)
                        {
                           await financeService.ExportVisualReportAsync(user);
                        }
                        dataService.SaveUsers(users);
                        return;

                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }
    }
}
