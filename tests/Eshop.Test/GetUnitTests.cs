namespace Eshop.Test;

using Eshop.Domain;
using Eshop.Domain.DTOs;
using Eshop.Persistence.Repositories;
using Eshop.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

public class GetUnitTests
{
    [Fact]
    public void Get_OrdersAvailable_ReturnsOk()
    {
        // Arrange
        var repositoryMock = Substitute.For<IRepository<Order>>();
        var controller = new OrdersController(repositoryMock);

        List<Order> ordersFromRepository = new() {
           new() {
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
            }
        };

        repositoryMock.Read().Returns(ordersFromRepository);

        List<OrderGetResponseDto> expectedOrdersDto = new() {
           new() {
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
            }
        };

        // Act
        var result = controller.Read();

        // Assert
        var resultResult = Assert.IsType<OkObjectResult>(result.Result);
        var realOrders = resultResult.Value as List<OrderGetResponseDto>;
        Assert.Equivalent(expectedOrdersDto, realOrders, true);
        Assert.NotNull(realOrders);
        repositoryMock.Received(1).Read();
    }

    [Fact]
    public void Get_OrdersNull_ReturnsNotFound()
    {
        // Arrange
        var repositoryMock = Substitute.For<IRepository<Order>>();
        var controller = new OrdersController(repositoryMock);
        var noOrders = new List<Order>();

        repositoryMock.Read().Returns(noOrders);

        // Act
        var result = controller.Read();

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
        repositoryMock.Received(1).Read();
    }

    [Fact]
    public void Get_UnexpectedError_Returns500()
    {
        // Arrange
        var repositoryMock = Substitute.For<IRepository<Order>>();
        var controller = new OrdersController(repositoryMock);

        repositoryMock.Read().Throws(new Exception());

        // Act
        var actionResult = controller.Read();

        // Assert
        var resultResult = Assert.IsType<ObjectResult>(actionResult.Result);
        Assert.Equal(StatusCodes.Status500InternalServerError, resultResult.StatusCode);
        repositoryMock.Received(1).Read();
    }
}
