using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared
{
    public class Compiler
    {
        public string Render()
        {
            var builder = new StringBuilder();
            Render(builder);
            return builder.ToString();
        }

        public void Render(StringBuilder builder)
        {

        }
    }
}
