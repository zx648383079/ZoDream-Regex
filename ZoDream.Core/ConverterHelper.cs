using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Core
{
    public static class ConverterHelper
    {
        /// <summary>
        /// 分割符转驼峰
        /// </summary>
        /// <param name="val"></param>
        /// <param name="firstUpper">首字母是否大写</param>
        /// <returns></returns>
        public static string Studly(string val, bool firstUpper = true)
        {
            if (string.IsNullOrWhiteSpace(val))
            {
                return "";
            }
            var res = new StringBuilder();
            var isFirst = true;
            var nextConverter = true;
            for (int i = 0; i < val.Length; i++)
            {
                var chr = val[i];
                if (chr == ' ' || chr == '_' || chr == '-')
                {
                    nextConverter = true;
                    continue;
                }
                if (!firstUpper && isFirst)
                {
                    res.Append(Char.ToLower(chr));
                    nextConverter = false;
                    isFirst = false;
                    continue;
                }
                if (nextConverter)
                {
                    res.Append(Char.ToUpper(chr));
                    nextConverter = false;
                    continue;
                }
                res.Append(chr);
            }
            return res.ToString();
        }

        /// <summary>
        /// 驼峰转下划线
        /// </summary>
        /// <param name="val"></param>
        /// <param name="link">链接线</param>
        /// <returns></returns>
        public static string UnStudly(string val, char link = '_')
        {
            if (string.IsNullOrWhiteSpace(val))
            {
                return "";
            }
            var res = new StringBuilder();
            for (int i = 0; i < val.Length; i++)
            {
                var chr = val[i];
                var code = Convert.ToInt32(chr);
                if (code >= 65 && code <= 90)
                {
                    if (i > 0)
                    {
                        res.Append(link);
                    }
                    res.Append(Char.ToLower(chr));
                    continue;
                }
                res.Append(chr);
            }
            return res.ToString();
        }
    }
}
