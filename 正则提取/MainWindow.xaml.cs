using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using 正则提取.Model;

namespace 正则提取
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ObservableCollection<MatchItem> _lists = new ObservableCollection<MatchItem>();
        private MatchCollection _matches;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void _load()
        {
            var fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "\\regex.txt", FileMode.OpenOrCreate);
            var reader = new StreamReader(fs, Encoding.UTF8);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                RegexTb.Items.Add(line);
            }
            reader.Close();
            fs.Close();
        }

        private void _save()
        {
            var list = new string[RegexTb.Items.Count];
            RegexTb.Items.CopyTo(list, 0);
            var writer = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\regex.txt", false,
                Encoding.UTF8);
            var lists = list.ToList().Distinct();
            foreach (var item in lists)
            {
                
                writer.WriteLine(item);
            }
            writer.Close();
        }

        private void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            RegexTb.Items.Add(RegexTb.Text);
            _lists.Clear();
            if (string.IsNullOrWhiteSpace(MatchTb.Text)) return;
            _matches = Regex.Matches(MatchTb.Text, RegexTb.Text);
            foreach (Match m in _matches)
            {
                var index = 0;
                foreach (Group item in m.Groups)
                {
                    _lists.Add(new MatchItem(index, item.Value));
                    index ++;
                }
            }
            CountLb.Text = "添加了 " + _lists.Count.ToString(CultureInfo.InvariantCulture) + " 条数据";
        }

        private void Listbox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_lists.Count < 1) return;
            const string pattern = @"{for(\(([\d,]+)\))?([\s\S]+?)end}|{([\w\.]+?)}";
            //先去除注释
            var content = ReplaceTb.Text;
            Task.Factory.StartNew(() =>
            {

                //先去除注释
                content = Regex.Replace(content, @"//.*?\n|/\*[\s\S]*?\*/", "");
                var matches = Regex.Matches(content, pattern);
                foreach (Match item in matches)
                {
                    string replace;
                    if (string.IsNullOrWhiteSpace(item.Groups[3].Value))
                    {
                        var keys = item.Groups[4].Value.Split('.');
                        if (keys.Length == 1)
                        {
                            replace = _matches[int.Parse(keys[0])].Value;
                        }
                        else
                        {
                            var key = keys[1];
                            if (IsNumberic(key))
                            {
                                var index = 0;
                                if (!string.IsNullOrWhiteSpace(key))
                                {
                                    index = int.Parse(key);
                                }
                                replace = _matches[int.Parse(keys[0])].Groups[index].Value;
                            }
                            else
                            {
                                replace = _matches[int.Parse(keys[0])].Groups[key].Value;
                            }
                        }
                    }
                    else
                    {
                        var start = 0;
                        var length = _matches.Count;
                        if (!string.IsNullOrWhiteSpace(item.Groups[2].Value))
                        {
                            var nums = item.Groups[2].Value.Split(',');
                            if (nums.Length == 1)
                            {
                                length = Math.Min(length, int.Parse(nums[0]));
                            }
                            else
                            {
                                start = int.Parse(nums[0]);
                                length = string.IsNullOrWhiteSpace(nums[1]) ? (length - start) : Math.Min(length - start, int.Parse(nums[1]));
                            }
                        }
                        var text = item.Groups[3].Value;
                        var replaces = new StringBuilder();
                        var textMatches = Regex.Matches(text, @"{(\w*?)}");
                        for (var i = 0; i < length; i++)
                        {
                            var replacedText = text;
                            var itemm = _matches[start + i];
                            foreach (Match textMatch in textMatches)
                            {
                                var key = textMatch.Groups[1].Value;
                                if (IsNumberic(key))
                                {
                                    var index = 0;
                                    if (!string.IsNullOrWhiteSpace(key))
                                    {
                                        index = int.Parse(key);
                                    }
                                    replacedText = replacedText.Replace(textMatch.Value, itemm.Groups[index].Value);
                                }
                                else
                                {
                                    replacedText = replacedText.Replace(textMatch.Value, itemm.Groups[key].Value);
                                }
                                //replacedText = replacedText.Replace(textMatch.Value, IsNumberic(key) ? item.Groups[int.Parse(key)].Value : item.Groups[key].Value);
                            }
                            replaces.Append(replacedText);
                        }
                        replace = replaces.ToString();
                    }
                    content = content.Replace(item.Value, replace);
                }
                Dispatcher.Invoke(() =>
                {
                    var chooseWindow = new ChooseWindow {Content = content};
                    chooseWindow.Show();
                });
            });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AbouLb.Inlines.Add(new Bold(new Run("   关于")));
            AbouLb.Inlines.Add(new Run("\n包含两种用法："));
            AbouLb.Inlines.Add(new Italic(new Run("")));
            AbouLb.Inlines.Add(new Run("1.普通输出{} 可以应{1.tt}输出第二个匹配中tt标志的内容"));
            AbouLb.Inlines.Add(new Italic(new Run("\n2.循环输出{for end}\n")));
            AbouLb.Inlines.Add(new Run("for 无参数时输出所有"));
            AbouLb.Inlines.Add(new Italic(new Run("\nfor(10) 一个参数时，从第一个开始输出10个\n")));
            AbouLb.Inlines.Add(new Run("for(1,10) 两个参数时，从第二个开始输出10个"));
            AbouLb.Inlines.Add(new Run("支持注释// 或/* */ "));
            Listbox.ItemsSource = _lists;
            ReplaceTb.Text = "{for {} end}";
            _load();
        }

        protected bool IsNumberic(string message)
        {
            return Regex.IsMatch(message, @"^\d*$");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Task.Factory.StartNew(_save);
        }
    }
}
