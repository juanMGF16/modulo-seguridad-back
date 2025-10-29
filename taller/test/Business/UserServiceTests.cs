﻿using AutoMapper;
using Business.Interfaces.IBusinessImplements;
using Business.Services;
using Data.Interfaces.IDataImplement;
using Entity.Domain.Models.Implements;
using Entity.DTOs.Default;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Utilities.Exceptions;

namespace test.Business
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly Mock<IRolUserService> _mockRolUserService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<UserService>> _mockLogger;
        private readonly UserService _service;

        public UserServiceTests()
        {
            _mockUserRepo = new Mock<IUserRepository>();
            _mockRolUserService = new Mock<IRolUserService>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<UserService>>();

            _service = new UserService(
                _mockUserRepo.Object,
                _mockLogger.Object,
                _mockMapper.Object,
                _mockRolUserService.Object
            );
        }

        // ================================================================
        // ✅ TEST 1: CreateAsyncUser crea el usuario correctamente
        // ================================================================
        [Fact]
        public async Task CreateAsyncUser_ShouldReturnMappedDto_WhenSuccess()
        {
            // Arrange
            var dto = new UserDto { Id = 1, Email = "test@mail.com" };
            var userEntity = new User { Id = 1, Email = "test@mail.com" };

            _mockMapper.Setup(m => m.Map<User>(dto)).Returns(userEntity);
            _mockUserRepo.Setup(r => r.CreateAsync(userEntity)).ReturnsAsync(userEntity);
            _mockMapper.Setup(m => m.Map<UserDto>(userEntity)).Returns(dto);

            // Act
            var result = await _service.CreateAsyncUser(dto);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Email.Should().Be("test@mail.com");

            // Verificamos que se llame a AsignateUserRTo con el ID del usuario creado
            _mockRolUserService.Verify(s => s.AsignateUserRTo(userEntity.Id), Times.Once);
        }

        // ================================================================
        // ❌ TEST 2: CreateAsyncUser lanza excepción si el DTO es nulo
        // ================================================================
        [Fact]
        public async Task CreateAsyncUser_ShouldThrow_WhenDtoIsNull()
        {
            // Act
            Func<Task> act = async () => await _service.CreateAsyncUser(null!);

            // Assert
            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage("*El DTO no puede ser nulo*");
        }

        // ================================================================
        // ❌ TEST 3: CreateAsyncUser lanza BusinessException si falla el repositorio
        // ================================================================
        [Fact]
        public async Task CreateAsyncUser_ShouldThrowBusinessException_WhenRepositoryFails()
        {
            var dto = new UserDto { Id = 1, Email = "test@mail.com" };
            var userEntity = new User { Id = 1, Email = "test@mail.com" };

            _mockMapper.Setup(m => m.Map<User>(dto)).Returns(userEntity);
            _mockUserRepo.Setup(r => r.CreateAsync(It.IsAny<User>()))
                         .ThrowsAsync(new Exception("DB error"));

            Func<Task> act = async () => await _service.CreateAsyncUser(dto);

            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage("*Error al crear el registro*");
        }

        // ================================================================
        // ✅ TEST 4: UpdateAsyncUser actualiza correctamente el usuario
        // ================================================================
        [Fact]
        public async Task UpdateAsyncUser_ShouldReturnTrue_WhenUpdateSucceeds()
        {
            // Arrange
            var dto = new UserDto { Id = 1, Email = "test@mail.com" };
            var entity = new User { Id = 1, Email = "test@mail.com" };

            _mockMapper.Setup(m => m.Map<User>(dto)).Returns(entity);
            _mockUserRepo.Setup(r => r.UpdateAsync(entity)).ReturnsAsync(true);

            // Act
            var result = await _service.UpdateAsyncUser(dto);

            // Assert
            result.Should().BeTrue();
        }

        // ================================================================
        // ❌ TEST 5: UpdateAsyncUser lanza excepción si DTO es nulo
        // ================================================================
        [Fact]
        public async Task UpdateAsyncUser_ShouldThrow_WhenDtoIsNull()
        {
            Func<Task> act = async () => await _service.UpdateAsyncUser(null!);

            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage("*El DTO no puede ser nulo*");
        }

        // ================================================================
        // ❌ TEST 6: UpdateAsyncUser retorna false si repositorio falla
        // ================================================================
        [Fact]
        public async Task UpdateAsyncUser_ShouldReturnFalse_WhenRepositoryFails()
        {
            // Arrange
            var dto = new UserDto { Id = 1, Email = "test@mail.com" };
            var entity = new User { Id = 1, Email = "test@mail.com" };

            _mockMapper.Setup(m => m.Map<User>(dto)).Returns(entity);
            _mockUserRepo.Setup(r => r.UpdateAsync(entity)).ReturnsAsync(false);

            // Act
            var result = await _service.UpdateAsyncUser(dto);

            // Assert
            result.Should().BeFalse();
        }
    }
}
