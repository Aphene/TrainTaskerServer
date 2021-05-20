using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AphAsyncHandler
{
    public class User
    {
        public string userID = "";
        public string screenName = "";
        public string email = "";
        public string password = "";
        public string balance = "0";
        public string isRequester = "False";

        public User()
        {
        }

        public User(string data)
        {
            parse(data);
        }


        public void parse(string data)
        {
            data = data.Replace("\"", "");
            string[] pairs = data.Split(new char[] { ',' });
            for (int i = 0; i < pairs.Length; ++i)
            {
                string[] fields = pairs[i].Split(new char[] { ':' });
                if (fields.Length == 2)
                {
                    string name = fields[0];
                    string value = fields[1];
                    if (name == "userID") userID = value;
                    if (name == "screenName") screenName = value;
                    if (name == "email") email = value;
                    if (name == "password") password = value;
                    if (name == "balance") balance = value;
                    if (name == "isRequester") isRequester = value;
                }
            }
        }

        public string serialize()
        {
            string s = "{";
            s += "\"userID:\"" + userID + "\",";
            s += "\"screenName:\"" + screenName + "\",";
            s += "\"email\":\"" + email + "\",";
            s += "\"password\":\"" + password + "\",";
            s += "\"balance\":\"" + balance+ "\",";
            s += "\"isRequester\":\"" + isRequester + "\"}";

            return s;
        }

    }


}
