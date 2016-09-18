using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using ZoDream.Core;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace UWP_Regex
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private RegexAnalyze _regex;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void RegexTb_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key != Windows.System.VirtualKey.Enter)
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(RegexTb.Text) || string.IsNullOrWhiteSpace(ContentTb.Text))
            {
                return;
            }
            MainPivot.SelectedIndex = 1;
            MatchList.Items.Clear();
            _regex = new RegexAnalyze(ContentTb.Text, RegexTb.Text);
            _regex.Match();
            foreach (Match item in _regex.Matches)
            {
                if (item.Groups.Count < 2)
                {
                    MatchList.Items.Add(item.Value);
                    continue;
                }
                var arg = new StringBuilder();
                arg.AppendLine(item.Value);
                for (int i = 0; i < item.Groups.Count; i++)
                {
                    var group = item.Groups[i];
                    arg.AppendLine(i.ToString() + "\t" + group.Value);
                    if (group.Captures.Count < 2)
                    {
                        continue;
                    }
                    for (int j = 0; j < group.Captures.Count; j++)
                    {
                        arg.AppendLine(j.ToString() + "\t\t" + group.Captures[j].Value);
                    }
                }
                MatchList.Items.Add(arg.ToString());
             }
        }

        private async void MatchList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TemplateTb.Text) || _regex == null)
            {
                MainPivot.SelectedIndex = 2;
                return;
            }
            var template = TemplateTb.Text;
            var dialog = new ResultDialog();
            dialog.Result = _regex.Compiler(template);
            await dialog.ShowAsync();
        }

        private void ContentTb_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
            e.DragUIOverride.Caption = "拖放此处即可添加文件 o(^▽^)o";
        }

        private async void ContentTb_Drop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var items = await e.DataView.GetStorageItemsAsync();
                
                //文件过滤 只取vcf文件 PS:如果拖过来的是文件夹 则需要对文件夹处理 取出文件夹文件
                if (items != null && items.Count > 0)
                {
                    var storageFile = items[0] as StorageFile;
                    var stream = await storageFile.OpenStreamForReadAsync();
                    using (var sr = new StreamReader(stream))
                    {
                        ContentTb.Text = await sr.ReadToEndAsync();
                    }
                }
            }
        }
    }
}
