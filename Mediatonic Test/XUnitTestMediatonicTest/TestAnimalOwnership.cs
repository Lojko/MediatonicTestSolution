using MediatonicTest.Data;
using MediatonicTest.Models;
using Microsoft.EntityFrameworkCore;
using System;
using Xunit;

namespace XUnitTestMediatonicTest
{
    public class TestAnimalOwnership
    {
        private DataContext context;

        private FakePersistantDataStorage db;

        //Default ID to use when using constructed objects for testing
        private const long ID_TO_FIND = 1;

        public TestAnimalOwnership()
        {
            var options = new DbContextOptionsBuilder<DataContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            context = new DataContext(options);
            db = new FakePersistantDataStorage();
            db.PopulateContext(context);
        }

        [Fact]
        public void TestFeed()
        {
            AnimalOwnership ownedAnimal = context.AnimalOwnership.Find(ID_TO_FIND);
            int currentHunger = ownedAnimal.Hunger;
            ownedAnimal.Feed();
            Assert.Equal(currentHunger - 1, ownedAnimal.Hunger);
        }

        [Fact]
        public void TestStroke()
        {
            AnimalOwnership ownedAnimal = context.AnimalOwnership.Find(ID_TO_FIND);
            int currentHappiness = ownedAnimal.Happiness;
            ownedAnimal.Stroke();
            Assert.Equal(currentHappiness + 1, ownedAnimal.Happiness);
        }

        [Fact]
        public void TestDegradeStatsByOne()
        {
            AnimalOwnership ownedAnimal = context.AnimalOwnership.Find(ID_TO_FIND);
            Animal ownedAnimalDefintion = context.Animal.Find(ownedAnimal.AnimalId);

            ownedAnimal.Animal = ownedAnimalDefintion;;
            TestStatDegradation(ownedAnimal, ownedAnimalDefintion, 1);
        }

        [Fact]
        public void TestDegradeStatsByZero()
        {
            AnimalOwnership ownedAnimal = context.AnimalOwnership.Find(ID_TO_FIND);
            Animal ownedAnimalDefintion = context.Animal.Find(ownedAnimal.AnimalId);

            ownedAnimal.Animal = ownedAnimalDefintion;
            TestStatDegradation(ownedAnimal, ownedAnimalDefintion, 0);
        }

        [Fact]
        public void TestDegradeStatsByInappropriateValue()
        {
            AnimalOwnership ownedAnimal = context.AnimalOwnership.Find(ID_TO_FIND);
            Animal ownedAnimalDefintion = context.Animal.Find(ownedAnimal.AnimalId);
            ownedAnimal.Animal = ownedAnimalDefintion;

            int multiplier = -1;
            int currentHunger = ownedAnimal.Hunger;
            int currentHappiness = ownedAnimal.Happiness;

            ownedAnimal.DegradeStats(multiplier);

            Assert.Equal(currentHunger, ownedAnimal.Hunger);
            Assert.Equal(currentHappiness, ownedAnimal.Happiness);
        }

        private void TestStatDegradation(AnimalOwnership ownedAnimal, Animal animal, int multiplier)
        {
            int currentHappiness = ownedAnimal.Happiness;
            int currentHunger = ownedAnimal.Hunger;

            ownedAnimal.DegradeStats(multiplier);

            Assert.Equal(currentHappiness -= (animal.HappinessDecrease * multiplier), ownedAnimal.Happiness);
            Assert.Equal(currentHunger += (animal.HungerIncrease * multiplier), ownedAnimal.Hunger);
        }

        [Fact]
        public void TestSetToDefaults()
        {
            AnimalOwnership ownedAnimal = context.AnimalOwnership.Find(ID_TO_FIND);
            Animal ownedAnimalDefintion = context.Animal.Find(ownedAnimal.AnimalId);
            ownedAnimal.Animal = ownedAnimalDefintion;

            ownedAnimal.Feed();
            ownedAnimal.Stroke();

            Assert.NotEqual(ownedAnimalDefintion.HappinessDefault, ownedAnimal.Happiness);
            Assert.NotEqual(ownedAnimalDefintion.HungerDefault, ownedAnimal.Hunger);

            ownedAnimal.SetToDefaults(ownedAnimalDefintion);
            Assert.Equal(ownedAnimalDefintion.HappinessDefault, ownedAnimal.Happiness);
            Assert.Equal(ownedAnimalDefintion.HungerDefault, ownedAnimal.Hunger);
        }
    }
}
