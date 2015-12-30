using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace hidden_tear
{
    internal sealed class NativeMethods
    {
        // I am not at all intimate with this particular method of the Windows API so I did not 
        // tweak the signature. You should research this, though. 
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern int SystemParametersInfo(
            int uAction, 
            int uParam, 
            string lpvParam, 
            int fuWinIni);
    }

    public static class DesktopBackground
    {   
        public static void SetFromUrl(string url)
        {
            string directory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Backgrounds");

            Directory.CreateDirectory(directory);

            string path;
            for (int backgroundNumber = 0; ; backgroundNumber++)
            {
                path = Path.Combine(directory, string.Concat("background", backgroundNumber, ".bmp"));
                if (File.Exists(path) == false) break;
            }

            using (var client = new WebClient())
            {
                client.DownloadFile(url, path);
            }

            using (var key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true))
            {
                key.SetValue("PicturePosition", "10");
                key.SetValue("TileWallpaper", "0");
            }

            // Remember that the constants only serve as self-documentation in this case.
            const int SET_DESKTOP_BACKGROUND = 20;
            const int UPDATE_INI_FILE = 1;
            const int SEND_WINDOWS_INI_CHANGE = 2;
            NativeMethods.SystemParametersInfo(SET_DESKTOP_BACKGROUND, 0, path, UPDATE_INI_FILE | SEND_WINDOWS_INI_CHANGE);
        }
    }
}