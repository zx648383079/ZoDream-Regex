using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared
{
    public class RegexConverter
    {
        public IDictionary<string, string> Parameters { get; set; }

        public RegexConverter()
        {

        }

        public RegexConverter(IDictionary<string, string> parameter)
        {
            Parameters = parameter;
        }

        public string Get(string key)
        {
            return Get(key, string.Empty);
        }

        public string Get(string key, string defaultValue)
        {
            if (Parameters.TryGetValue(key, out string arg))
            {
                return arg;
            }
            return defaultValue;
        }
    }
}
