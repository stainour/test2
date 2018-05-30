using ListRand;
using NUnit.Framework;
using System.IO;

namespace IntegrationTests
{
    [TestFixture]
    public class ListRandTests
    {
        private const string FileName = "ser.txt";
        private static readonly ListRand.ListRand _listToSerialize = new ListRand.ListRand();

        public ListRandTests()
        {
            _listToSerialize.Head = new ListNode
            {
                Data = "0_data",
            };
            var currentListNode = _listToSerialize.Head;
            for (var i = 1; i <= 10; i++)
            {
                var next = new ListNode
                {
                    Data = $"{i} data",
                    Rand = currentListNode,
                    Prev = currentListNode
                };
                currentListNode.Next = next;
                currentListNode = next;
            }

            _listToSerialize.Tail = currentListNode;
            _listToSerialize.Count = 11;
        }

        [OneTimeTearDown]
        public static void Cleanup()
        {
            File.Delete(FileName);
        }

        [Test, Order(1)]
        public void Deserialize()
        {
            var deserializedList = new ListRand.ListRand();
            using (var fileStream = new FileStream(FileName, FileMode.Open))
            {
                deserializedList.Deserialize(fileStream);
            }

            Assert.AreEqual(_listToSerialize.Count, deserializedList.Count);
            Assert.AreEqual(_listToSerialize.Head.Data, deserializedList.Head.Data);
            Assert.AreEqual(_listToSerialize.Tail.Data, deserializedList.Tail.Data);

            var serializedCurrentNode = _listToSerialize.Head;
            var deserializedCurrentNode = _listToSerialize.Head;

            for (var i = 0; i < deserializedList.Count; i++)
            {
                Assert.AreEqual(serializedCurrentNode.Data, deserializedCurrentNode.Data);

                if (serializedCurrentNode.Next != null)
                {
                    Assert.AreEqual(serializedCurrentNode.Next.Data, deserializedCurrentNode.Next.Data);
                }

                if (serializedCurrentNode.Prev != null)
                {
                    Assert.AreEqual(serializedCurrentNode.Prev.Data, deserializedCurrentNode.Prev.Data);
                }

                if (serializedCurrentNode.Rand != null)
                {
                    Assert.AreEqual(serializedCurrentNode.Rand.Data, deserializedCurrentNode.Rand.Data);
                }
                serializedCurrentNode = serializedCurrentNode.Next;
                deserializedCurrentNode = deserializedCurrentNode.Next;
            }
        }

        [Test, Order(0)]
        public void Serialize()
        {
            using (var fileStream = new FileStream(FileName, FileMode.Create))
            {
                _listToSerialize.Serialize(fileStream);
            }
            Assert.IsTrue(File.Exists(FileName));
        }
    }
}