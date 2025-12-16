using ET.Application.Contracts;
using ET.Application.DTOs;
using ET.Domain.Entities;
using ET.Infrastructure.Persistance.Context;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace ET.Infrastructure.Services;

public class TransactionService : ITransactionService
{
    private readonly ExpenseTrackerContext _dbcontext;
    public TransactionService(ExpenseTrackerContext dbcontext)
    {
        _dbcontext = dbcontext;
    }

    public async Task<bool> AddTransaction(TransactionModel transactionModel)
    {
        try
        {
            Transaction transactions = new()
            {
                CategoryId = transactionModel.CategoryId,
                Amount = transactionModel.Amount,
                TransactionDate = transactionModel.TransactionDate,
                CreatedAt = transactionModel.CreatedAt,
                Description = transactionModel.Description,
                Type = transactionModel.Type,
                Title = transactionModel.Title,
                UserId = transactionModel.UserId,
                AccountId = transactionModel.AccountTypeId
            };

            await _dbcontext.AddAsync(transactions);
            await _dbcontext.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<int> GetTotalTransactionCount()
    {
        return await _dbcontext.Transactions.CountAsync();
    }

    public async Task<List<TransactionModel>> GetTransaction(TransactionFilterModel transactionFilterModel)
    {
        DateTime startDate;
        DateTime endDate; 

        var query = _dbcontext.Transactions.AsQueryable();

        if (!string.IsNullOrEmpty(transactionFilterModel.Type))
            query = query.Where(t => t.Type == transactionFilterModel.Type);

        if (!string.IsNullOrEmpty(transactionFilterModel.StartDate) && DateTime.TryParse(transactionFilterModel.StartDate, out startDate))
            query = query.Where(t => t.TransactionDate >= startDate);

        if (!string.IsNullOrEmpty(transactionFilterModel.EndDate) && DateTime.TryParse(transactionFilterModel.EndDate, out endDate))
            query = query.Where(t => t.TransactionDate <= endDate);

        if (!string.IsNullOrWhiteSpace(transactionFilterModel.Filters.Title))
        {
            string titleFilter = transactionFilterModel.Filters.Title;
            query = query.Where(t => t.Title != null && t.Title.Contains(titleFilter));
        }

        if (!string.IsNullOrWhiteSpace(transactionFilterModel.Filters.Description))
        {
            string descFilter = transactionFilterModel.Filters.Description;
            query = query.Where(t => t.Description != null && t.Description.Contains(descFilter));
        }

        if (!string.IsNullOrWhiteSpace(transactionFilterModel.Filters.CategoryName))
        {
            string categoryFilter = transactionFilterModel.Filters.CategoryName;
            query = query.Where(t => t.Category != null && t.Category.Name != null && t.Category.Name.Contains(categoryFilter));
        }

        if (transactionFilterModel.Filters.Amount > 0)
        {
            int amountFilter = transactionFilterModel.Filters.Amount;
            query = query.Where(t => t.Amount == amountFilter);
        }

        int pageNumber = transactionFilterModel.PageNumber <= 0 ? 1 : transactionFilterModel.PageNumber;
        int pageSize = transactionFilterModel.PageSize <= 0 ? 10 : transactionFilterModel.PageSize;

        if (!string.IsNullOrEmpty(transactionFilterModel.SortbyColumn))
        {
            bool isAscending = string.Equals(transactionFilterModel.SortbyOrder, "asc", StringComparison.OrdinalIgnoreCase);

            query = transactionFilterModel.SortbyColumn.ToLower() switch
            {
                "title" => isAscending ? query.OrderBy(t => t.Title) : query.OrderByDescending(t => t.Title),
                "description" => isAscending ? query.OrderBy(t => t.Description) : query.OrderByDescending(t => t.Description),
                "categoryname" => isAscending ? query.OrderBy(t => t.Category.Name) : query.OrderByDescending(t => t.Category.Name),
                "transactiondate" => isAscending ? query.OrderBy(t => t.TransactionDate) : query.OrderByDescending(t => t.TransactionDate),
                "amount" => isAscending ? query.OrderBy(t => t.Amount) : query.OrderByDescending(t => t.Amount),
                _ => query.OrderByDescending(t => t.TransactionDate),
            };
        }
        else
        {
            query = query.OrderByDescending(t => t.TransactionDate);
        }

        query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

        List<TransactionModel> transactions = await query.Select(t => new TransactionModel
        {
            TransactionId = t.TransactionId,
            Amount = t.Amount,
            CategoryId = t.CategoryId,
            Description = t.Description,
            CreatedAt = t.CreatedAt,
            AccountTypeId = t.AccountId.HasValue ? (int)t.AccountId : 0,
            TransactionDate = t.TransactionDate,
            Type = t.Type,
            Title = t.Title,
            CategoryName = t.Category.Name
        }).AsNoTracking().ToListAsync();
        return transactions;
    }

    public async Task<TransactionModel?> GetTransactionById(int id)
    {
        return await _dbcontext.Transactions.AsNoTracking().Where(t => t.TransactionId == id)
            .Select(t => new TransactionModel
            {
                TransactionId = t.TransactionId,
                Amount = t.Amount,
                CategoryId = t.CategoryId,
                Description = t.Description,
                AccountTypeId = (int)t.AccountId,
                CreatedAt = t.CreatedAt,
                TransactionDate = t.TransactionDate,
                Type = t.Type,
                Title = t.Title,
                CategoryName = t.Category.Name
            }).FirstOrDefaultAsync();
    }

    public async Task<bool> UpdateTransaction(UpdateTransactionModel transactionModel)
    {
        try
        {
            var transaction = await _dbcontext.Transactions.Where(x => transactionModel.TransactionId == x.TransactionId).FirstOrDefaultAsync();
            if (transaction == null) return false;

            transaction.Amount = transactionModel.Amount;
            transaction.TransactionDate = transactionModel.TransactionDate;
            transaction.Description = transactionModel.Description;
            transaction.Title = transactionModel.Title;
            transaction.AccountId = transactionModel.AccountId;
            _dbcontext.Update(transaction);
            await _dbcontext.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<bool> DeleteTransaction(int id)
    {
        try
        {
            var transaction = await _dbcontext.Transactions.Where(x => id == x.TransactionId).FirstOrDefaultAsync();
            if (transaction == null) return false;

            _dbcontext.Remove(transaction);
            await _dbcontext.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<List<Category>> GetCategories()
    {
        return await _dbcontext.Categories.AsNoTracking().ToListAsync();
    }

    public async Task<List<Account>> GetAccountType()
    {
        return await _dbcontext.Accounts.AsNoTracking().ToListAsync();
    }

    public async Task<byte[]> ExportTransactionsToCsv(TransactionFilterModel filter)
    {
        filter.PageNumber = 1;
        filter.PageSize = int.MaxValue;

        var transactions = await GetTransaction(filter);

        if (transactions == null || transactions.Count == 0)
            return Array.Empty<byte>();

        var csv = new StringBuilder();
        csv.AppendLine("Title,Description,Category,Date,Type,Amount");

        foreach (var t in transactions)
        {
            csv.AppendLine(
                $"{t.Title},{t.Description},{t.CategoryName},{t.TransactionDate:yyyy-MM-dd},{t.Type},{t.Amount}"
            );
        }

        return Encoding.UTF8.GetBytes(csv.ToString());
    }

}
