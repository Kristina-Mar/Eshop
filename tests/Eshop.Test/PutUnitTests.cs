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
    public void Put_ValidId_ReturnsOk()
    {
        // Arrange
        var repositoryMock = Substitute.For<IRepository<Order>>();
        var controller = new OrdersController(repositoryMock);

        Order order = new () { Status = Order.OrderStatus.New };

        repositoryMock.When(r => r.Update(Arg.Any<int>(), true)).Do(callInfo => order.Status = Order.OrderStatus.Paid);

        // Act
        var result = controller.Update(1, true);

        // Assert
        Assert.IsType<NoContentResult>(result);
        repositoryMock.Received(1).Update(Arg.Any<int>(), Arg.Any<bool>());
        Assert.Equal(Order.OrderStatus.Paid, order.Status);
    }

    [Fact]
    public void Put_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var repositoryMock = Substitute.For<IRepository<Order>>();
        var controller = new OrdersController(repositoryMock);

        repositoryMock.When(r => r.Update(Arg.Any<int>(), Arg.Any<bool>())).Throws(new KeyNotFoundException());

        // Act
        var result = controller.Update(-1, false);

        // Assert
        Assert.IsType<NotFoundResult>(result);
        repositoryMock.Received(1).Update(Arg.Any<int>(), Arg.Any<bool>());
    }

    [Fact]
    public void Put_UnexpectedError_Returns500()
    {
        // Arrange
        var repositoryMock = Substitute.For<IRepository<Order>>();
        var controller = new OrdersController(repositoryMock);

        repositoryMock.When(r => r.Update(Arg.Any<int>(), Arg.Any<bool>())).Throws(new Exception());

        // Act
        var actionResult = controller.Update(-1, true);

        // Assert
        var resultResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal(StatusCodes.Status500InternalServerError, resultResult.StatusCode);
        repositoryMock.Received(1).Update(Arg.Any<int>(), Arg.Any<bool>());
    }
}
