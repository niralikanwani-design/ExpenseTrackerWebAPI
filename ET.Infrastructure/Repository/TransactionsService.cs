using ET.Domain.DTO;
using ET.Domain.IRepository;
using ET.Domain.Models;
using ET.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace ET.Infrastructure.Repository
{
    public class TransactionsService : ITransactionsService
    {
        private readonly ExpenseTrackerContext _dbcontext;
        public TransactionsService(ExpenseTrackerContext dbcontext)
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
                    UserId = 1
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

            if (!string.IsNullOrWhiteSpace(transactionFilterModel.Filters.CategoryId))
            {
                string categoryFilter = transactionFilterModel.Filters.CategoryId;
                query = query.Where(t => t.CategoryId.ToString().Contains(categoryFilter));
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
                    "categoryId" => isAscending ? query.OrderBy(t => t.CategoryId) : query.OrderByDescending(t => t.CategoryId),
                    "date" => isAscending ? query.OrderBy(t => t.TransactionDate) : query.OrderByDescending(t => t.TransactionDate),
                    _ => query.OrderByDescending(t => t.TransactionDate),
                };
            }
            else
            {
                query = query.OrderByDescending(t => t.TransactionDate);
            }

            query = query.OrderBy(t => t.TransactionId).Skip((pageNumber - 1) * pageSize).Take(pageSize);

            List<TransactionModel> transactions = await query.Select(t => new TransactionModel
            {
                TransactionId = t.TransactionId,
                Amount = t.Amount,
                CategoryId = t.CategoryId,
                Description = t.Description,
                CreatedAt = t.CreatedAt,
                TransactionDate = t.TransactionDate,
                Type = t.Type,
                Title = t.Title
            }).AsNoTracking().ToListAsync();
            return transactions;
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

    }
}
