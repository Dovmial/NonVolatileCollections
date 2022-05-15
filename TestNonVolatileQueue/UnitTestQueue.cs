using Xunit;
using NonVolatileCollections.DatabaseHandler;
using NonVolatileCollections;
using TestInConsole.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestNonVolatileQueue
{
    public class UnitTestQueue
    {
        private static List<Person> persons = new()
        {
            new Person { Age = 12, LastName = "qwert1", Name = "asdf1" },
            new Person { Age = 24, LastName = "qwert2", Name = "asdf2" },
            new Person { Age = 36, LastName = "qwert2", Name = "asdf3" }
        };

        public static IEnumerable<object[]> DataPersons()
        {
            yield return new object[] { persons };
        }

        public static IEnumerable<object[]> DataNumbers()
        {
            yield return new object[] { 1, 3, 5, 6, 8, 7, 9, 2, 10 };
            yield return new object[] { 7, 9, 2, 10, 8, 9, 5, 4, 1 };
            yield return new object[] { 6, 8, 2, 4, 52, 1, 23, 3, 25 };
        }


        [Fact]
        public void TestInitQueue()
        {
            using (var people = new NonVolatileQueue<Person>(new QueueDBSerializeHandler<Person>("people")))
            {
                Assert.NotNull(people);
            }
        }

        [Theory]
        [MemberData(nameof(DataPersons))]
        public async Task TestInitQueue2(IEnumerable<Person> p)
        {
            using (var people2 = new NonVolatileQueue<Person>(new QueueDBSerializeHandler<Person>("people2"), p))
            {

                Assert.NotNull(people2);
                Assert.Equal((p as List<Person>)!.Count, people2.Count);
                //проверка очистки коллекции без удаления файла
                await people2.Clear();
                Assert.Empty(people2);
                var people = new NonVolatileQueue<Person>(new QueueDBSerializeHandler<Person>("people2"));
                Assert.Empty(people);
                people = null;
            }

        }

        [Fact]
        public async Task TestDequeue()
        {
            using (var people3 = new NonVolatileQueue<Person>(new QueueDBSerializeHandler<Person>("people3"), persons))
            {
                Assert.Equal(persons.Count, people3.Count);
                var person = await people3.Dequeue();
                Assert.Equal(12, person.Age);
                Assert.Equal(24, people3.Peek().Age);
                Assert.Equal(persons.Count - 1, people3.Count);
            }
        }

        [Theory]
        [MemberData(nameof(DataNumbers))]
        public async Task TestOtherQueue(int n1, int n2, int n3, int n4, int n5, int n6, int n7, int n8, int n9)
        {
            List<NumbersModel> listModel = new List<NumbersModel>()
            {
                new NumbersModel(n1),
                new NumbersModel(n2),
                new NumbersModel(n3),
                new NumbersModel(n4),
                new NumbersModel(n5),
                new NumbersModel(n6),
                new NumbersModel(n7),
                new NumbersModel(n8),
                new NumbersModel(n9),
            };

            //проверка очистки коллекции
            var list = new List<int>() { n1, n2, n3, n4, n5, n6, n7, n8, n9 };
            using (var numbersQ1 = new NonVolatileQueue<NumbersModel>(new QueueDBSerializeHandler<NumbersModel>("numbersQ1"), listModel))
            {
                await numbersQ1.Clear();
                Assert.Empty(numbersQ1);
            }

            //проверка методов и удаление файла
            using (var numbersQ2 = new NonVolatileQueue<NumbersModel>(new QueueDBSerializeHandler<NumbersModel>("numbersQ2"), 9))
            {
                Assert.Empty(numbersQ2);
                foreach (var number in list)
                {
                    await numbersQ2.Enqueue(new NumbersModel(number));
                }
                Assert.Equal(n1, (await numbersQ2.Dequeue()).Number);
                var test = new NumbersModel(155);
                numbersQ2.TryDequeue(out test);
                Assert.Equal(test!.Number, n2);
            }

            //проверка, что файл удалился
            using (var numbersQ3 = new NonVolatileQueue<NumbersModel>(new QueueDBSerializeHandler<NumbersModel>("numbersQ2")))
            {
                Assert.Empty(numbersQ3);
            }

            //проверка удаления n - элементов
            var numbersQ4 =
                new NonVolatileQueue<NumbersModel>(new QueueDBSerializeHandler<NumbersModel>("numbersQ4"), listModel);

            Assert.Equal(9, numbersQ4.Count);
            await numbersQ4.RemoveRangeAsync(5); //удалить 5 элементов
            //потеря ссылки без удаления файла (неожиданное отключение)
            numbersQ4 = null;

            //восстановление очереди из файла после отключения
            using (numbersQ4 = new NonVolatileQueue<NumbersModel>(new QueueDBSerializeHandler<NumbersModel>("numbersQ4")))
            {
                Assert.Equal(4, numbersQ4.Count);
                Assert.Equal(n6, numbersQ4.Peek().Number);//удалено 5 элементов
            }
        }
    }
}