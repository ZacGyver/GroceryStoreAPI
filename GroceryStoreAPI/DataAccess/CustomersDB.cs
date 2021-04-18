using System.Collections.Generic;

namespace GroceryStoreAPI.DataAccess
{
    public class CustomersDB
    {
        // List instead of array so we can quickly roll CRUD operations...
        public List<Models.Customer> customers { get; set; }
    }
}
