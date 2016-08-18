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
using KagSharp;
using KagSharp.Expressions;
using KagSharp.Lexer;
using Tjs2;
using Tjs2.Engine;
using Lexer = KagSharp.Lexer.Lexer;

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
        

        private void LexKagFileButton_Click(object sender, RoutedEventArgs e)
        {
            string filename = FilenameTextBox.Text;
            string contents;
            try
            {
                contents = CheckFile(filename);
            }
            catch (Exception ex)
            {
                GraupelTextBox.Text = ex.ToString();
                return;
            }
            LexKag(contents, filename);
        }

        private void ParseKagFileButton_Click(object sender, RoutedEventArgs e)
        {
            string filename = FilenameTextBox.Text;
            string contents;
            try
            {
                contents = CheckFile(filename);
            }
            catch (Exception ex)
            {
                GraupelTextBox.Text = ex.ToString();
                return;
            }
            ParseKag(contents, filename);
        }

        private void RunTjsFileButton_Click(object sender, RoutedEventArgs e)
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
            RunTjs(contents);
        }


        private string CheckFile(string filename)
        {
            var fileInfo = new FileInfo(filename);
            if (fileInfo.Length > 1024 * 256)
                throw new InvalidOperationException("File is too large.");
            return File.ReadAllText(filename);
        }


        private void LexKagInputButton_Click(object sender, RoutedEventArgs e)
        {
            string contents = InputTextBox.Text;
            Task.Run(() => LexKag(contents));
        }

        private void ParseKagInputButton_Click(object sender, RoutedEventArgs e)
        {
            string contents = InputTextBox.Text;
            Task.Run(() => ParseKag(contents));
        }

        private void RunTjsInputButton_Click(object sender, RoutedEventArgs e)
        {
            string contents = InputTextBox.Text;
            Task.Run(() => RunTjs(contents));
        }


        private void LexKag(string input, string title = "input")
        {
            ClearLog();
            Token token;
            var reader = new StringSourceReader(title, input);
            var lexer = new Lexer(reader);
            var morpher = new Morpher(lexer);
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

        private void ParseKag(string input, string title = "input")
        {
            ClearLog();
            var reader = new StringSourceReader(title, input);
            var lexer = new Lexer(reader);
            var morpher = new Morpher(lexer);
            var parser = new KagParser(morpher);
            var sb = new StringBuilder();

            int numErrors = 0;
            var errors = new StringBuilder();
            IExpression expression = null;
            while (!(expression is EofExpression))
            {
                expression?.Print(sb, true);
                try
                {
                    expression = parser.ParseExpression<DocumentExpression>();
                }
                catch (ParseException ex)
                {
                    string error = Environment.NewLine +
                        ex.Position + Environment.NewLine +
                        ex.GetType() + Environment.NewLine +
                        ex.Message + Environment.NewLine;
                    sb.AppendLine(error);
                    errors.AppendLine(error);
                    numErrors++;
                }
                catch (Exception ex)
                {
                    string error = Environment.NewLine +
                        ex.GetType() + Environment.NewLine +
                        ex.Message + Environment.NewLine;
                    sb.AppendLine(error);
                    errors.AppendLine(error);
                    numErrors++;
                }
            }
            Log(sb.ToString());
            if (errors.Length > 0)
            {
                Log(numErrors+" Errors:");
                Log(errors.ToString());
            }
        }

        private void RunTjs(string input)
        {
            ClearLog();
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
            catch (TjsScriptError ex)
            {
                Log("Line: " + ex.GetSourceLine());
                Log("Pos: " + ex.GetPosition());
                Log("Block: " + ex.GetBlockName());
                Log("Trace: ");
                Log(ex.GetTrace());
                Log(ex + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Log(ex + Environment.NewLine);
            }
            Log("Done." + Environment.NewLine);
        }

        public void Log(string data)
        {
            Dispatcher.BeginInvoke(new Action(() => GraupelTextBox.AppendText(data + Environment.NewLine)));
        }

        public void ClearLog()
        {
            Dispatcher.BeginInvoke(new Action(() => GraupelTextBox.Clear()));
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
