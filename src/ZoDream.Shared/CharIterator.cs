﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared
{
    public class CharIterator : IEnumerator<char>, IEnumerable<char>
    {

        public readonly string Content;
        public int Position { get; set; } = -1;
        public CharIterator(string content)
        {
            Content = content;
        }

        public int Length => Content.Length;

        public char Current => Content[Position];

        object IEnumerator.Current => Content[Position];

        public void Dispose()
        {

        }

        public IEnumerator<char> GetEnumerator()
        {
            return this;
        }

        public bool MoveNext()
        {
            if (!CanNext)
            {
                return false;
            }
            Position++;
            return true;
        }

        public void Reset()
        {
            Position = -1;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }

        public bool CanNext => Position < Content.Length - 1;
        public bool CanBack => Position > 1;

        public int IndexOf(char c, int offset = 0)
        {
            return Content.IndexOf(c, Position + offset);
        }
        public int IndexOf(string s, int offset = 0)
        {
            return Content.IndexOf(s, Position + offset);
        }

        public string Read(int length = 1, int offset = 0)
        {
            if (length == 0)
            {
                return string.Empty;
            }
            var pos = (length < 0 ? Position + length : Position) + offset;
            if (pos > Content.Length - 1)
            {
                return string.Empty;
            }
            var len = length < 0 ? -length : length;
            return Content.Substring(pos, len);
        }

        public string ReadSeek(int position, int length = 1)
        {
            return Content.Substring(position, length);
        }

        public bool NextIs(params char[] items)
        {
            if (!CanNext)
            {
                return false;
            }
            var c = Content[Position + 1];
            foreach (var item in items)
            {
                if (c == item)
                {
                    return true;
                }
            }
            return false;
        }

        public bool NextIs(params string[] items)
        {
            if (!CanNext)
            {
                return false;
            }
            foreach (var item in items)
            {
                if (string.IsNullOrEmpty(item))
                {
                    continue;
                }
                if (Content.Substring(Position + 1, item.Length) == item)
                {
                    return true;
                }
            }
            return false;
        }

        public int MinIndex(params char[] items)
        {
            var index = -1;
            var min = -1;
            for (int i = items.Length - 1; i >= 0; i--)
            {
                var j = IndexOf(items[i]);
                if (j >= 0 && (min < 0 || j <= min))
                {
                    index = i;
                    min = j;
                }
            }
            return index;
        }

        public int MinIndex(params string[] items)
        {
            var index = -1;
            var min = -1;
            for (int i = items.Length - 1; i >= 0; i--)
            {
                var j = IndexOf(items[i]);
                if (j >= 0 && (min < 0 || j <= min))
                {
                    index = i;
                    min = j;
                }
            }
            return index;
        }
    }
}
