using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AphAsyncHandler
{
    public class StoryBoard
    {

        public static StoryBoard instance = null;
        string dir = "C:\\inetpub\\storyboard";
        string dataDir = "C:\\StoryBoard\\Projects";
       

        public StoryBoard()
        {
            instance = this;
        }

        public string processRequest(Dictionary<string, string> parms)
        {
            if (parms.ContainsKey("Command"))
            {
                string command = parms["Command"];
                switch (command)
                {
                    case "NextPage":
                        return nextPage(parms);
                    case "PrevPage":
                        return prevPage(parms);
                    case "EditPage":
                        return editPage(parms);
                    case "UpdatePage":
                        return updatePage(parms);
                    case "ReturnToPanelPage":
                        return returnToPanelPage(parms);
                    case "BackgroundPicker":
                        return getBackgroundList();
                    case "CharacterPicker":
                        return getCharacterList();
                    case "ShotSizePicker":
                        return getShotSizeList();
                    case "LookToPicker":
                        return getLookToList();
                    case "EyePicker":
                        return eyePicker();
                    case "MouthPicker":
                        return mouthPicker();
                    case "BrowPicker":
                        return browPicker();
                    case "SuprisePicker":
                        return suprisePicker();
                }
            }


            return "";
        }

        public string nextPage(Dictionary<string, string> parms)
        {
            string pageIndexStr = parms["PageIndex"];
            int pageIndex = int.Parse(pageIndexStr);
            ++pageIndex;
            string template = "var pageIndex = 0;";
            string replace = "var pageIndex = "+pageIndex.ToString()+";";
            string html = File.ReadAllText(dir + "\\panelView.html");
            html = html.Replace(template, replace);
            return html;
        }

        public string prevPage(Dictionary<string, string> parms)
        {
            string pageIndexStr = parms["PageIndex"];
            int pageIndex = int.Parse(pageIndexStr);
            if (pageIndex>0) --pageIndex;
            string template = "var pageIndex = 0;";
            string replace = "var pageIndex = "+pageIndex.ToString()+";";
            string html = File.ReadAllText(dir + "\\panelView.html");
            html = html.Replace(template, replace);
            return html;
        }


        public string editPage(Dictionary<string, string> parms)
        {
            string html = File.ReadAllText(dir + "\\storyboard.html");
            return html;
        }


        public string updatePage(Dictionary<string, string> parms)
        {
            saveParms(parms);
            string pageIndexStr = parms["PageIndex"];
            int pageIndex = int.Parse(pageIndexStr);
            string template = "var pageIndex = 0;";
            string replace = "var pageIndex = " + pageIndex.ToString() + ";";
            string html = File.ReadAllText(dir + "\\panelView.html");
            html = html.Replace(template, replace);
            return html;
        }



        public string returnToPanelPage(Dictionary<string, string> parms)
        {
            string pageIndexStr = parms["PageIndex"];
            int pageIndex = int.Parse(pageIndexStr);
            string template = "var pageIndex = 0;";
            string replace = "var pageIndex = " + pageIndex.ToString() + ";";
            string html = File.ReadAllText(dir + "\\panelView.html");
            html = html.Replace(template, replace);
            return html;
        }

        void saveParms(Dictionary<string, string> parms)
        {
            string pageIndexStr = parms["PageIndex"];
            string projectName = parms["ProjectName"];
            string dir = dataDir + "\\" + projectName;
            Directory.CreateDirectory(dir);
            string path = dir + "\\Panel" + pageIndexStr + ".txt";
            string s = "";
            foreach (KeyValuePair<string, string> kvp in parms)
            {
                s += kvp.Key + "|" + kvp.Value + "\r\n";
            }
            File.WriteAllText(path, s);
            
        }

        public string getBackgroundList()
        {
            return "Back1|Back2|Back3";
        }

        public string getCharacterList()
        {
            return "Char1|Char2|Char3";
        }

        public string getShotSizeList()
        {
            return "Far|Mid|Near";
        }

        public string getLookToList()
        {
            return "Left|Front|Right";
        }

        public string eyePicker()
        {
            return "Left|Up|Right|Down";
        }

        public string mouthPicker()
        {
            return "DeepFrown|Frown|Neutral|Smile|DeepSmile";
        }

        public string browPicker()
        {
            return "DeepSad|Sad|Neutral|Mad|DeepMad";
        }

        public string suprisePicker()
        {
            return "None|Little|Lot";
        }

    }
}
