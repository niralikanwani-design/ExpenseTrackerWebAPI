using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET.Domain.DTO
{
    public class TransactionModel
    {
        public int CategoryId { get; set; }

        public decimal Amount { get; set; }

        public string Type { get; set; } = null!;

        public string Description { get; set; }

        public DateTime TransactionDate { get; set; }

        public DateTime? CreatedAt { get; set; }
    }

    public class UpdateTransactionModel
    {
        public int? TransactionId { get; set; }

        public decimal Amount { get; set; }

        public string Type { get; set; } = null!;

        public string Description { get; set; }

        public DateTime TransactionDate { get; set; }
    }

    public class TransactionFilterModel
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string Type { get; set; }
        public DateTime? StartDate { get; set; } 
        public DateTime? EndDate { get; set; }
    }

}
