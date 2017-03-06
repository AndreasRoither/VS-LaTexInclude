using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LatechInclude.HelperClasses
{
    class ExplorerContextMenu
    {

        /// <summary>
        /// Registers a key in the registry to add a context menu item in the explorer
        /// </summary>
        /// <param name="fileType">Filetype</param>
        /// <param name="shellKeyName">Name in registry</param>
        /// <param name="menuText">Menu item text</param>
        /// <param name="menuCommand">Full path to self + params</param>
        public void Register_File(string fileType, string shellKeyName, string menuText, string menuCommand)
        {
            // create path to registry location
            string regPath = string.Format(@"{0}\shell\{1}", fileType, shellKeyName);

            // add context menu to the registry
            using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(regPath))
            {
                key.SetValue(null, menuText);
            }

            // add command that is invoked to the registry
            using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(
                string.Format(@"{0}\command", regPath)))
            {
                key.SetValue(null, menuCommand);
            }
        }

        /// <summary>
        /// Unregister key in registry
        /// </summary>
        /// <param name="fileType">Filetype</param>
        /// <param name="shellKeyName">Name od the registry entry</param>
        public void Unregister_File(string fileType, string shellKeyName)
        {
            Debug.Assert(!string.IsNullOrEmpty(fileType) &&
                !string.IsNullOrEmpty(shellKeyName));

            // path to the registry location
            string regPath = string.Format(@"{0}\shell\{1}", fileType, shellKeyName);

            // remove context menu from the registry
            Registry.ClassesRoot.DeleteSubKeyTree(regPath);
        }

        public void Register_Folder(string shellKeyName, string menuText, string path)
        {
            RegistryKey _key = Registry.ClassesRoot.OpenSubKey("Folder\\Shell", true);
            RegistryKey newkey = _key.CreateSubKey(shellKeyName);
            RegistryKey subNewkey = newkey.CreateSubKey("Command");
            subNewkey.SetValue(menuText, path);
            subNewkey.Close();
            newkey.Close();
            _key.Close();
        }

        public void Unregister_Folder(string shellKeyName)
        {
            RegistryKey _key = Registry.ClassesRoot.OpenSubKey("Folder\\Shell\\", true);
            _key.DeleteSubKey(shellKeyName);
            _key.Close();
        }
    }
}
