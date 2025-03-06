using Microsoft.EntityFrameworkCore;
using Moq;
using PNE_core.Models;
using PNE_core.Services.Interfaces;
using PNE_core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNE_tests
{
    public class StationLavageTest
    {
        private static readonly StationLavage _station = new()
        {
            Id = "1",
            Nom = "Estrie"
        };

        private static readonly StationLavage _station2 = new()
        {
            Id = "2",
            Nom = "Mort"
        };

        private static readonly StationLavage[] _list = [
            _station,
            _station2
        ];
        internal StationLavageService Service { get; private set; }
        internal Mock<DbSet<StationLavage>> mockStation { get; private set; }
        internal Mock<IPneDbContext> mockContext { get; private set; }

        public StationLavageTest()
        {
            mockStation = MockExtensions.CreateDbSetMock(_list.AsQueryable());
            mockContext = new Mock<IPneDbContext>();

            mockContext.Setup(m => m.StationLavages).Returns(mockStation.Object);
            mockContext.Setup(m => m.SaveChangesAsync());

            Service = new StationLavageService(mockContext.Object);
        }

        [Fact]
        public async Task Create_shouldAddToDB()
        {
            //act
            await Service.CreateAsync(_station);

            //Assert
            mockStation.Verify(m => m.Add(It.IsAny<StationLavage>()), Times.Once());
            mockContext.Verify(m => m.SaveChangesAsync(), Times.Once());
        }

        [Fact]
        public async Task Delete_shouldRemoveFromDB()
        {
            //act
            await Service.DeleteAsync(_station.Id);

            //Assert
            mockStation.Verify(m => m.Remove(It.Is<StationLavage>(m => m == _station)), Times.Once());
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
            var result = await Service.GetByIdAsync(_station.Id);

            //Assert
            Assert.Equal(_station, result);
        }

        [Fact]
        public async Task GetForDetailAsync_shouldGetRightItem()
        {
            //act
            var result = await Service.GetForDetailAsync(_station.Id);

            //Assert
            Assert.Equal(_station, result);
        }

        [Fact]
        public async Task IsExist_getsTrue()
        {
            //act
            var result = await Service.IsExist(_station.Id);

            //Assert
            Assert.True(result);
            mockContext.Verify(m => m.SaveChangesAsync(), Times.Never());
        }

        [Fact]
        public async Task IsExist_getsFalse()
        {
            //act
            var result = await Service.IsExist("94");

            //Assert
            Assert.False(result);
            mockContext.Verify(m => m.SaveChangesAsync(), Times.Never());
        }

        [Fact]
        public async Task UpdateAsync_updates()
        {
            //act
            await Service.UpdateAsync(_station);

            //Assert
            mockStation.Verify(m => m.Update(It.IsAny<StationLavage>()), Times.Once());
            mockContext.Verify(m => m.SaveChangesAsync(), Times.Once());
        }




    }
}
