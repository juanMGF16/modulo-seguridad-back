using AutoMapper;
using Business.Services;
using Data.Interfaces.DataBasic;
using Data.Interfaces.IDataImplement;
using Entity.Domain.Enums;
using Entity.Domain.Models.Implements;
using Entity.DTOs.Select;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Utilities.Exceptions;

namespace test.Business
{
    public class RolFormPermissionServiceTests
    {
        private readonly Mock<IData<RolFormPermission>> _mockData;
        private readonly Mock<IRolFormPermissionRepository> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<RolFormPermissionService>> _mockLogger;
        private readonly RolFormPermissionService _service;

        public RolFormPermissionServiceTests()
        {
            _mockData = new Mock<IData<RolFormPermission>>();
            _mockRepository = new Mock<IRolFormPermissionRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<RolFormPermissionService>>();

            _service = new RolFormPermissionService(
                _mockData.Object,
                _mockMapper.Object,
                _mockRepository.Object,
                _mockLogger.Object
            );
        }

        // ================================================================
        // ✅ TEST 1: GetAllAsync retorna los registros correctamente
        // ================================================================
        [Fact]
        public async Task GetAllAsync_ShouldReturnMappedDtos()
        {
            // Arrange
            var entities = new List<RolFormPermission>
            {
                new RolFormPermission { Id = 1, RolId = 10, FormId = 100, PermissionId = 1000 },
                new RolFormPermission { Id = 2, RolId = 11, FormId = 101, PermissionId = 1001 }
            };

            var expectedDtos = new List<RolFormPermissionSelectDto>
            {
                new RolFormPermissionSelectDto { Id = 1, RolId = 10, FormId = 100, PermissionId = 1000 },
                new RolFormPermissionSelectDto { Id = 2, RolId = 11, FormId = 101, PermissionId = 1001 }
            };

            // Simulamos que el repositorio devuelve entidades
            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(entities);

            // Simulamos el mapeo correcto a DTO
            _mockMapper.Setup(m => m.Map<IEnumerable<RolFormPermissionSelectDto>>(entities))
                       .Returns(expectedDtos);

            // Act
            var result = await _service.GetAllAsync(GetAllType.GetAll);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(expectedDtos);
        }

        // ================================================================
        // ✅ TEST 2: GetAllAsync lanza BusinessException si falla el repositorio
        // ================================================================
        [Fact]
        public async Task GetAllAsync_ShouldThrowBusinessException_WhenRepositoryFails()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetAllAsync()).ThrowsAsync(new Exception("DB connection error"));

            // Act
            Func<Task> act = async () => await _service.GetAllAsync(GetAllType.GetAll);

            // Assert
            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage("*Error al obtener todos los registros*");
        }

        // ================================================================
        // ✅ TEST 3: GetByIdAsync retorna un DTO mapeado correctamente
        // ================================================================
        [Fact]
        public async Task GetByIdAsync_ShouldReturnMappedDto_WhenEntityExists()
        {
            // Arrange
            var entity = new RolFormPermission { Id = 5, RolId = 50, FormId = 500, PermissionId = 5000 };
            var expectedDto = new RolFormPermissionSelectDto { Id = 5, RolId = 50, FormId = 500, PermissionId = 5000 };

            _mockRepository.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(entity);
            _mockMapper.Setup(m => m.Map<RolFormPermissionSelectDto?>(entity)).Returns(expectedDto);

            // Act
            var result = await _service.GetByIdAsync(5);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(5);
            result.RolId.Should().Be(50);
            result.FormId.Should().Be(500);
        }

        // ================================================================
        // ❌ TEST 4: GetByIdAsync lanza excepción si ID es inválido
        // ================================================================
        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public async Task GetByIdAsync_ShouldThrow_WhenIdIsInvalid(int invalidId)
        {
            // Act
            Func<Task> act = async () => await _service.GetByIdAsync(invalidId);

            // Assert
            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage($"*Error al obtener el registro con ID {invalidId}*");
        }

        // ================================================================
        // ❌ TEST 5: GetByIdAsync lanza BusinessException si falla el repositorio
        // ================================================================
        [Fact]
        public async Task GetByIdAsync_ShouldThrowBusinessException_WhenRepositoryFails()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                           .ThrowsAsync(new Exception("DB error"));

            // Act
            Func<Task> act = async () => await _service.GetByIdAsync(10);

            // Assert
            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage("*Error al obtener el registro con ID 10*");
        }
    }
}
