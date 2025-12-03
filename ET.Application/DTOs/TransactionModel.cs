namespace ET.Application.DTOs;

public class TransactionModel
{
    public int? TransactionId { get; set; }
    public int CategoryId { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; } = null!;
    public string? Description { get; set; }
    public string? Title { get; set; }
    public int UserId { get; set; }
    public DateTime TransactionDate { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? CategoryName { get; set; }
}

public class UpdateTransactionModel
{
    public int? TransactionId { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; } = null!;
    public string? Description { get; set; }
    public string? Title { get; set; }
    public DateTime TransactionDate { get; set; }
}

public class FilterColumns
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public int Amount { get; set; }
}

public class TransactionFilterModel
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Type { get; set; }
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
    public string? SortbyColumn { get; set; } = "date";
    public string? SortbyOrder { get; set; } = "asc";
    public FilterColumns Filters { get; set; } = new();
}

public class CategoryModel
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; }
}
