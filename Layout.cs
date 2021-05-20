using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AphAsyncHandler
{
    public class Layout
    {
        public static string path = "C:\\inetpub\\wwwroot\\Layout\\Projects\\";

        public static string loadPage(string page,string project) {
            if (page.IndexOf(".") == -1) page = page + ".txt";
            string filePath=path+project+"\\"+page;
            if (!File.Exists(filePath)) return "";
            string data = File.ReadAllText(filePath);
            return data;
        }
    }
}
