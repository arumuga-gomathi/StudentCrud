using Microsoft.AspNetCore.Mvc;
using StudentCrud.Controllers;
using StudentCrud.Models;
using StudentCrud.Tests.Helpers;
using Xunit;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace StudentCrud.Tests.Controllers
{
    public class StudentsControllerTests
    {
        [Fact]
        public async Task Create_ValidStudent_ShouldRedirectToIndex()
        {
            var context = DbContextMock.GetContextWithData(Guid.NewGuid().ToString());
            var controller = new StudentsController(context);

            var student = new Student
            {
                Name = "Charlie",
                Email = "charlie@email.com",
                Dob = DateTime.Now.AddYears(-25),
                Password = "hashedpassword"
            };

            var result = await controller.Create(student);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async Task Create_InvalidModel_ShouldReturnView()
        {
            var context = DbContextMock.GetContextWithData(Guid.NewGuid().ToString());
            var controller = new StudentsController(context);
            controller.ModelState.AddModelError("Email", "Required");

            var student = new Student { Name = "Test" };

            var result = await controller.Create(student);
            var view = Assert.IsType<ViewResult>(result);
            Assert.False(view.ViewData.ModelState.IsValid);
        }

        [Fact]
        public async Task Edit_ExistingStudent_ShouldUpdateAndRedirect()
        {
            var context = DbContextMock.GetContextWithData(Guid.NewGuid().ToString());
            var controller = new StudentsController(context);
            var existing = await context.Students.FindAsync(1);
            existing.Name = "Updated Name";

            var result = await controller.Edit(existing.Id, existing);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async Task Edit_NonExistingStudent_ShouldReturnNotFound()
        {
            var context = DbContextMock.GetContextWithData(Guid.NewGuid().ToString());
            var controller = new StudentsController(context);
            var student = new Student { Id = 999, Name = "Test", Email = "test@email.com", Dob = DateTime.Now, Password = "pass" };

            var result = await controller.Edit(student.Id, student);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteConfirmed_ShouldDeleteStudent()
        {
            var context = DbContextMock.GetContextWithData(Guid.NewGuid().ToString());
            var controller = new StudentsController(context);

            await controller.Delete(1);

            var students = context.Students.ToList();
            Assert.Single(students);  // Only 1 student left after deleting one
        }
    }
}
