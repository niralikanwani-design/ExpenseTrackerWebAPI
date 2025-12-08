using ET.Application.DTOs;
using ET.Domain.Entities;

namespace ET.Application.Contracts;

public interface ITransactionService
{
    Task<bool> AddTransaction(TransactionModel transactionModel);
    Task<int> GetTotalTransactionCount();
    Task<List<TransactionModel>> GetTransaction(TransactionFilterModel transactionFilterModel);
    Task<TransactionModel?> GetTransactionById(int id);
    Task<bool> UpdateTransaction(UpdateTransactionModel transactionModel);
    Task<bool> DeleteTransaction(int id);
    Task<List<Category>> GetCategories();
    Task<List<Account>> GetAccountType();
    Task<byte[]> ExportTransactionsToCsv(TransactionFilterModel filter);
}
