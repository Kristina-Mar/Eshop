using Eshop.Domain.DTOs;
using Eshop.Persistence.Repositories;
using Microsoft.AspNetCore.Mvc;
using Eshop.Domain;
using Eshop.WebApi.KafkaProducers;
using Confluent.Kafka;
using System.Text.Json;

namespace Eshop.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController(IRepositoryAsync<Order> repository, IKafkaProducer producer) : ControllerBase
    {
        private readonly IRepositoryAsync<Order> _repository = repository;
        private readonly IKafkaProducer _producer = producer;

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

                var result = await _producer.ProduceAsync("order-status-update", new Message<int, string>
                {
                    Key = orderId,
                    Value = JsonSerializer.Serialize(orderToUpdate)
                });
            }
            catch (KafkaException ke)
            {
                return Problem(ke.Message, null, StatusCodes.Status502BadGateway, "Error producing message to Kafka.");
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, StatusCodes.Status500InternalServerError);
            }
            return Accepted();
        }
    }
}