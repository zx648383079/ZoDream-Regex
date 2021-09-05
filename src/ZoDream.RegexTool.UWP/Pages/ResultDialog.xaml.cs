
using ZoDream.RegexTool.Utils;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml.Controls;

// “内容对话框”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上进行了说明

namespace ZoDream.RegexTool.Pages
{
    public sealed partial class ResultDialog : ContentDialog
    {
        public ResultDialog()
        {
            this.InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var dataPackage = new DataPackage();
            dataPackage.SetText(Result);
            Clipboard.SetContent(dataPackage);
            Toast.ShowToastNotification("StoreLogo.png", Helper.GetString("CopySuccess"), NotificationAudioNames.Default);
            args.Cancel = true;
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        public string Result
        {
            get { return ResultTb.Text; }
            set { ResultTb.Text = value; }
        }

    }
}
