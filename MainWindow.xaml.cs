using FlexibleCSVE;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
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

namespace FlexibleCSVE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public IList<string> _contentList = new ObservableCollection<string>();
        private FontFamily _textFont = new FontFamily("Bahnschrift Semibold");
        public IList<string> commaSpecifiers = new List<string>();

        private string[] columnNamesList;

        public MainWindow()
        {
            InitializeComponent();
            contentListBox.ItemsSource = _contentList;

            commaSpecifiers.Add(";");
            commaSpecifiers.Add(":");
            commaSpecifiers.Add(",");
            specifierComboBox.ItemsSource = commaSpecifiers;
            specifierComboBox.SelectedIndex = 0;
        }

        private void openItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog? csv = csvFileDialog();
            if (csv != null)
            {   // File selected:
                _contentList.Clear();
                string[] totalContentList;
                using (StreamReader reader = new StreamReader(csv.FileName))
                {
                    columnNamesList = reader.ReadLine().Split(specifierComboBox.SelectedValue.ToString());
                    totalContentList = reader.ReadToEnd().Split("\n");
                }
                foreach (string line in totalContentList)
                {
                    _contentList.Add(line);
                }

                drawEditor(editCanvas, columnNamesList);
            }

        }

        private OpenFileDialog? csvFileDialog()
        {
            OpenFileDialog csvFile = new OpenFileDialog();
            csvFile.Filter = "CSV Files | *.csv|Any file | .*";
            csvFile.Title = "Open CSV File";
            csvFile.Multiselect = false;
            csvFile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (csvFile.ShowDialog() == true)
            {
                return csvFile;
            } else
            {
                return null;
            }
        }

        private void contentListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (contentListBox.SelectedIndex == -1) { return; }

            string[] data;
            data = _contentList[contentListBox.SelectedIndex].Split(specifierComboBox.SelectedValue.ToString());

            for (int i = editCanvas.Children.Count / 2; i < editCanvas.Children.Count; i++)
            {
                TextBox textBox = (TextBox)editCanvas.Children[i];
                try
                {
                    textBox.Text = data[i - (editCanvas.Children.Count / 2)];
                } catch (IndexOutOfRangeException)
                {
                    textBox.Text = "0";
                }
            }
        }

        private void saveDataButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = editCanvas.Children.Count / 2; i < editCanvas.Children.Count; i++)
            {
                sb.Append(((TextBox)editCanvas.Children[i]).Text);

                if (!(i == editCanvas.Children.Count - 1))
                {
                    sb.Append(";");
                }
            }
            _contentList[contentListBox.SelectedIndex] = sb.ToString();
        }

        private void newRowButton_Click(object sender, RoutedEventArgs e)
        {
            _contentList.Add(new string(Char.Parse(specifierComboBox.SelectedValue.ToString()), columnNamesList.Length - 1));
        }

        private void exitItem_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void saveItem_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "CSV Object file | .csv";
            if (saveFile.ShowDialog() == true)
            {
                StringBuilder stringBuilder = new StringBuilder();

                // column names
                for (int i = 0; i < columnNamesList.Length; i++)
                {
                    stringBuilder.Append(columnNamesList[i]);
                    if (i < columnNamesList.Length - 1)
                    {
                        stringBuilder.Append(specifierComboBox.SelectedValue);
                    }
                }
                stringBuilder.Append("\n");

                // data
                for (int i = 0; i < _contentList.Count; i++)
                {
                    stringBuilder.Append(_contentList[i]);
                }

                // save File
                using (StreamWriter writer = new StreamWriter(saveFile.FileName))
                {
                    writer.Write(stringBuilder.ToString());
                }

                MessageBox.Show("Saved Successfully", "Saved CSV file", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void deleteRowButton_Click(object sender, RoutedEventArgs e)
        {
            _contentList.RemoveAt(contentListBox.SelectedIndex);
            contentListBox.UpdateLayout();
        }

        private void drawEditor(Canvas drawingCanvas, string[] columnNames)
        {
            drawingCanvas.Children.Clear();

            for (int i = 0; i < columnNames.Length; i++)
            {
                Label label = new Label()
                {
                    FontFamily = _textFont,
                    FontSize = 16,
                    Content = columnNames[i],
                    Margin = new Thickness(10, 10 + i * 30, 0, 0)
                };
                drawingCanvas.Children.Add(label);
            }
            for (int i = 0; i < columnNames.Length; i++)
            {
                TextBox textBox = new TextBox()
                {
                    FontFamily = _textFont,
                    FontSize = 16,
                    Width = 200,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Margin = new Thickness(0, 10 + i * 30, 0, 0)
                };
                drawingCanvas.Children.Add(textBox);
                Canvas.SetRight(textBox, 10);

                saveDataButton.IsEnabled = true;
                newRowButton.IsEnabled = true;
                deleteRowButton.IsEnabled = true;
            }
        }
    }
}
