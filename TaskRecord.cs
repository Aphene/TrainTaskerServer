using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AphAsyncHandler
{
    class TaskRecord : Record
    {
        public string RequesterID = "";
        public string TaskerID = "";
        public string TaskID = "";
        public string JobID="";
        public string TaskType="";
        public string ResourcePath = "";
        public string TimeStamp = "";
        public string Result = "";
        public int MaxTrains=0;
        public int TrainCount=0;
    }
}
