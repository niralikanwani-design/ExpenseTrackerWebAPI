using ET.Domain.DTO;
using ET.Domain.IRepository;
using ET.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET.Infrastructure.Repository
{
    public class Transactions : ITransactions
    {
        private readonly ExpensTrackerContext _dbcontext;
        public Transactions(ExpensTrackerContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<bool> AddTransaction(TransactionModel transactionModel)
        {
            try
            {
                Transaction transactions = new Transaction();
                transactions.CategoryId = transactionModel.CategoryId;
                transactions.Amount = transactionModel.Amount;
                transactions.TransactionDate = transactionModel.TransactionDate;
                transactions.CreatedAt = transactionModel.CreatedAt;
                transactions.Description = transactionModel.Description;
                transactions.UserId = transactions.UserId;
                transactions.Type = transactionModel.Type;
                transactions.UserId = 1;
                _dbcontext.AddAsync(transactions);
                await _dbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        public async Task<List<TransactionModel>> GetTransaction(TransactionFilterModel transactionFilterModel)
        {
            var query = _dbcontext.Transactions.AsQueryable();

            if (!string.IsNullOrEmpty(transactionFilterModel.Type))
                query = query.Where(t => t.Type == transactionFilterModel.Type);

            if (transactionFilterModel.StartDate.HasValue)
                query = query.Where(t => t.TransactionDate >= transactionFilterModel.StartDate.Value);

            if (transactionFilterModel.EndDate.HasValue)
                query = query.Where(t => t.TransactionDate <= transactionFilterModel.EndDate.Value);

            var totalCount = await query.CountAsync();

            int pageNumber = transactionFilterModel.PageNumber <= 0 ? 1 : transactionFilterModel.PageNumber;
            int pageSize = transactionFilterModel.PageSize <= 0 ? 10 : transactionFilterModel.PageSize;

            query = query.OrderBy(t => t.TransactionId).Skip((pageNumber - 1) * pageSize).Take(pageSize);

            List<TransactionModel> transactions = await query.Select(t => new TransactionModel
            {
                Amount = t.Amount,
                CategoryId = t.CategoryId,
                Description = t.Description,
                CreatedAt = t.CreatedAt,
                TransactionDate = t.TransactionDate,
                Type = t.Type,
            }).AsNoTracking().ToListAsync();
            return transactions;
        }

        public async Task<bool> UpdateTransaction(UpdateTransactionModel transactionModel)
        {
            try
            {
                var transaction = await _dbcontext.Transactions.Where(x => transactionModel.TransactionId == x.TransactionId).FirstOrDefaultAsync();
                transaction.Amount = transactionModel.Amount;
                transaction.TransactionDate = transactionModel.TransactionDate;
                transaction.Description = transactionModel.Description;
                _dbcontext.Update(transaction);
                await _dbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        public async Task<int> DeleteTransaction(int id)
        {
            try
            {
                var transaction = await _dbcontext.Transactions.Where(x => id == x.TransactionId).FirstOrDefaultAsync();
                _dbcontext.Remove(transaction);
                await _dbcontext.SaveChangesAsync();
                return id;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

    }
}
