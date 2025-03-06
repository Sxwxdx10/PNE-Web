using Microsoft.EntityFrameworkCore;
using Moq;
using PNE_core.Models;
using PNE_core.Services;
using PNE_core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PNE_tests
{
    public class CertificationTest
    {
        private static readonly Certification[] _list = [
            new Certification(){
                CodeCertification = "1",
                NomFormation = "Forklift Certified"
            },
            new Certification(){
                CodeCertification = "2",
                NomFormation = "Lavages de bateaux"
            }
        ];


        internal CertificationService Service { get; private set; }
        internal Mock<DbSet<Certification>> mockCerti { get; private set; }
        internal Mock<IPneDbContext> mockContext { get; private set; }
        //s'execute une fois par test
        public CertificationTest()
        {
            mockCerti = MockExtensions.CreateDbSetMock(_list.AsQueryable());
            mockContext = new Mock<IPneDbContext>();

            mockContext.Setup(m => m.Certifications).Returns(mockCerti.Object);
            mockContext.Setup(m => m.SaveChangesAsync());

            Service = new CertificationService(mockContext.Object);
        }

        [Fact]
        public async Task CertifyUser_test()
        {
            Utilisateur[] _listUsers = [
                new Utilisateur(){
                    Id = "1",
                    DisplayName = "Bob",
                }
            ];
            CertificationUtilisateur[] _listCertiUsers = [
                new CertificationUtilisateur(){

                }
            ];

            var mockUser = MockExtensions.CreateDbSetMock(_listUsers.AsQueryable());
            var mockCertiUsers = MockExtensions.CreateDbSetMock(_listCertiUsers.AsQueryable());

            mockContext.Setup(m => m.Utilisateurs).Returns(mockUser.Object);
            mockContext.Setup(m => m.CertificationUtilisateurs).Returns(mockCertiUsers.Object);

            await Service.CertifyUser(_list[0].CodeCertification, _listUsers[0].Id);

            mockCertiUsers.Verify(m => m.Add(It.IsAny<CertificationUtilisateur>()), Times.Once());
            mockContext.Verify(m => m.SaveChangesAsync(), Times.Once());
        }

        [Fact]
        public async Task Create_shouldAddToDB()
        {
            await Service.CreateAsync(new Certification());

            mockCerti.Verify(m => m.Add(It.IsAny<Certification>()), Times.Once());
            mockContext.Verify(m => m.SaveChangesAsync(), Times.Once());
        }

        [Fact]
        public async Task Delete_shouldRemoveFromDB()
        {
            await Service.DeleteAsync(_list[1].CodeCertification);

            mockCerti.Verify(m => m.Remove(It.IsAny<Certification>()), Times.Once());
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
            var res = await Service.GetByIdAsync(_list[0].CodeCertification);

            Assert.Equal(_list[0], res);
        }

        [Fact]
        public async Task GetForDetailAsync_shouldGetRightItem()
        {
            var res = await Service.GetForDetailAsync(_list[0].CodeCertification);

            Assert.Equal(_list[0], res);
        }

        [Fact]
        public async Task IsExist_getsTrue()
        {
            bool res = await Service.IsExist(_list[0].CodeCertification);
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
            _list[1].NomFormation = "livreur";
            await Service.UpdateAsync(_list[1]);

            mockCerti.Verify(m => m.Update(It.IsAny<Certification>()), Times.Once());
            mockContext.Verify(m => m.SaveChangesAsync(), Times.Once());
        }
    }
}
