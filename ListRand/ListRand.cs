using System;
using System.IO;
using System.Runtime.Serialization;

namespace ListRand
{
    public class ListRand
    {
        public int Count;
        public ListNode Head;
        public ListNode Tail;

        public void Deserialize(FileStream s)
        {
            if (s == null) throw new ArgumentNullException(nameof(s));
            if (!s.CanRead) throw new ArgumentException("FileStream is not readable!");

            using (TextReader reader = new StreamReader(s))
            {
                var linenumber = 0;
                try
                {
                    Count = int.Parse(reader.ReadLine());
                    if (Count > 0)
                    {
                        var nodes = new ListNode[Count];
                        var randomRef = new int[Count];

                        for (var i = 0; i < Count; i++)
                        {
                            ++linenumber;
                            randomRef[i] = int.Parse(reader.ReadLine());
                            ++linenumber;
                            nodes[i] = new ListNode
                            {
                                Data = reader.ReadLine()
                            };
                        }
                        RestoreNodeReferences(nodes, randomRef);
                    }
                }
                catch (FormatException e)
                {
                    throw new FormatException($"Deserialization failed at the line: {linenumber}", e);
                }
            }
        }

        public void Serialize(FileStream s)
        {
            if (s == null) throw new ArgumentNullException(nameof(s));
            if (!s.CanWrite) throw new ArgumentException("FileStream is not writable!");

            var listNodes = new ListNode[Count];
            var generator = new ObjectIDGenerator();
            var currentNode = Head;

            while (currentNode != null)
            {
                var id = generator.GetId(currentNode, out _);
                listNodes[id - 1] = currentNode;
                currentNode = currentNode.Next;
            }

            using (TextWriter writer = new StreamWriter(s))
            {
                writer.WriteLine(Count);
                currentNode = Head;
                while (currentNode != null)
                {
                    writer.WriteLine(FindIndex(currentNode.Rand, generator));
                    writer.WriteLine(currentNode.Data);
                    currentNode = currentNode.Next;
                }
            }
        }

        private long FindIndex(ListNode node, ObjectIDGenerator generator)
        {
            if (node == null)
            {
                return -1;
            }

            if (generator.HasId(node, out _) == 0)
            {
                return -1;
            }

            return generator.GetId(node, out _) - 1;
        }

        private void RestoreNodeReferences(ListNode[] nodes, int[] randomReference)
        {
            if (nodes == null) throw new ArgumentNullException(nameof(nodes));
            if (randomReference == null) throw new ArgumentNullException(nameof(randomReference));
            for (var i = 0; i < Count; i++)
            {
                var node = nodes[i];
                var randIndex = randomReference[i];
                if (randIndex != -1)
                {
                    node.Rand = nodes[randIndex];
                }
                var prevIndex = i - 1;
                var nextNode = i + 1;
                if (prevIndex >= 0)
                {
                    node.Prev = nodes[prevIndex];
                }

                if (nextNode < Count)
                {
                    node.Next = nodes[nextNode];
                }
            }

            Head = nodes[0];
            Tail = nodes[Count - 1];
        }
    }
}