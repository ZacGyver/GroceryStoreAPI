namespace GroceryStoreAPI.DataAccess
{
    /// <summary>
    /// Very basic interface for an item in a repo. It has an id.
    /// </summary>
    public interface IRepositoryItem<T>
    {
        public T id { get; set; }
    }
}
