using AutoMapper;
using EventBookingAPI.DTOs;
using EventBookingAPI.Interfaces.Repository;
using EventBookingAPI.Models;
using EventBookingAPI.Service;
using FluentAssertions;
using Moq;

namespace EventBookingAPI.Tests;

public class BookingServiceTests
{
    private readonly Mock<IBookingRepository> _bookingRepositoryMock;
    private readonly Mock<IEventsRepository> _eventsRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly BookingService _service;

    public BookingServiceTests()
    {
        _bookingRepositoryMock = new Mock<IBookingRepository>();
        _eventsRepositoryMock = new Mock<IEventsRepository>();
        _mapperMock =  new Mock<IMapper>();

        _service = new BookingService(
            _bookingRepositoryMock.Object,
            _eventsRepositoryMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task CancelBookingAsync_WhenBookingIsActive_ShouldCancelBooking()
    {
        // Arrange
        var bookingId = 1;
        var userId = 5;

        var booking = new Booking
        {
            Id = bookingId,
            UserId = userId,
            EventId = 10,
            SeatsCount = 2,
            Status  = BookingStatus.Active,
            CancelledAt = null,
            Event = new Event
            {
                Id = 10,
                AvailableSeats = 8
            }
        };

        var responseDto = new BookingResponseDto
        {
            Id = bookingId,
            UserId = userId,
            EventId = 10,
            SeatsCount = 2,
            Status = BookingStatus.Cancelled,
        };

        _bookingRepositoryMock
            .Setup(r => r.GetBookingByIdAsync(bookingId ,userId))
            .ReturnsAsync(booking);

        _mapperMock
            .Setup(m => m.Map<BookingResponseDto>(booking))
            .Returns(responseDto);

        // Act
        var result = await _service.CancelBookingAsync(bookingId , userId);

        // Assert
        booking.Status.Should().Be(BookingStatus.Cancelled);
        booking.CancelledAt.Should().NotBeNull();
        booking.Event.AvailableSeats.Should().Be(10);

        result.Should().NotBeNull();
        result!.Status.Should().Be(BookingStatus.Cancelled);

        _bookingRepositoryMock.Verify(
            r => r.GetBookingByIdAsync(bookingId,userId),
            Times.Once
        );

        _bookingRepositoryMock.Verify(
            r => r.SaveChangesAsync(),
            Times.Once
        );
    }
    [Fact]
    public async Task CancelBookingAsync_WhenBookingNotFound_ShouldThrowException()
    {
        // Arrange
        var bookingId = 1;
        var userId = 5;

        _bookingRepositoryMock
            .Setup(r => r.GetBookingByIdAsync(bookingId,userId))
            .ReturnsAsync((Booking?)null);

        // Act
        // Act
        var result = await _service.CancelBookingAsync(bookingId, userId);

        // Assert
        result.Should().BeNull();

        _bookingRepositoryMock.Verify(
            r => r.GetBookingByIdAsync(bookingId,userId),
            Times.Once
        );
    }
}