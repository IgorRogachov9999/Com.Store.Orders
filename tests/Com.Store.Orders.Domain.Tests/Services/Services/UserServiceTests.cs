using AutoFixture.AutoMoq;
using AutoFixture;
using Com.Store.Orders.Domain.Tests.Customizations;
using Moq;
using Com.Store.Orders.Domain.Data.Repositories.Contracts;
using Com.Store.Orders.Domain.Services.Services;
using Com.Store.Orders.Domain.Services.Exceptions;
using Com.Store.Orders.Domain.Data.Models;
using FluentAssertions;

namespace Com.Store.Orders.Domain.Tests.Services.Services
{
    public class UserServiceTests
    {
        private Mock<IUserRepository> _userRepository;
        private UserService _service;
        private IFixture _fixture;

        public UserServiceTests()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new AutoMapperCustomization());

            _userRepository = _fixture.Freeze<Mock<IUserRepository>>();
            _service = _fixture.Create<UserService>();
        }

        [Fact]
        public async Task GetByEmailAndPassowrdAsync_Should_ThrowDomainEntityNotFoundException_When_NoUser()
        {
            // Arrange
            var ct = CancellationToken.None;
            var email = _fixture.Create<string>();
            var password = _fixture.Create<string>();
            _userRepository
                .Setup(x => x.GetByEmailAndPasswordAsync(It.IsAny<string>(), It.IsAny<string>(), ct))
                .ReturnsAsync((UserModel)null);

            // Act & Assert
            await Assert.ThrowsAsync<DomainEntityNotFoundException>(
                async () => await _service.GetByEmailAndPassowrdAsync(email, password, ct));
        }

        [Fact]
        public async Task GetByEmailAndPassowrdAsync_Should_ReturnUser()
        {
            // Arrange
            var ct = CancellationToken.None;
            var email = _fixture.Create<string>();
            var password = _fixture.Create<string>();
            var userModel = _fixture.Create<UserModel>();
            _userRepository
                .Setup(x => x.GetByEmailAndPasswordAsync(It.IsAny<string>(), It.IsAny<string>(), ct))
                .ReturnsAsync(userModel);

            // Act
            var result = await _service.GetByEmailAndPassowrdAsync(email, password, ct);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(userModel.Id);
        }
    }
}
