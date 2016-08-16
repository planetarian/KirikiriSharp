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
using KirikiriSharp;
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
        private readonly object _lock = new object();

        public MainWindow()
        {
            InitializeComponent();

            GraupelTextBox.TextChanged += (o, a) => GraupelTextBox.ScrollToEnd();
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
            if (fileInfo.Length > 1024 * 256)
                throw new InvalidOperationException("File is too large.");
            return File.ReadAllText(fileName);
        }

        private void TestKagStringButton_Click(object sender, RoutedEventArgs e)
        {
            string contents = InputTextBox.Text;
            Task.Run(()=>TestKag(contents));
        }

        private void TestTjsStringButton_Click(object sender, RoutedEventArgs e)
        {
            string contents = InputTextBox.Text;
            Task.Run(() => TestTjs(contents));
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
                catch (ParseException ex)
                {
                    Log(Environment.NewLine +
                        ex.Position + Environment.NewLine +
                        ex.GetType() + Environment.NewLine +
                        ex.Message + Environment.NewLine);
                    break;
                }
                catch (Exception ex)
                {
                    Log(Environment.NewLine +
                        ex.GetType() + Environment.NewLine +
                        ex.Message + Environment.NewLine);
                    break;
                }
                Log(token.ToString());
            } while (token.Type != TokenType.Eof);
            Log("Done." + Environment.NewLine);
        }

        private void TestTjs(string input)
        {
            //GraupelTextBox.Text = String.Empty;
            
            try
            {
                Tjs.mStorage = null;
                Tjs.Initialize();
                var mScriptEngine = new Tjs();
                Tjs.SetConsoleOutput(new DelegateConsoleOutput(Log));

                Dispatch2 dsp = mScriptEngine.GetGlobal();
                var ret = new Variant();

                mScriptEngine.ExecScript(input, ret, dsp, null, 0);
            }
            catch (Exception ex)
            {
                Log(ex + Environment.NewLine);
            }
            Log("Done." + Environment.NewLine);
        }

        public void Log(string data)
        {
            Dispatcher.BeginInvoke(new Action(() =>GraupelTextBox.AppendText(data + Environment.NewLine)));
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
