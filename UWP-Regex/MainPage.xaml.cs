using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UWP_Regex.Models;
using UWP_Regex.Utils;
using UWP_Regex.ViewModels;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
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

        public MainViewModel ViewModel = new MainViewModel();

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ReplaceTb.Text = "{for} {} {end}";
        }

        private void RegexTb_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key != Windows.System.VirtualKey.Enter)
            {
                return;
            }
            try
            {
                StartMatch();
            }
            catch
            {
                CountLb.Text = Helper.GetString("RegexError");
            }
        }

        private void StartMatch()
        {
            if (string.IsNullOrWhiteSpace(RegexTb.Text) || string.IsNullOrWhiteSpace(ContentTb.Text))
            {
                return;
            }
            ViewModel.AddHistory(RegexTb.Text);
            ViewModel.MatchItems.Clear();
            _regex = new RegexAnalyze(ContentTb.Text, RegexTb.Text);
            _regex.Match();
            foreach (Match item in _regex.Matches)
            {
                ViewModel.MatchItems.Add(new MatchItem(item));
            }
        }

        private void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StartMatch();
            }
            catch
            {
                CountLb.Text = Helper.GetString("RegexError");
            }
        }


        private void ContentTb_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
            e.DragUIOverride.Caption = Helper.GetString("DragTip");
        }

        private async void ContentTb_Drop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var items = await e.DataView.GetStorageItemsAsync();
                
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

        private void MatchList_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ReplaceTb.Text) || _regex == null)
            {
                return;
            }
            var template = ReplaceTb.Text;
            var dialog = new ResultDialog
            {
                Result = _regex.Compiler(template)
            };
            _ = dialog.ShowAsync();
        }

        private void HelpBtn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var dialog = new HelpDialog();
            _ = dialog.ShowAsync();
        }

        private void RegexTb_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {

        }

        private void AcceptTabTb_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if(e.Key == Windows.System.VirtualKey.Tab) {
                var textBox = (TextBox)e.OriginalSource;
                var originalStartPosition = textBox.SelectionStart;
                var startPosition = GetRealStartPositionTakingCareOfNewLines(originalStartPosition, textBox.Text);
                var beforeText = textBox.Text.Substring(0, startPosition);
                var afterText = textBox.Text.Substring(startPosition, textBox.Text.Length - startPosition);
                var tabSpaces = 4;
                var tab = new string(' ', tabSpaces);
                textBox.Text = beforeText + tab + afterText;
                textBox.SelectionStart = originalStartPosition + tabSpaces;

                e.Handled = true;
            }
        }

        private int GetRealStartPositionTakingCareOfNewLines(int startPosition, string text)
        {
            int newStartPosition = startPosition;
            int currentPosition = 0;
            bool previousWasReturn = false;
            foreach (var character in text)
            {
                if (character == '\n')
                {
                    if (previousWasReturn)
                    {
                        newStartPosition++;
                    }
                }

                if (newStartPosition <= currentPosition)
                {
                    break;
                }

                if (character == '\r')
                {
                    previousWasReturn = true;
                }
                else
                {
                    previousWasReturn = false;
                }

                currentPosition++;
            }

            return newStartPosition;
        }
    }
}
