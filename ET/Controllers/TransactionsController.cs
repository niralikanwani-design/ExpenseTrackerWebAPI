using ET.Domain.DTO;
using ET.Domain.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace ET.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionsService _Itransactions;

        public TransactionsController(ITransactionsService transactions)
        {
            _Itransactions = transactions;
        }

        [HttpPost("GetTransaction")]
        public async Task<IActionResult> GetTransaction([FromBody] TransactionFilterModel transactionFilterModel)
        {
            var result = await _Itransactions.GetTransaction(transactionFilterModel);
            if (result == null || !result.Any())
                return BadRequest("No transactions found");
            return Ok(result);
        }

        [HttpPost("AddTransaction")]
        public async Task<IActionResult> AddTransaction([FromBody] TransactionModel transactionModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new JsonResultObj(400, "Invalid model!"));
            }

            try
            {
                var result = await _Itransactions.AddTransaction(transactionModel);
                return Ok(result);
            }
            catch(Exception)
            {
                return new JsonResult(new JsonResultObj(StatusCodes.Status500InternalServerError, "Error Occured while generating the transaction!"));
            }
        }

        [HttpPut("UpdateTransaction")]
        public async Task<IActionResult> UpdateTransaction(UpdateTransactionModel transactionModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new JsonResultObj(400, "Invalid model!"));
            }

            try
            {
                var result = await _Itransactions.UpdateTransaction(transactionModel);
                return Ok(result);
            }
            catch (Exception)
            {
                return new JsonResult(new JsonResultObj(StatusCodes.Status500InternalServerError, "Error Occured while updating the transaction!"));
            }
        }

        [HttpDelete("DeleteTransaction")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            if (id < 1)
            {
                return BadRequest(new JsonResultObj(400, "Invalid model!"));
            }

            try
            {
                var result = await _Itransactions.DeleteTransaction(id);
                return Ok(result);
            }
            catch (Exception)
            {
                return new JsonResult(new JsonResultObj(StatusCodes.Status500InternalServerError, "Error Occured while deleting the transaction!"));
            }
        }
    }
}
