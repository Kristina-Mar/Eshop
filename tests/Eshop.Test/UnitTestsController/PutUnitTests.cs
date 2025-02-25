namespace Eshop.Test;

using System.Text.Json;
using Confluent.Kafka;
using Eshop.Domain;
using Eshop.Persistence.Repositories;
using Eshop.WebApi.Controllers;
using Eshop.WebApi.KafkaProducers;
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
        var producerMock = Substitute.For<IKafkaProducer>();
        var controller = new OrdersController(repositoryMock, producerMock);

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

        Order updatedOrderForKafka = new()
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
            Status = Order.OrderStatus.Zaplacena
        };

        var message = new Message<int, string>
        {
            Key = orderFromRepository.OrderId,
            Value = JsonSerializer.Serialize(updatedOrderForKafka)
        };

        repositoryMock.ReadByIdAsync(Arg.Any<int>()).Returns(orderFromRepository);

        // Act
        var actionResult = await controller.UpdateAsync(orderFromRepository.OrderId, true);

        // Assert
        Assert.IsType<AcceptedResult>(actionResult);
        await repositoryMock.Received(1).ReadByIdAsync(orderFromRepository.OrderId);
        await producerMock.Received(1).ProduceAsync("order-status-update", Arg.Is<Message<int, string>>(m => m.Key == message.Key
        && m.Value == message.Value));

        Assert.Equal(Order.OrderStatus.Zaplacena, orderFromRepository.Status);
    }

    [Fact]
    public async Task Put_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var repositoryMock = Substitute.For<IRepositoryAsync<Order>>();
        var producerMock = Substitute.For<IKafkaProducer>();
        var controller = new OrdersController(repositoryMock, producerMock);

        repositoryMock.ReadByIdAsync(Arg.Any<int>()).ReturnsNull();

        // Act
        var actionResult = await controller.UpdateAsync(-1, false);

        // Assert
        Assert.IsType<NotFoundResult>(actionResult);
        await repositoryMock.Received(1).ReadByIdAsync(-1);
        await producerMock.Received(0).ProduceAsync(Arg.Any<string>(), Arg.Any<Message<int, string>>());
    }

    [Fact]
    public async Task Put_KafkaError_Returns502()
    {
        // Arrange
        var repositoryMock = Substitute.For<IRepositoryAsync<Order>>();
        var producerMock = Substitute.For<IKafkaProducer>();
        var controller = new OrdersController(repositoryMock, producerMock);

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

        Order updatedOrderForKafka = new()
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
            Status = Order.OrderStatus.Zaplacena
        };

        var message = new Message<int, string>
        {
            Key = orderFromRepository.OrderId,
            Value = JsonSerializer.Serialize(updatedOrderForKafka)
        };

        repositoryMock.ReadByIdAsync(Arg.Any<int>()).Returns(orderFromRepository);
        producerMock.ProduceAsync(Arg.Any<string>(), Arg.Any<Message<int, string>>()).Throws(new KafkaException(ErrorCode.Local_Fail));

        // Act
        var actionResult = await controller.UpdateAsync(orderFromRepository.OrderId, true);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(actionResult);
        var details = objectResult.Value as ProblemDetails;
        Assert.Equal(StatusCodes.Status502BadGateway, objectResult.StatusCode);
        Assert.Equal("Error producing message to Kafka.", details.Title);
        await repositoryMock.Received(1).ReadByIdAsync(orderFromRepository.OrderId);
        await producerMock.Received(1).ProduceAsync("order-status-update", Arg.Is<Message<int, string>>(m => m.Key == message.Key
        && m.Value == message.Value));
    }

    [Fact]
    public async Task Put_UnexpectedError_Returns500()
    {
        // Arrange
        var repositoryMock = Substitute.For<IRepositoryAsync<Order>>();
        var producerMock = Substitute.For<IKafkaProducer>();
        var controller = new OrdersController(repositoryMock, producerMock);

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
        await producerMock.Received(0).ProduceAsync(Arg.Any<string>(), Arg.Any<Message<int, string>>());
    }
}