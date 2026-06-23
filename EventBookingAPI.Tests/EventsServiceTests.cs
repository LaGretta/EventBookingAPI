using AutoMapper;
using EventBookingAPI.DTOs;
using EventBookingAPI.Interfaces.Repository;
using EventBookingAPI.Interfaces.Service;
using EventBookingAPI.Models;
using EventBookingAPI.Service;
using FluentAssertions;
using Moq;

namespace EventBookingAPI.Tests;

public class EventsServiceTests
{
    private readonly Mock<IEventsRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly IEventsService _service;
    
    public EventsServiceTests()
    {
        _repositoryMock = new Mock<IEventsRepository>();
        _mapperMock = new Mock<IMapper>();
        
        _service = new EventsService(
            _repositoryMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task GetByIdAsync_WhenEventIdExists_ReturnsEvent()
    {
        // Arrange
        var eventId = 10;

        var ev = new Event
        {
            Id = eventId,
            Title = "Test Title",
            Description = "Test Description",
            Location = "Test Location",
            Price = 100,
            TotalSeats = 10,
            AvailableSeats = 5,
            Status = EventStatus.Published
        };

        var responseDto = new EventResponseDto
        {
            Id = eventId,
            Title = "Test Title",
            Description = "Test Description",
            Location = "Test Location",
            Price = 100,
            TotalSeats = 10,
            AvailableSeats = 5,
            Status = EventStatus.Published
        };
        
        _repositoryMock
            .Setup(r => r.GetEventByIdAsync(eventId))
            .ReturnsAsync(ev);
        
        _mapperMock
            .Setup(m => m.Map<EventResponseDto>(ev))
            .Returns(responseDto);
        
        // Act
        var result = await _service.GetByIdAsync(eventId);
       
        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(eventId);
        result.Title.Should().Be("Test Title");
        result.Description.Should().Be("Test Description");
        result.Status.Should().Be(EventStatus.Published);
       
        _repositoryMock.Verify(r => r.GetEventByIdAsync(eventId), 
            Times.Once
        );
        _mapperMock.Verify(m => m.Map<EventResponseDto>(ev), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEventNotFound_ShouldThrowException()
    {
        var eventId = 10;

        _repositoryMock.Setup(r => r.GetEventByIdAsync(eventId))
            .ReturnsAsync((Event?)null);
        
        var action = async () => await _service.GetByIdAsync(eventId);
        
        await action.Should().ThrowAsync<Exception>();
        
        _repositoryMock.Verify(r => r.GetEventByIdAsync(eventId), Times.Once);
      
        _mapperMock.Verify(m => m.Map<EventResponseDto>(It.IsAny<Event>()),
            Times.Never);
    }
}