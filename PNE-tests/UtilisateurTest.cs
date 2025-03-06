using Microsoft.EntityFrameworkCore;
using PNE_core.Models;
using PNE_core.Services;
using PNE_core.Services.Interfaces;
using Moq;

namespace PNE_tests
{
    public class UtilisateurTest
    {
        private static readonly Utilisateur[] _list = [
            new Utilisateur
            {
                DisplayName = "Bob",
                DateCreation = DateTime.Now,
                Id = "1"
            },
            new Utilisateur
            {
                DisplayName = "Jean",
                DateCreation = DateTime.Now,
                Id = "2"
            }
        ];

        internal UtilisateursService Service { get; private set; }
        internal Mock<DbSet<Utilisateur>> mockUtilisateurs { get; private set; }
        internal Mock<IPneDbContext> mockContext { get; private set; }
        //s'execute une fois par test
        public UtilisateurTest()
        {
            mockUtilisateurs = MockExtensions.CreateDbSetMock(_list.AsQueryable());
            mockContext = new Mock<IPneDbContext>();

            mockContext.Setup(m => m.Utilisateurs).Returns(mockUtilisateurs.Object);
            mockContext.Setup(m => m.SaveChangesAsync());

            Service = new UtilisateursService(mockContext.Object);
        }
        [Fact]
        public async void Create_shouldAddToDB()
        {
            await Service.CreateAsync(_list[0]);

            //Assert
            mockUtilisateurs.Verify(u => u.Add(_list[0]), Times.Once());
            mockContext.Verify(u => u.SaveChangesAsync(), Times.Once());

        }
        [Fact]
        public async void Delete_shouldRemoveFromDB()
        {
            await Service.DeleteAsync(_list[0].Id);

            //Assert
            mockUtilisateurs.Verify(m => m.Remove(It.Is<Utilisateur>(m => m == _list[0])), Times.Once());
            mockContext.Verify(m => m.SaveChangesAsync(), Times.Once());
        }

        [Fact]
        public async Task GetAll_ShouldReturnList()
        {
            var res = await Service.GetAllAsync();

            Assert.Equal(_list, res);
        }

        [Fact]
        public async Task GetByIdAsync_shouldGetRightItem()
        {
            var res = await Service.GetByIdAsync(_list[0].Id);

            Assert.Equal(_list[0], res);
        }

        [Fact]
        public async Task GetForDetailAsync_shouldGetRightItem()
        {
            var res = await Service.GetForDetailAsync(_list[0].Id);

            Assert.Equal(_list[0], res);
        }

        [Fact]
        public async Task IsExist_getsTrue()
        {
            bool res = await Service.IsExist(_list[0].Id);
            Assert.True(res);
        }

        [Fact]
        public async Task IsExist_getsFalse()
        {
            bool res = await Service.IsExist(null);
            Assert.False(res);
        }

        [Fact]
        public async Task UpdateAsync_updates()
        {
            _list[1].DisplayName = "Daniel";
            await Service.UpdateAsync(_list[1]);

            mockUtilisateurs.Verify(m => m.Update(It.IsAny<Utilisateur>()), Times.Once());
            mockContext.Verify(m => m.SaveChangesAsync(), Times.Once());
        }
    }
}