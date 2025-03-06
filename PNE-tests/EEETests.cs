using Microsoft.EntityFrameworkCore;
using Moq;
using PNE_core.Enums;
using PNE_core.Models;
using PNE_core.Services.Interfaces;
using PNE_core.Services;

namespace PNE_tests
{
    public class EEETests
    {
        private static readonly Planeau[] _PlanEau = [
            new Planeau() {
                IdPlanEau = "1",
                Nom = "lac",
                NiveauCouleur = Niveau.Jaune
            },
            new Planeau(){
                IdPlanEau = "2",
                Nom = "l'autre lac",
                NiveauCouleur = Niveau.Vert
            }
        ];

        private static readonly EEE[] _EEE = [
            new EEE() {
                Id = "1",
                Name = "pieuvre",
                NiveauCouleur = Niveau.Jaune
            },
            new EEE(){
                Id = "2",
                Name = "calemar",
                NiveauCouleur = Niveau.Jaune
            },
            new EEE(){
                Id = "3",
                Name = "john",
                NiveauCouleur = Niveau.Rouge
            }
        ];

        private static readonly EEEPlanEau[] _EEEPlanEau = [
            new EEEPlanEau(){
                Id = "1",
                IdEEE = "1",
                IdPlanEau = "1",
                EEE = _EEE[0],
                PlanEauNavigation = _PlanEau[0],
                Validated = true
            },
            new EEEPlanEau(){
                Id = "2",
                IdEEE = "3",
                IdPlanEau = "2",
                EEE = _EEE[2],
                PlanEauNavigation = _PlanEau[1],
                Validated = false
            }
        ];
        internal EEEService Service { get; private set; }
        internal Mock<DbSet<Planeau>> mockPlaneau { get; private set; }
        internal Mock<DbSet<EEEPlanEau>> mockEEEPlaneau { get; private set; }
        internal Mock<DbSet<EEE>> mockEEE { get; private set; }
        internal Mock<IPneDbContext> mockContext { get; private set; }

        public EEETests()
        {
            mockPlaneau = MockExtensions.CreateDbSetMock(_PlanEau.AsQueryable());
            mockEEEPlaneau = MockExtensions.CreateDbSetMock(_EEEPlanEau.AsQueryable());
            mockEEE = MockExtensions.CreateDbSetMock(_EEE.AsQueryable());
            mockContext = new Mock<IPneDbContext>();

            mockContext.Setup(m => m.Planeaus).Returns(mockPlaneau.Object);
            mockContext.Setup(m => m.EEEs).Returns(mockEEE.Object);
            mockContext.Setup(m => m.EEEPlanEaus).Returns(mockEEEPlaneau.Object);
            mockContext.Setup(m => m.SaveChangesAsync());

            Service = new EEEService(mockContext.Object);
        }

        [Fact]
        public async Task SignaleEEE_ExistePas()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(() => Service.SignalerEEE("",""));
            mockContext.Verify(m => m.SaveChangesAsync(), Times.Never());
        }

        [Fact]
        public async Task SignaleEEE_EEEDejaPresente()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(() => Service.SignalerEEE("1", "1"));
            mockContext.Verify(m => m.SaveChangesAsync(), Times.Never());
        }

        [Fact]
        public async Task SignaleEEE_valid()
        {
            await Service.SignalerEEE("3", "1");

            mockEEEPlaneau.Verify(m => m.Add(It.Is<EEEPlanEau>(e => 
                e.IdPlanEau == "1" &&
                e.IdEEE == "3" &&
                e.Validated == false
            )), Times.Once());
            mockContext.Verify(m => m.SaveChangesAsync(), Times.Once());
        }

        [Fact]
        public async Task ConfirmerEEE_ExistePas()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(() => Service.ConfirmerEEE("", ""));
            mockContext.Verify(m => m.SaveChangesAsync(), Times.Never());
        }

        [Fact]
        public async Task ConfirmerEEE_pasSignalisation()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(() => Service.ConfirmerEEE("2", "1"));
            mockContext.Verify(m => m.SaveChangesAsync(), Times.Never());
        }

        [Fact]
        public async Task ConfirmerEEE_SignalisationConfirme()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(() => Service.ConfirmerEEE("1", "1"));
            mockContext.Verify(m => m.SaveChangesAsync(), Times.Never());
        }

        [Fact]
        public async Task ConfirmerEEE_ValidAndColorUpdate()
        {
            await Service.ConfirmerEEE("3", "2");

            mockEEEPlaneau.Verify(m => m.Update(It.Is<EEEPlanEau>(e =>
                e.IdPlanEau == "2" &&
                e.IdEEE == "3" &&
                e.Validated == true
            )), Times.Once());
            
            mockPlaneau.Verify(m => m.Update(It.Is<Planeau>(e =>
                e.IdPlanEau == _PlanEau[1].IdPlanEau &&
                e.Nom == _PlanEau[1].Nom &&
                e.NiveauCouleur == _EEE[2].NiveauCouleur
            )), Times.Once());

            mockContext.Verify(m => m.SaveChangesAsync(), Times.Once());        
        }

        [Fact]
        public async Task RetirerEEE_PlanEauInvalide()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(() => Service.RetirerEEE("", ""));
            mockContext.Verify(m => m.SaveChangesAsync(), Times.Never());
        }

        [Fact]
        public async Task RetirerEEE_pasSignale()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(() => Service.RetirerEEE("2", "2"));
            mockContext.Verify(m => m.SaveChangesAsync(), Times.Never());
        }

        [Fact]
        public async Task RetirerEEE_validEtUpdateCouleur()
        {
            await Service.RetirerEEE("1", "1");

            mockEEEPlaneau.Verify(m => m.Remove(It.Is<EEEPlanEau>(e =>
                e.IdPlanEau == "1" &&
                e.IdEEE == "1"
            )), Times.Once());
            mockPlaneau.Verify(m => m.Update(It.Is<Planeau>(e =>
                e.IdPlanEau == _PlanEau[0].IdPlanEau &&
                e.Nom == _PlanEau[0].Nom  &&
                e.NiveauCouleur == Niveau.Vert
            )),Times.Once());
            mockContext.Verify(m => m.SaveChangesAsync(), Times.Once());
        }

        [Fact]
        public async Task RetirerEEE_validEtUpdatePasCouleur()
        {
            await Service.RetirerEEE("3", "2");

            mockEEEPlaneau.Verify(m => m.Remove(It.Is<EEEPlanEau>(e =>
                e.IdPlanEau == "2" &&
                e.IdEEE == "3"
            )), Times.Once());
            mockPlaneau.Verify(m => m.Update(It.IsAny<Planeau>()), Times.Never());
            mockContext.Verify(m => m.SaveChangesAsync(), Times.Once());
        }

        [Fact]
        public async Task Create_shouldAddToDB()
        {
            //act
            await Service.CreateAsync(_EEE[0]);

            //Assert
            mockEEE.Verify(m => m.Add(It.IsAny<EEE>()), Times.Once());
            mockContext.Verify(m => m.SaveChangesAsync(), Times.Once());
        }

        [Fact]
        public async Task Delete_shouldRemoveFromDB()
        {
            //act
            await Service.DeleteAsync(_EEE[0].Id);

            //Assert
            mockEEE.Verify(m => m.Remove(It.Is<EEE>(m => m == _EEE[0])), Times.Once());
            mockContext.Verify(m => m.SaveChangesAsync(), Times.Once());
        }

        [Fact]
        public async Task GetAll_ShouldReturnList()
        {
            //act
            var result = await Service.GetAllAsync();

            //Assert
            Assert.Equal(_EEE, result);
        }

        [Fact]
        public async Task GetByIdAsync_shouldGetRightItem()
        {
            //act
            var result = await Service.GetByIdAsync(_EEE[0].Id);

            //Assert
            Assert.Equal(_EEE[0], result);
        }

        [Fact]
        public async Task GetForDetailAsync_shouldGetRightItem()
        {
            //act
            var result = await Service.GetForDetailAsync(_EEE[0].Id);

            //Assert
            Assert.Equal(_EEE[0], result);
        }

        [Fact]
        public async Task IsExist_getsTrue()
        {
            //act
            var result = await Service.IsExist(_EEE[0].Id);

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
            await Service.UpdateAsync(_EEE[0]);

            //Assert
            mockEEE.Verify(m => m.Update(It.IsAny<EEE>()), Times.Once());
            mockContext.Verify(m => m.SaveChangesAsync(), Times.Once());
        }
    }
}
