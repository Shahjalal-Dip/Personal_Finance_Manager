using Personal_Finance_Manager.Models;
using Personal_Finance_Manager.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personal_Finance_Manager.Services
{
    public delegate void ExpenseLimitExceededHandler(User user, decimal totalExpense);
    public class FinanceManagerService
    {
        private readonly List<User> _users;

        public FinanceManagerService(List<User> users)
        {
            _users = users;
        }
        public void AddTransaction(User user, decimal amount, string description, string categoryName, TransactionType type)
        {
            var category = new Category(categoryName, type);
            var transaction = new Transaction(amount, DateTime.Now, category, description);
            user.Transactions.Add(transaction);

            Console.WriteLine("✅ Transaction added successfully!");
        }
        public decimal GetBalance(User user)
        {
            var income = user.Transactions
                .Where(t => t.Category.Type == TransactionType.Income)
                .Sum(t => t.Amount);

            var expense = user.Transactions
                .Where(t => t.Category.Type == TransactionType.Expense)
                .Sum(t => t.Amount);

            return income - expense;
        }
        public void ViewTransactions(User user)
        {
            if (user.Transactions.Count == 0)
            {
                Console.WriteLine("No transactions found.");
                return;
            }

            Console.WriteLine("==== Your Transactions ====");
            foreach (var t in user.Transactions)
            {
                Console.WriteLine($"{t.Date:yyyy-MM-dd} | {t.Category.Type} | {t.Category.Name} | {t.Amount} BDT | {t.Description}");
            }
        }
        public void GenerateMonthlyReport(User user, int month, int year)
        {
            var transactions = user.Transactions
                .Where(t => t.Date.Month == month && t.Date.Year == year)
                .ToList();

            if (transactions.Count == 0)
            {
                Console.WriteLine("No transactions found for that period.");
                return;
            }

            decimal totalIncome = transactions
                .Where(t => t.Category.Type == TransactionType.Income)
                .Sum(t => t.Amount);

            decimal totalExpense = transactions
                .Where(t => t.Category.Type == TransactionType.Expense)
                .Sum(t => t.Amount);

            Console.WriteLine($"\n==== Report for {month}/{year} ====");
            Console.WriteLine($"Total Income: {totalIncome} BDT");
            Console.WriteLine($"Total Expense: {totalExpense} BDT");
            Console.WriteLine($"Net Balance: {totalIncome - totalExpense} BDT");

            var expenseByCategory = transactions
                .Where(t => t.Category.Type == TransactionType.Expense)
                .GroupBy(t => t.Category.Name)
                .Select(g => new
                {
                    Category = g.Key,
                    Total = g.Sum(t => t.Amount)
                })
                .OrderByDescending(x => x.Total);

            Console.WriteLine("\nExpenses by Category:");
            foreach (var item in expenseByCategory)
            {
                Console.WriteLine($"{item.Category} : {item.Total} BDT");
            }
        }

        public event ExpenseLimitExceededHandler ExpenseLimitExceeded;

        public decimal MonthlyExpenseLimit { get; set; } = 10000m; // Example limit

        public void CheckMonthlyExpenses(User user, int month, int year)
        {
            var totalExpense = user.Transactions
                .Where(t => t.Date.Month == month && t.Date.Year == year && t.Category.Type == TransactionType.Expense)
                .Sum(t => t.Amount);

            if (totalExpense > MonthlyExpenseLimit)
            {
                ExpenseLimitExceeded?.Invoke(user, totalExpense);
            }
        }
        public void ExportMonthlyReportToCsv(User user, int month, int year)
        {
            var transactions = user.Transactions
                .Where(t => t.Date.Month == month && t.Date.Year == year)
                .ToList();

            if (transactions.Count == 0)
            {
                Console.WriteLine("No transactions found for that period.");
                return;
            }

            StringBuilder csv = new StringBuilder();
            csv.AppendLine("Date,Type,Category,Amount,Description");

            foreach (var t in transactions)
            {
                csv.AppendLine($"{t.Date:yyyy-MM-dd},{t.Category.Type},{t.Category.Name},{t.Amount},{t.Description}");
            }

            string fileName = $"Report_{user.Username}_{month}_{year}.csv";
            File.WriteAllText(fileName, csv.ToString());
            Console.WriteLine($"✅ Report exported to file: {fileName}");
        }
        public  async Task ExportVisualReportAsync(User user)
        {
            var sb = new StringBuilder();

            sb.AppendLine("===== Personal Finance Report =====");
            sb.AppendLine();
            sb.AppendLine($"User: {user.Username}");
            sb.AppendLine($"Date: {DateTime.Now:yyyy-MM-dd}");
            sb.AppendLine();
            sb.AppendLine("Transactions:");
            sb.AppendLine("--------------------------------------------");
            sb.AppendLine($"{"Date",-12} {"Type",-10} {"Category",-12} {"Amount",-10} Description");

            foreach (var t in user.Transactions)
            {
                sb.AppendLine($"{t.Date:yyyy-MM-dd} {t.Category.Type,-10} {t.Category.Name,-12} {t.Amount,-10} {t.Description}");
            }

            sb.AppendLine("--------------------------------------------");

            decimal income = user.Transactions
                .Where(t => t.Category.Type == TransactionType.Income)
                .Sum(t => t.Amount);

            decimal expense = user.Transactions
                .Where(t => t.Category.Type == TransactionType.Expense)
                .Sum(t => t.Amount);

            decimal balance = income - expense;

            sb.AppendLine($"Total Income: {income} BDT");
            sb.AppendLine($"Total Expense: {expense} BDT");
            sb.AppendLine($"Balance: {balance} BDT");

            string fileName = $"VisualReport_{user.Username}.txt";
            await File.WriteAllTextAsync(fileName, sb.ToString());
            Console.WriteLine($"✅ Visual report saved to {fileName}");
        }

        public bool RegisterUser(string username, string password)
        {
            // Check if user exists
            if (_users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }

            string hashed = SecurityHelper.HashPassword(password);
            var newUser = new User(username, hashed);
            _users.Add(newUser);
            return true;
        }

        public User LoginUser(string username, string password)
        {
            var user = _users.FirstOrDefault(u =>
                u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

            if (user == null)
                return null;

            string hashedInput = SecurityHelper.HashPassword(password);

            if (user.Password == hashedInput)
                return user;
            else
                return null;
        }
    }
}
