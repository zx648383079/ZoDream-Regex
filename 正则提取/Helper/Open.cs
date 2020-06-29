using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ZoDream.Helper.Local
{
    public class Open
    {
        

        /// <summary>
        /// 读文本文件
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string Read(string file)
        {
            if (!File.Exists(file))
            {
                return null;
            }
            var fs = new FileStream(file, FileMode.Open);
            var reader = new StreamReader(fs, (new TxtEncoder()).GetEncoding(fs));
            var content = reader.ReadToEnd();
            reader.Close();
            return content;
        }

    }
}
