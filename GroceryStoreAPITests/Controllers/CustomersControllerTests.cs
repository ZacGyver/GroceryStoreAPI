using Microsoft.VisualStudio.TestTools.UnitTesting;
using GroceryStoreAPI.Controllers;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace GroceryStoreAPI.Controllers.Tests
{
    [TestClass()]
    public class CustomersControllerTests
    {
        // Rather than adding in a mock framework, lets just use an actual test database... not a great idea in a real environment, but we're just running a quick test here.
        private string dbFile = @"testdb.json";
        private DataAccess.ICustomersRepository repo;
        private CustomersController controller;

        public CustomersControllerTests()
        {
            repo = new DataAccess.CustomersRepository(dbFile);
            controller = new CustomersController(repo);
        }

        [TestMethod()]
        public void GetAllOk()
        {
            // This just needs to return an Ok with object - there might not be anything in the database.
            Assert.IsInstanceOfType(controller.Get().Result, typeof(OkObjectResult));
        }

        [TestMethod()]
        public void GetCorrectItem()
        {
            // Add an item directly to the repo, then ensure we get it back by id from the controller... 
            string customerName = "Test Customer " + DateTime.Now.ToLongTimeString();
            int? testId = repo.Add(new Models.Customer() { name = customerName });

            ActionResult testResult = controller.Get(testId.Value).Result;
            Assert.IsInstanceOfType(testResult, typeof(OkObjectResult));

            Models.Customer testResultObject = ((OkObjectResult)testResult).Value as Models.Customer;

            Assert.IsTrue(testResultObject.name.Equals(customerName));
        }

        [TestMethod()]
        public void PostTest()
        {
            // Add an item via the controller, then ensure we get it back with an id ... 
            string customerName = "Test Customer " + DateTime.Now.ToLongTimeString();

            ActionResult testResult = controller.Post(new Models.Customer() { name = customerName }).Result;
            Assert.IsInstanceOfType(testResult, typeof(CreatedAtActionResult));

            Models.Customer testResultObject = ((CreatedAtActionResult)testResult).Value as Models.Customer;

            Assert.IsTrue(testResultObject.id.HasValue);
            Assert.IsTrue(testResultObject.name.Equals(customerName));
        }

        /// <summary>
        /// Test that an invalid POST is handled correctly.
        /// </summary>
        [TestMethod()]
        public void PostInvalidTest()
        {
            // Add an item with invalid data, then ensure we get a bad request
            string customerName = "";

            ActionResult testResult = controller.Post(new Models.Customer() { name = customerName }).Result;
            Assert.IsInstanceOfType(testResult, typeof(BadRequestObjectResult));
        }

        [TestMethod()]
        public void PutTest()
        {
            // Add an item directly to the repo, then update it and finally compare the data in the repo... 
            string customerName = "Test Customer " + DateTime.Now.ToLongTimeString();
            int? testId = repo.Add(new Models.Customer() { name = customerName });

            string updatedCustomerName = "Updated Customer " + DateTime.Now.ToLongTimeString();

            ActionResult testResult = controller.Put(testId.Value, new Models.Customer() { id = testId, name = updatedCustomerName });

            // Assert that the put operation was ok
            Assert.IsInstanceOfType(testResult, typeof(OkResult));

            // Now go find the customer in the repo
            Models.Customer found = repo.GetOne(testId);
            Assert.IsNotNull(found);
            Assert.IsTrue(found.name.Equals(updatedCustomerName));
        }


        /// <summary>
        /// Test that an invalid PUT is handled correctly.
        /// </summary>
        [TestMethod()]
        public void PutInvalidTest()
        {
            // Add an item directly to the repo, then update it with invalid data ... 
            string customerName = "Test Customer " + DateTime.Now.ToLongTimeString();
            int? testId = repo.Add(new Models.Customer() { name = customerName });

            string updatedCustomerName = "";

            ActionResult testResult = controller.Put(testId.Value, new Models.Customer() { id = testId, name = updatedCustomerName });

            // Assert that the put operation was bad
            Assert.IsInstanceOfType(testResult, typeof(BadRequestObjectResult));
        }

        /// <summary>
        /// Test that an invalid PUT is handled correctly.
        /// </summary>
        [TestMethod()]
        public void PutNotExistsTest()
        {
            // Attempt to put (update) a non-existent item...

            // Could just put with a negative id, but that seems a bit too "happen to know the inner workings and this will conveniently fail"
            // Instead, lets find out the highest current id and then go higher...

            // this could be null if there aren't any yet:
            int testID = 314;
            var allCustomers = repo.GetAll();
            if (allCustomers?.Count() > 0)
                testID += allCustomers.Max(c => c.id).Value;
            
            string updatedCustomerName = "This customer doesn't exist so this should fail!";

            ActionResult testResult = controller.Put(testID, new Models.Customer() { id = testID, name = updatedCustomerName });

            // Assert that the put operation was Not Found
            Assert.IsInstanceOfType(testResult, typeof(NotFoundResult));
        }
    }
}