namespace Eshop.Test;

using Eshop.Domain;
using Eshop.Domain.DTOs;
using Eshop.Persistence.Repositories;
using Eshop.WebApi.Controllers;
using Eshop.WebApi.KafkaProducers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

public class PostUnitTests
{
    [Fact]
    public async Task Post_ValidRequest_ReturnsOk()
    {
        // Arrange
        var repositoryMock = Substitute.For<IRepositoryAsync<Order>>();
        var producerMock = Substitute.For<IKafkaProducer>();
        var controller = new OrdersController(repositoryMock, producerMock);

        OrderCreateRequestDto orderRequestDto = new(
            "customer",
            new DateTime(2025, 1, 11),
            new(){
                new("computer", 5, 14999),
                new("monitor", 10, 7999)
                }
                );

        OrderGetResponseDto orderDtoFromRepository = new()
        {
            OrderId = 1,
            CustomerName = "customer",
            OrderDate = new DateTime(2025, 1, 11),
            OrderItems = new(){
                new(){
                    OrderLineId = 1,
                    ItemName = "computer",
                    NumberOfItems = 5,
                    ItemPrice = 14999
                },
                new(){
                    OrderLineId = 2,
                    ItemName = "monitor",
                    NumberOfItems = 10,
                    ItemPrice = 7999
                }
            },
            Status = Order.OrderStatus.Nová
        };

        repositoryMock.When(r => r.CreateAsync(Arg.Any<Order>())).Do(callInfo =>
        {
            var order = callInfo.Arg<Order>();
            order.OrderId = 1;
            order.OrderItems[0].OrderLineId = 1;
            order.OrderItems[1].OrderLineId = 2;
            order.Status = Order.OrderStatus.Nová;
        });

        // Act
        var actionResult = await controller.CreateAsync(orderRequestDto);

        // Assert
        var objectResult = Assert.IsType<CreatedAtActionResult>(actionResult);
        var realOrderDto = objectResult.Value as OrderGetResponseDto;
        Assert.Equivalent(orderDtoFromRepository, realOrderDto, true);
        Assert.NotNull(realOrderDto);
        await repositoryMock.Received(1).CreateAsync(Arg.Any<Order>());
    }

    [Fact]
    public async Task Post_UnexpectedError_Returns500()
    {
        // Arrange
        var repositoryMock = Substitute.For<IRepositoryAsync<Order>>();
        var producerMock = Substitute.For<IKafkaProducer>();
        var controller = new OrdersController(repositoryMock, producerMock);

        repositoryMock.When(r => r.CreateAsync(Arg.Any<Order>())).Throws(new Exception());

        OrderCreateRequestDto orderRequestDto = new(
            "customer",
            new DateTime(2025, 1, 11),
            new(){
                new("computer", 5, 14999),
                new("monitor", 10, 7999)
                }
                );

        // Act
        var actionResult = await controller.CreateAsync(orderRequestDto);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
        await repositoryMock.Received(1).CreateAsync(Arg.Any<Order>());
    }
}
