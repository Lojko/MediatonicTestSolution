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
    public class TestAnimalController
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

        public TestAnimalController()
        {
            var options = new DbContextOptionsBuilder<DataContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            context = new DataContext(options);
            db = new FakePersistantDataStorage();
            db.PopulateContext(context);
        }

        [Fact]
        public async Task TestGetAllAnimal()
        {
            var controller = new AnimalsController(context);
            var animals = await controller.GetAnimals();
            Assert.Equal(EXPECTED_SIZE_OF_ALL, animals.Value.Count());
        }

        [Fact]
        public async Task TestGetAnimalAppropriateId()
        {
            var controller = new AnimalsController(context);
            var animal = await controller.GetAnimal(ID_TO_FIND);

            var actionResult = Assert.IsType<ActionResult<Animal>>(animal);
            Assert.NotNull(actionResult);
            Assert.Equal(ID_TO_FIND, actionResult.Value.Id);
        }

        [Fact]
        public async Task TestGetAnimalInappropriateId()
        {
            var controller = new AnimalsController(context);
            var animal = await controller.GetAnimal(INAPPROPRIATE_ID_TO_FIND);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Animal>>(animal);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task TestPutAnimalAppropriateId()
        {
            var controller = new AnimalsController(context);
            Animal animal = context.Animal.Find(ID_TO_FIND);

            animal.Type = CHANGED_TEXT;

            var modifiedAnimal = await controller.PutAnimal(animal.Id, animal);

            var actionResult = Assert.IsType<ActionResult<Animal>>(modifiedAnimal);
            Assert.NotNull(actionResult);
            Assert.Equal(CHANGED_TEXT, actionResult.Value.Type);
            animal = context.Animal.Find(ID_TO_FIND);
            Assert.Equal(CHANGED_TEXT, animal.Type);
        }

        [Fact]
        public async Task TestPutAnimalInappropriateId()
        {
            var controller = new AnimalsController(context);
            Animal animal = context.Animal.Find(ID_TO_FIND);

            var modifiedOwnedAnimal = await controller.PutAnimal(INAPPROPRIATE_ID_TO_FIND, animal);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Animal>>(modifiedOwnedAnimal);
            Assert.IsType<BadRequestResult>(actionResult.Result);
        }

        [Fact]
        public async Task TestPostAnimalAppropriateId()
        {
            var controller = new AnimalsController(context);
            Animal animal = context.Animal.Find(ID_TO_FIND);
            animal.Id = EXPECTED_SIZE_OF_ALL + 1;
            animal.Type = CHANGED_TEXT;
            var newAnimal = await controller.PostAnimal(animal);
            var actionResult = Assert.IsType<ActionResult<Animal>>(newAnimal);
            Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            Assert.NotNull(actionResult);
            long expectedSize = EXPECTED_SIZE_OF_ALL + 1;
            animal = context.Animal.Find(expectedSize);
            Assert.Equal(CHANGED_TEXT, animal.Type);
            Assert.Equal(expectedSize, animal.Id);
        }


        [Fact]
        public async Task TestDeleteAnimalAppropriateId()
        {
            var controller = new AnimalsController(context);
            Animal animal = context.Animal.Find(ID_TO_FIND);
            Assert.False(animal.IsDeleted);

            var deletedAnimal = await controller.DeleteAnimal(ID_TO_FIND);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Animal>>(deletedAnimal);
            Assert.True(deletedAnimal.Value.IsDeleted);
            animal = context.Animal.Find(ID_TO_FIND);
            Assert.True(animal.IsDeleted);
        }

        [Fact]
        public async Task TestDeleteAnimalInappropriateId()
        {
            var controller = new AnimalsController(context);
            var ownedAnimal = await controller.DeleteAnimal(INAPPROPRIATE_ID_TO_FIND);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Animal>>(ownedAnimal);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }
    }
}
