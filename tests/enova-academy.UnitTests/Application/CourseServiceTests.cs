using Xunit;
using Moq;
using System;
using System.Threading.Tasks;
using enova_academy.Domain.Repositories;
using enova_academy.Domain.Entities;
using enova_academy.Application.Services;
using enova_academy.Application.DTOs;

namespace MyApp.UnitTests.Application
{
    public class CourseServiceTests
    {
        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenSlugAlreadyExists()
        {
            // Arrange
            var repoMock = new Mock<ICourseRepository>();
            repoMock.Setup(r => r.GetBySlugAsync("dotnet", false))
                    .ReturnsAsync(new Course("Existente", "dotnet", 100m, 10));

            var service = new CourseService(repoMock.Object);

            // Ajuste a construção do DTO conforme o seu DTO real (construtor/props)
            var dto = new CourseDto { Title = "Novo", Slug = "dotnet", Price = 200m, Capacity = 30 };

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.CreateAsync(dto));

            // Verifica que GetBySlugAsync foi chamado
            repoMock.Verify(r => r.GetBySlugAsync("dotnet", false), Times.Once);
        }
    }
}
