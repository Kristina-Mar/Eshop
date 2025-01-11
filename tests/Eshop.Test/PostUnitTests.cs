namespace Eshop.Test;

using Eshop.Domain;
using Eshop.Domain.DTOs;
using Eshop.Domain.Models;
using Eshop.Persistence.Repositories;
using Eshop.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

public class PostUnitTests
{
    [Fact]
    public async Task Post_ValidRequest_ReturnsOk()
    {
        // Arrange
        var repositoryMock = Substitute.For<IRepositoryAsync<Order>>();
        var controller = new OrdersController(repositoryMock);

        OrderCreateRequestDto orderRequestDto = new(
            "customer",
            new DateTime(2025, 1, 11),
            new(){
                new("computer", 5, 14999),
                new("monitor", 10, 7999)
                }
                );

        OrderGetResponseDto orderReturnedFromRepository = new()
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
        var result = await controller.CreateAsync(orderRequestDto);

        // Assert
        var resultResult = Assert.IsType<CreatedAtActionResult>(result);
        var realOrder = resultResult.Value as OrderGetResponseDto;
        Assert.Equivalent(orderReturnedFromRepository, realOrder, true);
        Assert.NotNull(realOrder);
        await repositoryMock.Received(1).CreateAsync(Arg.Any<Order>());
    }

    [Fact]
    public async Task Post_UnexpectedError_Returns500()
    {
        // Arrange
        var repositoryMock = Substitute.For<IRepositoryAsync<Order>>();
        var controller = new OrdersController(repositoryMock);

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
        var resultResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal(StatusCodes.Status500InternalServerError, resultResult.StatusCode);
        await repositoryMock.Received(1).CreateAsync(Arg.Any<Order>());
    }
}
