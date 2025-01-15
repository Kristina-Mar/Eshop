namespace Eshop.Test;

using Eshop.Domain;
using Eshop.Persistence;
using Eshop.Persistence.Repositories;
using Eshop.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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
        var queue = Substitute.For<IProcessingQueue<Order>>();
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

        repositoryMock.ReadByIdAsync(1).Returns(orderFromRepository);

        // Act
        var result = await controller.UpdateAsync(orderFromRepository.OrderId, true);

        // Assert
        Assert.IsType<AcceptedResult>(result);
        await repositoryMock.Received(1).ReadByIdAsync(Arg.Any<int>());
        queue.Received(1).Add(Arg.Any<Order>());
        Assert.Equal(Order.OrderStatus.Zaplacena, orderFromRepository.Status);
    }

    [Fact]
    public async Task Put_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var repositoryMock = Substitute.For<IRepositoryAsync<Order>>();
        var queue = Substitute.For<IProcessingQueue<Order>>();
        var controller = new OrdersController(repositoryMock, queue);

        repositoryMock.ReadByIdAsync(1).ReturnsNull();

        // Act
        var result = await controller.UpdateAsync(-1, false);

        // Assert
        Assert.IsType<NotFoundResult>(result);
        await repositoryMock.Received(1).ReadByIdAsync(Arg.Any<int>());
        queue.Received(0).Add(Arg.Any<Order>());
    }

    [Fact]
    public async Task Put_UnexpectedError_Returns500()
    {
        // Arrange
        var repositoryMock = Substitute.For<IRepositoryAsync<Order>>();
        var queue = Substitute.For<IProcessingQueue<Order>>();
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

        repositoryMock.ReadByIdAsync(Arg.Any<int>()).Throws(new Exception());

        // Act
        var actionResult = await controller.UpdateAsync(orderFromRepository.OrderId, true);

        // Assert
        var resultResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal(StatusCodes.Status500InternalServerError, resultResult.StatusCode);
        await repositoryMock.Received(1).ReadByIdAsync(Arg.Any<int>());
        queue.Received(0).Add(Arg.Any<Order>());
    }
}