using System.Windows;

namespace 正则提取
{
    /// <summary>
    /// Interaction logic for ChooseWindow.xaml
    /// </summary>
    public partial class ChooseWindow : Window
    {
        public ChooseWindow()
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
