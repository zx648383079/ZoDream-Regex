using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace 正则提取
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            Listbox.Items.Clear();
            if (MatchTb.Text.Trim() == "") return;
            MatchCollection ms = Regex.Matches(MatchTb.Text, RegexTb.Text);
            foreach (Match m in ms)
            {
                foreach (Group item in m.Groups)
                {
                    Listbox.Items.Add(item.Value);
                }
            }
            CountLb.Text = "添加了 " + Listbox.Items.Count.ToString(CultureInfo.InvariantCulture) + " 条数据";
        }

        private void Listbox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int sum = Listbox.Items.Count;
            int index = Listbox.SelectedIndex;
            CountLb.Text = "当前状态："+(index+1).ToString(CultureInfo.InvariantCulture)+"/"+sum.ToString(CultureInfo.InvariantCulture);
            if (index < 0 || index >= sum) return;
            string temp = ReplaceTb.Text;
            if (temp.IndexOf("$foreach", System.StringComparison.Ordinal)>=0)
            {
                Match m= Regex.Match(temp, @"^(\$foreach)(?<Text>.*?)(end\$)$");
                string temp1 = m.Groups["Text"].Value;
                string text = Listbox.Items.Cast<object>().Aggregate("", (current, t) => current + (temp1.Replace("$$", t.ToString()) + "\n"));
                temp = temp.Replace(m.Value, text);
            }
            else if (temp.IndexOf("$for", System.StringComparison.Ordinal) >=0)
            {
                Match m = Regex.Match(temp, @"^(\$for)(?<Text>[\s\S]*?)(end\$)$");      //正则表达式匹配任意字符（包括换行符）的写法:([\s\S]*) 同时，也可以用 “([\d\D]*)”、“([\w\W]*)” 来表示。
                string temp1 = m.Groups["Text"].Value;
                int count= Regex.Matches(temp1, @"\$\$").Count;
                if (count==sum)
                {
                    Regex reg = new Regex(@"\$\$");
                    for (int i = 0; i < count; i++)
                    {
                        temp1=reg.Replace(temp1,Listbox.Items[i].ToString(),1);
                    }
                    temp = temp.Replace(m.Value, temp1);
                }
                else
                {
                    MessageBox.Show("需要替换的地方有"+count.ToString(CultureInfo.InvariantCulture)+"个\n列表中有"+sum.ToString(CultureInfo.InvariantCulture)+"个", "提示");
                }
            }
            else
            {
                temp = temp.Replace("$$", Listbox.Items[index].ToString());
            }
            ChooseWindow chooseWindow = new ChooseWindow {Content = temp};
            chooseWindow.Show();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //AbouLb.Text = "关于\n用$$代替要替换的内容；可以使用\n$foreach $$ end$\n输出列表中所有内容。";
            AbouLb.Inlines.Add(new Bold(new Run("   关于")));
            AbouLb.Inlines.Add(new Run("\n用"));
            AbouLb.Inlines.Add(new Italic(new Run("$$")));
            AbouLb.Inlines.Add(new Run("代替要替换的内容；可以使用"));
            AbouLb.Inlines.Add(new Italic(new Run("\n$foreach $$ end$\n")));
            AbouLb.Inlines.Add(new Run("输出列表中所有内容，还可以用"));
            AbouLb.Inlines.Add(new Italic(new Run("\n$for $$ end$\n")));
            AbouLb.Inlines.Add(new Run("输出列表中所有内容，但必须保证要替换的个数与列表个数相同。"));
        }
    }
}
