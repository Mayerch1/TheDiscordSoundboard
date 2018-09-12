using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DicsordBot
{
#pragma warning disable CS1591

    /// <summary>
    /// Interaction logic for SearchMode.xaml
    /// </summary>
    public partial class SearchMode : UserControl, INotifyPropertyChanged
    {
        private ObservableCollection<Data.FileData> filteredFiles;
        public ObservableCollection<Data.FileData> FilteredFiles { get { return filteredFiles; } set { filteredFiles = value; OnPropertyChanged("FilteredFiles"); } }

        public delegate void ListItemClickedHandler(uint tag);

        public ListItemClickedHandler ListItemClicked;

        public SearchMode()
        {
            //make deep copy
            FilteredFiles = new ObservableCollection<Data.FileData>(Handle.Data.Files);

            InitializeComponent();
            this.DataContext = this;
        }

        private bool checkContainLowerCase(Data.FileData file, string filter)
        {
            string filterLow = filter.ToLower();

            if (file.Name.ToLower().Contains(filterLow) || file.Author.ToLower().Contains(filterLow)
                || file.Album.ToLower().Contains(filterLow) || file.Genre.ToLower().Contains(filterLow))
            {
                return true;
            }
            else
                return false;
        }

        private void filterListBox(string filter)
        {
            //clear list and apply filter

            if (!string.IsNullOrEmpty(filter))
            {
                FilteredFiles.Clear();

                foreach (var file in Handle.Data.Files)
                {
                    //add all files matching
                    if (checkContainLowerCase(file, filter))
                        FilteredFiles.Add(file);
                }
            }
            else
            {
                //reset filter if empty
                //make deep copy
                FilteredFiles = new ObservableCollection<Data.FileData>(Handle.Data.Files);
            }
        }

        #region event

        private void box_Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            filterListBox(((TextBox)sender).Text);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion event

        private void stack_list_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            uint tag = (uint)((StackPanel)sender).Tag;

            ListItemClicked(tag);
        }
    }

#pragma warning restore CS1591
}