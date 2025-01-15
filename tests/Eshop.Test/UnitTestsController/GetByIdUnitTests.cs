namespace Eshop.Test;

using Eshop.Domain;
using Eshop.Domain.DTOs;
using Eshop.Persistence;
using Eshop.Persistence.Repositories;
using Eshop.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;

public class GetByIdUnitTests
{
    [Fact]
    public async Task GetById_ValidId_ReturnsOk()
    {
        // Arrange
        var repositoryMock = Substitute.For<IRepositoryAsync<Order>>();
        var queue = new PaymentProcessingQueue();
        var controller = new OrdersController(repositoryMock, queue);

        Order orderFromRepository = new()
        {
            OrderId = 1,
            CustomerName = "customer",
            OrderDate = new DateTime(2025, 1, 11),
            OrderItems = new(){
                new(){
                    OrderLineId = 1,
                    ItemName = "computer",
                    Quantity = 5,
                    ItemPrice = 14999
                },
                new(){
                    OrderLineId = 2,
                    ItemName = "monitor",
                    Quantity = 10,
                    ItemPrice = 7999
                }
            },
            Status = Order.OrderStatus.Nová
        };

        repositoryMock.ReadByIdAsync(Arg.Any<int>()).Returns(orderFromRepository);

        OrderGetResponseDto expectedOrderDto = new()
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

        // Act
        var actionResult = await controller.ReadByIdAsync(1);

        // Assert
        var objectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var realOrderDto = objectResult.Value as OrderGetResponseDto;
        Assert.Equivalent(expectedOrderDto, realOrderDto, true);
        Assert.NotNull(realOrderDto);
        await repositoryMock.Received(1).ReadByIdAsync(1);
    }

    [Fact]
    public async Task GetById_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var repositoryMock = Substitute.For<IRepositoryAsync<Order>>();
        var queue = new PaymentProcessingQueue();
        var controller = new OrdersController(repositoryMock, queue);

        repositoryMock.ReadByIdAsync(Arg.Any<int>()).ReturnsNull();

        // Act
        var actionResult = await controller.ReadByIdAsync(-1);

        // Assert
        Assert.IsType<NotFoundResult>(actionResult.Result);
        await repositoryMock.Received(1).ReadByIdAsync(-1);
    }

    [Fact]
    public async Task GetById_UnexpectedError_Returns500()
    {
        // Arrange
        var repositoryMock = Substitute.For<IRepositoryAsync<Order>>();
        var queue = new PaymentProcessingQueue();
        var controller = new OrdersController(repositoryMock, queue);

        repositoryMock.ReadByIdAsync(Arg.Any<int>()).Throws(new Exception());

        // Act
        var actionResult = await controller.ReadByIdAsync(-1);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(actionResult.Result);
        Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
        await repositoryMock.Received(1).ReadByIdAsync(-1);
    }
}
