namespace Eshop.Test;

using Eshop.Domain;
using Eshop.Domain.DTOs;
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
        var controller = new OrdersController(repositoryMock);

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
        var result = await controller.ReadByIdAsync(1);

        // Assert
        var resultResult = Assert.IsType<OkObjectResult>(result.Result);
        var realOrder = resultResult.Value as OrderGetResponseDto;
        Assert.Equivalent(expectedOrderDto, realOrder, true);
        Assert.NotNull(realOrder);
        await repositoryMock.Received(1).ReadByIdAsync(1);
    }

    [Fact]
    public async Task GetById_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var repositoryMock = Substitute.For<IRepositoryAsync<Order>>();
        var controller = new OrdersController(repositoryMock);

        repositoryMock.ReadByIdAsync(-1).ReturnsNull();

        // Act
        var result = await controller.ReadByIdAsync(-1);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
        await repositoryMock.Received(1).ReadByIdAsync(-1);
    }

    [Fact]
    public async Task GetById_UnexpectedError_Returns500()
    {
        // Arrange
        var repositoryMock = Substitute.For<IRepositoryAsync<Order>>();
        var controller = new OrdersController(repositoryMock);

        repositoryMock.ReadByIdAsync(-1).Throws(new Exception());

        // Act
        var actionResult = await controller.ReadByIdAsync(-1);

        // Assert
        var resultResult = Assert.IsType<ObjectResult>(actionResult.Result);
        Assert.Equal(StatusCodes.Status500InternalServerError, resultResult.StatusCode);
        await repositoryMock.Received(1).ReadByIdAsync(-1);
    }
}
