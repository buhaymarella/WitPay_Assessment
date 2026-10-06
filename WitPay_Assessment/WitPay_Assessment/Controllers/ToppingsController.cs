using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WitPay_Assessment.Commands.PizzaCommands;
using WitPay_Assessment.Commands.ToppingsCommands;
using WitPay_Assessment.Entity;
using WitPay_Assessment.Repository;

namespace WitPay_Assessment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToppingsController : ControllerBase
    {
        private readonly IToppingsRepository<Toppings> _toppingsRepository;

        public ToppingsController(IToppingsRepository<Toppings> toppingsRepository)
        {
            _toppingsRepository = toppingsRepository;
        }


        [HttpPost("CreateTopping")]
        public async Task<IActionResult> Create([FromBody] CreateToppingsCommand cmd)
        {
            var result = await _toppingsRepository.CreateToppingAsync(cmd).ConfigureAwait(false);
            if (!result.IsSuccess)
                return Error(result);

            return StatusCode(StatusCodes.Status201Created, result);
        }
        [HttpPut("UpdateTopping")]
        public async Task<IActionResult> Update([FromBody] UpdateToppingsCommand cmd)
        {
            var result = await _toppingsRepository.UpdateToppingAsync(cmd).ConfigureAwait(false);
            return result.IsSuccess ? Ok(result) : Error(result);
        }
        [HttpDelete("DeleteTopping")]
        public async Task<IActionResult> Delete([FromBody] DeleteToppingsCommand cmd)
        {
            var result = await _toppingsRepository.DeleteToppingAsync(cmd).ConfigureAwait(false);
            return result.IsSuccess ? Ok(result) : Error(result);
        }
        [HttpGet("GetAllToppings")]
        public async Task<IActionResult> GetAll([FromQuery] GetAllToppingsCommand cmd)
        {
            var result = await _toppingsRepository.GetAllToppingsAsync(cmd).ConfigureAwait(false);
            if (!result.IsSuccess) return Error(result);

            return Ok(result);
        }
        [HttpGet("GetToppingById")]
        public async Task<IActionResult> GetById([FromQuery] GetToppingsByIdCommand cmd)
        {
            var result = await _toppingsRepository.GetToppingByIdAsync(cmd).ConfigureAwait(false);
            if (!result.IsSuccess) return Error(result);

            return Ok(result);
        }
        private IActionResult Error(Result result)
        {
            var message = result.Message ?? "";

            if (message.Contains("not found", StringComparison.OrdinalIgnoreCase) ||
                message.Contains("does not exist", StringComparison.OrdinalIgnoreCase))
                return NotFound(new { message });

            if (message.Contains("already exists", StringComparison.OrdinalIgnoreCase))
                return Conflict(new { message });

            if (message.Contains("required", StringComparison.OrdinalIgnoreCase) ||
                message.Contains("invalid", StringComparison.OrdinalIgnoreCase))
                return BadRequest(new { message });

            return StatusCode(500, new { message });
        }
    }
}
