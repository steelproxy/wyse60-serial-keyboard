using System;
using System.IO.Ports;
using Interceptor;
using System.Windows.Forms;
using Keys = Interceptor.Keys;

namespace SerialTerminal
{
    class Program
    {
        static public Tuple<Keys, bool> CharacterToKeysEnum(char c)
        {
            switch (Char.ToLower(c))
            {
                case 'a':
                    return new Tuple<Keys, bool>(Keys.A, false);
                case 'b':
                    return new Tuple<Keys, bool>(Keys.B, false);
                case 'c':
                    return new Tuple<Keys, bool>(Keys.C, false);
                case 'd':
                    return new Tuple<Keys, bool>(Keys.D, false);
                case 'e':
                    return new Tuple<Keys, bool>(Keys.E, false);
                case 'f':
                    return new Tuple<Keys, bool>(Keys.F, false);
                case 'g':
                    return new Tuple<Keys, bool>(Keys.G, false);
                case 'h':
                    return new Tuple<Keys, bool>(Keys.H, false);
                case 'i':
                    return new Tuple<Keys, bool>(Keys.I, false);
                case 'j':
                    return new Tuple<Keys, bool>(Keys.J, false);
                case 'k':
                    return new Tuple<Keys, bool>(Keys.K, false);
                case 'l':
                    return new Tuple<Keys, bool>(Keys.L, false);
                case 'm':
                    return new Tuple<Keys, bool>(Keys.M, false);
                case 'n':
                    return new Tuple<Keys, bool>(Keys.N, false);
                case 'o':
                    return new Tuple<Keys, bool>(Keys.O, false);
                case 'p':
                    return new Tuple<Keys, bool>(Keys.P, false);
                case 'q':
                    return new Tuple<Keys, bool>(Keys.Q, false);
                case 'r':
                    return new Tuple<Keys, bool>(Keys.R, false);
                case 's':
                    return new Tuple<Keys, bool>(Keys.S, false);
                case 't':
                    return new Tuple<Keys, bool>(Keys.T, false);
                case 'u':
                    return new Tuple<Keys, bool>(Keys.U, false);
                case 'v':
                    return new Tuple<Keys, bool>(Keys.V, false);
                case 'w':
                    return new Tuple<Keys, bool>(Keys.W, false);
                case 'x':
                    return new Tuple<Keys, bool>(Keys.X, false);
                case 'y':
                    return new Tuple<Keys, bool>(Keys.Y, false);
                case 'z':
                    return new Tuple<Keys, bool>(Keys.Z, false);
                case '1':
                    return new Tuple<Keys, bool>(Keys.One, false);
                case '2':
                    return new Tuple<Keys, bool>(Keys.Two, false);
                case '3':
                    return new Tuple<Keys, bool>(Keys.Three, false);
                case '4':
                    return new Tuple<Keys, bool>(Keys.Four, false);
                case '5':
                    return new Tuple<Keys, bool>(Keys.Five, false);
                case '6':
                    return new Tuple<Keys, bool>(Keys.Six, false);
                case '7':
                    return new Tuple<Keys, bool>(Keys.Seven, false);
                case '8':
                    return new Tuple<Keys, bool>(Keys.Eight, false);
                case '9':
                    return new Tuple<Keys, bool>(Keys.Nine, false);
                case '0':
                    return new Tuple<Keys, bool>(Keys.Zero, false);
                case '-':
                    return new Tuple<Keys, bool>(Keys.DashUnderscore, false);
                case '+':
                    return new Tuple<Keys, bool>(Keys.PlusEquals, false);
                case '[':
                    return new Tuple<Keys, bool>(Keys.OpenBracketBrace, false);
                case ']':
                    return new Tuple<Keys, bool>(Keys.CloseBracketBrace, false);
                case ';':
                    return new Tuple<Keys, bool>(Keys.SemicolonColon, false);
                case '\'':
                    return new Tuple<Keys, bool>(Keys.SingleDoubleQuote, false);
                case ',':
                    return new Tuple<Keys, bool>(Keys.CommaLeftArrow, false);
                case '.':
                    return new Tuple<Keys, bool>(Keys.PeriodRightArrow, false);
                case '/':
                    return new Tuple<Keys, bool>(Keys.ForwardSlashQuestionMark, false);
                case '{':
                    return new Tuple<Keys, bool>(Keys.OpenBracketBrace, true);
                case '}':
                    return new Tuple<Keys, bool>(Keys.CloseBracketBrace, true);
                case ':':
                    return new Tuple<Keys, bool>(Keys.SemicolonColon, true);
                case '\"':
                    return new Tuple<Keys, bool>(Keys.SingleDoubleQuote, true);
                case '<':
                    return new Tuple<Keys, bool>(Keys.CommaLeftArrow, true);
                case '>':
                    return new Tuple<Keys, bool>(Keys.PeriodRightArrow, true);
                case '?':
                    return new Tuple<Keys, bool>(Keys.ForwardSlashQuestionMark, true);
                case '\\':
                    return new Tuple<Keys, bool>(Keys.BackslashPipe, false);
                case '|':
                    return new Tuple<Keys, bool>(Keys.BackslashPipe, true);
                case '`':
                    return new Tuple<Keys, bool>(Keys.Tilde, false);
                case '~':
                    return new Tuple<Keys, bool>(Keys.Tilde, true);
                case '!':
                    return new Tuple<Keys, bool>(Keys.One, true);
                case '@':
                    return new Tuple<Keys, bool>(Keys.Two, true);
                case '#':
                    return new Tuple<Keys, bool>(Keys.Three, true);
                case '$':
                    return new Tuple<Keys, bool>(Keys.Four, true);
                case '%':
                    return new Tuple<Keys, bool>(Keys.Five, true);
                case '^':
                    return new Tuple<Keys, bool>(Keys.Six, true);
                case '&':
                    return new Tuple<Keys, bool>(Keys.Seven, true);
                case '*':
                    return new Tuple<Keys, bool>(Keys.Eight, true);
                case '(':
                    return new Tuple<Keys, bool>(Keys.Nine, true);
                case ')':
                    return new Tuple<Keys, bool>(Keys.Zero, true);
                case ' ':
                    return new Tuple<Keys, bool>(Keys.Space, true);
                case (char) 8: // adjusted for wy60
                    return new Tuple<Keys, bool>(Keys.Backspace, true);
                case (char) 13:
                    return new Tuple<Keys, bool>(Keys.Enter, false);
                case (char) 20:
                    return new Tuple<Keys, bool>(Keys.F1, false);
                default:
                    return new Tuple<Keys, bool>(Keys.ForwardSlashQuestionMark, true);
            }
        }
        static SerialPort _serialPort;
        static void Main(string[] args)
        {
            Input input = new Input();
            KeysConverter keyconvertobj = new KeysConverter();
            int arrow_escape = 0;
            input.KeyboardFilterMode = KeyboardFilterMode.All;
            input.Load();
            if (!input.IsLoaded)
                return;
            _serialPort = new SerialPort("COM8", 19200, Parity.None, 8, StopBits.One);
            _serialPort.Handshake = Handshake.None;
            Console.WriteLine("Reading terminal settings...");
            _serialPort.ReadTimeout = 5000000;
            _serialPort.WriteTimeout = 500;
            _serialPort.Open();
            if(!_serialPort.IsOpen)
            {
                Console.WriteLine("Could not open serial port!");
                return;
            }
            for (; ; )
            {
                string inputstring = _serialPort.ReadExisting(); ;
                foreach (char a in inputstring)
                {

                    if (inputstring == null)
                    {
                        _serialPort.DiscardInBuffer();
                        break;
                    }
                    bool caps = false;
                    var tuple = CharacterToKeysEnum(a);
                    if (a == 27 || arrow_escape > 0) // gotta read next two values for arrows
                    {
                        switch(inputstring)
                        {
                            case "\u001B\u004F\u0041":
                                Console.WriteLine("got up");
                                input.SendKey(Keys.Up);
                                break;

                            case "\u001B\u004F\u0042":
                                Console.WriteLine("got down");
                                input.SendKey(Keys.Down);
                                break;

                            case "\u001B\u004F\u0043":
                                Console.WriteLine("got right");
                                input.SendKey(Keys.Right);
                                break;

                            case "\u001B\u004F\u0044":
                                Console.WriteLine("got left");
                                input.SendKey(Keys.Left);
                                break;

                        // F1 in OG Tuple

                            case "\u001B\u005B\u003f\u0033\u0069":
                                Console.WriteLine("got f2");
                                input.SendKey(Keys.Left);
                                break;

                            case "\u001B\u005B\u0032\u0069":
                                Console.WriteLine("got f3");
                                input.SendKey(Keys.Left);
                                break;

                            case "\u001B\u005B\u0040":
                                Console.WriteLine("got f4");
                                input.SendKey(Keys.Left);
                                break;

                            case "\u001B\u005B\u004d":
                                Console.WriteLine("got f5");
                                input.SendKey(Keys.Left);
                                break;

                            case "\u001B\u005B\u0031\u0037\u007e":
                                Console.WriteLine("got f6");
                                input.SendKey(Keys.Left);
                                break;

                            case "\u001B\u005B\u0031\u0038\u007e":
                                Console.WriteLine("got f7");
                                input.SendKey(Keys.Left);
                                break;

                            case "\u001B\u005B\u0031\u0039\u007e":
                                Console.WriteLine("got f8");
                                input.SendKey(Keys.Left);
                                break;

                            case "\u001B\u005B\u0032\u0030\u007e":
                                Console.WriteLine("got f9");
                                input.SendKey(Keys.Left);
                                break;

                            case "\u001B\u005B\u0032\u0031\u007e":
                                Console.WriteLine("got f10");
                                input.SendKey(Keys.Left);
                                break;

                            case "\u001B\u005B\u0032\u0033\u007e":
                                Console.WriteLine("got f11");
                                input.SendKey(Keys.Left);
                                break;

                            case "\u001B\u005B\u0032\u0034\u007e":
                                Console.WriteLine("got f12");
                                input.SendKey(Keys.Left);
                                break;

                            case "\u001B\u005B\u0032\u0035\u007e":
                                Console.WriteLine("got f13");
                                input.SendKey(Keys.Left);
                                break;

                            case "\u001B\u005B\u0032\u0036\u007e":
                                Console.WriteLine("got f14");
                                input.SendKey(Keys.Left);
                                break;

                            case "\u001B\u005B\u0032\u0038\u007e":
                                Console.WriteLine("got f15");
                                input.SendKey(Keys.Left);
                                break;

                            case "\u001B\u005B\u0032\u0039\u007e":
                                Console.WriteLine("got f16");
                                input.SendKey(Keys.Left);
                                break;
                        }
                        _serialPort.DiscardInBuffer();
                        _serialPort.DiscardOutBuffer();
                        continue;
                    }
                    if (a > 40 && a < 91)
                        caps = true;    
                    if(caps == true)
                        input.SendKey(Keys.LeftShift, KeyState.Down);
                    input.SendKey(tuple.Item1);
                    if (caps == true)
                    {
                        input.SendKey(Keys.LeftShift, KeyState.Up);
                        caps = false;
                    }
                }
            }
        }
    }
}
