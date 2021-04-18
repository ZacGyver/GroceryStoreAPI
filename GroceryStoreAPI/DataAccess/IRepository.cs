using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GroceryStoreAPI.DataAccess
{
    /// <summary>
    /// Basic repository interface.
    /// Anything we need to load/store in the database should have an IRepository implementation. Then there's a common (largely copy-and-paste-able) data access layer for every type of data we have.
    /// </summary>
    /// <typeparam name="TItem">The type of Item in this repo</typeparam>
    /// <typeparam name="TId">The identifier type for the item</typeparam>
    public interface IRepository<TItem, TId>
        where TItem : IRepositoryItem<TId>
    {
        public IEnumerable<TItem> GetAll();
        public TItem GetOne(TId itemId);
        public TId Add(TItem item);
        public bool Update(TItem item);

        // Delete not in scope
        //public bool Delete(TItem item);
    }
}
