using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WitPay_Assessment.Commands.PizzaCommands;
using WitPay_Assessment.Entity;
using WitPay_Assessment.Repository;

namespace WitPay_Assessment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PizzaController : ControllerBase
    {
        private readonly IPizzaRepository<Pizza> _pizzaRepository;

        public PizzaController(IPizzaRepository<Pizza> pizzaRepository)
        {
            _pizzaRepository = pizzaRepository;
        }
        [HttpPost("CreatePizza")]
        public async Task<IActionResult> Create([FromBody] CreatePizzaCommand cmd)
        {
            var result = await _pizzaRepository.CreatePizzaAsync(cmd).ConfigureAwait(false);
            if (!result.IsSuccess)
                return Error(result);

            return StatusCode(StatusCodes.Status201Created, result);
        }
        [HttpPut("UpdatePizza")]
        public async Task<IActionResult> Update([FromBody] UpdatePizzaCommand cmd)
        {
            var result = await _pizzaRepository.UpdatePizzaAsync(cmd).ConfigureAwait(false);
            return result.IsSuccess ? Ok(result) : Error(result);
        }
        [HttpDelete("DeletePizza")]
        public async Task<IActionResult> Delete([FromBody] DeletePizzaCommand cmd)
        {
            var result = await _pizzaRepository.DeletePizzaAsync(cmd).ConfigureAwait(false);
            return result.IsSuccess ? Ok(result) : Error(result);
        }
        [HttpGet("GetAllPizza")]
        public async Task<IActionResult> GetAll([FromQuery] GetAllPizzaCommand cmd)
        {
            var result = await _pizzaRepository.GetAllPizzaAsync(cmd).ConfigureAwait(false);
            if (!result.IsSuccess) return Error(result);

            return Ok(result);
        }
        [HttpGet("GetPizzaById")]
        public async Task<IActionResult> GetById([FromQuery] GetPizzaByIdCommand cmd)
        {
            var result = await _pizzaRepository.GetPizzaByIdAsync(cmd).ConfigureAwait(false);
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
