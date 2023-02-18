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
            Dictionary<ListNode, HashSet<ListNode>> findingBy = new Dictionary<ListNode, HashSet<ListNode>>(); //словарь для определения элементов, "ищущих" данный

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
                    if (indexes.TryGetValue(node.Rand, out int indexRand)) //если известен индекс произвольного элемента
                        WriteAsString(bw, node.Data, counter, indexRand); //записать элемент вместе с этим индексом
                    else //иначе отметить, что текущий элемент "ищет" произвольный
                    {
                        if (findingBy.TryGetValue(node.Rand, out HashSet<ListNode> finders)) //если набор элементов, "ищущих" текущий, уже имеется
                            finders.Add(node); //добавить текущий элемент в этот набор
                        else //иначе создать новый набор элементов, "ищущих" текущий, и добавить в него текущий элемент
                        {
                            HashSet<ListNode> findersNew = new HashSet<ListNode>();
                            findersNew.Add(node);
                            findingBy.Add(node.Rand, findersNew);
                        }
                    }
                }
                else //записать элемент сразу
                    WriteAsString(bw, node.Data, counter, -1);

                //если текущий элемент кто-то "ищет"
                if (findingBy.TryGetValue(node, out HashSet<ListNode> findersNodes))
                {
                    //записать все "ищущие" элементы вместе с индексом текущего
                    foreach (ListNode finder in findersNodes)
                        WriteAsString(bw, finder.Data, indexes[finder], counter); //записать "ищущий" элемент вместе с индексом текущего
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
            Dictionary<int, HashSet<ListNode>> findingBy = new Dictionary<int, HashSet<ListNode>>(); //словарь для определения элементов, "ищущих" данный

            //пока поток не закончился
            while (s.Position < s.Length)
            {
                ListNode node = new ListNode();

                //распарсить очередную запись
                ParseString(br, out string data, out int index, out int indexRand);
                node.Data = data;

                //поместить текущий элемент в указанное место по индексу
                list[index] = node;

                //связать соседние элементы
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

                //если произвольный элемент был задан
                if (indexRand != -1)
                {
                    if (list[indexRand] != null) //если произвольный элемент имеется в списке
                        node.Rand = list[indexRand]; //задать ссылку Rand на него
                    else //иначе отметить, что текущий элемент "ищет" произвольный по индексу
                    {
                        if (findingBy.TryGetValue(indexRand, out HashSet<ListNode> finders)) //если набор элементов, "ищущих" произвольный, уже имеется
                            finders.Add(node); //добавить текущий элемент в этот набор
                        else //иначе создать новый набор элементов, "ищущих" произвольный, и добавить в него текущий элемент
                        {
                            HashSet<ListNode> findersNew = new HashSet<ListNode>();
                            findersNew.Add(node);
                            findingBy.Add(indexRand, findersNew);
                        }
                    }
                }

                //если текущий элемент кто-то "ищет"
                if (findingBy.TryGetValue(index, out HashSet<ListNode> findersNodes))
                {
                    //задать ссылку Rand всех "ищущего" элемента на текущий элемент
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
