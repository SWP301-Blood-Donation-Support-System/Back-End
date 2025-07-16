using BusinessLayer.IService;
using BusinessLayer.Service;
using DataAccessLayer.DTO;
using DataAccessLayer.Entity;
using DataAccessLayer.IRepository;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace BloodDonationSupportSystemTest
{
    public class BloodUnitServiceErrorTests
    {
        private readonly Mock<IBloodUnitRepository> _mockBloodUnitRepository;
        private readonly Mock<IBloodRequestRepository> _mockBloodRequestRepository;
        private readonly Mock<IBloodRequestService> _mockBloodRequestService;
        private readonly Mock<ILogger<BloodUnitService>> _mockLogger;
        private readonly Mock<AutoMapper.IMapper> _mockMapper;
        private readonly BloodUnitService _bloodUnitService;

        public BloodUnitServiceErrorTests()
        {
            // Setup mocks
            _mockBloodUnitRepository = new Mock<IBloodUnitRepository>();
            _mockBloodRequestRepository = new Mock<IBloodRequestRepository>();
            _mockBloodRequestService = new Mock<IBloodRequestService>();
            _mockLogger = new Mock<ILogger<BloodUnitService>>();
            _mockMapper = new Mock<AutoMapper.IMapper>();

            // Setup service with mocks
            _bloodUnitService = new BloodUnitService(
                _mockBloodUnitRepository.Object,
                _mockBloodRequestRepository.Object,
                _mockBloodRequestService.Object,
                _mockMapper.Object);
        }

        [Fact]
        public async Task AssignBloodUnitToNonExistentRequest_ShouldThrow_NullReferenceException()
        {
            // Arrange
            int validBloodUnitId = 1;
            int nonExistentRequestId = 999;

            // Setup a valid blood unit
            var bloodUnit = new BloodUnit
            {
                BloodUnitId = validBloodUnitId,
                BloodTypeId = 1,
                ComponentId = 1,
                Volume = 450,
                BloodUnitStatusId = 1
            };

            // Setup the mock to return the blood unit but null for the request
            _mockBloodUnitRepository.Setup(repo => repo.GetByIdAsync(validBloodUnitId))
                .ReturnsAsync(bloodUnit);
            
            _mockBloodRequestRepository.Setup(repo => repo.GetByIdAsync(nonExistentRequestId))
                .ReturnsAsync((BloodRequest)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NullReferenceException>(() => 
                _bloodUnitService.AssignBloodUnitToRequestAsync(validBloodUnitId, nonExistentRequestId));
            
            // Verify that the repositories were called correctly
            _mockBloodUnitRepository.Verify(repo => repo.GetByIdAsync(validBloodUnitId), Times.Once);
            _mockBloodRequestRepository.Verify(repo => repo.GetByIdAsync(nonExistentRequestId), Times.Once);
            
            // Verify that the update methods were never called (since we got an exception first)
            _mockBloodUnitRepository.Verify(repo => repo.UpdateAsync(It.IsAny<BloodUnit>()), Times.Never);
            _mockBloodUnitRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task AssignBloodUnitToNonExistentBloodUnit_ShouldThrow_KeyNotFoundException()
        {
            // Arrange
            int nonExistentBloodUnitId = 999;
            int validRequestId = 1;

            // Setup the mock to return null for the blood unit (non-existent unit)
            _mockBloodUnitRepository.Setup(repo => repo.GetByIdAsync(nonExistentBloodUnitId))
                .ReturnsAsync((BloodUnit)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => 
                _bloodUnitService.AssignBloodUnitToRequestAsync(nonExistentBloodUnitId, validRequestId));
            
            // Verify exception message
            Assert.Equal("Blood unit not found.", exception.Message);
            
            // Verify that repository was called correctly
            _mockBloodUnitRepository.Verify(repo => repo.GetByIdAsync(nonExistentBloodUnitId), Times.Once);
        }

        [Fact]
        public async Task AssignBloodUnitWithProperErrorHandling_ShouldCatchAndReportError()
        {
            // Arrange
            int validBloodUnitId = 1;
            int nonExistentRequestId = 999;
            var exceptionMessage = "Request not found";

            // Setup a valid blood unit
            var bloodUnit = new BloodUnit
            {
                BloodUnitId = validBloodUnitId,
                BloodTypeId = 1,
                ComponentId = 1,
                Volume = 450,
                BloodUnitStatusId = 1
            };

            // Mock the repositories to simulate the error scenario
            _mockBloodUnitRepository.Setup(repo => repo.GetByIdAsync(validBloodUnitId))
                .ReturnsAsync(bloodUnit);
            
            _mockBloodRequestRepository.Setup(repo => repo.GetByIdAsync(nonExistentRequestId))
                .ThrowsAsync(new KeyNotFoundException(exceptionMessage));

            // Create a wrapper service that implements proper error handling without logging
            var errorHandlingService = new BloodUnitErrorHandlingService(_bloodUnitService);

            // Act
            var result = await errorHandlingService.TryAssignBloodUnitToRequestAsync(validBloodUnitId, nonExistentRequestId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(exceptionMessage, result.ErrorMessage);
            
            // Verify the exception was caught (indirectly, by checking the result)
            Assert.NotNull(result.ErrorMessage);
        }

        [Fact]
        public async Task AssignBloodUnitSuccessfully_ShouldReturnSuccess()
        {
            // Arrange
            int validBloodUnitId = 1;
            int validRequestId = 2;
            
            // Setup valid entities
            var bloodUnit = new BloodUnit
            {
                BloodUnitId = validBloodUnitId,
                BloodTypeId = 1,
                ComponentId = 1,
                Volume = 450,
                BloodUnitStatusId = 1
            };
            
            var bloodRequest = new BloodRequest
            {
                RequestId = validRequestId,
                BloodTypeId = 1,
                BloodComponentId = 1,
                Volume = 500,
                RemainingVolume = 500
            };

            // Setup the mocks to return valid objects
            _mockBloodUnitRepository.Setup(repo => repo.GetByIdAsync(validBloodUnitId))
                .ReturnsAsync(bloodUnit);
                
            _mockBloodRequestRepository.Setup(repo => repo.GetByIdAsync(validRequestId))
                .ReturnsAsync(bloodRequest);
                
            _mockBloodUnitRepository.Setup(repo => repo.UpdateAsync(It.IsAny<BloodUnit>()))
                .ReturnsAsync(bloodUnit);
                
            _mockBloodRequestRepository.Setup(repo => repo.UpdateAsync(It.IsAny<BloodRequest>()))
                .ReturnsAsync(bloodRequest);
                
            _mockBloodUnitRepository.Setup(repo => repo.SaveChangesAsync())
                .ReturnsAsync(true);
                
            _mockBloodRequestRepository.Setup(repo => repo.SaveChangesAsync())
                .ReturnsAsync(true);

            // Create error handling service
            var errorHandlingService = new BloodUnitErrorHandlingService(_bloodUnitService);

            // Act
            var result = await errorHandlingService.TryAssignBloodUnitToRequestAsync(validBloodUnitId, validRequestId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.ErrorMessage);
        }
    }

    // Error handling service without logging dependency
    public class BloodUnitErrorHandlingService
    {
        private readonly IBloodUnitService _bloodUnitService;

        public BloodUnitErrorHandlingService(IBloodUnitService bloodUnitService)
        {
            _bloodUnitService = bloodUnitService;
        }

        public async Task<OperationResult> TryAssignBloodUnitToRequestAsync(int bloodUnitId, int requestId)
        {
            try
            {
                bool result = await _bloodUnitService.AssignBloodUnitToRequestAsync(bloodUnitId, requestId);
                return OperationResult.Success();
            }
            catch (KeyNotFoundException ex)
            {
                // Capture error details without logging
                return OperationResult.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                // Capture generic errors without logging
                return OperationResult.Failure(ex.Message);
            }
        }
    }

    // Result pattern for error handling
    public class OperationResult
    {
        public bool IsSuccess { get; private set; }
        public string ErrorMessage { get; private set; }

        private OperationResult(bool isSuccess, string errorMessage = null)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
        }

        public static OperationResult Success() => new OperationResult(true);
        public static OperationResult Failure(string errorMessage) => new OperationResult(false, errorMessage);
    }
}