using ET.Domain.DTO;

namespace ET.Domain.IRepository
{
    public interface ITransactionsService
    {
        Task<bool> AddTransaction(TransactionModel transactionModel);
        Task<List<TransactionModel>> GetTransaction(TransactionFilterModel transactionFilterModel);
        Task<bool> UpdateTransaction(UpdateTransactionModel transactionModel);
        Task<bool> DeleteTransaction(int id);
    }
}
