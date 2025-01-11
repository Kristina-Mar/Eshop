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
    public void GetById_ValidId_ReturnsOk()
    {
        // Arrange
        var repositoryMock = Substitute.For<IRepository<Order>>();
        var controller = new OrdersController(repositoryMock);

        Order orderFromRepository = new() {
            OrderId = 1,
            CustomerName = "customer",
            OrderDate = new DateTime (2025, 1,11),
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
            Status = Order.OrderStatus.New
            };

        repositoryMock.ReadById(1).Returns(orderFromRepository);

        OrderGetResponseDto expectedOrderDto = new() {
            OrderId = 1,
            CustomerName = "customer",
            OrderDate = new DateTime (2025, 1,11),
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
            Status = Order.OrderStatus.New
            };

        // Act
        var result = controller.ReadById(1);

        // Assert
        var resultResult = Assert.IsType<OkObjectResult>(result.Result);
        var realOrder = resultResult.Value as OrderGetResponseDto;
        Assert.Equivalent(expectedOrderDto, realOrder, true);
        Assert.NotNull(realOrder);
        repositoryMock.Received(1).ReadById(1);
    }

    [Fact]
    public void GetById_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var repositoryMock = Substitute.For<IRepository<Order>>();
        var controller = new OrdersController(repositoryMock);

        repositoryMock.ReadById(-1).ReturnsNull();

        // Act
        var result = controller.ReadById(-1);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
        repositoryMock.Received(1).ReadById(-1);
    }

    [Fact]
    public void GetById_UnexpectedError_Returns500()
    {
        // Arrange
        var repositoryMock = Substitute.For<IRepository<Order>>();
        var controller = new OrdersController(repositoryMock);

        repositoryMock.ReadById(-1).Throws(new Exception());

        // Act
        var actionResult = controller.ReadById(-1);

        // Assert
        var resultResult = Assert.IsType<ObjectResult>(actionResult.Result);
        Assert.Equal(StatusCodes.Status500InternalServerError, resultResult.StatusCode);
        repositoryMock.Received(1).ReadById(-1);
    }
}
