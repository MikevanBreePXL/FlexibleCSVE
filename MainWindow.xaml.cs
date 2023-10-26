using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FlexibleCSVE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly FontFamily _textFont = new("Bahnschrift Semibold");
        private readonly IList<string> commaSpecifiers = new List<string> { ";", ":", "," };

        private string filePath;
        private string[] columnNamesList;

        public MainWindow()
        {
            InitializeComponent();
            specifierComboBox.ItemsSource = commaSpecifiers;
        }

        private void openMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog? csv = csvFileDialog();
            if (csv != null)
            {   // File selected:
                filePath = csv.FileName;
                contentListBox.Items.Clear();
                string[] totalContentList;
                using (StreamReader reader = new StreamReader(filePath))
                {
                    columnNamesList = reader.ReadLine().Split(specifierComboBox.SelectedValue.ToString());
                    totalContentList = reader.ReadToEnd().Split("\n");
                }
                foreach (string line in totalContentList)
                {
                    contentListBox.Items.Add(line);
                }
                contentListBox.UpdateLayout();
                drawEditor(editCanvas, columnNamesList);
                contentListBox.SelectedIndex = 0;
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
            data = ((string) contentListBox.Items[contentListBox.SelectedIndex]).Split(specifierComboBox.SelectedValue.ToString());

            for (int i = editCanvas.Children.Count / 2; i < editCanvas.Children.Count; i++)
            {
                TextBox textBox = (TextBox)editCanvas.Children[i];
                try
                {
                    textBox.Text = data[i - (editCanvas.Children.Count / 2)];
                } catch (IndexOutOfRangeException)
                {
                    textBox.Text = "";
                }
            }
        }

        private void saveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "CSV Object file | .csv";
            if (saveFile.ShowDialog() == true)
            {
                StringBuilder stringBuilder = new StringBuilder();

                // column names
                for (int i = 0; i < columnNamesList.Length - 1; i++)
                {
                    stringBuilder.Append(columnNamesList[i]);
                    if (i < columnNamesList.Length - 1)
                    {
                        stringBuilder.Append(specifierComboBox.SelectedValue);
                    }
                }
                stringBuilder.Append('\n');

                // data
                for (int i = 0; i < contentListBox.Items.Count - 1; i++)
                {
                    stringBuilder.AppendLine((string)contentListBox.Items[i]);
                }

                // save File
                using (StreamWriter writer = new StreamWriter(saveFile.FileName))
                {
                    writer.Write(stringBuilder.ToString());
                }

                MessageBox.Show("Saved Successfully", "Saved CSV file", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void searchMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SearchWindow searchWindow = new SearchWindow();
            searchWindow.ShowDialog();
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
            contentListBox.Items[contentListBox.SelectedIndex] = sb.ToString();
        }

        private void newRowButton_Click(object sender, RoutedEventArgs e)
        {
            contentListBox.Items.Add(new string(Char.Parse(specifierComboBox.SelectedValue.ToString()), columnNamesList.Length - 1));
        }

        private void deleteRowButton_Click(object sender, RoutedEventArgs e)
        {
            int toDeleteIndex = contentListBox.SelectedIndex;
            if (contentListBox.SelectedIndex == 0)
            {
                contentListBox.SelectedIndex = 1;
            }
            else
            {
                contentListBox.SelectedIndex = contentListBox.SelectedIndex - 1;
            }
            contentListBox.Items.RemoveAt(toDeleteIndex);

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

        private void specifierComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (specifierComboBox.SelectedIndex < 0 || filePath == null) { return; }
            contentListBox.Items.Clear();
            string[] totalContentList;
            using (StreamReader reader = new StreamReader(filePath))
            {
                columnNamesList = reader.ReadLine().Split(specifierComboBox.SelectedValue.ToString());
                totalContentList = reader.ReadToEnd().Split("\n");
            }
            foreach (string line in totalContentList)
            {
                contentListBox.Items.Add(line);
            }

            drawEditor(editCanvas, columnNamesList);
        }
    }
}
