using NonVolatileCollections;

namespace TestInConsole.Models
{
    public class NumbersModel : IModel
    {
        public int Id { get ; set; }
        public int Number { get; set; }

        public NumbersModel(int number)
        {
            Number = number;
        }
    }
}
