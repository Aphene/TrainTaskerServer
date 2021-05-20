using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AphAsyncHandler
{
    public class UserRecord : Record
    {
        public string userID = "";
        public string screenName = "";
        public string email = "";
        public string password = "";
        public string balance = "0";
        public string isRequester = "False";

        public UserRecord()
        {
        }

        public UserRecord(string data)
        {
            parse(data);
        }
    }
}
