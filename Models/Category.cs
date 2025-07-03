namespace Personal_Finance_Manager.Models
{
    public class Category
    {
        public string Name { get; set; }
        public TransactionType Type { get; set; } // Income or Expense

        public Category(string name, TransactionType type)
        {
            Name = name;
            Type = type;
        }
    }

    public enum TransactionType
    {
        Income,
        Expense
    }
}
