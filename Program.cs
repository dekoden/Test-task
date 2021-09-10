using System;

using System.Collections.Generic;
using System.IO;

namespace Serialization
{
    class ListNode
    {
        public ListNode Previous;
        public ListNode Next;
        public ListNode Random; // произвольный элемент внутри списка
        public string Data;
    }

    class ListRandom
    {
        public ListNode Head;
        public ListNode Tail;
        public int Count;

        public void Serialize(Stream s)
        {
            List<ListNode> arr = new List<ListNode>();
            ListNode temp = new ListNode();
            temp = Head;

            // заполняем массив элементов по очереди с начала
            do
            {
                arr.Add(temp);
                temp = temp.Next;
            } while (temp != null);

            // записываем данные каждого элемента в текстовый файл построчно
            using (StreamWriter w = new StreamWriter(s))
            {
                foreach (ListNode n in arr)
                {
                    w.WriteLine(n.Data.ToString() + ":" + arr.IndexOf(n.Random).ToString());
                }
            }
        }

        public void Deserialize(Stream s)
        {
            List<ListNode> arr = new List<ListNode>();
            ListNode temp = new ListNode();
            Count = 0;
            Head = temp;
            string line;

            // считываем элементы из файла построчно
            try
            {
                using (StreamReader sr = new StreamReader(s))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (!line.Equals(""))
                        {
                            Count++;
                            temp.Data = line;
                            ListNode next = new ListNode();
                            temp.Next = next;
                            arr.Add(temp);
                            next.Previous = temp;
                            temp = next;
                        }
                    }
                }

                // задаем хвост двусвязного списка
                Tail = temp.Previous;
                Tail.Next = null;

                // разделяем считанные данные и записываем их в соответствующие атрибуты
                foreach (ListNode n in arr)
                {
                    n.Random = arr[Convert.ToInt32(n.Data.Split(':')[1])];
                    n.Data = n.Data.Split(':')[0];
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.Read();
            }
        }
    }



    class Program
    {

        static Random random = new Random();

        // добавляет элемент со случайными данными
        static ListNode addNode(ListNode prev)
        {
            ListNode result = new ListNode();
            result.Previous = prev;
            result.Next = null;
            result.Data = random.Next(0, 100).ToString();
            prev.Next = result;
            return result;
        }

        // возвращает случайный элемент двусвязного списка
        static ListNode randomNode(ListNode _head, int _length)
        {
            int k = random.Next(0, _length);
            int i = 0;
            ListNode result = _head;
            while (i < k)
            {
                result = result.Next;
                i++;
            }
            return result;
        }

        static void Main(string[] args)
        {
            int length = 15;

            ListNode head = new ListNode();
            ListNode tail = new ListNode();
            ListNode temp = new ListNode();

            head.Data = random.Next(0, 1000).ToString();

            tail = head;

            for (int i = 1; i < length; i++)
            {
                tail = addNode(tail);
            }

            temp = head;

            for (int i = 0; i < length; i++)
            {
                temp.Random = randomNode(head, length);
                temp = temp.Next;
            }

            // создаем первый заполненный двусвязный список
            ListRandom first = new ListRandom();
            first.Head = head;
            first.Tail = tail;
            first.Count = length;

            // проводим сериализацию в файл data.txt
            FileStream fs = new FileStream("data.txt", FileMode.OpenOrCreate);
            first.Serialize(fs);

            // создаем второй пустой двусвязный список
            ListRandom second = new ListRandom();

            //проводим десериализацию из файла data.txt
            try
            {
                fs = new FileStream("data.txt", FileMode.Open);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.Read();
            }
            second.Deserialize(fs);

            // если хвосты списков совпадают, то сериализация и десериализация выполнены успешно
            if (second.Tail.Data == first.Tail.Data)
            {
                Console.WriteLine("Success");
            }
            Console.Read();
        }
    }
}