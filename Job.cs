using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AphAsyncHandler
{
    public class Job
    {
        //{"userID":"User1","name":"","title":"","description":"","instructions":"","bounty":"0.5","budget":"20.00"}

        public string userID = "";
        public string name = "";
        public string title = "";
        public string description = "";
        public string instructions = "";
        public string bounty = "0";
        public string budget = "0";
        public string isActive = "False";
        public string startDate = "";
        public string endDate = "";
        public string budgetLeft = "0";
        public bool active = false;
        public DateTime start = new DateTime(1899, 1, 1);
        public DateTime end = new DateTime(1899, 1, 1);


        public Job(string data)
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
                    if (name == "name") name = value;
                    if (name == "title") title = value;
                    if (name == "description") description = value;
                    if (name == "instructions") instructions = value;
                    if (name == "bounty") bounty = value;
                    if (name == "budget") budget = value;
                    if (name == "budgetLeft") budgetLeft = value;
                    if (name == "isActive")
                    {
                        isActive = value;
                        if(value.ToLower().Contains("true")) active=true;
                        else active=false;
                    }
                    if (name == "startDate")
                    {
                        startDate = value;
                        start = parseDate(value);
                    }
                    if (name == "endDate")
                    {
                        endDate = value;
                        end = parseDate(value);
                    }
                }
            }

        }

        public DateTime parseDate(string ds)
        {
            try
            {
                DateTime dt = DateTime.Parse(ds);
                return dt;
            }
            catch (Exception err)
            {
                return DateTime.Now;
            }
        }

        public string serialize()
        {
            string s = "{";
            s += "\"userID\":\"" + userID + "\",";
            s += "\"name\":\"" + name + "\",";
            s += "\"title\":\"" + title + "\",";
            s += "\"description\":\"" + description + "\",";
            s += "\"instructions\":\"" + instructions + "\",";
            s += "\"bounty\":\"" + bounty + "\",";
            s += "\"budget\":\"" + budget + "\",";
            s += "\"budgetLeft\":\"" + budgetLeft + "\",";
            s += "\"isActive\":\"" + isActive + "\",";
            s += "\"startDate\":\"" + startDate + "\",";
            s += "\"endDate\":\"" + endDate + "\"}";
            return s;
        }
    }
}
