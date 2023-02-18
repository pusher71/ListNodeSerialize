using System;
using System.IO;

namespace list_node_serialize
{
    class Program
    {
        private const string fileName = "ListTest";

        static void Main(string[] args)
        {
            Console.Write("Enter mode (s - Serialize; d - Deserialize): ");
            string mode = Console.ReadLine();

            if (mode == "s")
                SerializeTest();
            else if (mode == "d")
                DeserializeTest();
            else
                Console.WriteLine("Mode is incorrect!");

            Console.ReadKey();
        }

        private static void SerializeTest()
        {
            //создать список элементов
            const int count = 8;
            ListNode[] nodes = new ListNode[count]
            {
                new ListNode() { Data = "Data0" },
                new ListNode() { Data = "Data1" },
                new ListNode() { Data = "Data2" },
                new ListNode() { Data = "Data3" },
                new ListNode() { Data = "Data4" },
                new ListNode() { Data = "Data5" },
                new ListNode() { Data = "Data6" },
                new ListNode() { Data = "Data7" }
            };

            //задать соседей у элементов
            for (int i = 0; i < count; i++)
            {
                if (i > 0)
                    nodes[i].Prev = nodes[i - 1];
                if (i < count - 1)
                    nodes[i].Next = nodes[i + 1];
            }

            //задать ссылки на произвольные элементы
            nodes[0].Rand = nodes[3];
            nodes[1].Rand = nodes[3];
            nodes[3].Rand = nodes[5];
            nodes[4].Rand = nodes[2];
            nodes[5].Rand = nodes[3];
            nodes[6].Rand = nodes[5];
            nodes[7].Rand = nodes[3];

            //создать двусвязаный список
            ListRand list = new ListRand
            {
                Head = nodes[0],
                Tail = nodes[count - 1],
                Count = count
            };

            //записать его в файл
            FileStream s = new FileStream(fileName, FileMode.Create);
            list.Serialize(s);
            s.Close();

            Console.WriteLine("File saved");
        }

        private static void DeserializeTest()
        {
            //считать двусвязаный список из файла
            FileStream s = new FileStream(fileName, FileMode.Open);
            ListRand list = new ListRand();
            list.Deserialize(s);
            s.Close();

            //для каждого элемента списка вывести в консоль его данные и данные соответствующего произвольного элемента
            ListNode node = list.Head;
            while (node != null)
            {
                Console.WriteLine($"Data: {node.Data}{(node.Rand != null ? $", Rand data: {node.Rand.Data}" : "")}");
                node = node.Next;
            }
        }
    }
}
