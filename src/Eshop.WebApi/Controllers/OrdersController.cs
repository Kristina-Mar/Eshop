using Eshop.Domain.DTOs;
using Eshop.Persistence.Repositories;
using Microsoft.AspNetCore.Mvc;
using Eshop.Domain;
using Eshop.Persistence;

namespace Eshop.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController(IRepositoryAsync<Order> repository, IProcessingQueue<Order> processingQueue) : ControllerBase
    {
        private readonly IRepositoryAsync<Order> _repository = repository;
        private readonly IProcessingQueue<Order> _processingQueue = processingQueue;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderGetResponseDto>>> ReadAsync()
        {
            IEnumerable<Order> allOrders;

            try
            {
                allOrders = await _repository.ReadAsync();
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
                order = await _repository.ReadByIdAsync(orderId);
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
                await _repository.CreateAsync(newOrder);
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
                var orderToUpdate = await _repository.ReadByIdAsync(orderId);

                if (orderToUpdate is null)
                {
                    return NotFound();
                }
                if (isPaid)
                {
                    orderToUpdate.Status = Order.OrderStatus.Zaplacena;
                }
                else
                {
                    orderToUpdate.Status = Order.OrderStatus.Zrušena;
                }
                _processingQueue.Add(orderToUpdate);
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, StatusCodes.Status500InternalServerError);
            }
            return Accepted();
        }
    }
}