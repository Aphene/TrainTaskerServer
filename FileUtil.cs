using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AphAsyncHandler
{
    public class FileUtil
    {
        static List<string> fileList = new List<string>();
        static List<string> dirList = new List<string>();

       

        public static string getAllFiles(string path)
        {
            if (path=="") path = "C:\\Toons\\Users";
            string reply = "";
            fileList.Clear();
            dirList.Clear();
            dirSearch(path);
            fileSearch();
            for (int i = 0; i < dirList.Count; ++i)
            {
                if (i > 0) reply += "\r";
                reply += dirList[i];
            }
            reply += "\r__FILES__\r";
            for (int i = 0; i < fileList.Count; ++i)
            {
                if (i > 0) reply += "\r";
                reply += fileList[i];
            }
            return reply;
        }


        static void dirSearch(string sDir)
        {
            string[] dirs = Directory.GetDirectories(sDir);
            for (int i = 0; i < dirs.Length; ++i)
            {
                dirList.Add(dirs[i]);
                dirSearch(dirs[i]);
            }
        }

        static void fileSearch()
        {
            for (int i = 0; i < dirList.Count; ++i)
            {
                string[] files = Directory.GetFiles(dirList[i]);
                for (int j = 0; j < files.Length; ++j)
                {
                    fileList.Add(files[j]);
                }
            }
        }

        public static int deleteFIles(string path)
        {
            string[] files = Directory.GetFiles(path);
            int fileCount = files.Length;
            for (int j = 0; j < files.Length; ++j)
            {
                File.Delete(files[j]);
            }
            return fileCount;
        }
    }
}
