
using NonVolatileCollections;

namespace TestInConsole.Models
{
    public class Person : IModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public int? Age { get; set; }
    }
}