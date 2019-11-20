using MediatonicTest.Controllers;
using MediatonicTest.Data;
using MediatonicTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Xunit;

namespace XUnitTestMediatonicTest
{
    public class TestMediatonicTestController
    {
        private DataContext context;

        private FakePersistantDataStorage db;

        //Default ID to use when using constructed objects for testing, expected ID to return
        private const long ID_TO_FIND = 1;

        //Default ID to use when using constructed objects for testing, expected ID to return
        private const long ID_OF_ANIMAL_CREATED_IN_PAST_BY_AN_HOUR = 8;

        //Default ID to use when using constructed objects for testing, expected ID to return
        private const long ID_OF_ANIMAL_CREATED_IN_PAST = 9;

        //Default ID to use when using constructed objects for testing, expected ID to return
        private const long INAPPROPRIATE_ID_TO_FIND = -1;

        //int value representing the count of the mock data context
        private const int EXPECTED_SIZE_OF_ALL = 9;

        //int value representing the count of the mock data context
        private const int EXPECTED_SIZE_PER_USER = 3;

        //Modification of default happiness and hunger are resolved through creation and thus cannot be set directly, they are 0 by default
        private const int MOCK_ANIMAL_DEFAULT_HUNGER = 0;

        //Modification of default happiness and hunger are resolved through creation and thus cannot be set directly, they are 0 by default
        private const int MOCK_ANIMAL_DEFAULT_HAPPINESS = 0;


        public TestMediatonicTestController()
        {
            var options = new DbContextOptionsBuilder<DataContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            context = new DataContext(options);
            db = new FakePersistantDataStorage();
            db.PopulateContext(context);
        }

        [Fact]
        public void TestGetAllOwnedAnimals()
        {
            var controller = new MediatonicTestController(context);
            IEnumerable<OwnedAnimal> ownedAnimals = controller.Get();

            int i = 0;
            foreach (OwnedAnimal animal in ownedAnimals)
            {
                i++;
            }

            Assert.Equal(EXPECTED_SIZE_OF_ALL, i);
        }

        [Fact]
        public void TestGetAnimalsForUser()
        {
            var controller = new MediatonicTestController(context);
            IEnumerable<OwnedAnimal> ownedAnimals = controller.GetAnimalsForUser(ID_TO_FIND);

            int i = 0;
            foreach (OwnedAnimal animal in ownedAnimals)
            {
                i++;
            }

            Assert.Equal(EXPECTED_SIZE_PER_USER, i);
        }

        [Fact]
        public async void TestGetAnimalAppropriately()
        {
            var controller = new MediatonicTestController(context);
            var ownedAnimal = await controller.GetAnimal(ID_TO_FIND);

            var actionResult = Assert.IsType<ActionResult<OwnedAnimal>>(ownedAnimal);
            Assert.NotNull(actionResult);
            Assert.Equal(ID_TO_FIND, actionResult.Value.Id);
        }

        [Fact]
        public async void TestGetAnimalDegradation()
        {
            var controller = new MediatonicTestController(context);
            var animalOwnership = context.AnimalOwnership.Find(ID_OF_ANIMAL_CREATED_IN_PAST_BY_AN_HOUR);
            DateTime updatedTime = animalOwnership.LastUpdated;
            Assert.Equal(DateTime.Now.Hour - 1, updatedTime.Hour);
            var animal = context.Animal.Find(animalOwnership.AnimalId);

            //Assert current state is equal to defaults
            Assert.Equal(MOCK_ANIMAL_DEFAULT_HAPPINESS, animalOwnership.Happiness);
            Assert.Equal(MOCK_ANIMAL_DEFAULT_HUNGER, animalOwnership.Hunger);

            var ownedAnimal = await controller.GetAnimal(ID_OF_ANIMAL_CREATED_IN_PAST_BY_AN_HOUR);
            var actionResult = Assert.IsType<ActionResult<OwnedAnimal>>(ownedAnimal);
            Assert.NotNull(actionResult);
            Assert.Equal(ID_OF_ANIMAL_CREATED_IN_PAST_BY_AN_HOUR, actionResult.Value.Id);
            Assert.Equal(MOCK_ANIMAL_DEFAULT_HUNGER + animal.HungerIncrease, ownedAnimal.Value.Hunger);
            Assert.Equal(MOCK_ANIMAL_DEFAULT_HAPPINESS - animal.HappinessDecrease, ownedAnimal.Value.Happiness);
        }

        [Fact]
        public async void TestGetAnimalInappropriately()
        {
            var controller = new MediatonicTestController(context);
            var ownedAnimal = await controller.GetAnimal(INAPPROPRIATE_ID_TO_FIND);

            var actionResult = Assert.IsType<ActionResult<OwnedAnimal>>(ownedAnimal);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async void TestGetAnimalCreatedInThePast()
        {
            var controller = new MediatonicTestController(context);
            var animalOwnership = context.AnimalOwnership.Find(ID_OF_ANIMAL_CREATED_IN_PAST);
            Assert.Equal(db.PAST_CREATION_DATE, animalOwnership.LastUpdated);
            var ownedAnimal = await controller.GetAnimal(ID_OF_ANIMAL_CREATED_IN_PAST);

            var actionResult = Assert.IsType<ActionResult<OwnedAnimal>>(ownedAnimal);
            Assert.NotNull(actionResult);
            Assert.Equal(ID_OF_ANIMAL_CREATED_IN_PAST, actionResult.Value.Id);

            animalOwnership = context.AnimalOwnership.Find(ID_OF_ANIMAL_CREATED_IN_PAST);
            Assert.Equal(DateTime.Now.Year, animalOwnership.LastUpdated.Year);
        }

        [Fact]
        public async void TestStrokeAnimalAppropriate()
        {
            var controller = new MediatonicTestController(context);
            var ownedAnimal = await controller.GetAnimal(ID_TO_FIND);
            int initialHappiness = ownedAnimal.Value.Happiness;
            ownedAnimal = await controller.StrokeAnimal(ID_TO_FIND);

            var actionResult = Assert.IsType<ActionResult<OwnedAnimal>>(ownedAnimal);
            Assert.Equal(initialHappiness + 1, actionResult.Value.Happiness);
        }

        [Fact]
        public async void TestStrokeAnimalInappropriate()
        {
            var controller = new MediatonicTestController(context);
            var ownedAnimal = await controller.StrokeAnimal(INAPPROPRIATE_ID_TO_FIND);

            var actionResult = Assert.IsType<ActionResult<OwnedAnimal>>(ownedAnimal);
            Assert.IsType<BadRequestResult>(actionResult.Result);
        }

        [Fact]
        public async void TestStrokeAnimalCreatedInPast()
        {
            var controller = new MediatonicTestController(context);
            var animalOwnership = context.AnimalOwnership.Find(ID_OF_ANIMAL_CREATED_IN_PAST);
            Assert.Equal(db.PAST_CREATION_DATE, animalOwnership.LastUpdated);

            var ownedAnimal = await controller.GetAnimal(ID_OF_ANIMAL_CREATED_IN_PAST);
            int initialHappiness = ownedAnimal.Value.Happiness;
            ownedAnimal = await controller.StrokeAnimal(ID_TO_FIND);

            var actionResult = Assert.IsType<ActionResult<OwnedAnimal>>(ownedAnimal);
            Assert.Equal(initialHappiness + 1, actionResult.Value.Happiness);

            animalOwnership = context.AnimalOwnership.Find(ID_OF_ANIMAL_CREATED_IN_PAST);
            Assert.Equal(DateTime.Now.Year, animalOwnership.LastUpdated.Year);
        }

        [Fact]
        public async void TestFeedAnimalAppropriate()
        {
            var controller = new MediatonicTestController(context);
            var ownedAnimal = await controller.GetAnimal(ID_TO_FIND);
            int initialHunger = ownedAnimal.Value.Hunger;

            ownedAnimal = await controller.FeedAnimal(ID_TO_FIND);

            var actionResult = Assert.IsType<ActionResult<OwnedAnimal>>(ownedAnimal);
            Assert.Equal(initialHunger - 1, actionResult.Value.Hunger);
        }

        [Fact]
        public async void TestFeedAnimalCreatedInPast()
        {
            var controller = new MediatonicTestController(context);
            var animalOwnership = context.AnimalOwnership.Find(ID_OF_ANIMAL_CREATED_IN_PAST);
            Assert.Equal(db.PAST_CREATION_DATE, animalOwnership.LastUpdated);

            var ownedAnimal = await controller.GetAnimal(ID_OF_ANIMAL_CREATED_IN_PAST);
            int initialHunger = ownedAnimal.Value.Hunger;
            ownedAnimal = await controller.FeedAnimal(ID_TO_FIND);

            var actionResult = Assert.IsType<ActionResult<OwnedAnimal>>(ownedAnimal);
            Assert.Equal(initialHunger - 1, actionResult.Value.Hunger);

            animalOwnership = context.AnimalOwnership.Find(ID_OF_ANIMAL_CREATED_IN_PAST);
            Assert.Equal(DateTime.Now.Year, animalOwnership.LastUpdated.Year);
        }

        [Fact]
        public async void TestFeedAnimalInappropriate()
        {
            var controller = new MediatonicTestController(context);
            var ownedAnimal = await controller.FeedAnimal(INAPPROPRIATE_ID_TO_FIND);

            var actionResult = Assert.IsType<ActionResult<OwnedAnimal>>(ownedAnimal);
            Assert.IsType<BadRequestResult>(actionResult.Result);
        }
    }
}
