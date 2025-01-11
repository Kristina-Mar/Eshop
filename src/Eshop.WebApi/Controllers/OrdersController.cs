using Eshop.Domain.DTOs;
using Eshop.Persistence.Repositories;
using Microsoft.AspNetCore.Mvc;
using Eshop.Domain;

namespace Eshop.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController(IRepositoryAsync<Order> repository) : ControllerBase
    {
        private readonly IRepositoryAsync<Order> repository = repository;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderGetResponseDto>>> ReadAsync()
        {
            IEnumerable<Order> allOrders;

            try
            {
                allOrders = await repository.ReadAsync();
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, StatusCodes.Status500InternalServerError);
            }

            return (allOrders.Count() != 0) ? Ok(allOrders.Select(OrderGetResponseDto.FromDomain).ToList()) : NotFound();
        }

        [HttpGet("{orderId:int}")]
        [ActionName(nameof(ReadByIdAsync))]
        public async Task<ActionResult<OrderGetResponseDto>> ReadByIdAsync(int orderId)
        {
            Order order;

            try
            {
                order = await repository.ReadByIdAsync(orderId);
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, StatusCodes.Status500InternalServerError);
            }

            return (order != null) ? Ok(OrderGetResponseDto.FromDomain(order)) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(OrderCreateRequestDto orderCreateRequest)
        {
            Order newOrder = orderCreateRequest.ToDomain();
            try
            {
                await repository.CreateAsync(newOrder);
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, StatusCodes.Status500InternalServerError);
            }
            return CreatedAtAction(nameof(ReadByIdAsync), new { orderId = newOrder.OrderId }, OrderGetResponseDto.FromDomain(newOrder));
        }

        [HttpPut("{orderId:int}")]
        public async Task<IActionResult> UpdateAsync(int orderId, [FromBody] bool isPaid)
        {
            try
            {
                await repository.UpdateAsync(orderId, isPaid);
            }
            catch (Exception e)
            {
                if (e is KeyNotFoundException)
                {
                    return NotFound();
                }
                else
                {
                    return Problem(e.Message, null, StatusCodes.Status500InternalServerError);
                }
            }
            return NoContent();
        }
    }
}