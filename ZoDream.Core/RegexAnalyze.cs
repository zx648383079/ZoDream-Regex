using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ZoDream.Core
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

        public string Compiler(string template) {
            template = RandomValue(replaceNote(template));
            var matches = Regex.Matches(template, @"{(for|~)((\d+)?(,(\d+)?)?)?}([\S\s]+?){(end|!)}");
            foreach (Match item in matches)
            {
                template = template.Replace(item.Value, GetForValue(item));
            }
            matches = Regex.Matches(template, @"{([^{}]+?)}([\S\s]+?){(end|!)}");
            foreach (Match item in matches)
            {
                var tag = item.Groups[1].Value;
                var content = item.Groups[2].Value.TrimStart();
                template = template.Replace(item.Value, tag.IndexOf("...") >= 0 ? IntFor(content, tag) : StringFor(content, tag));
            }
            return ReplaceLineBreak(MatchValue(template));
        }

        public string GetForValue(Match item) {
            var content = item.Groups[6].Value.TrimStart();
            if (string.IsNullOrEmpty(item.Groups[3].Value)) {
                if (string.IsNullOrEmpty(item.Groups[5].Value)) {
                    return MatchFor(content);
                }
                return MatchFor(content, Convert.ToInt32(item.Groups[5].Value));
            }
            if (!string.IsNullOrEmpty(item.Groups[5].Value)) {
                return MatchFor(content, Convert.ToInt32(item.Groups[3].Value), Convert.ToInt32(item.Groups[5].Value));
            }
            if (string.IsNullOrEmpty(item.Groups[4].Value)) {
                return MatchFor(content, Convert.ToInt32(item.Groups[3].Value));
            }
            var start = Convert.ToInt32(item.Groups[3].Value);
            return MatchFor(content, start, Matches.Count - start);
        }

        /// 包含 ...
        public string IntFor(string content, string tag) {
            var mathes = Regex.Matches(tag, @"\d+");
            var start = 0;
            if (mathes.Count > 1) {
                start = Convert.ToInt32(mathes[0].Value);
            }
            
            var length = Convert.ToInt32(mathes[mathes.Count - 1].Value);
            var offset = mathes.Count > 2 ? Convert.ToInt32(mathes[1].Value) : 1;
            var arg = new StringBuilder();
            for (int i = start; i <= length; i += offset)
            {
                arg.Append(content.Replace("{}", i.ToString()));
            }
            return arg.ToString();
        }

        // 以 , 分开
        public string StringFor(string content, string tag) {
            var args = tag.Split(',');
            var arg = new StringBuilder();
            foreach (var item in args) {
                arg.Append(content.Replace("{}", item));
            }
            return arg.ToString();
        }

        public string MatchFor(string content) {
            var arg = new StringBuilder();
            var args = Regex.Matches(content, @"{([^\.]*?)}");
            foreach (Match item in Matches)
            {
                var temp = content;
                foreach (Match m in args)
                {
                    temp = temp.Replace(m.Value, getMatchValue(item, m.Groups[1].Value));
                }
                arg.Append(temp);
            }
            return arg.ToString();
        }

        public string MatchFor(string content, int start, int length) {
            var arg = new StringBuilder();
            var args = Regex.Matches(content, @"{([^\.]+)}");
            length = Math.Min(length, Matches.Count - start);
            for (int i = 0; i < length; i++)
            {
                var temp = content;
                foreach (Match m in args)
                {
                    temp = temp.Replace(m.Value, getMatchValue(Matches[start + i], m.Groups[1].Value));
                }
                arg.Append(temp);
            }
            return arg.ToString();
        }

        public string MatchFor(string content, int length) {
            return MatchFor(content, 0, length);
        }

        public string MatchValue(string arg) {
            var matches = Regex.Matches(arg, @"{(\d+)?(\.(.+))?}");
            foreach (Match item in matches)
            {
                var index = Convert.ToInt32(item.Groups[1].Value);
                if (index >= Matches.Count) {
                    continue;
                }
                arg = arg.Replace(item.Value, getMatchValue(Matches[index], item.Groups[3].Value));
            }
            return arg;
        }

        public string RandomValue(string arg) {
            var matches = Regex.Matches(arg, @"{(\d+)~(\d+)}");
            var random = new Random();
            foreach (Match item in matches)
            {
                arg = arg.Replace(item.Value, 
                    random.Next(Convert.ToInt32(item.Groups[1].Value), 
                    Convert.ToInt32(item.Groups[2].Value)).ToString()
                );
            }
            return arg;
        }

        protected string replaceNote(string template) { 
            return Regex.Replace(template, @"//.*?\n|/\*[\s\S]*?\*/", "");
        }

        protected string getMatchValue(Match arg, string tag) {
            if (string.IsNullOrWhiteSpace(tag)) {
                return arg.Value;
            }
            if (IsNumberic(tag)) {
                return arg.Groups[Convert.ToInt32(tag)].Value;
            }
            return arg.Groups[tag].Value;
        }

        public string ReplaceLineBreak(string arg) {
            return arg.Replace("\\r\\n",  Environment.NewLine);
        }

        protected bool IsNumberic(string message)
        {
            return Regex.IsMatch(message, @"^\d*$");
        }
    }
}
