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
    public class TestAnimalOwnershipsController
    {
        private DataContext context;

        private FakePersistantDataStorage db;

        //Default ID to use when using constructed objects for testing, expected ID to return
        private const long ID_TO_FIND = 1;

        //Default ID to use when using constructed objects for testing, expected ID to return
        private const long INAPPROPRIATE_ID_TO_FIND = -1;

        //int value representing the count of the mock data context
        private const int EXPECTED_SIZE_OF_ALL = 9;

        //Default test string for altering a string on an object
        private const string CHANGED_TEXT = "CHANGED";

        public TestAnimalOwnershipsController()
        {
            var options = new DbContextOptionsBuilder<DataContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            context = new DataContext(options);
            db = new FakePersistantDataStorage();
            db.PopulateContext(context);
        }

        [Fact]
        public async Task TestGetAllAnimalOwnership()
        {
            var controller = new AnimalOwnershipsController(context);
            var ownedAnimals = await controller.GetAnimalOwnership();
            Assert.Equal(EXPECTED_SIZE_OF_ALL, ownedAnimals.Value.Count());
        }

        [Fact]
        public async Task TestGetAnimalOwnershipAppropriateId()
        {
            var controller = new AnimalOwnershipsController(context);
            var ownedAnimal = await controller.GetAnimalOwnership(ID_TO_FIND);

            var actionResult = Assert.IsType<ActionResult<AnimalOwnership>>(ownedAnimal);
            Assert.NotNull(actionResult);
            Assert.Equal(ID_TO_FIND, actionResult.Value.Id);
        }

        [Fact]
        public async Task TestGetAnimalOwnershipInappropriateId()
        {
            var controller = new AnimalOwnershipsController(context);
            var ownedAnimal = await controller.GetAnimalOwnership(INAPPROPRIATE_ID_TO_FIND);

            // Assert
            var actionResult = Assert.IsType<ActionResult<AnimalOwnership>>(ownedAnimal);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task TestPutAnimalOwnershipAppropriateId()
        {
            var controller = new AnimalOwnershipsController(context);
            AnimalOwnership ownedAnimal = context.AnimalOwnership.Find(ID_TO_FIND);

            ownedAnimal.Name = CHANGED_TEXT;

            var modifiedOwnedAnimal = await controller.PutAnimalOwnership(ownedAnimal.Id, ownedAnimal);

            var actionResult = Assert.IsType<ActionResult<AnimalOwnership>>(modifiedOwnedAnimal);
            Assert.NotNull(actionResult);
            Assert.Equal(CHANGED_TEXT, actionResult.Value.Name);
            ownedAnimal = context.AnimalOwnership.Find(ID_TO_FIND);
            Assert.Equal(CHANGED_TEXT, ownedAnimal.Name);
        }

        [Fact]
        public async Task TestPutAnimalOwnershipInappropriateId()
        {
            var controller = new AnimalOwnershipsController(context);
            AnimalOwnership ownedAnimal = context.AnimalOwnership.Find(ID_TO_FIND);

            var modifiedOwnedAnimal = await controller.PutAnimalOwnership(INAPPROPRIATE_ID_TO_FIND, ownedAnimal);

            // Assert
            var actionResult = Assert.IsType<ActionResult<AnimalOwnership>>(modifiedOwnedAnimal);
            Assert.IsType<BadRequestResult>(actionResult.Result);
        }

        [Fact]
        public async Task TestPostAnimalOwnershipAppropriateId()
        {
            var controller = new AnimalOwnershipsController(context);
            AnimalOwnership ownedAnimal = context.AnimalOwnership.Find(ID_TO_FIND);
            ownedAnimal.Id = EXPECTED_SIZE_OF_ALL + 1;
            ownedAnimal.Name = CHANGED_TEXT;
            var newOwnedAnimal = await controller.PostAnimalOwnership(ownedAnimal);
            var actionResult = Assert.IsType<ActionResult<AnimalOwnership>>(newOwnedAnimal);
            Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            Assert.NotNull(actionResult);
            long expectedSize = EXPECTED_SIZE_OF_ALL + 1;
            ownedAnimal = context.AnimalOwnership.Find(expectedSize);
            Assert.Equal(CHANGED_TEXT, ownedAnimal.Name);
            Assert.Equal(expectedSize, ownedAnimal.Id);
        }

        [Fact]
        public async Task TestPostAnimalOwnershipAppropriateIdResetDefaults()
        {
            var controller = new AnimalOwnershipsController(context);
            AnimalOwnership ownedAnimal = context.AnimalOwnership.Find(ID_TO_FIND);
            //Increment the values
            ownedAnimal.Feed();
            ownedAnimal.Stroke();
            ownedAnimal.Id = EXPECTED_SIZE_OF_ALL + 1;
            ownedAnimal.Name = CHANGED_TEXT;
            var newOwnedAnimal = await controller.PostAnimalOwnership(ownedAnimal);
            var actionResult = Assert.IsType<ActionResult<AnimalOwnership>>(newOwnedAnimal);
            Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            Assert.NotNull(actionResult);
            long expectedSize = EXPECTED_SIZE_OF_ALL + 1;
            ownedAnimal = context.AnimalOwnership.Find(expectedSize);
            Assert.Equal(CHANGED_TEXT, ownedAnimal.Name);
            Assert.Equal(expectedSize, ownedAnimal.Id);
            Assert.Equal(ownedAnimal.Animal.HungerDefault, ownedAnimal.Hunger);
            Assert.Equal(ownedAnimal.Animal.HappinessDefault, ownedAnimal.Happiness);
        }

        [Fact]
        public async Task TestPostAnimalOwnershipInappropriateId()
        {
            var controller = new AnimalOwnershipsController(context);
            AnimalOwnership ownedAnimal = context.AnimalOwnership.Find(ID_TO_FIND);
            ownedAnimal.AnimalId = INAPPROPRIATE_ID_TO_FIND;
            var newOwnedAnimal = await controller.PostAnimalOwnership(ownedAnimal);

            // Assert
            var actionResult = Assert.IsType<ActionResult<AnimalOwnership>>(newOwnedAnimal);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task TestDeleteAnimalOwnershipAppropriateId()
        {
            var controller = new AnimalOwnershipsController(context);
            AnimalOwnership ownedAnimal = context.AnimalOwnership.Find(ID_TO_FIND);
            Assert.False(ownedAnimal.IsDeleted);

            var deletedOwnedAnimal = await controller.DeleteAnimalOwnership(ID_TO_FIND);

            // Assert
            var actionResult = Assert.IsType<ActionResult<AnimalOwnership>>(deletedOwnedAnimal);
            Assert.True(deletedOwnedAnimal.Value.IsDeleted);
            ownedAnimal = context.AnimalOwnership.Find(ID_TO_FIND);
            Assert.True(ownedAnimal.IsDeleted);
        }

        [Fact]
        public async Task TestDeleteAnimalOwnershipInappropriateId()
        {
            var controller = new AnimalOwnershipsController(context);
            var ownedAnimal = await controller.DeleteAnimalOwnership(INAPPROPRIATE_ID_TO_FIND);

            // Assert
            var actionResult = Assert.IsType<ActionResult<AnimalOwnership>>(ownedAnimal);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }
    }
}
