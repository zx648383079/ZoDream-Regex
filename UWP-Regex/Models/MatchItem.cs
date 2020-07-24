using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UWP_Regex.Models
{
    public class MatchItem: INotifyPropertyChanged
    {

        private ObservableCollection<MatchItem> children;

        public ObservableCollection<MatchItem> Children
        {
            get { 
                if (children == null)
                {
                    children = new ObservableCollection<MatchItem>();
                }
                return children; 
            }
            set { children = value; }
        }


        private string content;

        public string Content
        {
            get { return content; }
            set { 
                if (content != value)
                {
                    content = value;
                    NotifyPropertyChanged("Content");
                }
            }
        }


        public MatchItem()
        {

        }

        public MatchItem(string content)
        {
            Children = new ObservableCollection<MatchItem>();
            Content = content;
        }

        public MatchItem(Match item)
        {
            Children = new ObservableCollection<MatchItem>();
            Content = item.Value;
            if (item.Groups.Count < 2)
            {
                return;
            }
            foreach (Group group in item.Groups)
            {
                Children.Add(new MatchItem(group));
            }
        }

        public MatchItem(Group group)
        {
            Children = new ObservableCollection<MatchItem>();
            Content = group.Value;
            if (group.Captures.Count < 2)
            {
                return;
            }
            foreach (Capture item in group.Captures)
            {
                Children.Add(new MatchItem(item));
            }
        }

        public MatchItem(Capture item)
        {
            Children = new ObservableCollection<MatchItem>();
            Content = item.Value;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
