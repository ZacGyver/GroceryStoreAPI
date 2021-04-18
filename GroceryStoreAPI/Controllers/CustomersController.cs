using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using GroceryStoreAPI.DataAccess;
using GroceryStoreAPI.Models;

namespace GroceryStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        readonly ICustomersRepository repo = null;
        public CustomersController(ICustomersRepository repo)
        {
            this.repo = repo;
        }

        // GET: api/<Customers>
        [HttpGet]
        public ActionResult<IEnumerable<Customer>> Get()
        {
            return Ok(repo.GetAll());
        }

        // GET api/<Customers>/5
        [HttpGet("{id}")]
        public ActionResult<Customer> Get(int id)
        {
            Customer result = repo.GetOne(id);
            if (result != null && result.id.HasValue)
                return Ok(result);

            return NotFound();
        }

        // POST api/<Customers>
        //public ActionResult<Customer> Post([FromBody] string value)
        [HttpPost]
        public ActionResult<Customer> Post([FromBody] Customer newCustomer)
        {

            int? newId = repo.Add(newCustomer);

            if (newId.HasValue)
            {
                return CreatedAtAction(nameof(Get), new { id = newId }, newCustomer);
            }

            // failed...
            return BadRequest(newCustomer);

        }

        // PUT api/<Customers>/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] Customer customer)
        {
            // Just in case there was anything amiss, make sure we're editing the right item
            customer.id = id;

            if (repo.Update(customer))
            {
                return Ok();
            }

            // failed...
            return NotFound();
        }

        // DELETE api/<Customers>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            // Delete not in scope...
        }
    }
}
