using System.Windows;
using System.Windows.Controls;

namespace FlexibleCSVE
{
    /// <summary>
    /// Interaction logic for SearchWindow.xaml
    /// </summary>
    public partial class SearchWindow : Window
    {
        Window mainWindow = Application.Current.MainWindow;

        public SearchWindow()
        {
            InitializeComponent();
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            ListBox contentListBox = (ListBox)mainWindow.FindName("contentListBox");

            int startIndex = 0;
            if (searchFromStartCheckBox.IsChecked == false)
            {
                startIndex = contentListBox.SelectedIndex + 1;
            } else
            {
                searchFromStartCheckBox.IsChecked = false;
                searchFromStartCheckBox.UpdateLayout();
            }

            for (int i = startIndex; i < contentListBox.Items.Count - 1; i++)
            {
                string entryLine = (string)contentListBox.Items.GetItemAt(i);
                string searchText = searchTextBox.Text;

                if (caseSensitiveCheckBox.IsChecked == false)
                {
                    entryLine = entryLine.ToLower();
                    searchText = searchText.ToLower();
                }

                if (entryLine.Contains(searchText))
                {
                    contentListBox.SelectedIndex = i;
                    contentListBox.ScrollIntoView(contentListBox.SelectedItem);
                    return;
                }
            }

            MessageBox.Show("The given value was not found");
        }
    }
}
