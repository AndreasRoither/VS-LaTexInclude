using LatechInclude.ViewModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

namespace LatechInclude.View
{
    /// <summary>
    /// Interaktionslogik für AddExtension.xaml
    /// </summary>
    public partial class AddExtension : UserControl
    {
        AddExtensionViewModel aevm = null;

        /// <summary>
        /// AddExtension Constructor
        /// </summary>
        public AddExtension()
        {
            InitializeComponent();

            aevm = new AddExtensionViewModel();
            aevm.CurrentLanguage = comboBox.SelectedItem as string;
        }

        /// <summary>
        /// Sets the language in the ViewModel
        /// </summary>
        private void OnLanguageSelection_Changed(object sender, SelectionChangedEventArgs e)
        {
            aevm.CurrentLanguage = (sender as ComboBox).SelectedItem as string;
        }

        /// <summary>
        /// Saves changes to the ViewModel
        /// </summary>
        private void OnTxtBox_KeyUp(object sender, KeyEventArgs e)
        {
            string temp = textBox.Text.ToString();

            temp = new string(temp.Where(c => !char.IsWhiteSpace(c)).ToArray());
            aevm.TxtBoxInput = temp;
        }

        /// <summary>
        /// When a key is pressed down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            char c = GetCharFromKey(e.Key);

            // Check for a naughty character in the KeyDown event.
            if (System.Text.RegularExpressions.Regex.IsMatch(c.ToString(), @"[^A-Za-z]"))
            {
                // Stop the character from being entered into the control since it is illegal.
                e.Handled = true;
            }
        }

        public enum MapType : uint
        {
            MAPVK_VK_TO_VSC = 0x0,
            MAPVK_VSC_TO_VK = 0x1,
            MAPVK_VK_TO_CHAR = 0x2,
            MAPVK_VSC_TO_VK_EX = 0x3,
        }

        [DllImport("user32.dll")]
        public static extern int ToUnicode(
            uint wVirtKey,
            uint wScanCode,
            byte[] lpKeyState,
            [Out, MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 4)]
            StringBuilder pwszBuff,
            int cchBuff,
            uint wFlags);

        [DllImport("user32.dll")]
        public static extern bool GetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll")]
        public static extern uint MapVirtualKey(uint uCode, MapType uMapType);

        public static char GetCharFromKey(Key key)
        {
            char ch = ' ';

            int virtualKey = KeyInterop.VirtualKeyFromKey(key);
            byte[] keyboardState = new byte[256];
            GetKeyboardState(keyboardState);

            uint scanCode = MapVirtualKey((uint)virtualKey, MapType.MAPVK_VK_TO_VSC);
            StringBuilder stringBuilder = new StringBuilder(2);

            int result = ToUnicode((uint)virtualKey, scanCode, keyboardState, stringBuilder, stringBuilder.Capacity, 0);
            switch (result)
            {
                case -1:
                    break;
                case 0:
                    break;
                case 1:
                    {
                        ch = stringBuilder[0];
                        break;
                    }
                default:
                    {
                        ch = stringBuilder[0];
                        break;
                    }
            }
            return ch;
        }
    }
}
