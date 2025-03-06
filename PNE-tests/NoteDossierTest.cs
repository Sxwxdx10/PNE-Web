using Microsoft.EntityFrameworkCore;
using PNE_core.Models;
using PNE_core.Services;
using PNE_core.Services.Interfaces;
using Moq;

namespace PNE_tests
{
    public class NoteDossierTest
    {
        private static readonly Notedossier[] _list = [
            new Notedossier {
                Idnote = "1",
                Date = DateTime.Now,
            },
            new Notedossier {
                Idnote = "2",
                Date = DateTime.Now,
            },
            new Notedossier {
                Idnote = "3",
                Date = DateTime.Now,
            }
        ];

        internal NoteDossierService Service { get; private set; }
        internal Mock<DbSet<Notedossier>> mockNotes { get; private set; }
        internal Mock<IPneDbContext> mockContext { get; private set; }
        //s'execute une fois par test
        public NoteDossierTest()
        {
            mockNotes = MockExtensions.CreateDbSetMock(_list.AsQueryable());
            mockContext = new Mock<IPneDbContext>();

            mockContext.Setup(m => m.Notedossiers).Returns(mockNotes.Object);
            mockContext.Setup(m => m.SaveChangesAsync());

            Service = new NoteDossierService(mockContext.Object);
        }

        [Fact]
        public async Task Create_shouldAddToDB()
        {
            await Service.CreateAsync(new Notedossier
            {
                Idnote = "4",
                Date = DateTime.Now,
            });

            mockNotes.Verify(m => m.Add(It.IsAny<Notedossier>()), Times.Once());
            mockContext.Verify(m => m.SaveChangesAsync(), Times.Once());
        }

        [Fact]
        public async Task Delete_shouldRemoveFromDB()
        {
            await Service.DeleteAsync(_list[1].Idnote);

            mockNotes.Verify(m => m.Remove(It.IsAny<Notedossier>()), Times.Once());
            mockContext.Verify(m => m.SaveChangesAsync(), Times.Once());
        }

        [Fact]
        public async Task GetAll_ShouldReturnList()
        {
            var result = await Service.GetAllAsync();

            Assert.Equal(_list, result);
        }

        [Fact]
        public async Task GetByIdAsync_shouldGetRightItem()
        {
            var result = await Service.GetByIdAsync(_list[1].Idnote);

            Assert.Equal(_list[1], result);
        }

        [Fact]
        public async Task GetForDetailAsync_shouldGetRightItem()
        {
            var result = await Service.GetByIdAsync(_list[1].Idnote);

            Assert.Equal(_list[1], result);
        }

        [Fact]
        public async Task IsExist_getsTrue()
        {
            bool exists = await Service.IsExist(_list[1].Idnote);

            Assert.True(exists);
        }

        [Fact]
        public async Task IsExist_getsFalse()
        {
            bool exists = await Service.IsExist("abracadabra");

            Assert.False(exists);
        }
    }
}
