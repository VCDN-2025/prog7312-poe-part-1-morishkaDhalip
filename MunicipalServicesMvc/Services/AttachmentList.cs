using System.Collections;
using System.Collections.Generic;

namespace MunicipalServicesMvc.Services
{
    public class AttachmentList : IEnumerable<string>
    {
        private class Node
        {
            public string Value;
            public Node? Next;
            public Node(string v) { Value = v; }
        }

        private Node? _head, _tail;
        private int _count;

        public int Count => _count;

        public void Add(string path)
        {
            var n = new Node(path);
            if (_head == null) _head = _tail = n;
            else { _tail!.Next = n; _tail = n; }
            _count++;
        }

        public IEnumerator<string> GetEnumerator()
        {
            var cur = _head;
            while (cur != null) { yield return cur.Value; cur = cur.Next; }
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
