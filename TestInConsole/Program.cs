using NonVolatileCollections.DatabaseHandler;
using NonVolatileCollections;
using TestInConsole.Models;
using System.Diagnostics;

Stopwatch stopwatch = new Stopwatch();
int amountElements = 1000;

TestHeader($"Создание новой очереди и заполнение {amountElements} элементов (без удаления файла)");
stopwatch.Start();
var people = new NonVolatileQueue<Person>(serializer: new QueueDBSerializeHandler<Person>("people"));
for(int i = 0; i < amountElements; i++)
{
    await people.Enqueue(new Person { Age = i*2-1, Name=$"Person{i+1}", LastName=$"Nickname{i+1}" });
}
stopwatch.Stop();
Info(people, stopwatch);

people = null; //потеря ссылки/разрыв связи.
TestHeader($"Потеря данных. queue = null");
TestHeader($"Восстановление данных из файла, {amountElements} элементов");
stopwatch.Restart();
using (people = new NonVolatileQueue<Person>(serializer: new QueueDBSerializeHandler<Person>("people"), amountElements))
{
    stopwatch.Stop();
    Info(people, stopwatch);

    TestHeader($"Удаление {(int)(0.1 * amountElements)} элементов");
    List<Person> list = new();
    stopwatch.Restart();
    while (people.Count > (int)(0.9*amountElements))
    {
        list.Add(await people.Dequeue());
    }
    stopwatch.Stop();
    Console.WriteLine($"\nЛист [{list.Count}]");
    Info(people, stopwatch);

    TestHeader($"Сравнение производительности массового удаления ({(int)(0.44*amountElements)})");
    Console.WriteLine("\tМетод RemoveRangeAsync:");
    stopwatch.Restart();
    await people.RemoveRangeAsync((int)(0.44*amountElements));
    stopwatch.Stop();
    Info(people, stopwatch);

    Console.WriteLine("\tМетод Deque");
    stopwatch.Restart();
    for (int i = 0; i < (int)(0.44*amountElements); ++i)
        await people.Dequeue();
    stopwatch.Stop();
    Info(people, stopwatch);

    Print(people.TakeLast(20));
}

Console.ReadKey();


///вспомогательные функции
void TestHeader(string description)
{
    Console.ForegroundColor = ConsoleColor.DarkGreen;
    Console.WriteLine($"\t\t{description}");
    Console.ForegroundColor = ConsoleColor.White;
}
void Info(in NonVolatileQueue<Person> people, Stopwatch sw)
{
    Console.WriteLine($"Очередь: новый размер[{people.Count}]\t time = {stopwatch.Elapsed}\n");
}

void Print(in IEnumerable<Person> collection)
{
    foreach (Person item in collection)
        Console.WriteLine(item.Name);
}
