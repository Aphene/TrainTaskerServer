using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AphAsyncHandler
{
    class UrlUtil
    {

        public static Dictionary<string,string> parseUrl(string url, string body)
        {
            url = url.Replace("%20", "");
            Dictionary<string, string> table = new Dictionary<string, string>();
            string[] fields = url.Split(new char[] { '?' });
            string page = fields[0];


            if (body == "")
            {
                if (fields.Length > 1) body = fields[1].Trim();
            }
            if (body != "")
            {
                string[] pairs = body.Split(new char[] { '&' });
                for (int i = 0; i < pairs.Length; ++i)
                {
                    string[] nv = pairs[i].Split(new char[] { '=' });
                    string name = nv[0];
                    string value = nv[1];
                    table.Add(name, value);
                }
            }
            return table;
        }
    }
}
