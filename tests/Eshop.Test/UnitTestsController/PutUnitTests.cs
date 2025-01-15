namespace Eshop.Test;

using Eshop.Domain;
using Eshop.Persistence;
using Eshop.Persistence.Repositories;
using Eshop.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;

public class PutUnitTests
{
    [Fact]
    public async Task Put_ValidId_ReturnsOk()
    {
        // Arrange
        var repositoryMock = Substitute.For<IRepositoryAsync<Order>>();
        var queueMock = Substitute.For<IProcessingQueue<Order>>();
        var controller = new OrdersController(repositoryMock, queueMock);

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

        // Act
        var actionResult = await controller.UpdateAsync(orderFromRepository.OrderId, true);

        // Assert
        Assert.IsType<AcceptedResult>(actionResult);
        await repositoryMock.Received(1).ReadByIdAsync(orderFromRepository.OrderId);
        queueMock.Received(1).Add(orderFromRepository);
        Assert.Equal(Order.OrderStatus.Zaplacena, orderFromRepository.Status);
    }

    [Fact]
    public async Task Put_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var repositoryMock = Substitute.For<IRepositoryAsync<Order>>();
        var queueMock = Substitute.For<IProcessingQueue<Order>>();
        var controller = new OrdersController(repositoryMock, queueMock);

        repositoryMock.ReadByIdAsync(Arg.Any<int>()).ReturnsNull();

        // Act
        var actionResult = await controller.UpdateAsync(-1, false);

        // Assert
        Assert.IsType<NotFoundResult>(actionResult);
        await repositoryMock.Received(1).ReadByIdAsync(-1);
        queueMock.Received(0).Add(Arg.Any<Order>());
    }

    [Fact]
    public async Task Put_UnexpectedError_Returns500()
    {
        // Arrange
        var repositoryMock = Substitute.For<IRepositoryAsync<Order>>();
        var queueMock = Substitute.For<IProcessingQueue<Order>>();
        var controller = new OrdersController(repositoryMock, queueMock);

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

        repositoryMock.ReadByIdAsync(Arg.Any<int>()).Throws(new Exception());

        // Act
        var actionResult = await controller.UpdateAsync(orderFromRepository.OrderId, true);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
        await repositoryMock.Received(1).ReadByIdAsync(orderFromRepository.OrderId);
        queueMock.Received(0).Add(Arg.Any<Order>());
    }
}