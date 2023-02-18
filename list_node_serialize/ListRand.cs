using System.Collections.Generic;
using System.IO;

namespace list_node_serialize
{
    class ListRand
    {
        public ListNode Head;
        public ListNode Tail;
        public int Count;

        public void Serialize(FileStream s)
        {
            BinaryWriter bw = new BinaryWriter(s);

            //записать количество элементов
            bw.Write(Count);

            Dictionary<ListNode, int> indexes = new Dictionary<ListNode, int>(); //словарь для определения индексов (номеров) элементов
            Dictionary<ListNode, HashSet<ListNode>> findingBy = new Dictionary<ListNode, HashSet<ListNode>>(); //словарь для определения элементов, ищущих данный

            ListNode node = Head; //текущий элемент
            int counter = 0;

            //для всех элементов
            while (node != null)
            {
                //добавить в словарь индексов
                indexes.Add(node, counter);

                //если ссылка на произвольный элемент имеется
                if (node.Rand != null)
                {
                    //если известен индекс произвольного элемента
                    if (indexes.TryGetValue(node.Rand, out int indexRand))
                    {
                        //записать элемент вместе с этим индексом
                        WriteAsString(bw, node.Data, counter, indexRand);
                    }
                    else
                    {
                        //отметить, что текущий элемент ищет произвольный...

                        //если набор элементов, ищущих произвольный, уже имеется
                        if (findingBy.TryGetValue(node.Rand, out HashSet<ListNode> finders))
                        {
                            //добавить текущий элемент в этот набор
                            finders.Add(node);
                        }
                        else
                        {
                            //создать новый набор элементов, ищущих произвольный, и добавить в него текущий элемент
                            HashSet<ListNode> findersNew = new HashSet<ListNode>();
                            findersNew.Add(node);
                            findingBy.Add(node.Rand, findersNew);
                        }
                    }
                }
                else
                {
                    //записать элемент, отметив, что ссылка на произвольный элемент отсутствует
                    WriteAsString(bw, node.Data, counter, -1);
                }

                //если имеется набор элементов, ищущих текущий
                if (findingBy.TryGetValue(node, out HashSet<ListNode> findersNodes))
                {
                    //записать все ищущие элементы из набора вместе с индексом текущего
                    foreach (ListNode finder in findersNodes)
                        WriteAsString(bw, finder.Data, indexes[finder], counter);
                }

                //перейти к следующему элементу
                node = node.Next;
                counter++;
            }

            bw.Close();
        }

        public void Deserialize(FileStream s)
        {
            BinaryReader br = new BinaryReader(s);

            //считать количество элементов
            Count = br.ReadInt32();

            ListNode[] list = new ListNode[Count]; //список элементов
            Dictionary<int, HashSet<ListNode>> findingBy = new Dictionary<int, HashSet<ListNode>>(); //словарь для определения элементов, ищущих данный

            //пока поток не закончился
            while (s.Position < s.Length)
            {
                ListNode node = new ListNode();

                //распарсить очередную запись
                ParseString(br, out string data, out int index, out int indexRand);
                node.Data = data;

                //поместить текущий элемент в указанное место списка по индексу
                list[index] = node;

                //связать текущий элемент с соседними
                if (index > 0 && list[index - 1] != null)
                {
                    node.Prev = list[index - 1];
                    list[index - 1].Next = node;
                }
                if (index < Count - 1 && list[index + 1] != null)
                {
                    node.Next = list[index + 1];
                    list[index + 1].Prev = node;
                }

                //если индекс произвольного элемента задан
                if (indexRand != -1)
                {
                    //если произвольный элемент имеется в списке
                    if (list[indexRand] != null)
                    {
                        //задать ссылку на него
                        node.Rand = list[indexRand];
                    }
                    else
                    {
                        //отметить, что текущий элемент ищет произвольный по индексу...

                        //если набор элементов, ищущих произвольный, уже имеется
                        if (findingBy.TryGetValue(indexRand, out HashSet<ListNode> finders))
                        {
                            //добавить текущий элемент в этот набор
                            finders.Add(node);
                        }
                        else
                        {
                            //создать новый набор элементов, ищущих произвольный, и добавить в него текущий элемент
                            HashSet<ListNode> findersNew = new HashSet<ListNode>();
                            findersNew.Add(node);
                            findingBy.Add(indexRand, findersNew);
                        }
                    }
                }

                //если имеется набор элементов, ищущих текущий
                if (findingBy.TryGetValue(index, out HashSet<ListNode> findersNodes))
                {
                    //у всех ищущих элементов из набора задать ссылку на текущий элемент
                    foreach (ListNode finder in findersNodes)
                        finder.Rand = node;
                }
            }

            //задать ссылки на крайние элементы списка
            Head = list[0];
            Tail = list[Count - 1];

            br.Close();
        }

        private void WriteAsString(BinaryWriter bw, string data, int index, int indexRand) =>
            bw.Write($"{data}\\{index}\\{indexRand}");

        private void ParseString(BinaryReader br, out string data, out int index, out int indexRand)
        {
            string[] record = br.ReadString().Split(new char[1] { '\\' });
            data = record[0]; //данные элемента
            index = int.Parse(record[1]); //индекс элемента
            indexRand = int.Parse(record[2]); //индекс произвольного элемента
        }
    }
}
