using MediatonicTest.Controllers;
using MediatonicTest.Data;
using MediatonicTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTestMediatonicTest
{
    public class TestUsersController
    {
        private DataContext context;

        private FakePersistantDataStorage db;

        //Default ID to use when using constructed objects for testing, expected ID to return
        private const long ID_TO_FIND = 1;

        //Default ID to use when using constructed objects for testing, expected ID to return
        private const long INAPPROPRIATE_ID_TO_FIND = -1;

        //int value representing the count of the mock data context
        private const int EXPECTED_SIZE_OF_ALL = 3;

        //Default test string for altering a string on an object
        private const string CHANGED_TEXT = "CHANGED";

        public TestUsersController()
        {
            var options = new DbContextOptionsBuilder<DataContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            context = new DataContext(options);
            db = new FakePersistantDataStorage();
            db.PopulateContext(context);
        }

        [Fact]
        public async Task TestGetAllUsers()
        {
            var controller = new UsersController(context);
            var users = await controller.GetUser();
            Assert.Equal(EXPECTED_SIZE_OF_ALL, users.Value.Count());
        }

        [Fact]
        public async Task TestGetUserAppropriateId()
        {
            var controller = new UsersController(context);
            var user = await controller.GetUser(ID_TO_FIND);

            var actionResult = Assert.IsType<ActionResult<User>>(user);
            Assert.NotNull(actionResult);
            Assert.Equal(ID_TO_FIND, actionResult.Value.Id);
        }

        [Fact]
        public async Task TestGetUserInappropriateId()
        {
            var controller = new UsersController(context);
            var user = await controller.GetUser(INAPPROPRIATE_ID_TO_FIND);

            // Assert
            var actionResult = Assert.IsType<ActionResult<User>>(user);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task TestPutUserAppropriateId()
        {
            var controller = new UsersController(context);
            User user = context.User.Find(ID_TO_FIND);

            user.Username = CHANGED_TEXT;

            var modifiedUser = await controller.PutUser(user.Id, user);

            var actionResult = Assert.IsType<ActionResult<User>>(modifiedUser);
            Assert.NotNull(actionResult);
            Assert.Equal(CHANGED_TEXT, actionResult.Value.Username);
            user = context.User.Find(ID_TO_FIND);
            Assert.Equal(CHANGED_TEXT, user.Username);
        }

        [Fact]
        public async Task TestPutUserInappropriateId()
        {
            var controller = new UsersController(context);
            User user = context.User.Find(ID_TO_FIND);

            var modifiedUser = await controller.PutUser(INAPPROPRIATE_ID_TO_FIND, user);

            // Assert
            var actionResult = Assert.IsType<ActionResult<User>>(modifiedUser);
            Assert.IsType<BadRequestResult>(actionResult.Result);
        }

        [Fact]
        public async Task TestPostUserAppropriateId()
        {
            var controller = new UsersController(context);
            User user = context.User.Find(ID_TO_FIND);
            user.Id = EXPECTED_SIZE_OF_ALL + 1;
            user.Username = CHANGED_TEXT;
            var newUser = await controller.PostUser(user);
            var actionResult = Assert.IsType<ActionResult<User>>(newUser);
            Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            Assert.NotNull(actionResult);
            long expectedSize = EXPECTED_SIZE_OF_ALL + 1;
            user = context.User.Find(expectedSize);
            Assert.Equal(CHANGED_TEXT, user.Username);
            Assert.Equal(expectedSize, user.Id);
        }

        [Fact]
        public async Task TestDeleteUserAppropriateId()
        {
            var controller = new UsersController(context);
            User user = context.User.Find(ID_TO_FIND);
            Assert.False(user.IsDeleted);

            var deletedUser = await controller.DeleteUser(ID_TO_FIND);

            // Assert
            var actionResult = Assert.IsType<ActionResult<User>>(deletedUser);
            Assert.True(deletedUser.Value.IsDeleted);
            user = context.User.Find(ID_TO_FIND);
            Assert.True(user.IsDeleted);
        }

        [Fact]
        public async Task TestDeleteUserInappropriateId()
        {
            var controller = new UsersController(context);
            var user = await controller.DeleteUser(INAPPROPRIATE_ID_TO_FIND);

            // Assert
            var actionResult = Assert.IsType<ActionResult<User>>(user);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }
    }
}
