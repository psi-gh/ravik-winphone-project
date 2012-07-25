using System;
using System.Linq;
using System.Windows;
using System.ComponentModel;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows.Controls;

namespace TestApp
{
    
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
            this.Loaded += (object sender, RoutedEventArgs e) =>
            {
                if (!((MainViewModel)DataContext).IsDataLoaded)
                {
                    ((MainViewModel)DataContext).LoadData();
                }
            };
        }

        private void StackPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ((MainViewModel)DataContext).DelItem((ItemViewModel)(((StackPanel)sender).DataContext));
        }

        private void Button_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
           ((MainViewModel)DataContext).AddItem(new ItemViewModel() { Element = "Element", Type = "Type"});
        }
    }

    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            this.items = new ObservableCollection<ItemViewModel>();
            CreateItemsSource(items);
        }

        public IEnumerable Items { get; private set; }
        private ObservableCollection<ItemViewModel> items { get; set; }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        public void LoadData()
        {
            var j = 1;
            for (var i = 1; i <= 10; i++)
            {
                if (j > 4) j = 1;
                this.items.Add(new ItemViewModel() { Element = "Element " + i.ToString(), Type = "Type " + j.ToString() });
                j++;
            }
            CreateItemsSource(items);
            this.IsDataLoaded = true;
        }

        public void AddItem(ItemViewModel Item)
        {
            this.items.Add(Item);
            CreateItemsSource(items);
        }

        public void DelItem(ItemViewModel Item)
        {
            this.items.Remove(Item);
            CreateItemsSource(items);
        }

        private void CreateItemsSource(ObservableCollection<ItemViewModel> items)
        {
            this.Items = from item in items
                         group item by item.Type into n
                         orderby n.Key
                         select new GroupingLayer<string, ItemViewModel>(n);
            NotifyPropertyChanged("Items");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class ItemViewModel
    {
        public string Element
        {
            get;
            set;
        }
        public string Type
        {
            get;
            set;
        }
    }

    public class GroupingLayer<TKey, TElement> : IGrouping<TKey, TElement>
    {
        private readonly IGrouping<TKey, TElement> grouping;
        public GroupingLayer(IGrouping<TKey, TElement> unit)
        {
            grouping = unit; 
        }
        public TKey Key
        {
            get
            {
                return grouping.Key; 
            } 
        }
        public override bool Equals(object obj)
        {
            GroupingLayer<TKey, ItemViewModel> that = obj as GroupingLayer<TKey, ItemViewModel>;
            return (that != null) && (this.Key.Equals(that.Key));
        }
        public override int GetHashCode() 
        {
            return base.GetHashCode(); 
        }
        public IEnumerator<TElement> GetEnumerator()
        { 
            return grouping.GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() 
        {
            return grouping.GetEnumerator(); 
        }
    }
}