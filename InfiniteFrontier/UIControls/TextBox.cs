using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;

using static Arwic.InfiniteFrontier.Main;

namespace Arwic.InfiniteFrontier
{
    public class TextBox : IFormComponent
    {
        /// <summary>
        /// Transform
        /// </summary>
        public Transform Transform { get; set; }
        
        /// <summary>
        /// If the characters should be masked
        /// </summary>
        public bool Masked { get; set; }

        /// <summary>
        /// The max number of character that can be entered
        /// </summary>
        public int MaxChars { get; set; }

        /// <summary>
        /// The text in the text box
        /// </summary>
        private string _text;
        public string Text { get { return _text; } set { _text = value; caretIndex = Text.Length; if (caretIndex == -1) caretIndex = 0; } }

        /// <summary>
        /// Font used to render title text
        /// </summary>
        public Font TitleFont { get; set; }

        /// <summary>
        /// Font used to render text
        /// </summary>
        public Font ContentFont { get; set; }

        /// <summary>
        /// The background sprite
        /// </summary>
        public Sprite BackgroundSprite { get; set; }
        
        /// <summary>
        /// If the text box is ready for input
        /// </summary>
        public bool Selected { get; set; }

        /// <summary>
        /// Title if the text box
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Char to mask text with if required
        /// </summary>
        public char MaskedChar { get; set; }
        
        /// <summary>
        /// If the user can click to select this text box
        /// </summary>
        public bool ClickToSelect { get; set; }
        
        private Timer keyRepeatTimer = new Timer(150);
        private Timer caretDisplayTimer = new Timer(300);
        private Keys[] lastKeys, keys;
        private int caretIndex = 0;
        private bool displayCaret = true;
        
        // constructor
        public TextBox(Transform transform, string title = "", string text = "", Sprite backSprite = null, int maxchars = int.MaxValue, bool clickToselect = true, bool masked = false, char maskedChar = '*')
        {
            if (backSprite == null) backSprite = textBox;

            TitleFont = Font.InfoLabel;
            ContentFont = Font.InfoText;
            Transform = transform;
            BackgroundSprite = backSprite;
            Title = title;
            Text = text;
            Masked = masked;
            MaxChars = maxchars;
            ClickToSelect = clickToselect;

            keys = Keyboard.GetState().GetPressedKeys();
            lastKeys = keys;
        }

        /// <summary>
        /// Updates the state of the component
        /// </summary>
        /// <param name="lastKeyboardState"></param>
        /// <param name="lastMouseState"></param>
        /// <param name="gameTime"></param>
        /// <param name="scale"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public bool Update(KeyboardState lastKeyboardState, MouseState lastMouseState, GameTime gameTime, float scale = 1f, Vector2? origin = null)
        {
            if (ClickToSelect && InputUtil.WasLeftMouseButton && Transform.Contains(InputUtil.MouseScreenPos, scale, origin))
                Selected = !Selected;

            caretDisplayTimer.Update(gameTime);
            keyRepeatTimer.Update(gameTime);

            keys = Keyboard.GetState().GetPressedKeys();

            if (Selected)
            {
                // Blink the caret
                if (caretDisplayTimer.isExpired())
                {
                    displayCaret = !displayCaret;
                    caretDisplayTimer.Start();
                }

                foreach (Keys currentkey in keys)
                {
                    if (caretIndex > Text.Length)
                        caretIndex = Text.Length;
                    if (caretIndex < 0)
                        caretIndex = 0;
                    int lastCaretIndex = caretIndex;
                    int lastTextLength = Text.Length;

                    if (keyRepeatTimer.isExpired() || !lastKeys.Contains(currentkey))
                    {
                        switch (currentkey)
                        {
                            case Keys.Back: // Backspace
                                if (caretIndex != 0)
                                {
                                    Text = Text.Remove(caretIndex - 1, 1);
                                    caretIndex = lastCaretIndex - 1;
                                }
                                break;
                            case Keys.Delete: // Delete
                                if (caretIndex != Text.Length)
                                {
                                    Text = Text.Remove(caretIndex, 1);
                                    caretIndex = lastCaretIndex;
                                }
                                break;
                            case Keys.Right: // Caret Right
                                caretIndex++;
                                if (caretIndex > Text.Length)
                                    caretIndex = Text.Length;
                                break;
                            case Keys.Left: // Caret Left
                                caretIndex--;
                                if (caretIndex < 0)
                                    caretIndex = 0;
                                break;
                            case Keys.PageDown:
                                caretIndex = 0; // Caret min
                                break;
                            case Keys.PageUp: // Caret max
                                caretIndex = Text.Length;
                                break;
                            case Keys.Escape: // Deselect
                                Selected = false;
                                break;
                            // Modifier keys
                            case Keys.CapsLock:
                            case Keys.LeftShift:
                            case Keys.RightShift:
                            case Keys.LeftAlt:
                            case Keys.RightAlt:
                            case Keys.LeftControl:
                            case Keys.RightControl:
                            case Keys.LeftWindows:
                            case Keys.RightWindows:
                            // Control pad
                            case Keys.Insert:
                            case Keys.Home:
                            case Keys.End:
                            // Function keys
                            case Keys.PrintScreen:
                            case Keys.Scroll:
                            case Keys.Pause:
                            case Keys.F1:
                            case Keys.F2:
                            case Keys.F3:
                            case Keys.F4:
                            case Keys.F5:
                            case Keys.F6:
                            case Keys.F7:
                            case Keys.F8:
                            case Keys.F9:
                            case Keys.F10:
                            case Keys.F11:
                            case Keys.F12:
                            // Numpad
                            case Keys.NumLock:
                            // Alpha numeric
                            case Keys.Tab:
                                break;
                            default:
                                HandleKey(currentkey, lastKeyboardState);
                                if (Text.Length != lastTextLength) caretIndex = lastCaretIndex + 1;
                                break;
                        }
                        keyRepeatTimer.Start();
                    }

                    if (currentkey == Keys.Right || currentkey == Keys.Left)
                    {
                        displayCaret = true;
                    }
                }
            }
            else
            {
                displayCaret = false;
            }

            lastKeys = keys;

            if (Transform.Contains(InputUtil.MouseScreenPos, scale, origin))
                return true;
            return false;
        }
        private void HandleKey(Keys currentKey, KeyboardState lastKeyboardState)
        {
            string keyString = currentKey.ToString();
            string toAdd = "";

            #region shift
            if (lastKeyboardState.IsKeyDown(Keys.RightShift) || lastKeyboardState.IsKeyDown(Keys.LeftShift) || System.Windows.Forms.Control.IsKeyLocked(System.Windows.Forms.Keys.CapsLock))
            {
                switch (currentKey)
                {
                    case Keys.A:
                        toAdd += "A";
                        break;
                    case Keys.Add:
                        toAdd += "+";
                        break;
                    case Keys.Apps:
                        break;
                    case Keys.Attn:
                        break;
                    case Keys.B:
                        toAdd += "B";
                        break;
                    case Keys.BrowserBack:
                        break;
                    case Keys.BrowserFavorites:
                        break;
                    case Keys.BrowserForward:
                        break;
                    case Keys.BrowserHome:
                        break;
                    case Keys.BrowserRefresh:
                        break;
                    case Keys.BrowserSearch:
                        break;
                    case Keys.BrowserStop:
                        break;
                    case Keys.C:
                        toAdd += "C";
                        break;
                    case Keys.CapsLock:
                        break;
                    case Keys.ChatPadGreen:
                        break;
                    case Keys.ChatPadOrange:
                        break;
                    case Keys.Crsel:
                        break;
                    case Keys.D:
                        toAdd += "D";
                        break;
                    case Keys.D0:
                        toAdd += ")";
                        break;
                    case Keys.D1:
                        toAdd += "!";
                        break;
                    case Keys.D2:
                        toAdd += "@";
                        break;
                    case Keys.D3:
                        toAdd += "#";
                        break;
                    case Keys.D4:
                        toAdd += "$";
                        break;
                    case Keys.D5:
                        toAdd += "%";
                        break;
                    case Keys.D6:
                        toAdd += "^";
                        break;
                    case Keys.D7:
                        toAdd += "&";
                        break;
                    case Keys.D8:
                        toAdd += "*";
                        break;
                    case Keys.D9:
                        toAdd += "(";
                        break;
                    case Keys.Decimal:
                        toAdd += ">";
                        break;
                    case Keys.Delete:

                        break;
                    case Keys.Divide:
                        toAdd += "/";
                        break;
                    case Keys.Down:
                        break;
                    case Keys.E:
                        toAdd += "E";
                        break;
                    case Keys.End:
                        break;
                    case Keys.Enter:

                        break;
                    case Keys.EraseEof:
                        break;
                    case Keys.Escape:
                        Selected = false;
                        break;
                    case Keys.Execute:
                        break;
                    case Keys.Exsel:
                        break;
                    case Keys.F:
                        toAdd += "F";
                        break;
                    case Keys.G:
                        toAdd += "G";
                        break;
                    case Keys.H:
                        toAdd += "H";
                        break;
                    case Keys.Help:
                        break;
                    case Keys.Home:
                        break;
                    case Keys.I:
                        toAdd += "I";
                        break;
                    case Keys.ImeConvert:
                        break;
                    case Keys.ImeNoConvert:
                        break;
                    case Keys.Insert:
                        break;
                    case Keys.J:
                        toAdd += "J";
                        break;
                    case Keys.K:
                        toAdd += "K";
                        break;
                    case Keys.Kana:
                        break;
                    case Keys.Kanji:
                        break;
                    case Keys.L:
                        toAdd += "L";
                        break;
                    case Keys.LaunchApplication1:
                        break;
                    case Keys.LaunchApplication2:
                        break;
                    case Keys.LaunchMail:
                        break;
                    case Keys.Left:
                        break;
                    case Keys.LeftAlt:
                        break;
                    case Keys.LeftControl:
                        break;
                    case Keys.LeftShift:
                        break;
                    case Keys.LeftWindows:
                        break;
                    case Keys.M:
                        toAdd += "M";
                        break;
                    case Keys.MediaNextTrack:
                        break;
                    case Keys.MediaPlayPause:
                        break;
                    case Keys.MediaPreviousTrack:
                        break;
                    case Keys.MediaStop:
                        break;
                    case Keys.Multiply:
                        break;
                    case Keys.N:
                        toAdd += "N";
                        break;
                    case Keys.None:
                        break;
                    case Keys.NumLock:
                        break;
                    case Keys.NumPad0:
                        toAdd += "0";
                        break;
                    case Keys.NumPad1:
                        toAdd += "1";
                        break;
                    case Keys.NumPad2:
                        toAdd += "2";
                        break;
                    case Keys.NumPad3:
                        toAdd += "3";
                        break;
                    case Keys.NumPad4:
                        toAdd += "4";
                        break;
                    case Keys.NumPad5:
                        toAdd += "5";
                        break;
                    case Keys.NumPad6:
                        toAdd += "6";
                        break;
                    case Keys.NumPad7:
                        toAdd += "7";
                        break;
                    case Keys.NumPad8:
                        toAdd += "8";
                        break;
                    case Keys.NumPad9:
                        toAdd += "9";
                        break;
                    case Keys.O:
                        toAdd += "O";
                        break;
                    case Keys.Oem8:
                        break;
                    case Keys.OemAuto:
                        break;
                    case Keys.OemBackslash:
                        toAdd += "|";
                        break;
                    case Keys.OemClear:
                        break;
                    case Keys.OemCloseBrackets:
                        toAdd += ")";
                        break;
                    case Keys.OemComma:
                        toAdd += "<";
                        break;
                    case Keys.OemCopy:
                        break;
                    case Keys.OemEnlW:
                        break;
                    case Keys.OemMinus:
                        toAdd += "_";
                        break;
                    case Keys.OemOpenBrackets:
                        toAdd += "(";
                        break;
                    case Keys.OemPeriod:
                        toAdd += ">";
                        break;
                    case Keys.OemPipe:
                        break;
                    case Keys.OemPlus:
                        toAdd += "+";
                        break;
                    case Keys.OemQuestion:
                        toAdd += "?";
                        break;
                    case Keys.OemQuotes:
                        toAdd += "\"";
                        break;
                    case Keys.OemSemicolon:
                        toAdd += ":";
                        break;
                    case Keys.OemTilde:
                        //toAdd += "~";
                        break;
                    case Keys.P:
                        toAdd += "P";
                        break;
                    case Keys.Pa1:
                        break;
                    case Keys.PageDown:
                        break;
                    case Keys.PageUp:
                        break;
                    case Keys.Pause:
                        break;
                    case Keys.Play:
                        break;
                    case Keys.Print:
                        break;
                    case Keys.PrintScreen:
                        break;
                    case Keys.ProcessKey:
                        break;
                    case Keys.Q:
                        toAdd += "Q";
                        break;
                    case Keys.R:
                        toAdd += "R";
                        break;
                    case Keys.Right:
                        break;
                    case Keys.RightAlt:
                        break;
                    case Keys.RightControl:
                        break;
                    case Keys.RightShift:
                        break;
                    case Keys.RightWindows:
                        break;
                    case Keys.S:
                        toAdd += "S";
                        break;
                    case Keys.Scroll:
                        break;
                    case Keys.Select:
                        break;
                    case Keys.SelectMedia:
                        break;
                    case Keys.Separator:
                        break;
                    case Keys.Sleep:
                        break;
                    case Keys.Space:
                        toAdd += " ";
                        break;
                    case Keys.Subtract:
                        toAdd += "-";
                        break;
                    case Keys.T:
                        toAdd += "T";
                        break;
                    case Keys.Tab:
                        break;
                    case Keys.U:
                        toAdd += "U";
                        break;
                    case Keys.Up:
                        break;
                    case Keys.V:
                        toAdd += "V";
                        break;
                    case Keys.VolumeDown:
                        break;
                    case Keys.VolumeMute:
                        break;
                    case Keys.VolumeUp:
                        break;
                    case Keys.W:
                        toAdd += "W";
                        break;
                    case Keys.X:
                        toAdd += "X";
                        break;
                    case Keys.Y:
                        toAdd += "Y";
                        break;
                    case Keys.Z:
                        toAdd += "Z";
                        break;
                    case Keys.Zoom:
                        break;
                    default:
                        //this.toAdd += keyString;
                        //COut.Write("Unrecognised character \"" + keyString + "\" please notify Arwic if its an important one", MsgType.Failed);
                        break;
                }
            }
            #endregion

            #region no mod
            else
            {
                switch (currentKey)
                {
                    case Keys.A:
                        toAdd += "a";
                        break;
                    case Keys.Add:
                        toAdd += "+";
                        break;
                    case Keys.Apps:
                        break;
                    case Keys.Attn:
                        break;
                    case Keys.B:
                        toAdd += "b";
                        break;
                    case Keys.BrowserBack:
                        break;
                    case Keys.BrowserFavorites:
                        break;
                    case Keys.BrowserForward:
                        break;
                    case Keys.BrowserHome:
                        break;
                    case Keys.BrowserRefresh:
                        break;
                    case Keys.BrowserSearch:
                        break;
                    case Keys.BrowserStop:
                        break;
                    case Keys.C:
                        toAdd += "c";
                        break;
                    case Keys.CapsLock:
                        break;
                    case Keys.ChatPadGreen:
                        break;
                    case Keys.ChatPadOrange:
                        break;
                    case Keys.Crsel:
                        break;
                    case Keys.D:
                        toAdd += "d";
                        break;
                    case Keys.D0:
                        toAdd += "0";
                        break;
                    case Keys.D1:
                        toAdd += "1";
                        break;
                    case Keys.D2:
                        toAdd += "2";
                        break;
                    case Keys.D3:
                        toAdd += "3";
                        break;
                    case Keys.D4:
                        toAdd += "4";
                        break;
                    case Keys.D5:
                        toAdd += "5";
                        break;
                    case Keys.D6:
                        toAdd += "6";
                        break;
                    case Keys.D7:
                        toAdd += "7";
                        break;
                    case Keys.D8:
                        toAdd += "8";
                        break;
                    case Keys.D9:
                        toAdd += "9";
                        break;
                    case Keys.Decimal:
                        toAdd += ".";
                        break;
                    case Keys.Delete:

                        break;
                    case Keys.Divide:
                        toAdd += "/";
                        break;
                    case Keys.Down:
                        break;
                    case Keys.E:
                        toAdd += "e";
                        break;
                    case Keys.End:
                        break;
                    case Keys.Enter:

                        break;
                    case Keys.EraseEof:
                        break;
                    case Keys.Escape:
                        Selected = false;
                        break;
                    case Keys.Execute:
                        break;
                    case Keys.Exsel:
                        break;
                    case Keys.F:
                        toAdd += "f";
                        break;
                    case Keys.F1:
                        break;
                    case Keys.F10:
                        break;
                    case Keys.F11:
                        break;
                    case Keys.F12:
                        break;
                    case Keys.F13:
                        break;
                    case Keys.F14:
                        break;
                    case Keys.F15:
                        break;
                    case Keys.F16:
                        break;
                    case Keys.F17:
                        break;
                    case Keys.F18:
                        break;
                    case Keys.F19:
                        break;
                    case Keys.F2:
                        break;
                    case Keys.F20:
                        break;
                    case Keys.F21:
                        break;
                    case Keys.F22:
                        break;
                    case Keys.F23:
                        break;
                    case Keys.F24:
                        break;
                    case Keys.F3:
                        break;
                    case Keys.F4:
                        break;
                    case Keys.F5:
                        break;
                    case Keys.F6:
                        break;
                    case Keys.F7:
                        break;
                    case Keys.F8:
                        break;
                    case Keys.F9:
                        break;
                    case Keys.G:
                        toAdd += "g";
                        break;
                    case Keys.H:
                        toAdd += "h";
                        break;
                    case Keys.Help:
                        break;
                    case Keys.Home:
                        break;
                    case Keys.I:
                        toAdd += "i";
                        break;
                    case Keys.ImeConvert:
                        break;
                    case Keys.ImeNoConvert:
                        break;
                    case Keys.Insert:
                        break;
                    case Keys.J:
                        toAdd += "j";
                        break;
                    case Keys.K:
                        toAdd += "k";
                        break;
                    case Keys.Kana:
                        break;
                    case Keys.Kanji:
                        break;
                    case Keys.L:
                        toAdd += "l";
                        break;
                    case Keys.LaunchApplication1:
                        break;
                    case Keys.LaunchApplication2:
                        break;
                    case Keys.LaunchMail:
                        break;
                    case Keys.Left:
                        break;
                    case Keys.LeftAlt:
                        break;
                    case Keys.LeftControl:
                        break;
                    case Keys.LeftShift:
                        break;
                    case Keys.LeftWindows:
                        break;
                    case Keys.M:
                        toAdd += "m";
                        break;
                    case Keys.MediaNextTrack:
                        break;
                    case Keys.MediaPlayPause:
                        break;
                    case Keys.MediaPreviousTrack:
                        break;
                    case Keys.MediaStop:
                        break;
                    case Keys.Multiply:
                        break;
                    case Keys.N:
                        toAdd += "n";
                        break;
                    case Keys.None:
                        break;
                    case Keys.NumLock:
                        break;
                    case Keys.NumPad0:
                        toAdd += "0";
                        break;
                    case Keys.NumPad1:
                        toAdd += "1";
                        break;
                    case Keys.NumPad2:
                        toAdd += "2";
                        break;
                    case Keys.NumPad3:
                        toAdd += "3";
                        break;
                    case Keys.NumPad4:
                        toAdd += "4";
                        break;
                    case Keys.NumPad5:
                        toAdd += "5";
                        break;
                    case Keys.NumPad6:
                        toAdd += "6";
                        break;
                    case Keys.NumPad7:
                        toAdd += "7";
                        break;
                    case Keys.NumPad8:
                        toAdd += "8";
                        break;
                    case Keys.NumPad9:
                        toAdd += "9";
                        break;
                    case Keys.O:
                        toAdd += "o";
                        break;
                    case Keys.Oem8:
                        break;
                    case Keys.OemAuto:
                        break;
                    case Keys.OemBackslash:
                        toAdd += "\\";
                        break;
                    case Keys.OemClear:
                        break;
                    case Keys.OemCloseBrackets:
                        toAdd += ")";
                        break;
                    case Keys.OemComma:
                        toAdd += ",";
                        break;
                    case Keys.OemCopy:
                        break;
                    case Keys.OemEnlW:
                        break;
                    case Keys.OemMinus:
                        toAdd += "-";
                        break;
                    case Keys.OemOpenBrackets:
                        toAdd += "(";
                        break;
                    case Keys.OemPeriod:
                        toAdd += ".";
                        break;
                    case Keys.OemPipe:
                        break;
                    case Keys.OemPlus:
                        toAdd += "=";
                        break;
                    case Keys.OemQuestion:
                        toAdd += "/";
                        break;
                    case Keys.OemQuotes:
                        toAdd += "'";
                        break;
                    case Keys.OemSemicolon:
                        toAdd += ";";
                        break;
                    case Keys.OemTilde:
                        //toAdd += "'";
                        break;
                    case Keys.P:
                        toAdd += "p";
                        break;
                    case Keys.Pa1:
                        break;
                    case Keys.PageDown:
                        break;
                    case Keys.PageUp:
                        break;
                    case Keys.Pause:
                        break;
                    case Keys.Play:
                        break;
                    case Keys.Print:
                        break;
                    case Keys.PrintScreen:
                        break;
                    case Keys.ProcessKey:
                        break;
                    case Keys.Q:
                        toAdd += "q";
                        break;
                    case Keys.R:
                        toAdd += "r";
                        break;
                    case Keys.Right:
                        break;
                    case Keys.RightAlt:
                        break;
                    case Keys.RightControl:
                        break;
                    case Keys.RightShift:
                        break;
                    case Keys.RightWindows:
                        break;
                    case Keys.S:
                        toAdd += "s";
                        break;
                    case Keys.Scroll:
                        break;
                    case Keys.Select:
                        break;
                    case Keys.SelectMedia:
                        break;
                    case Keys.Separator:
                        break;
                    case Keys.Sleep:
                        break;
                    case Keys.Space:
                        toAdd += " ";
                        break;
                    case Keys.Subtract:
                        toAdd += "-";
                        break;
                    case Keys.T:
                        toAdd += "t";
                        break;
                    case Keys.Tab:
                        break;
                    case Keys.U:
                        toAdd += "u";
                        break;
                    case Keys.Up:
                        break;
                    case Keys.V:
                        toAdd += "v";
                        break;
                    case Keys.VolumeDown:
                        break;
                    case Keys.VolumeMute:
                        break;
                    case Keys.VolumeUp:
                        break;
                    case Keys.W:
                        toAdd += "w";
                        break;
                    case Keys.X:
                        toAdd += "x";
                        break;
                    case Keys.Y:
                        toAdd += "y";
                        break;
                    case Keys.Z:
                        toAdd += "z";
                        break;
                    case Keys.Zoom:
                        break;
                    default:
                        //this.toAdd += keyString;
                        //COut.Write("Unrecognised character \"" + keyString + "\" please notify Arwic if its an imprtant one", MsgType.Failed);
                        break;
                }
            }
            #endregion

            Text = Text.Insert(caretIndex, toAdd);
        }

        /// <summary>
        /// Draws the component
        /// </summary>
        /// <param name="sb"> SpriteBatch to draw the component with </param>
        /// <param name="scale"> Scale to draw the component by, defaults to (1f, 1f) </param>
        /// <param name="origin"> Origin of the component's parent, defaults to (0f, 0f) </param>
        /// <param name="colour"> Tint to draw the component with, defaults to white </param>
        public void Draw(SpriteBatch sb, float scale = 1f, Color? colour = null, Vector2? origin = null)
        {
            if (colour == null) colour = Color.White;
            if (origin == null) origin = Vector2.Zero;
            if (Selected) colour = new Color(colour.Value.R - 40, colour.Value.G - 40, colour.Value.B - 40);

            Transform t = new Transform(Transform.Translation.X + origin.Value.X, Transform.Translation.Y + origin.Value.Y, Transform.Bounds.Width, Transform.Bounds.Height, Transform.Rotation, Transform.RotationOrigin, Transform.Scale, Transform.Flip);

            BackgroundSprite.DrawNineCut(sb, t, null, colour);

            if (Masked)
            {
                string maskedText = "";
                for (int i = 0; i < Text.Length; i++)
                    maskedText += MaskedChar;
                sb.DrawString(fonts[(int)ContentFont], maskedText, new Vector2(t.Translation.X + 10, t.Translation.Y + 10), Color.White);
            }
            else
            {
                sb.DrawString(fonts[(int)ContentFont], Text, new Vector2(t.Translation.X + 10, t.Translation.Y + 10), Color.White);
            }

            if (displayCaret)
            {
                string contentBeforeCaret = "";
                for (int i = 0; i < caretIndex; i++)
                {
                    if (i < Text.Length) contentBeforeCaret += Text[i];
                }
                float caretPositon = fonts[(int)ContentFont].MeasureString(contentBeforeCaret).X + 10 + origin.Value.X;
                GraphicsUtil.DrawLine(sb, new Vector2(caretPositon, t.Translation.Y + 5), new Vector2(caretPositon, t.Translation.Y + t.Bounds.Height - 1), 2, Color.White);
            }

            // draws the title of the text box
            sb.DrawString(fonts[(int)TitleFont], Title, new Vector2(t.Translation.X + 10, t.Translation.Y - 25), Color.White);
        }
    }
}
