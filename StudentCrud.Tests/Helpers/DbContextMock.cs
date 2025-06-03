using Microsoft.EntityFrameworkCore;
using StudentCrud.Models;
using System;

namespace StudentCrud.Tests.Helpers
{
    public static class DbContextMock
    {
        public static AppDbContext GetContextWithData(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var context = new AppDbContext(options);

            context.Students.AddRange(
                new Student { Id = 1, Name = "Alice", Email = "alice@email.com", Dob = DateTime.Now.AddYears(-20), Password = "test" },
                new Student { Id = 2, Name = "Bob", Email = "bob@email.com", Dob = DateTime.Now.AddYears(-22), Password = "test" }
            );
            context.SaveChanges();

            return context;
        }
    }
}

