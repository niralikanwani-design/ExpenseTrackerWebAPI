using ET.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET.Domain.IRepository
{
    public interface ITransactions
    {
        Task<bool> AddTransaction(TransactionModel transactionModel);
        Task<List<TransactionModel>> GetTransaction(TransactionFilterModel transactionFilterModel);
        Task<bool> UpdateTransaction(UpdateTransactionModel transactionModel);
        Task<int> DeleteTransaction(int id);
    }
}
