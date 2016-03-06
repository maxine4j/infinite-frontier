using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Arwic.InfiniteFrontier
{
    public enum Cursors
    {
        Main,
        Crosshairs
    }
    public class CursorUtil
    {
        private static List<System.Windows.Forms.Cursor> cursorList = new List<System.Windows.Forms.Cursor>();

        private static System.Windows.Forms.Form winForm;

        public static void Initialize(GameWindow gw)
        {
            winForm = (System.Windows.Forms.Form)System.Windows.Forms.Form.FromHandle(gw.Handle);

            for (int i = 0; i < Enum.GetNames(typeof(Cursors)).Length; i++)
            {
                cursorList.Add((System.Windows.Forms.Cursor)LoadCustomCursor(@"Content\Cursors\" + Enum.GetNames(typeof(Cursors))[i] + ".cur"));
            }
        }

        public static void Set(Cursors c)
        {
            winForm.Cursor = cursorList[(int)c];
        }

        /// <summary>
        /// Loads a cursor file for use
        /// </summary>
        /// <param name="path"> path to the desired cursor file </param>
        /// <returns> cursor object </returns>
        public static Cursor LoadCustomCursor(string path)
        {
            IntPtr hCurs = LoadCursorFromFile(path);
            if (hCurs == IntPtr.Zero) throw new Win32Exception();
            var curs = new System.Windows.Forms.Cursor(hCurs);
            // Note: force the cursor to own the handle so it gets released properly
            var fi = typeof(System.Windows.Forms.Cursor).GetField("ownHandle", BindingFlags.NonPublic | BindingFlags.Instance);
            fi.SetValue(curs, true);
            return curs;
        }
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr LoadCursorFromFile(string path);
    }
}
