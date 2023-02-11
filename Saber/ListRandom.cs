using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Saber
{
    /// <summary>
    /// Двусвязный список.
    /// </summary>
    public class ListRandom
    {
        #region Поля и свойства

        /// <summary>
        /// Голова.
        /// </summary>
        public ListNode Head;

        /// <summary>
        /// Хвост.
        /// </summary>
        public ListNode Tail;

        /// <summary>
        /// Количество объектов.
        /// </summary>
        public int Count;

        #endregion

        #region Методы

        /// <summary>
        /// Добавить узел в список.
        /// </summary>
        /// <param name="node"></param>
        public void Push(ListNode node)
        {
            if (this.Tail != null)
            {
                this.Tail.Next = node;
            }

            node.Previous = this.Tail;
            this.Tail = node;

            this.Head ??= node;
            this.Count++;
        }

        /// <summary>
        /// Вывести список.
        /// </summary>
        public void Print()
        {
            Console.WriteLine($"Количество элементов в списке: {this.Count}");
            if (this.Count > 1)
            {
                for (var node = this.Head; node != null; node = node.Next)
                {
                    Console.WriteLine(
                        $"Data: {node.Data} | " +
                        $"Prev: {node.Previous?.Data ?? "NULL"} | " +
                        $"Next: {node.Next?.Data ?? "NULL"} | " +
                        $"Random: {node.Random?.Data ?? "NULL"}");
                }
            }
            else
            {
                Console.WriteLine("Нечего выводить");
            }
        }

        public ListNode Get(int index)
        {
            if (index >= this.Count || index < 0)
            {
                return null;
            }

            var fromBegin = index < this.Count - index;
            var currentIndex = fromBegin ? 0 : this.Count - 1;
            var node = fromBegin ? this.Head : this.Tail;
            while (currentIndex != index)
            {
                node = fromBegin ? node.Next : node.Previous;
                currentIndex = fromBegin ? currentIndex + 1 : currentIndex - 1;
            }

            return node;
        }

        /// <summary>
        /// Сериализовать узел.
        /// </summary>
        /// <param name="list">Список сериализованных узлов.</param>
        /// <param name="sw">Писатель.</param>
        /// <param name="node">Узел.</param>
        private void Serialize(IList<ListNode> list, TextWriter sw, ListNode node)
        {
            if (node == null)
            {
                sw.WriteLine($"{SerializeTag.Node}|--");
                return;
            }

            var index = list.IndexOf(node);
            if (index != -1)
            {
                sw.WriteLine($"{SerializeTag.Node}|" + index);
            }
            else
            {
                sw.WriteLine($"{SerializeTag.Data}|" + node.Data);
                list.Add(node);
                this.Serialize(list, sw, node.Random);
                this.Serialize(list, sw, node.Next);
            }
        }

        /// <summary>
        /// Сериализовать.
        /// </summary>
        /// <param name="s">Поток.</param>
        public void Serialize(Stream s)
        {
            var map = new List<ListNode>();
            var sw = new StreamWriter(s);

            this.Serialize(map, sw, this.Head);

            sw.Close();
        }

        /// <summary>
        /// Десериализовать узел.
        /// </summary>
        /// <param name="list">Список существующих узлов.</param>
        /// <param name="sr">Чтец.</param>
        /// <param name="prev">Предыдущий узел.</param>
        /// <returns>Десериализованный узел.</returns>
        private ListNode Deserialize(ICollection<ListNode> list, TextReader sr, ListNode prev)
        {
            var line = sr.ReadLine();
            if (line == null)
            {
                return null;
            }

            var divIndex = line.IndexOf('|');
            var result = line[(divIndex + 1)..];
            if (line[..divIndex] == SerializeTag.Node.ToString() && int.TryParse(result, out var index))
            {
                var node = list.ElementAtOrDefault(index);
                if (node != null && prev != null)
                {
                    node.Previous = prev;
                }

                return node;
            }

            if (line[..divIndex] == SerializeTag.Data.ToString())
            {
                var newNode = new ListNode
                {
                    Data = result
                };

                if (prev != null)
                {
                    newNode.Previous = prev;
                }

                list.Add(newNode);
                newNode.Random = this.Deserialize(list, sr, null);
                newNode.Next = this.Deserialize(list, sr, newNode);
                if (newNode.Next == null)
                {
                    this.Tail = newNode;
                }

                this.Count++;
                return newNode;
            }

            return null;
        }

        /// <summary>
        /// Десериализация.
        /// </summary>
        /// <param name="s">Поток данных.</param>
        public void Deserialize(Stream s)
        {
            var map = new List<ListNode>();
            var sr = new StreamReader(s);

            this.Head = this.Deserialize(map, sr, null);

            sr.Close();
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is ListRandom listRandom)
            {
                if (listRandom.Count != this.Count)
                {
                    return false;
                }

                if (this.Head?.Data != listRandom.Head?.Data || this.Tail?.Data != listRandom.Tail?.Data)
                {
                    return false;
                }

                var otherNode = listRandom.Head;

                for (var node = this.Head; node != null; node = node.Next)
                {
                    var dataRight = node.Data == otherNode.Data;
                    var prevRight = node.Previous?.Data == otherNode.Previous?.Data;
                    var nextRight = node.Next?.Data == otherNode.Next?.Data;
                    var randRight = node.Random?.Data == otherNode.Random?.Data;

                    if (!dataRight || !prevRight || !nextRight || !randRight)
                    {
                        return false;
                    }

                    otherNode = otherNode.Next;
                }

                return true;
            }

            return base.Equals(obj);
        }

        #endregion
    }
}
