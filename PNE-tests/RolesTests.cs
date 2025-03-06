using Microsoft.EntityFrameworkCore;
using PNE_core.Models;
using PNE_core.Services;
using PNE_core.Services.Interfaces;
using Moq;

namespace PNE_tests
{
    public class RolesTests
    {
        internal static readonly Role[] _listRoles = [
            new Role{NomRole = "utilisateur"},
            new Role{NomRole = "gerant"},
            new Role{NomRole = "employe"},
        ];

        internal static readonly Utilisateur[] _listUsers = [
            new Utilisateur{
                DisplayName = "Bob",
                DateCreation = DateTime.Now,
                Id = "1"
            },
            new Utilisateur{
                DisplayName = "Alice",
                DateCreation = DateTime.Now,
                Id = "2"
            }
        ];

        internal static readonly RolesUtilisateurs[] _listRolesUsers = [
            new RolesUtilisateurs{
                IdRolesUtilisateurs = 1,
                nom_role = "utilisateur",
                IdUtilisateur = "1",
                Role = _listRoles[0],
                Utilisateur = _listUsers[0]
            },
            new RolesUtilisateurs{
                IdRolesUtilisateurs = 2,
                nom_role = "utilisateur",
                IdUtilisateur = "2",
                Role = _listRoles[0],
                Utilisateur = _listUsers[1]
            },
            new RolesUtilisateurs{
                IdRolesUtilisateurs = 3,
                nom_role = "employe",
                IdUtilisateur = "1",
                Role = _listRoles[2],
                Utilisateur = _listUsers[0]
            },
        ];

        internal RoleService Service { get; private set; }
        internal Mock<DbSet<Role>> mockRoles { get; private set; }
        internal Mock<DbSet<Utilisateur>> mockUsers { get; private set; }
        internal Mock<DbSet<RolesUtilisateurs>> mockRolesUsers { get; private set; }
        internal Mock<IPneDbContext> mockContext { get; private set; }
        //s'execute une fois par test
        public RolesTests()
        {
            mockRoles = MockExtensions.CreateDbSetMock(_listRoles.AsQueryable());
            mockUsers = MockExtensions.CreateDbSetMock(_listUsers.AsQueryable());
            mockRolesUsers = MockExtensions.CreateDbSetMock(_listRolesUsers.AsQueryable());
            mockContext = new Mock<IPneDbContext>();

            mockContext.Setup(m => m.Roles).Returns(mockRoles.Object);
            mockContext.Setup(m => m.Utilisateurs).Returns(mockUsers.Object);
            mockContext.Setup(m => m.RolesUtilisateurs).Returns(mockRolesUsers.Object);
            mockContext.Setup(m => m.SaveChangesAsync());

            Service = new RoleService(mockContext.Object);
        }

        [Fact]
        public async Task AddUserRole_ValidTest()
        {
            await Service.AddUserRole(_listUsers[1].Id, _listRoles[1].NomRole);

            mockRolesUsers.Verify(m => m.Add(It.IsAny<RolesUtilisateurs>()), Times.Once());
            mockContext.Verify(m => m.SaveChangesAsync(), Times.Once());
        }

        [Fact]
        public async Task AddUserRole_InvalidTest()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(() => Service.AddUserRole(null, _listRoles[1].NomRole));

            mockRolesUsers.Verify(m => m.Add(It.IsAny<RolesUtilisateurs>()), Times.Never());
            mockContext.Verify(m => m.SaveChangesAsync(), Times.Never());
        }

        [Fact]
        public async Task RemoveUserRole_ValidTest()
        {
            await Service.RemoveUserRole(_listUsers[0].Id, _listRoles[0].NomRole);

            mockRolesUsers.Verify(m => m.Remove(It.IsAny<RolesUtilisateurs>()), Times.Once());
            mockContext.Verify(m => m.SaveChangesAsync(), Times.Once());
        }

        [Fact]
        public async Task RemoveUserRole_UserInvalidTest()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(() => Service.RemoveUserRole(null, _listRoles[0].NomRole));

            mockRolesUsers.Verify(m => m.Remove(It.IsAny<RolesUtilisateurs>()), Times.Never());
            mockContext.Verify(m => m.SaveChangesAsync(), Times.Never());
        }

        [Fact]
        public async Task RemoveUserRole_PasDeRoleTest()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(() => Service.RemoveUserRole(_listUsers[0].Id, _listRoles[1].NomRole));

            mockRolesUsers.Verify(m => m.Remove(It.IsAny<RolesUtilisateurs>()), Times.Never());
            mockContext.Verify(m => m.SaveChangesAsync(), Times.Never());
        }

        [Fact]
        public async Task GetUserRoles()
        {
            //arrange
            List<Role> expectedRes = [
                _listRoles[0],
                _listRoles[2],
            ];

            var result = await Service.GetUserRoles(_listUsers[0].Id);

            Assert.Equal(expectedRes, result);
        }
    }
}