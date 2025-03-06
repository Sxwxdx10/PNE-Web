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
    public class PlanEauTest
    {
        private static readonly Planeau _plan = new()
        {
            IdPlanEau = "1",
            Nom = "lac"
        };

        private static readonly Planeau _plan2 = new()
        {
            IdPlanEau = "2",
            Nom = "l'autre lac"
        };

        private static readonly Planeau[] _list = [
            _plan,
            _plan2
        ];

        internal PlanEauService Service { get; private set; }
        internal Mock<DbSet<Planeau>> mockPlaneau { get; private set; }
        internal Mock<IPneDbContext> mockContext { get; private set; }
        //s'execute une fois par test
        public PlanEauTest()
        {
            mockPlaneau = MockExtensions.CreateDbSetMock(_list.AsQueryable());
            mockContext = new Mock<IPneDbContext>();

            mockContext.Setup(m => m.Planeaus).Returns(mockPlaneau.Object);
            mockContext.Setup(m => m.SaveChangesAsync());

            Service = new PlanEauService(mockContext.Object);
        }

        [Fact]
        public async Task Create_shouldAddToDB()
        {
            //act
            await Service.CreateAsync(_plan);

            //Assert
            mockPlaneau.Verify(m => m.Add(It.IsAny<Planeau>()), Times.Once());
            mockContext.Verify(m => m.SaveChangesAsync(), Times.Once());
        }

        [Fact]
        public async Task Delete_shouldRemoveFromDB()
        {
            //act
            await Service.DeleteAsync(_plan.IdPlanEau);

            //Assert
            mockPlaneau.Verify(m => m.Remove(It.Is<Planeau>(m => m == _plan)), Times.Once());
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
            var result = await Service.GetByIdAsync(_plan.IdPlanEau);

            //Assert
            Assert.Equal(_plan, result);
        }

        [Fact]
        public async Task GetForDetailAsync_shouldGetRightItem()
        {
            //act
            var result = await Service.GetForDetailAsync(_plan.IdPlanEau);

            //Assert
            Assert.Equal(_plan, result);
        }

        [Fact]
        public async Task IsExist_getsTrue()
        {
            //act
            var result = await Service.IsExist(_plan.IdPlanEau);

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
            await Service.UpdateAsync(_plan);

            //Assert
            mockPlaneau.Verify(m => m.Update(It.IsAny<Planeau>()), Times.Once());
            mockContext.Verify(m => m.SaveChangesAsync(), Times.Once());
        }
    }
}
