using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using Jint.Parser;
using KagSharp;
using KagSharp.Expressions;
using KagSharp.Lexer;
using Tjs2;
using Tjs2.Engine;
using Lexer = KagSharp.Lexer.Lexer;
using Token = KagSharp.Lexer.Token;

namespace KirikiriTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _useTjs;

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
            _useTjs = true;
            RunTjs(contents, FilenameTextBox.Text);
        }

        private void JintRunTjsFileButton_Click(object sender, RoutedEventArgs e)
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
            _useTjs = false;
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
            _useTjs = true;
            Task.Run(() => RunTjs(contents));
        }

        private void JintRunTjsInputButton_Click(object sender, RoutedEventArgs e)
        {
            string contents = InputTextBox.Text;
            _useTjs = false;
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

            if (_useTjs)
            {
                try
                {
                    Tjs.mStorage = null;
                    Tjs.Initialize();
                    var mScriptEngine = new Tjs();
                    Tjs.SetConsoleOutput(
                        new DelegateConsoleOutput(DisplayOutput(title) ? Log : (Action<string>) (m => { }), Log));

                    Dispatch2 dsp = mScriptEngine.GetGlobal();
                    var ret = new Variant();

                    mScriptEngine.ExecScript(input, ret, dsp, null, 0);
                }
                catch (TjsScriptError ex)
                {
                    int line, col;
                    input.TryGetPosition(ex.GetPosition(), out line, out col);
                    Log($"Line {line}, Col {col}");
                    Log(input.GetLine(line));
                    Log("↑".PadLeft(col, ' '));

                    string trace = ex.GetTrace();
                    if (!String.IsNullOrWhiteSpace(trace))
                        Log("Trace: " + trace);

                    Log(ex + Environment.NewLine);
                }
                catch (Exception ex)
                {
                    Log(ex + Environment.NewLine);
                }
            }
            else
            {
                var engine = new Jint.Engine();
                try
                {
                    engine.Execute(input, new ParserOptions {Tolerant=true});
                }
                catch (Jint.Parser.ParserException ex)
                {
                    Log($"Line {ex.LineNumber}, Col {ex.Column}");
                    string source = input.GetLine(ex.LineNumber);
                    Log(source);
                    string whited = Regex.Replace(source.Substring(0, ex.Column-1), @"[^\t]", " ", RegexOptions.Compiled) + "↑";
                    Log(whited);
                    Log(ex + Environment.NewLine);
                }
                catch (Jint.Runtime.JavaScriptException ex)
                {
                    Log($"Line {ex.LineNumber}, Col {ex.Column}");
                    string source = GetSource(input, ex.Location);
                    Log(source);
                    string whited = Regex.Replace(source.Substring(0,ex.Column-1), @"[^\t]", " ", RegexOptions.Compiled) + "↑";
                    Log(whited);
                    //Log("↑".PadLeft(ex.Column, ' '));
                    Log(ex + Environment.NewLine);
                }
                catch (Exception ex)
                {
                    Log(ex + Environment.NewLine);
                }
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

        private static string GetSource(string input, Jint.Parser.Location location)
        {
            return input.GetSegment(location.Start.Line, location.Start.Column, location.End.Line, location.End.Column);
        }

    }



}
