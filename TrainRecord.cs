using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AphAsyncHandler
{
    public class TrainRecord : Record
    {
        public decimal X = 0;
        public decimal Y = 0;
        public decimal W = 0;
        public decimal H = 0;
        public string Result = "";
        public int TaskRecordID = 0;
        public string ResourcePath="";
        public DateTime timeStamp = new DateTime(1800, 1, 1);
        public string UserID = "";
        public string RequesterID = "";
        public string JobID = "";

        public TrainRecord()
        {
            
        }

        public TrainRecord(string data)
        {
            parse(data);
        }


    }
}