using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ZoDream.RegexTool.Pages
{
    /// <summary>
    /// ResultView.xaml 的交互逻辑
    /// </summary>
    public partial class ResultView : Window
    {
        public ResultView()
        {
            InitializeComponent();
        }
        public new string Content { get; set; }

        private void Nobtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Yesbtn_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetData(DataFormats.Text, ContentTb.Text);//复制内容到剪切板
            Savetb.Text = "已成功复制到剪切板！";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ContentTb.Text = Content;
        }
    }
}
