using System.Collections.Generic;

namespace hw1
{
    public class Trie
    {
        private class TrieNode
        {
            private readonly TrieNode _parent;
            private bool _isFinal;

            internal int SubstreeCount { get; private set; }

            internal readonly Dictionary<char, TrieNode> Children = new Dictionary<char, TrieNode>();
            internal bool IsFinal
            {
                get => _isFinal;
                set
                {
                    if (value == _isFinal)
                        return;
                    _isFinal = value;
                    if (value)
                        for (var node = this; node != null; node = node._parent)
                            node.SubstreeCount += 1;
                    else
                        for (var node = this; node != null; node = node._parent)
                            node.SubstreeCount -= 1;
                }
            }

            public TrieNode(TrieNode parent = null)
            {
                _parent = parent;
            }
        }

        private readonly TrieNode _root = new TrieNode();

        private (int acceptedCharacters, TrieNode closestNode) FindClosestNode(string element)
        {
            var node = _root;
            for (var i = 0; i != element.Length; ++i)
            {
                if (!node.Children.ContainsKey(element[i]))
                    return (i, node);
                node = node.Children[element[i]];
            }
            return (element.Length, node);
        }

        private static bool Contains(int accepted, TrieNode node, string element)
        {
            return accepted == element.Length && node.IsFinal;
        }

        public bool Contains(string element)
        {
            var (accepted, node) = FindClosestNode(element);
            return Contains(accepted, node, element);
        }

        public bool Add(string element)
        {
            var (accepted, node) = FindClosestNode(element);
            if (Contains(accepted, node, element))
                return false;
            for (var i = accepted; i != element.Length; ++i)
                node = node.Children[element[i]] = new TrieNode(node);
            node.IsFinal = true;
            return true;
        }

        public bool Remove(string element)
        {
            var (accepted, node) = FindClosestNode(element);
            if (!Contains(accepted, node, element))
                return false;
            node.IsFinal = false;
            return true;
        }

        public int Size() => _root.SubstreeCount;

        public int HowManyStartsWithPrefix(string prefix)
        {
            var (accepted, node) = FindClosestNode(prefix);
            return accepted != prefix.Length ? 0 : node.SubstreeCount;
        }
    }
}
