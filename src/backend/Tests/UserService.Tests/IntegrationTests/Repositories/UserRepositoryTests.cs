using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserService.DAL.Models;
using UserService.DAL.Repositories;

namespace UserService.Tests.IntegrationTests.Repositories
{
    [CollectionDefinition("Sequence", DisableParallelization = true)]
    public class UserRepositoryTests : IClassFixture<PostgreSqlSubstitute>
    {
        private readonly PostgreSqlSubstitute _postgreSqlSubstitute;
        private readonly UserRepository _userRepository;

        public UserRepositoryTests(PostgreSqlSubstitute substitute)
        {
            _postgreSqlSubstitute = substitute;
            _userRepository = new(
                _postgreSqlSubstitute.UserManager,
                _postgreSqlSubstitute.RoleManager);
        }

        [Fact]
        public async Task AddRoleAsync_WhenNewRoleNameProvided_CreatesNewRole()
        {
            //Arrange
            var roleName = "TEST";

            //Act
            var result = await _userRepository.AddRoleAsync(roleName);

            //Assert
            result.Should().Be(IdentityResult.Success);
            var dbRole = _postgreSqlSubstitute.RoleManager.FindByNameAsync(roleName);
            dbRole.Should().NotBeNull();
        }

        [Fact]
        public async Task AddRoleAsync_WhenExistingNameProvided_ThrowsInvalidOperationException()
        {
            //Arrange
            var roleName = "Admin";

            //Act
            var result = () => _userRepository.AddRoleAsync(roleName);

            //Assert
            result.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task AddUserAsync_WhenCorrectDataProvided_ShouldCreateNewUser()
        {
            //Arrange
            var testUser = TestDataProvider.SampleTestUser;
            var pwd = "Password123!";

            //Act
            var result = await _userRepository.AddUserAsync(testUser, pwd);

            //Assert
            result.Should().Be(IdentityResult.Success);

            var dbUser = await _postgreSqlSubstitute.UserManager.FindByEmailAsync(testUser.Email);
            dbUser.Should().NotBeNull();
            dbUser.Should().BeEquivalentTo(testUser);
        }

        [Fact]
        public async Task AddUserAsync_WhenExistingEmailProvided_ThrowsInvalidOperationException()
        {
            //Arrange
            var testUser = new User()
            {
                UserName = "initial",
                Email = "initial@initial.com"
            };
            var pwd = "Password123!";

            //Act
            var result = () => _userRepository.AddUserAsync(testUser, pwd);

            //Assert
            result.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task AssignRoleAsync_WhenValidRoleProvided_ShouldAssignUserToRole()
        {
            //Arrange
            var user = await _postgreSqlSubstitute.UserManager.Users.FirstAsync();
            var role = await _postgreSqlSubstitute.RoleManager.Roles.FirstAsync(x => x.Name != "Admin");

            //Act
            var result = await _userRepository.AssignRoleAsync(user, role.Name);

            //Assert
            result.Should().Be(IdentityResult.Success);

            var userRoles = await _postgreSqlSubstitute.UserManager.GetRolesAsync(user);
            userRoles.Should().Contain(role.Name);
        }

        [Fact]
        public async Task AssignRoleAsync_WhenInvalidRoleProvided_ThrowsInvalidOperationException()
        {
            //Arrange
            var user = await _postgreSqlSubstitute.UserManager.Users.FirstAsync();
            var role = "dfljkhjgbfd";

            //Act
            var result = () => _userRepository.AssignRoleAsync(user, role);

            //Assert
            result.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task GetUserByIdAsync_WhenExistingIdProvided_ReturnsData()
        {
            //Arrange
            var user = await _postgreSqlSubstitute.UserManager.Users.FirstAsync();

            //Act
            var result = await _userRepository.GetUserByIdAsync(user.Id.ToString());

            //Assert
            result.Should().BeEquivalentTo(user);
        }

        [Fact]
        public async Task GetUserByIdAsync_WhenNonExistentIdProvided_ReturnsNull()
        {
            //Arrange
            var userId = Guid.NewGuid();

            //Act
            var result = await _userRepository.GetUserByIdAsync(userId.ToString());

            //Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetUserByEmailAsync_WhenExistingEmailProvided_ReturnsData()
        {
            //Arrange
            var user = await _postgreSqlSubstitute.UserManager.Users.FirstAsync();

            //Act
            var result = await _userRepository.GetUserByEmailAsync(user.Email);

            //Assert
            result.Should().BeEquivalentTo(user);
        }

        [Fact]
        public async Task GetUserByEmailAsync_WhenNonExistentEmailProvided_ReturnsNull()
        {
            //Arrange
            var email = "dfdkjgnfdgn@fdlkdjgnjnfd.com";

            //Act
            var result = await _userRepository.GetUserByEmailAsync(email);

            //Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task ListRolesAsync_ShouldReturnAllIdentityRoles()
        {
            //Arrange
            var roles = await _postgreSqlSubstitute.RoleManager.Roles.Select(x => x.Name).ToListAsync();

            //Act
            var result = await _userRepository.ListRolesAsync();

            //Assert
            result.Should().BeEquivalentTo(roles);
        }

        [Fact]
        public async Task RoleExistsAsync_WhenExistingRoleProvided_ReturnsTrue()
        {
            //Arrange
            var role = await _postgreSqlSubstitute.RoleManager.Roles.FirstAsync();

            //Act
            var result = await _userRepository.RoleExistsAsync(role.Name);

            //Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task RoleExistsAsync_WhenNonExistentRoleProvided_ReturnsFalse()
        {
            //Arrange
            var role = "dfignfdglfd";

            //Act
            var result = await _userRepository.RoleExistsAsync(role);

            //Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task ListUsersAsync_ShouldReturnAllUsers()
        {
            //Arrange
            var users = await _postgreSqlSubstitute.UserManager.Users.ToListAsync();

            //Act
            var result = await _userRepository.ListUsersAsync();

            //Assert
            result.Should().BeEquivalentTo(users);
        }

        [Fact]
        public async Task ListUsersByRoleAsync_WhenValidRoleProvided_ReturnsUsers()
        {
            //Arrange
            var role = await _postgreSqlSubstitute.RoleManager.Roles.FirstAsync();
            var users = await _postgreSqlSubstitute.UserManager.GetUsersInRoleAsync(role.Name);

            //act
            var result = await _userRepository.ListUsersByRoleAsync(role.Name);

            //Assert
            result.Should().BeEquivalentTo(users);
        }

        [Fact]
        public async Task ListUsersByRoleAsync_WhenInvalidRoleProvided_ReturnsNullOrEmpty()
        {
            //Arrange
            var role = "dfjgfjd";
            var users = await _postgreSqlSubstitute.UserManager.GetUsersInRoleAsync(role);

            //act
            var result = await _userRepository.ListUsersByRoleAsync(role);

            //Assert
            result.Should().BeNullOrEmpty();
        }

        [Fact]
        public async Task ListUserRolesAsync_WhenValidUserProvided_ReturnsRoles()
        {
            //Arrange
            var user = await _postgreSqlSubstitute.UserManager.Users.FirstAsync();
            var roles = await _postgreSqlSubstitute.UserManager.GetRolesAsync(user);

            //Act
            var result = await _userRepository.ListUserRolesAsync(user);

            //Assert
            result.Should().BeEquivalentTo(roles);
        }

        [Fact]
        public async Task ListUserRolesAsync_WhenInvalidUserProvided_ThrowsInvalidOperationException()
        {
            //Arrange
            var user = TestDataProvider.SampleTestUser;
            var roles = await _postgreSqlSubstitute.UserManager.GetRolesAsync(user);

            //Act
            var result = () => _userRepository.ListUserRolesAsync(user);

            //Assert
            result.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task UpdateUserAsync_WhenNoConflictingDataProvided_ReturnsSuccess()
        {
            //Arrange
            var user = TestDataProvider.SampleTestUser;
            var newEmail = "newemail@email.com";
            await _postgreSqlSubstitute.UserManager.CreateAsync(user, "Password134!");

            user.Email = newEmail;

            //Act
            var result = await _userRepository.UpdateUserAsync(user);

            //Assert
            result.Should().Be(IdentityResult.Success);
            var dbUser = await _postgreSqlSubstitute.UserManager.FindByIdAsync(user.Id.ToString());
            dbUser.Email.Should().BeEquivalentTo(newEmail);
        }

        [Fact]
        public async Task UpdateUserAsync_WhenConflictingDataProvided_ThrowsInvalidOperationException()
        {
            //Arrange
            var user = TestDataProvider.SampleTestUser;
            await _postgreSqlSubstitute.UserManager.CreateAsync(user, "Password134!");
            
            var existingUser = await _postgreSqlSubstitute.UserManager.Users.FirstAsync();
            var newEmail = existingUser.Email;
            user.Email = newEmail;

            //Act
            var result = () => _userRepository.UpdateUserAsync(user);

            //Assert
            result.Should().ThrowAsync<InvalidOperationException>();
        }
    }
}
