using System;
using System.Collections.Generic;
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
using KirikiriSharp.Lexer;

namespace KirikiriTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new Microsoft.Win32.OpenFileDialog();
            if (fileDialog.ShowDialog() == true)
            {
                string fileName = fileDialog.FileName;
                FilenameTextBox.Text = fileName;
                SubmitFile(fileName);
            }
        }

        private void SubmitFileButton_Click(object sender, RoutedEventArgs e)
        {
            SubmitFile(FilenameTextBox.Text);
        }

        private void SubmitFile(string fileName)
        {
            FileInfo fileInfo;
            try
            {
                fileInfo = new FileInfo(fileName);
            }
            catch (Exception ex)
            {
                GraupelTextBox.Text = ex.ToString();
                return;
            }
            if (fileInfo.Length > 1024 * 16)
            {
                GraupelTextBox.Text = "File too large!";
                return;
            }

            string contents;
            try
            {
                contents = File.ReadAllText(fileName);
            }
            catch (Exception ex)
            {
                GraupelTextBox.Text = ex.ToString();
                return;
            }

            TestScript(contents);
        }

        private void TestStringButton_Click(object sender, RoutedEventArgs e)
        {
            string contents = InputTextBox.Text;
            TestScript(contents);
        }

        private void TestScript(string input)
        {
            GraupelTextBox.Text = String.Empty;

            var reader = new StringSourceReader("input", input);
            var lexer = new Lexer(reader);

            var morpher = new Morpher(lexer);
            Token token;
            do
            {
                try
                {
                    token = morpher.ReadToken();
                }
                catch (Exception ex)
                {
                    GraupelTextBox.Text += Environment.NewLine + ex.Message + Environment.NewLine;
                    return;
                }
                GraupelTextBox.Text += token + Environment.NewLine;
            } while (token.Type != TokenType.Eof);
        }
    }
}
