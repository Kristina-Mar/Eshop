using Eshop.Domain.DTOs;
using Eshop.Persistence.Repositories;
using Microsoft.AspNetCore.Mvc;
using Eshop.Domain;

namespace Eshop.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController(IRepository<Order> repository) : ControllerBase
    {
        private readonly IRepository<Order> repository = repository;

        [HttpGet]
        public ActionResult<IEnumerable<OrderGetResponseDto>> Read()
        {
            IEnumerable<Order> allOrders;

            try
            {
                allOrders = repository.Read();
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, StatusCodes.Status500InternalServerError);
            }

            return (allOrders.Count() != 0) ? Ok(allOrders.Select(OrderGetResponseDto.FromDomain).ToList()) : NotFound();
        }

        [HttpGet("{orderId:int}")]
        public ActionResult<OrderGetResponseDto> ReadById(int orderId)
        {
            Order order;

            try
            {
                order = repository.ReadById(orderId);
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, StatusCodes.Status500InternalServerError);
            }

            return (order != null) ? Ok(OrderGetResponseDto.FromDomain(order)) : NotFound();
        }

        [HttpPost]
        public IActionResult CreateOrder(OrderCreateRequestDto orderCreateRequest)
        {
            Order newOrder = orderCreateRequest.ToDomain();
            try
            {
                repository.Create(newOrder);
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, StatusCodes.Status500InternalServerError);
            }
            return CreatedAtAction(nameof(ReadById), new { orderId = newOrder.OrderId }, OrderGetResponseDto.FromDomain(newOrder));
        }

        [HttpPut("{orderId:int}")]
        public IActionResult UpdateOrder(int orderId, [FromBody] bool isPaid)
        {
            try
            {
                repository.Update(orderId, isPaid);
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