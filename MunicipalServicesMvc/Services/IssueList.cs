using System.Collections;
using System.Collections.Generic;
using MunicipalServicesMvc.Models;

namespace MunicipalServicesMvc.Services
{
    public class IssueList : IEnumerable<Issue>
    {
        private class Node
        {
            public Issue Value;
            public Node? Next;
            public Node(Issue v) { Value = v; }
        }

        private Node? _head, _tail;
        private int _count;

        public int Count => _count;

        public void Add(Issue issue)
        {
            var n = new Node(issue);
            if (_head == null) _head = _tail = n;
            else { _tail!.Next = n; _tail = n; }
            _count++;
        }

        public Issue? FindById(int id)
        {
            var cur = _head;
            while (cur != null)
            {
                if (cur.Value.Id == id) return cur.Value;
                cur = cur.Next;
            }
            return null;
        }

        // Stream the last n items (no arrays/LINQ)
        public IEnumerable<Issue> Last(int n)
        {
            if (n <= 0 || _count == 0) yield break;

            int skip = _count > n ? _count - n : 0;
            int idx = 0;
            var cur = _head;
            while (cur != null)
            {
                if (idx++ >= skip) yield return cur.Value;
                cur = cur.Next;
            }
        }

        public IEnumerator<Issue> GetEnumerator()
        {
            var cur = _head;
            while (cur != null) { yield return cur.Value; cur = cur.Next; }
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
