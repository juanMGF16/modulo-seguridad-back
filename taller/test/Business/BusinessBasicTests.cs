using AutoMapper;
using Business.Repository;
using Data.Interfaces.DataBasic;
using Entity.Domain.Enums;
using FluentAssertions;
using Moq;
using test.Dtos;
using Utilities.Exceptions;

namespace test.Business
{
    public class BusinessBasicTests
    {
        private readonly Mock<IData<FakeEntity>> _mockData;
        private readonly Mock<IMapper> _mockMapper;
        private readonly BusinessBasic<FakeSelectDto, FakeCreateDto, FakeEntity> _service;

        public BusinessBasicTests()
        {
            _mockData = new Mock<IData<FakeEntity>>();
            _mockMapper = new Mock<IMapper>();

            _service = new BusinessBasic<FakeSelectDto, FakeCreateDto, FakeEntity>(_mockData.Object, _mockMapper.Object);
        }

        // ==========================================
        // ✅ TEST 1: GetAllAsync()
        // ==========================================
        [Fact]
        public async Task GetAllAsync_ShouldReturnMappedDtos()
        {
            // Arrange
            var entities = new List<FakeEntity> { new FakeEntity { Id = 1, Name = "A" } };
            _mockData.Setup(d => d.GetAllAsync()).ReturnsAsync(entities);
            _mockMapper.Setup(m => m.Map<IEnumerable<FakeSelectDto>>(entities))
                       .Returns(new List<FakeSelectDto> { new FakeSelectDto { Name = "A" } });

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            result.Should().NotBeNull().And.HaveCount(1);
            result.First().Name.Should().Be("A");
        }

        // ==========================================
        // ✅ TEST 2: GetByIdAsync()
        // ==========================================
        [Fact]
        public async Task GetByIdAsync_ShouldReturnMappedDto_WhenExists()
        {
            var entity = new FakeEntity { Id = 1, Name = "A" };
            _mockData.Setup(d => d.GetByIdAsync(1)).ReturnsAsync(entity);
            _mockMapper.Setup(m => m.Map<FakeSelectDto>(entity))
                       .Returns(new FakeSelectDto { Name = "A" });

            var result = await _service.GetByIdAsync(1);

            result.Should().NotBeNull();
            result!.Name.Should().Be("A");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrow_WhenIdIsInvalid()
        {
            Func<Task> act = async () => await _service.GetByIdAsync(0);
            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage("*Error al obtener el registro con ID*");
        }

        // ==========================================
        // ✅ TEST 3: CreateAsync()
        // ==========================================
        [Fact]
        public async Task CreateAsync_ShouldMapAndPersistEntity()
        {
            var dto = new FakeCreateDto { Name = "New" };
            var entity = new FakeEntity { Id = 1, Name = "New" };

            _mockMapper.Setup(m => m.Map<FakeEntity>(dto)).Returns(entity);
            _mockData.Setup(d => d.CreateAsync(entity)).ReturnsAsync(entity);
            _mockMapper.Setup(m => m.Map<FakeCreateDto>(entity)).Returns(dto);

            var result = await _service.CreateAsync(dto);

            result.Should().NotBeNull();
            result.Name.Should().Be("New");
        }

        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenDtoIsNull()
        {
            Func<Task> act = async () => await _service.CreateAsync(null!);
            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage("*Error al crear el registro*");
        }

        // ==========================================
        // ✅ TEST 4: UpdateAsync()
        // ==========================================
        [Fact]
        public async Task UpdateAsync_ShouldReturnTrue_WhenSuccessful()
        {
            var dto = new FakeCreateDto { Name = "Update" };
            var entity = new FakeEntity { Id = 1, Name = "Update" };

            _mockMapper.Setup(m => m.Map<FakeEntity>(dto)).Returns(entity);
            _mockData.Setup(d => d.UpdateAsync(entity)).ReturnsAsync(true);

            var result = await _service.UpdateAsync(dto);

            result.Should().BeTrue();
        }

        // ==========================================
        // ✅ TEST 5: DeleteAsync()
        // ==========================================
        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenDeleted()
        {
            _mockData.Setup(d => d.DeleteAsync(1)).ReturnsAsync(true);

            var result = await _service.DeleteAsync(1);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrow_WhenIdInvalid()
        {
            Func<Task> act = async () => await _service.DeleteAsync(0);
            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage("*Error al eliminar el registro con ID*");
        }

        // ==========================================
        // ✅ TEST 6: DeleteAsync with Strategy
        // ==========================================
        [Fact]
        public async Task DeleteAsync_WithStrategy_ShouldInvokeFactory()
        {
            // Simula que el Delete lógico devuelve true
            _mockData.Setup(d => d.DeleteLogicAsync(1)).ReturnsAsync(true);

            var result = await _service.DeleteAsync(1, DeleteType.Logical);

            result.Should().BeTrue();
        }


        // ==========================================
        // ✅ TEST 7: RestoreLogical()
        // ==========================================
        [Fact]
        public async Task RestoreLogical_ShouldReturnTrue_WhenRestored()
        {
            _mockData.Setup(d => d.RestoreAsync(1)).ReturnsAsync(true);

            var result = await _service.RestoreLogical(1);

            result.Should().BeTrue();
        }
    }
}
