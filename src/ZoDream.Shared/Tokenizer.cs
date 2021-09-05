using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared
{
    public class Tokenizer
    {

        public IList<Token> Render(string content)
        {
            return Render(new CharIterator(content));
        }

        public IList<Token> Render(CharIterator reader)
        {
            return RenderBlockInner(reader, false);
        }

        private IList<Token> RenderBlockInner(CharIterator reader, bool checkEnd = true)
        {
            var items = new List<Token>();
            var builder = new StringBuilder();
            while (reader.MoveNext())
            {
                if (reader.Current == '/' && reader.NextIs('*', '/'))
                {
                    items.Add(new Token() { Content = builder.ToString() });
                    builder.Clear();
                    items.Add(ReaderComment(reader));
                    continue;
                }
                if (reader.Current == '{' && checkEnd && reader.NextIs("end}", "!}"))
                {
                    reader.Position += reader.NextIs('!') ? 2 : 4;
                    break;
                }
                if (reader.Current != '{' || !IsBlock(reader))
                {
                    builder.Append(reader.Current);
                    continue;
                }
                if (reader.NextIs("for", "~"))
                {
                    items.Add(new Token() { Content = builder.ToString() });
                    builder.Clear();
                    items.Add(ReaderFor(reader));
                    continue;
                }
                if (reader.MinIndex("...", "}") == 0)
                {
                    items.Add(new Token() { Content = builder.ToString() });
                    builder.Clear();
                    items.Add(ReaderIntFor(reader));
                    continue;
                }
                if (reader.MinIndex(',', '}') == 0)
                {
                    items.Add(new Token() { Content = builder.ToString() });
                    builder.Clear();
                    items.Add(ReaderStringFor(reader));
                    continue;
                }
                if (reader.MinIndex(' ', '}') == 0)
                {
                    builder.Append(reader.Current);
                    continue;
                }
                items.Add(new Token() { Content = builder.ToString() });
                builder.Clear();
                items.Add(ReaderValue(reader));
            }
            items.Add(new Token() { Content = builder.ToString() });
            builder.Clear();
            return items;
        }

        private Token ReaderValue(CharIterator reader)
        {
            var end = reader.IndexOf('}');
            var content = reader.Read(end - reader.Position - 1, 1);
            reader.Position = end;
            return new Token() { Type = content.IndexOf('~') >= 0 ? TokenType.Random : TokenType.Value, Content = content };
        }

        private Token ReaderStringFor(CharIterator reader)
        {
            var end = reader.IndexOf('}');
            var content = reader.Read(end - reader.Position - 1, 1);
            reader.Position = end;
            return new BlockToken()
            {
                Type = TokenType.StringFor,
                Content = content,
                Children = RenderBlockInner(reader, true)
            };
        }

        private Token ReaderIntFor(CharIterator reader)
        {
            var end = reader.IndexOf('}');
            var content = reader.Read(end - reader.Position - 1, 1);
            reader.Position = end;
            return new BlockToken()
            {
                Type = TokenType.IntFor,
                Content = content,
                Children = RenderBlockInner(reader, true)
            };
        }

        private Token ReaderFor(CharIterator reader)
        {
            var end = reader.IndexOf('}');
            var start = reader.Position + (reader.NextIs('~') ? 2 : 4);
            var content = reader.ReadSeek(start, end - start);
            reader.Position = end;
            return new BlockToken()
            {
                Type = TokenType.For,
                Content = content,
                Children = RenderBlockInner(reader, true)
            };
        }

        private bool IsBlock(CharIterator reader)
        {
            var endIndex = reader.IndexOf('}');
            if (endIndex < 0)
            {
                return false;
            }
            var lineIndex = reader.IndexOf('\n');
            if (lineIndex > 0 && endIndex > lineIndex)
            {
                return false;
            }
            var startIndex = reader.IndexOf('{', 1);
            return startIndex > endIndex;
        }

        private Token ReaderComment(CharIterator reader)
        {
            var isMulti = reader.NextIs('*');
            var i = isMulti ? reader.IndexOf("*/") : reader.IndexOf("\n");
            var start = reader.Position + 2;
            var end = i < 0 ? reader.Length : i - 1;
            var content = reader.ReadSeek(start, end - start);
            reader.Position = i < 0 ? reader.Length : i + 1;
            return new Token() { Type = TokenType.Comment, Content = content };
        }
    }

    public class Token
    {
        public TokenType Type { get; set; } = TokenType.Text;

        public string Content { get; set; }
    }

    public class BlockToken : Token
    {
        public IList<Token> Children { get; set; }
    }

    public enum TokenType
    {
        Text,
        For,
        IntFor,
        StringFor,
        Value,
        Random,
        Format,
        Comment,
    }
}
