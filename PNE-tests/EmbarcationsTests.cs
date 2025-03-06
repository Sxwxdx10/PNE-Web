using Microsoft.EntityFrameworkCore;
using PNE_core.Models;
using PNE_core.Services;
using PNE_core.Services.Interfaces;
using Moq;

namespace PNE_tests
{
    public class EmbarcationsTests
    {
        private static readonly Embarcation _embarcation = new() {
            IdEmbarcation = "1",
            Description = "un Bateau",
            Marque  = "Chevrolet",
            Longueur = 15
        };

        private static readonly Embarcation _embarcation2 = new()
        {
            IdEmbarcation = "2",
            Description = "un second Bateau",
            Marque = "Cadilac",
            Longueur = 18
        };

        private static readonly Embarcation[] _list = [
            _embarcation,
            _embarcation2
        ];

        internal EmbarcationService Service { get; private set; }
        internal Mock<DbSet<Embarcation>> mockEmbarcations { get; private set; }
        internal Mock<IPneDbContext> mockContext { get; private set; }
        //s'execute une fois par test
        public EmbarcationsTests()
        {
            mockEmbarcations = MockExtensions.CreateDbSetMock(_list.AsQueryable());
            mockContext = new Mock<IPneDbContext>();

            mockContext.Setup(m => m.Embarcations).Returns(mockEmbarcations.Object);
            mockContext.Setup(m => m.SaveChangesAsync());

            Service = new EmbarcationService(mockContext.Object);
        }

        [Fact]
        public async Task Create_shouldAddToDB()
        {
            //act
            await Service.CreateAsync(_embarcation);

            //Assert
            mockEmbarcations.Verify(m => m.Add(It.IsAny<Embarcation>()), Times.Once());
            mockContext.Verify(m => m.SaveChangesAsync(), Times.Once());
        }

        [Fact]
        public async Task Delete_shouldRemoveFromDB()
        {
            //act
            await Service.DeleteAsync(_embarcation.IdEmbarcation);

            //Assert
            mockEmbarcations.Verify(m => m.Remove(It.Is<Embarcation>(m => m == _embarcation)), Times.Once());
            mockContext.Verify(m => m.SaveChangesAsync(), Times.Once());
        }

        [Fact]
        public async Task GetAll_ShouldReturnList()
        {
            //act
            var result = await Service.GetAllAsync();

            //Assert
            Assert.Equal(_list, result);
        }

        [Fact]
        public async Task GetByIdAsync_shouldGetRightItem()
        {
            //act
            var result = await Service.GetByIdAsync(_embarcation.IdEmbarcation);

            //Assert
            Assert.Equal(_embarcation, result);
        }

        [Fact]
        public async Task GetForDetailAsync_shouldGetRightItem()
        {
            //act
            var result = await Service.GetForDetailAsync(_embarcation.IdEmbarcation);

            //Assert
            Assert.Equal(_embarcation, result);
        }

        [Fact]
        public async Task IsExist_getsTrue()
        {
            //act
            var result = await Service.IsExist(_embarcation.IdEmbarcation);

            //Assert
            Assert.True(result);
            mockContext.Verify(m => m.SaveChangesAsync(), Times.Never());
        }

        [Fact]
        public async Task IsExist_getsFalse()
        {
            //act
            var result = await Service.IsExist("53");

            //Assert
            Assert.False(result);
            mockContext.Verify(m => m.SaveChangesAsync(), Times.Never());
        }

        [Fact]
        public async Task UpdateAsync_updates()
        {
            //act
            await Service.UpdateAsync(_embarcation);

            //Assert
            mockEmbarcations.Verify(m => m.Update(It.IsAny<Embarcation>()), Times.Once());
            mockContext.Verify(m => m.SaveChangesAsync(), Times.Once());
        }
    }
}
