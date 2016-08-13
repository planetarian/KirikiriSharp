using System;
using System.Collections.Generic;
using System.IO;
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
using Tjs2;
using Tjs2.Engine;

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
            var fileDialog = new Microsoft.Win32.OpenFileDialog {Filter = "KAG/TJS files (*.ks, *.tjs, *.js)|*.ks;*.tjs;*.js"};
            if (fileDialog.ShowDialog() == true)
            {
                string fileName = fileDialog.FileName;
                FilenameTextBox.Text = fileName;
            }
        }

        private void TestTjsFileButton_Click(object sender, RoutedEventArgs e)
        {
            string contents;
            try
            {
                contents = CheckFile(FilenameTextBox.Text);
            }
            catch (Exception ex)
            {
                GraupelTextBox.Text = ex.ToString();
                return;
            }
            TestTjs(contents);
        }

        private void TestKagFileButton_Click(object sender, RoutedEventArgs e)
        {
            string contents;
            try
            {
                contents = CheckFile(FilenameTextBox.Text);
            }
            catch (Exception ex)
            {
                GraupelTextBox.Text = ex.ToString();
                return;
            }
            TestKag(contents);
        }

        private string CheckFile(string fileName)
        {
            var fileInfo = new FileInfo(fileName);
            if (fileInfo.Length > 1024 * 16)
                throw new InvalidOperationException("File is too large.");
            return File.ReadAllText(fileName);
        }

        private void TestKagStringButton_Click(object sender, RoutedEventArgs e)
        {
            string contents = InputTextBox.Text;
            TestKag(contents);
        }

        private void TestTjsStringButton_Click(object sender, RoutedEventArgs e)
        {
            string contents = InputTextBox.Text;
            TestTjs(contents);
        }

        private void TestKag(string input)
        {
            var reader = new StringSourceReader("input", input);
            var lexer = new KirikiriSharp.Lexer.Lexer(reader);

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

        private void TestTjs(string input)
        {
            GraupelTextBox.Text = String.Empty;

            Action<string> outputAction = msg => GraupelTextBox.Text += msg + Environment.NewLine;
            try
            {
                Tjs.mStorage = null;
                Tjs.Initialize();
                var mScriptEngine = new Tjs();
                Tjs.SetConsoleOutput(new DelegateConsoleOutput(outputAction));

                Dispatch2 dsp = mScriptEngine.GetGlobal();
                var ret = new Variant();

                mScriptEngine.ExecScript(input, ret, dsp, null, 0);
            }
            catch (Exception ex)
            {
                outputAction(ex.ToString());
            }
        }

        public class MessageConsoleOutput : IConsoleOutput
        {
            public void ExceptionPrint(string msg)
            {
                MessageBox.Show(msg, "Error");
            }

            public void Print(string msg)
            {
                MessageBox.Show(msg, "Message");
            }
        }
    }



}
