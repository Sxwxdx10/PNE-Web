using Microsoft.EntityFrameworkCore;
using PNE_core.Models;
using PNE_core.Services;
using PNE_core.Services.Interfaces;
using Moq;
using Xunit.Sdk;

namespace PNE_tests
{
    public class MiseAEauTest
    {
        private static readonly Miseaeau _miseAEau = new()
        {
            IdMiseEau = "0",
            Date = DateTime.Now,
            IdPlanEau = "0",
            IdEmbarcation = "0"
        };
        private static readonly Miseaeau _miseAEau2 = new()
        {
            IdMiseEau = "1",
            Date = DateTime.Now,
            IdPlanEau = "1",
            IdEmbarcation = "1",
        };

        private static readonly Miseaeau[] _list = [
                _miseAEau,
                _miseAEau2
        ];

        internal MiseAEauService Service { get; private set; }
        internal Mock<DbSet<Miseaeau>> mockMiseAEau { get; private set; }
        internal Mock<IPneDbContext> mockContext { get; private set; }
        //s'execute une fois par test
        public MiseAEauTest()
        {
            mockMiseAEau = MockExtensions.CreateDbSetMock(_list.AsQueryable());
            mockContext = new Mock<IPneDbContext>();

            mockContext.Setup(m => m.Miseaeaus).Returns(mockMiseAEau.Object);
            mockContext.Setup(m => m.SaveChangesAsync());

            Service = new MiseAEauService(mockContext.Object);
        }

        [Fact]
        public async Task Create_shouldAddToDB()
        {
            //act
            await Service.CreateAsync(_miseAEau);

            //Assert
            mockMiseAEau.Verify(m => m.Add(It.IsAny<Miseaeau>()), Times.Once());
            mockContext.Verify(m => m.SaveChangesAsync(), Times.Once());
        }

        [Fact]
        public async Task Delete_shouldRemoveFromDB()
        {
            //act
            await Service.DeleteAsync(_miseAEau.IdMiseEau);

            //Assert
            mockMiseAEau.Verify(m => m.Remove(It.Is<Miseaeau>(m => m == _miseAEau)), Times.Once());
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
            var result = await Service.GetByIdAsync(_miseAEau.IdMiseEau);

            //Assert
            Assert.Equal(_miseAEau, result);
        }

        [Fact]
        public async Task GetForDetailAsync_shouldGetRightItem()
        {
            //act
            var result = await Service.GetForDetailAsync(_miseAEau.IdMiseEau);

            //Assert
            Assert.Equal(_miseAEau, result);
        }

        [Fact]
        public async Task IsExist_getsTrue()
        {
            //act
            var result = await Service.IsExist("0");

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsExist_getsFalse()
        {
            //act
            var result = await Service.IsExist("52");

            //Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateAsync_updates()
        {
            //act
            await Service.UpdateAsync(_miseAEau);

            //Assert
            mockMiseAEau.Verify(m => m.Update(It.IsAny<Miseaeau>()), Times.Once());
            mockContext.Verify(m => m.SaveChangesAsync(), Times.Once());
        }
    }
}
