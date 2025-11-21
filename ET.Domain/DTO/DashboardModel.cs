namespace ET.Domain.DTO
{
    public class DashboardModel
    {
        public DashboardItem Summary { get; set; }
        public List<CategoryExpenseItem> CategoryExpenses { get; set; }
        public List<MonthlyTrendItem> MonthlyTrend { get; set; }
    }

    public class DashboardItem
    {
        public decimal TotalExpenses { get; set; }
        public int NumberofExpenses { get; set; }
        public decimal AverageExpense { get; set; }
        public int ExpenseCategoriesUsed { get; set; }
        public decimal Last30DaysTotal { get; set; }
        public decimal Last30DaysAverage { get; set; }
    }

    public class CategoryExpenseItem
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class MonthlyTrendItem
    {
        public string Month { get; set; }
        public decimal TotalAmount { get; set; }
    }

}
