namespace GroceryStoreAPI.Models
{
    /// <summary>
    /// Basic data model.
    /// NOTE - did not add data transfer object. So far there isn't a reason for a DTO, but odds are it wont stay that way for long!
    /// (So keep in mind this class shows up in several places where it would need to be replaced if we need to make that switch)
    /// </summary>
    public class Customer : DataAccess.IRepositoryItem<int?>
    {
        public int? id { get; set; }
        public string name { get; set; }
    }
}
