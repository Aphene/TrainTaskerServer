using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AphAsyncHandler
{
    public class JobRecord : Record
    {
        //{"userID":"User1","name":"","title":"","description":"","instructions":"","bounty":"0.5","budget":"20.00"}

        public string userID = "";
        public string name = "";
        public string title = "";
        public string description = "";
        public string instructions = "";
        public string bounty = "0";
        public string budget = "0";
        public string budgetLeft = "0";
        public bool active = false;
        public string startTime = "";
        public string endTime = "";
        public string type = "";


        public JobRecord()
        {
        }

        public JobRecord(string data)
        {
            parse(data);
        }

    }
}
