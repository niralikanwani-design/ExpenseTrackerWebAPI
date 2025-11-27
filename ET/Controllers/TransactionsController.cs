using ET.Domain.DTO;
using ET.Domain.IRepository;
using ET.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace ET.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionsService _transactionsService;

        public TransactionsController(ITransactionsService transactionsService)
        {
            _transactionsService = transactionsService;
        }

        [HttpPost("GetTransaction")]
        public async Task<IActionResult> GetTransaction([FromBody] TransactionFilterModel transactionFilterModel)
        {
            var result = await _transactionsService.GetTransaction(transactionFilterModel);
            if (result == null || !result.Any()) return BadRequest("No Data found!");
            return Ok(result);
        }

        [HttpGet("GetTransactionById")]
        public async Task<IActionResult> GetTransactionById(int id)
        {
            var result = await _transactionsService.GetTransactionById(id);
            if (result == null) return BadRequest("No Data found!");
            return Ok(result);
        }

        [HttpGet("GetTotalTransactionCount")]
        public async Task<int> GetTotalTransactionCount()
        {
            return await _transactionsService.GetTotalTransactionCount();
        }

        [HttpGet("GetCategories")]
        public async Task<List<Category>> GetCategories()
        {
            return await _transactionsService.GetCategories();
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
                var result = await _transactionsService.AddTransaction(transactionModel);
                return Ok(result);
            }
            catch (Exception)
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
                var result = await _transactionsService.UpdateTransaction(transactionModel);
                return Ok(result);
            }
            catch (Exception)
            {
                return new JsonResult(new JsonResultObj(StatusCodes.Status500InternalServerError, "Error Occured while updating the transaction!"));
            }
        }

        [HttpDelete("DeleteTransaction/{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            if (id < 1)
            {
                return BadRequest(new JsonResultObj(400, "Invalid model!"));
            }

            try
            {
                var result = await _transactionsService.DeleteTransaction(id);
                return Ok(result);
            }
            catch (Exception)
            {
                return new JsonResult(new JsonResultObj(StatusCodes.Status500InternalServerError, "Error Occured while deleting the transaction!"));
            }
        }
    }
}
