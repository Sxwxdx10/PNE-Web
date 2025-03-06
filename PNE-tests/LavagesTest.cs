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
    public class LavagesTest
    {
        private static readonly Lavage _Lavage = new()
        {
            IdLavage = "0",
            Date = DateTime.Now,
            IdEmbarcation = "0",
            SelfServe = true
        };
        private static readonly Lavage _Lavage2 = new()
        {
            IdLavage = "1",
            Date = DateTime.Now,
            IdEmbarcation = "1",
            SelfServe = false
        };

        private static readonly Lavage[] _list = [
                _Lavage,
                _Lavage2
        ];

        internal LavageService Service { get; private set; }
        internal Mock<DbSet<Lavage>> mockLavage { get; private set; }
        internal Mock<IPneDbContext> mockContext { get; private set; }
        //s'execute une fois par test
        public LavagesTest()
        {
            mockLavage = MockExtensions.CreateDbSetMock(_list.AsQueryable());
            mockContext = new Mock<IPneDbContext>();

            mockContext.Setup(m => m.Lavages).Returns(mockLavage.Object);
            mockContext.Setup(m => m.SaveChangesAsync());

            Service = new LavageService(mockContext.Object);
        }

        [Fact]
        public async Task Create_shouldAddToDB()
        {
            //act
            await Service.CreateAsync(_Lavage);

            //Assert
            mockLavage.Verify(m => m.Add(It.IsAny<Lavage>()), Times.Once());
            mockContext.Verify(m => m.SaveChangesAsync(), Times.Once());
        }

        [Fact]
        public async Task Delete_shouldRemoveFromDB()
        {
            //act
            await Service.DeleteAsync(_Lavage.IdLavage);

            //Assert
            mockLavage.Verify(m => m.Remove(It.Is<Lavage>(m => m == _Lavage)), Times.Once());
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
            var result = await Service.GetByIdAsync(_Lavage.IdLavage);

            //Assert
            Assert.Equal(_Lavage, result);
        }

        [Fact]
        public async Task GetForDetailAsync_shouldGetRightItem()
        {
            //act
            var result = await Service.GetForDetailAsync(_Lavage.IdLavage);

            //Assert
            Assert.Equal(_Lavage, result);
        }

        [Fact]
        public async Task IsExist_getsTrue()
        {
            //act
            var result = await Service.IsExist(_Lavage.IdLavage);

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
            await Service.UpdateAsync(_Lavage);

            //Assert
            mockLavage.Verify(m => m.Update(It.IsAny<Lavage>()), Times.Once());
            mockContext.Verify(m => m.SaveChangesAsync(), Times.Once());
        }
    }
}