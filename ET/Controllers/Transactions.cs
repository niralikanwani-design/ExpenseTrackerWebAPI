using ET.Domain.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ET.Domain.DTO;

namespace ET.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Transactions : ControllerBase
    {
        private readonly ITransactions _Itransactions;

        public Transactions(ITransactions transactions)
        {
            _Itransactions = transactions;
        }

        [HttpPost("GetTransaction")]
        public async Task<IActionResult> GetTransaction([FromBody]TransactionFilterModel transactionFilterModel)
        {
            var result = await _Itransactions.GetTransaction(transactionFilterModel);
            if (result == null || !result.Any())
                return BadRequest("No transactions found");
            return Ok(result);
        }

        [HttpPost("AddTransaction")]
        public async Task<IActionResult> AddTransaction(TransactionModel transactionModel)
        {
            var result = await _Itransactions.AddTransaction(transactionModel);
            if (result == null)
                return BadRequest("Please enter data!");
            return Ok(result);
        }

        [HttpPut("UpdateTransaction")]
        public async Task<IActionResult> UpdateTransaction(UpdateTransactionModel transactionModel)
        {
            var result = await _Itransactions.UpdateTransaction(transactionModel);
            if (result == null)
                return BadRequest("Please enter mention fill");
            return Ok(result);
        }

        [HttpDelete("DeleteTransaction")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var result = await _Itransactions.DeleteTransaction(id);
            if(result == null)
                return BadRequest("Please select transaction!");
            return Ok(result);
        }

    }
}
