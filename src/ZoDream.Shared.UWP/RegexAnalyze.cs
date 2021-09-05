using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ZoDream.Shared
{
    public class RegexAnalyze
    {
        public string Pattern { get; set; }

        public string Content { get; set; }

        public MatchCollection Matches { get; set; }

        public RegexAnalyze()
        {

        }

        public RegexAnalyze(string content, string pattern)
        {
            Content = content;
            Pattern = pattern;
        }

        public MatchCollection Match()
        {
            return Matches = Regex.Matches(Content, Pattern);
        }

        public Task<string> CompilerAsync(string template)
        {
            return Task.Run(() =>
            {
                return Compiler(template);
            });
        }

        public string Compiler(string template)
        {
            var tokenItems = new Tokenizer().Render(template);
            var builder = new StringBuilder();
            Compiler(tokenItems, builder);
            return builder.ToString();
        }

        public void Compiler(IList<Token> items, StringBuilder builder)
        {
            foreach (var item in items)
            {
                switch (item.Type)
                {
                    case TokenType.Text:
                        builder.Append(item.Content);
                        break;
                    case TokenType.For:
                        CompilerFor(item as BlockToken, builder);
                        break;
                    case TokenType.IntFor:
                        CompilerIntFor(item as BlockToken, builder);
                        break;
                    case TokenType.StringFor:
                        CompilerStringFor(item as BlockToken, builder);
                        break;
                    case TokenType.Value:
                        builder.Append(CompilerValue(item.Content));
                        break;
                    case TokenType.Random:
                        builder.Append(CompilerRandom(item.Content));
                        break;
                    case TokenType.Format:
                        break;
                    case TokenType.Comment:
                        break;
                    default:
                        break;
                }
            }
        }

        private string SplitFunc(string content, out string func, string defFunc = "")
        {
            var i = content.IndexOf(':');
            if (i < 0)
            {
                func = defFunc;
                return content;
            }
            func = content.Substring(0, i);
            return content.Substring(i + 1);
        }

        private string SplitTag(string content, out string tag)
        {
            var i = content.IndexOf('.');
            if (i < 0)
            {
                tag = string.Empty;
                return content;
            }
            tag = content.Substring(i + 1);
            return content.Substring(0, i);
        }

        private string CompilerValue(string content)
        {
            content = SplitFunc(content, out var func);
            content = SplitTag(content, out var tag);
            return FormatGlobalValue(content, func, tag);
        }

        private string FormatGlobalValue(string content, string func, string tag)
        {
            var i = Convert.ToInt32(content);
            if (i >= Matches.Count)
            {
                return "";
            }
            return FormatMatchValue(Matches[i], tag, func);
        }

        private string FormatMatchValue(Match match, string tag, string func)
        {
            var val = string.IsNullOrWhiteSpace(tag) ? match.Value :
                IsNumberic(tag) ? match.Groups[Convert.ToInt32(tag)].Value : match.Groups[tag].Value;
            return FormatValue(val, func);
        }

        private void CompilerStringFor(BlockToken item, StringBuilder builder)
        {
            var content = SplitFunc(item.Content, out var defFunc);
            var args = content.Split(',');
            for (int i = 0; i < args.Length; i++)
            {
                FormatBlockInner(item.Children, args[i], i, defFunc, builder);
            }
        }

        private void FormatBlockInner(IList<Token> items, object value, int index, string defFunc, StringBuilder builder)
        {
            foreach (var token in items)
            {
                if (token.Type == TokenType.Text)
                {
                    builder.Append(token.Content);
                    continue;
                }
                if (token.Type != TokenType.Value)
                {
                    continue;
                }
                var val = SplitFunc(token.Content, out var func, defFunc);
                if (func == "index")
                {
                    builder.Append(index);
                    continue;
                }
                if (value == null)
                {
                    continue;
                }
                val = SplitTag(val, out var tag);
                if ((value is int || value is string) && !string.IsNullOrWhiteSpace(val))
                {
                    builder.Append(FormatGlobalValue(val, func, tag));
                    continue;
                }
                if (value is int)
                {
                    builder.Append(FormatIntValue((int)value, func));
                    continue;
                }
                if (value is string)
                {
                    builder.Append(FormatValue((string)value, func));
                    continue;
                }
                if (value is Match)
                {
                    builder.Append(FormatMatchValue((Match)value, tag, func));
                    continue;
                }
            }
        }

        private string FormatValue(string val, string func)
        {
            if (string.IsNullOrEmpty(func))
            {
                return val;
            }
            switch (func)
            {
                case "studly":
                    return ConverterHelper.Studly(val);
                case "lstudly":
                    return ConverterHelper.Studly(val, false);
                case "unstudly":
                    return ConverterHelper.UnStudly(val);
            }
            if (func[0] == '+')
            {
                var length = Convert.ToInt32(func.Substring(1));
                return val.PadLeft(length, '0');
            }
            return val;
        }

        private void CompilerIntFor(BlockToken item, StringBuilder builder)
        {
            var content = SplitFunc(item.Content, out var defFunc);
            var mathes = Regex.Matches(content, @"\d+");
            var start = 0;
            if (mathes.Count > 1)
            {
                start = Convert.ToInt32(mathes[0].Value);
            }
            var length = Convert.ToInt32(mathes[mathes.Count - 1].Value);
            var offset = mathes.Count > 2 ? Convert.ToInt32(mathes[1].Value) - start : 1;
            var i = -1;
            for (; start <= length; start += offset)
            {
                i++;
                FormatBlockInner(item.Children, start, i, defFunc, builder);
            }
        }

        private string FormatIntValue(int val, string func)
        {
            if (string.IsNullOrWhiteSpace(func))
            {
                return val.ToString();
            }
            if (IsNumberic(func))
            {
                return Convert.ToString(val, Convert.ToInt32(func));
            }
            if (func[0] == '+')
            {
                return FormatValue(val.ToString(), func);
            }
            try
            {
                return val.ToString(func);
            }
            catch (Exception)
            {
                return "{error}";
            }
        }

        private void CompilerFor(BlockToken item, StringBuilder builder)
        {
            var start = 0;
            var length = Matches.Count;
            if (!string.IsNullOrWhiteSpace(item.Content))
            {
                var args = item.Content.Split(',');
                if (args.Length < 2)
                {
                    length = Math.Min(Convert.ToInt32(args[0]), Matches.Count);
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(args[0]))
                    {
                        start = Math.Max(0, Convert.ToInt32(args[0]));
                    }
                    if (!string.IsNullOrWhiteSpace(args[1]))
                    {
                        length = Math.Min(Convert.ToInt32(args[1]), Matches.Count);
                    }
                }
            }
            for (; start < length; start++)
            {
                FormatBlockInner(item.Children, Matches[start], start, string.Empty, builder);
            }
        }

        private string CompilerRandom(string content)
        {
            var random = new Random();
            var args = content.Split('~');
            return random.Next(Convert.ToInt32(args[0]), Convert.ToInt32(args[1])).ToString();
        }

        public string ReplaceLineBreak(string arg)
        {
            return arg.Replace("\\r\\n", Environment.NewLine);
        }

        protected bool IsNumberic(string message)
        {
            return Regex.IsMatch(message, @"^\d*$");
        }
    }
}
