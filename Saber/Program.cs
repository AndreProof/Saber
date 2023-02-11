using System;
using System.IO;

namespace Saber
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var list = new ListRandom();
            for (var i = 0; i < 100; i++)
            {
                list.Push(new ListNode { Data = $"Node: {i}" });
            }

            var random = new Random();

            for (var node = list.Head; node != null; node = node.Next)
            {
                node.Random = list.Get(random.Next(list.Count));
            }

            var fileName = "test.txt";

            Console.WriteLine("Сериализуемый список\n");
            list.Print();

            var stream = File.Open(fileName, FileMode.Create);
            list.Serialize(stream);
            stream.Close();

            var deserializeList = new ListRandom();
            Console.WriteLine("\n\nНовый список до десериализации:\n");
            deserializeList.Print();

            var deserializeStream = File.Open(fileName, FileMode.Open);
            deserializeList.Deserialize(deserializeStream);
            deserializeStream.Close();

            Console.WriteLine("\n\nНовый список после десериализации:\n");
            deserializeList.Print();

            Console.WriteLine(list.Equals(deserializeList)
                ? "\n\nОригинальный и десериализованный списки совпадают\n"
                : "\n\nОригинальный и десериализованный списки не совпадают\n");
        }
    }
}
