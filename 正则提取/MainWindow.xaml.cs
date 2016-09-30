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
using ZoDream.Core;
using ZoDream.Helper.Local;
using 正则提取.Model;

namespace 正则提取
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ObservableCollection<MatchItem> _lists = new ObservableCollection<MatchItem>();
        private RegexAnalyze _regex;

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
            _regex = new RegexAnalyze(MatchTb.Text, RegexTb.Text);
            _regex.Match();
            foreach (Match item in _regex.Matches)
            {
                _lists.Add(new MatchItem(item));
            }
            Listbox.ItemsSource = _lists;
            CountLb.Text = "添加了 " + _lists.Count.ToString(CultureInfo.InvariantCulture) + " 条数据";
        }

        private void Listbox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_lists.Count < 1) return;
            var content = ReplaceTb.Text;
            Task.Factory.StartNew(() =>
            {
                content = _regex.Compiler(content);
                Dispatcher.Invoke(() =>
                {
                    var chooseWindow = new ChooseWindow {Content = content};
                    chooseWindow.Show();
                });
            });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Listbox.ItemsSource = _lists;
            ReplaceTb.Text = "{for} {} {end}";
            _load();
        }

        

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Task.Factory.StartNew(_save);
        }

        private void MatchTb_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
        }

        private void MatchTb_PreviewDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var file = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
                MatchTb.Text = Open.Read(file);
            }
        }
    }
}
