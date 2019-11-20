using MediatonicTest.Data;
using MediatonicTest.Models;
using System;
namespace XUnitTestMediatonicTest
{
    class FakePersistantDataStorage
    {
        //DateTime for a past creation date, as seen sometimes in defaulting database entries
        public DateTime PAST_CREATION_DATE = new DateTime(2018, 1, 1, 00, 00, 00);

        //Creation date an hour in the past for testing hunger / happiness degradation
        public DateTime CREATION_DATE_IN_PAST_BY_HOUR = DateTime.Now.AddHours(-1);

        public FakePersistantDataStorage()
        {
        }

        public void PopulateContext(DataContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            context.User.AddRange(
                new User() { Id = 1, Username = "Adam" },
                new User() { Id = 2, Username = "Brad" },
                new User() { Id = 3, Username = "Chris" }
            );

            context.Animal.AddRange(
                new Animal() { Id = 1, Type = "Aardvark", HappinessDefault = 10, HappinessDecrease = 1, HungerDefault = 0, HungerIncrease = 1},
                new Animal() { Id = 2, Type = "Bat", HappinessDefault = 5, HappinessDecrease = 2, HungerDefault = 5, HungerIncrease = 2 },
                new Animal() { Id = 3, Type = "Cat", HappinessDefault = 0, HappinessDecrease = 1, HungerDefault = 5, HungerIncrease = 1}
            );

            context.AnimalOwnership.AddRange(
                new AnimalOwnership() { Id = 1, UserId = 1, AnimalId = 1, Name = "Animal #1", LastUpdated = DateTime.Now },
                new AnimalOwnership() { Id = 2, UserId = 2, AnimalId = 2, Name = "Animal #2", LastUpdated = DateTime.Now },
                new AnimalOwnership() { Id = 3, UserId = 3, AnimalId = 3, Name = "Animal #3", LastUpdated = DateTime.Now },
                new AnimalOwnership() { Id = 4, UserId = 2, AnimalId = 1, Name = "Animal #4", LastUpdated = DateTime.Now },
                new AnimalOwnership() { Id = 5, UserId = 1, AnimalId = 2, Name = "Animal #5", LastUpdated = DateTime.Now },
                new AnimalOwnership() { Id = 6, UserId = 3, AnimalId = 3, Name = "Animal #6", LastUpdated = DateTime.Now },
                new AnimalOwnership() { Id = 7, UserId = 3, AnimalId = 1, Name = "Animal #7", LastUpdated = DateTime.Now },
                new AnimalOwnership() { Id = 8, UserId = 1, AnimalId = 2, Name = "Animal #8", LastUpdated = CREATION_DATE_IN_PAST_BY_HOUR },
                new AnimalOwnership() { Id = 9, UserId = 2, AnimalId = 3, Name = "Animal #9", LastUpdated = PAST_CREATION_DATE }
            );

            context.SaveChanges();
        }
    }
}
