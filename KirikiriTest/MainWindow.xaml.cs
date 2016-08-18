using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
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
            RunTjs(contents, FilenameTextBox.Text);
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
            DateTime opStart = DateTime.Now;

            Token token;
            var reader = new StringSourceReader(title, input);
            var lexer = new Lexer(reader);
            var morpher = new Morpher(lexer);
            var sb = new StringBuilder();
            do
            {
                try
                {
                    token = morpher.ReadToken();
                }
                catch (ParseException ex)
                {
                    sb.AppendLine(Environment.NewLine +
                        ex.Position + Environment.NewLine +
                        ex.GetType() + Environment.NewLine +
                        ex.Message + Environment.NewLine);
                    break;
                }
                catch (Exception ex)
                {
                    sb.AppendLine(Environment.NewLine +
                        ex.GetType() + Environment.NewLine +
                        ex.Message + Environment.NewLine);
                    break;
                }
                if (DisplayOutput(title))
                    sb.AppendLine(token.ToString());
            } while (token.Type != TokenType.Eof);
            
            DateTime opEnd = DateTime.Now;
            TimeSpan duration = opEnd - opStart;
            sb.AppendLine($"Done in {duration.TotalMilliseconds}ms." + Environment.NewLine);
            Log(sb.ToString());
        }

        private bool DisplayOutput(string inputName)
        {
            bool displayOutput = false;
            Dispatcher.Invoke(() =>
            {
                if (inputName == "input")
                    displayOutput = DisplayInputResult.IsChecked == true;
                else displayOutput = DisplayFileResult.IsChecked == true;
            });
            return displayOutput;
        }

        private void ParseKag(string input, string title = "input")
        {
            ClearLog();
            DateTime opStart = DateTime.Now;

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
                expression?.Print(sb, true, 0);
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
            if (DisplayOutput(title))
                Log(sb.ToString());
            if (errors.Length > 0)
            {
                Log(numErrors+" Errors:");
                Log(errors.ToString());
            }

            DateTime opEnd = DateTime.Now;
            TimeSpan duration = opEnd - opStart;
            Log($"Done in {duration.TotalMilliseconds}ms." + Environment.NewLine);
        }

        private void RunTjs(string input, string title = "input")
        {
            ClearLog();
            DateTime opStart = DateTime.Now;

            try
            {
                Tjs.mStorage = null;
                Tjs.Initialize();
                var mScriptEngine = new Tjs();
                Tjs.SetConsoleOutput(new DelegateConsoleOutput(DisplayOutput(title) ? Log : (Action<string>) (m => { }), Log));

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

            DateTime opEnd = DateTime.Now;
            TimeSpan duration = opEnd - opStart;
            Log($"Done in {duration.TotalMilliseconds}ms." + Environment.NewLine);
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
