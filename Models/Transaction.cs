using Personal_Finance_Manager.Models;
using System;

namespace Personal_Finance_Manager.Models
{
    public class Transaction
    {
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public Category Category { get; set; }
        public string Description { get; set; }

        public Transaction(decimal amount, DateTime date, Category category, string description)
        {
            Amount = amount;
            Date = date;
            Category = category;
            Description = description;
        }
    }
}
