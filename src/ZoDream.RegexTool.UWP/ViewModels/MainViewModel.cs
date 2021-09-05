using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.RegexTool.Models;
using ZoDream.RegexTool.Utils;

namespace ZoDream.RegexTool.ViewModels
{
    public class MainViewModel : BindableBase
    {
        const string HISTORY_KEY = "history";
        const string SPLIT_TAG = "Zz|oO";

        public MainViewModel()
        {
            LoadHistory();
        }

        private ObservableCollection<MatchItem> matchItems = new ObservableCollection<MatchItem>();

        public ObservableCollection<MatchItem> MatchItems
        {
            get => matchItems;
            set => Set(ref matchItems, value);
        }

        private ObservableCollection<string> histories = new ObservableCollection<string>();

        public ObservableCollection<string> Histories
        {
            get => histories;
            set => Set(ref histories, value);
        }


        public void LoadHistory()
        {
            var val = AppData.GetValue<string>(HISTORY_KEY);
            if (string.IsNullOrWhiteSpace(val))
            {
                return;
            }
            foreach (var item in val.Split(new string[] { SPLIT_TAG }, StringSplitOptions.RemoveEmptyEntries))
            {
                Histories.Add(item);
            }
        }

        public void AddHistory(string val)
        {
            if (Histories.Contains(val))
            {
                return;
            }
            Histories.Add(val);
            AppData.SetValue(HISTORY_KEY, string.Join(SPLIT_TAG, Histories));
        }

    }
}
