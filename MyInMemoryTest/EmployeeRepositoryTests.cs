using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MyInMemoryTest.Production;
using MyInMemoryTest.Production.Entities;
using NUnit.Framework;

namespace MyInMemoryTest
{
    public class EmployeeRepositoryTests
    {
        private IEmployeeRepository _repository;
        
        [SetUp]
        public void Setup()
        {
            _repository = GetInMemoryRepository();
        }

        [Test]
        public void add_a_employee()
        { 
            var employee2 = new EmployeeDto { FirstName = "Cynthia", LastName="Chuang", Department = (DepartmentEnum)2 };
            _repository.AddAsync(employee2);
            _repository.SaveChanges();
            var employee = _repository.Employees.Where(d => d.DepartmentId == 2);
            employee.First().As<Employee>().Name.Should().Be("Cynthia Chuang");
        }

        [Test]
        public void find_海外企業開發組()
        {
            var employee2 = new EmployeeDto { FirstName = "Cynthia", LastName = "Chuang", Department = (DepartmentEnum)2 };
            var employee3 = new EmployeeDto { FirstName = "JJ", LastName = "Wang", Department = (DepartmentEnum)1 };
            _repository.AddAsync(employee2);
            _repository.AddAsync(employee3);
            _repository.SaveChanges();
            var employee = _repository.Employees.Where(d => d.DepartmentId == 1);
            var expected = new List<Employee> {
        
                new Employee{ Name = "CC", DepartmentId = 1},
                new Employee{ Name = "JJ Wang", DepartmentId = 1},
            };
            expected.Should().BeEquivalentTo(employee, options => options.Excluding(o => o.Id));
            
        }
        private IEmployeeRepository GetInMemoryRepository()
        {
            var options = new DbContextOptionsBuilder<EmployeeContext>()
                .UseInMemoryDatabase(databaseName: "MockDB")
                .Options;

            var initContext = new EmployeeContext(options);

            initContext.Database.EnsureDeleted();

            SinyiMembers(initContext);

            var testContext = new EmployeeContext(options);

            var repository = new EmployeeRepository(testContext);

            return repository;
        }

        private void SinyiMembers(EmployeeContext context)
        {
            var employee1 = new Employee { Name = "CC", DepartmentId = 1};
           
            context.Add(employee1);
            
            context.SaveChanges();
        }
    }
}