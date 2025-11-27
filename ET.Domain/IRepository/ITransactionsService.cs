using ET.Domain.DTO;
using ET.Domain.Models;

namespace ET.Domain.IRepository
{
    public interface ITransactionsService
    {
        Task<bool> AddTransaction(TransactionModel transactionModel);
        Task<int> GetTotalTransactionCount();
        Task<List<TransactionModel>> GetTransaction(TransactionFilterModel transactionFilterModel);
        Task<TransactionModel?> GetTransactionById(int id);
        Task<bool> UpdateTransaction(UpdateTransactionModel transactionModel);
        Task<bool> DeleteTransaction(int id);
        Task<List<Category>> GetCategories();
    }
}
