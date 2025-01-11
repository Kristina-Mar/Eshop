namespace Eshop.Test;

using System.Dynamic;
using Eshop.Domain;
using Eshop.Domain.DTOs;
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
        var controller = new OrdersController(repositoryMock);

        Order order = new() { Status = Order.OrderStatus.New };

        repositoryMock.When(r => r.UpdateAsync(Arg.Any<int>(), true)).Do(callInfo => order.Status = Order.OrderStatus.Paid);

        // Act
        var result = await controller.UpdateAsync(1, true);

        // Assert
        Assert.IsType<NoContentResult>(result);
        await repositoryMock.Received(1).UpdateAsync(Arg.Any<int>(), Arg.Any<bool>());
        Assert.Equal(Order.OrderStatus.Paid, order.Status);
    }

    [Fact]
    public async Task Put_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var repositoryMock = Substitute.For<IRepositoryAsync<Order>>();
        var controller = new OrdersController(repositoryMock);

        repositoryMock.When(r => r.UpdateAsync(Arg.Any<int>(), Arg.Any<bool>())).Throws(new KeyNotFoundException());

        // Act
        var result = await controller.UpdateAsync(-1, false);

        // Assert
        Assert.IsType<NotFoundResult>(result);
        await repositoryMock.Received(1).UpdateAsync(Arg.Any<int>(), Arg.Any<bool>());
    }

    [Fact]
    public async Task Put_UnexpectedError_Returns500()
    {
        // Arrange
        var repositoryMock = Substitute.For<IRepositoryAsync<Order>>();
        var controller = new OrdersController(repositoryMock);

        repositoryMock.When(r => r.UpdateAsync(Arg.Any<int>(), Arg.Any<bool>())).Throws(new Exception());

        // Act
        var actionResult = await controller.UpdateAsync(-1, true);

        // Assert
        var resultResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal(StatusCodes.Status500InternalServerError, resultResult.StatusCode);
        await repositoryMock.Received(1).UpdateAsync(Arg.Any<int>(), Arg.Any<bool>());
    }
}
