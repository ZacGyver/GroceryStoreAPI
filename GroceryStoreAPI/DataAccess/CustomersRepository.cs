using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using GroceryStoreAPI.Models;

namespace GroceryStoreAPI.DataAccess
{
    /// <summary>
    /// Specific interface so we can easily inject
    /// </summary>
    public interface ICustomersRepository : IRepository<Customer, int?> { }

    /// <summary>
    /// Customers repo
    /// </summary>
    public class CustomersRepository : ICustomersRepository
    {
        protected CustomersDB database;
        protected string databaseFile;

        public CustomersRepository(string databaseFile = null)
        {
            if (string.IsNullOrEmpty(databaseFile))
                databaseFile = "database.json";

            this.databaseFile = databaseFile;

            if (!LoadDatabase(databaseFile))
            {
                // uh oh!
                throw new Exception("Failed to load customers database.", loadException);
            }
        }

        private Exception loadException = null;
        private bool LoadDatabase(string databaseFile)
        {
            try
            {
                // If the database file doesn't exist, just init a new database and we'll try creating the db file later - we're spinning up a new db.
                // (Obviously this is a questionable way to do things, but for fun sometimes I like having an easy install demo - put it about anywhere and as long as it has file permission, it'll just work.)
                if (!File.Exists(databaseFile))
                {
                    database = new CustomersDB();
                }
                else
                {
                    // Read in the raw database file...
                    string rawDBContents = File.ReadAllText(databaseFile);
                    // And process that into our repo ... pretty complicated stuff here, loading the raw binary data the way the database stored it... :)
                    database = System.Text.Json.JsonSerializer.Deserialize<CustomersDB>(rawDBContents);
                }
                return true;
            }
            catch (Exception ex)
            {
                loadException = ex;
                return false;
            }
        }

        private Exception saveException = null;
        private bool SaveDatabase()
        {
            try
            {
                // Straight to the point, just make json of the data and write it to the file.
                string rawDBContents = System.Text.Json.JsonSerializer.Serialize(database);
                File.WriteAllText(databaseFile, rawDBContents);
                return true;
            }
            catch (Exception ex)
            {
                saveException = ex;
                return false;
            }
        }

        /// <summary>
        /// Get a list of all the customers.
        /// </summary>
        public IEnumerable<Customer> GetAll()
        {
            return database.customers;
        }

        /// <summary>
        /// Get a single customer, if one with that id exists.
        /// </summary>
        public Customer GetOne(int? customerId)
        {
            return database.customers.Where(c => c.id == customerId).FirstOrDefault();
        }

        /// <summary>
        /// Add a new customer and return the id
        /// </summary>
        /// <returns>id of new customer</returns>
        public int? Add(Customer customer)
        {
            // Generate an id for the customer
            // New fresh, empty db? there would be no customers yet...
            if (database.customers == null)
            {
                database.customers = new List<Customer>();
                customer.id = 1;
            }
            else
            {
                customer.id = database.customers.Max(c => c.id) + 1;
            }


            database.customers.Add(customer);
            if (SaveDatabase())
                return customer.id;

            // Failed to add, no new customer id.
            return null;
        }

        /// <summary>
        /// Update details of the provided customer
        /// </summary>
        public bool Update(Customer customer)
        {
            // Find the customer if in the database...
            Customer dbCust = database.customers.Where(c => c.id == customer.id).FirstOrDefault();
            if (dbCust != null && dbCust.id.HasValue)
            {
                dbCust.name = customer.name;
                return SaveDatabase();
            }

            return false;
        }
    }
}
