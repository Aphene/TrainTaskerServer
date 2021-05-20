using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace AphAsyncHandler
{
    public class TrainTasker
    {
        public static string getUploader(string userID,string jobID)
        {
            string html = File.ReadAllText("c:\\inetpub\\wwwroot\\upload\\upload.html");
            html = html.Replace("{UserID}", userID);
            html = html.Replace("{JobID}", jobID);
            return html;
        }

        public static string publishJob1(string userID, string jobName, string data)
        {
            JobRecord job = new JobRecord(data);


            job.active = true;
            job.type = "ImageRegion";
            JobRecord oldjob = (JobRecord)job.selectOb("WHERE name='" + jobName + "'");
            if (oldjob!=null) {
                return "Error: Job name, " + jobName + " already in use.";
            }
            else {
                job.insertOb();
            }
            return job.serialize(); /// should include ID field.
        }

        public static string publishJob2(string userID, string jobName, string data)
        {
            JobRecord job = new JobRecord(data);


            job.active = true;
            job.type = "ImageRegion";
            JobRecord oldjob = (JobRecord)job.selectOb("WHERE name='" + jobName + "'");
            if (oldjob != null)
            {
                job.ID = oldjob.ID;
                job.updateOb();
            }
            else
            {
                job.insertOb();
            }
            createTaskRecords(userID, job.ID);
            return job.serialize(); /// should include ID field.
        }

        public static string getTaskImage(string taskID)
        {
            TaskRecord tr = new TaskRecord();
            tr = (TaskRecord)tr.selectOb("WHERE ID=" + taskID);
            if (tr == null) return "Error: TaskRecord not found";
            Bitmap bitmap = new Bitmap(tr.ResourcePath);
            string data = pngToString(bitmap);
            return data;
            //return "__ReturnFile__"+tr.ResourcePath;


        }

        public static string pngToString(Bitmap bitmap)
        {
            string s = "";
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                s = Convert.ToBase64String(stream.GetBuffer());
            }
            return s;
        }

        static void createTaskRecords(string userID, int jobID)
        {
            string path = "c:\\TrainTasker\\images\\" + userID + "\\" + jobID.ToString();
            if (!Directory.Exists(path)) return;
            string[] files = Directory.GetFiles(path);
            for (int i = 0; i < files.Length; ++i)
            {
                TaskRecord tr = new TaskRecord();
                tr.JobID = jobID.ToString();
                tr.RequesterID = userID;
                tr.ResourcePath = files[i];
                tr.MaxTrains = 100;
                tr.TaskType = "ImageRegion";
                tr.insertOb();
            }
        }

        public static string updateJob(string userID, string jobName, string data)
        {
            JobRecord job = new JobRecord(data);
            JobRecord oldJob = (JobRecord)job.selectOb("WHERE Name = '" + job.name+"'");
            if (oldJob == null)
            {
                return "Error: Job Record not found";
            }
            else
            {
                job.updateOb();
            }
            return job.serialize(); /// should include ID field.
        }

        public static string getJobs(string userID)
        {
            return JobManager.instance.getActiveJobs();
        }

        public static string taskerLogon(string email,string password) 
        {
            UserRecord ur = new UserRecord();
            ur = (UserRecord)ur.selectOb("WHERE email='" + email + "' AND password = '" + password + "'");
            if (ur == null) return "Error: Name or Passwor is incorrect";
            return ur.userID;
          
        }


        public static string taskerRegistration(string email, string password)
        {
            UserRecord ur = new UserRecord();
            ur = (UserRecord)ur.selectOb("WHERE email='" + email + "'");
            if (ur != null) return "Error: Email already in use.";
            ur = new UserRecord();
            ur.email = email;
            ur.password = password;
            ur.isRequester = "False";
         
            ur.userID = DateTime.Now.Ticks.ToString();
            ur.insertOb();
            return ur.userID;

        }

        public static string requesterRegistration(string email, string password)
        {
            UserRecord ur = new UserRecord();
            ur = (UserRecord)ur.selectOb("WHERE email='" + email +"'");
            if (ur != null) return "Error: Email already in use.";
            ur = new UserRecord();
            ur.email = email;
            ur.password = password;
            ur.isRequester = "True";
          
            ur.userID = DateTime.Now.Ticks.ToString();
            ur.insertOb();
            return ur.userID;
        }


        public static string requesterLogon(string email, string password)
        {
            UserRecord ur = new UserRecord();
            ur = (UserRecord)ur.selectOb("WHERE email='" + email + "' AND password = '" + password + "'");
            if (ur == null) return "Error: Name or Passwor is incorrect";
            if (ur.isRequester != "True") return "Error: User not registered for this application.";
            return ur.userID;


        }

        public static string requestNextAvailableTrain(string jobID,string userID)
        {
            TaskRecord tr = new TaskRecord();
            List<object> oblist = RTDB.selectObList("SELECT * FROM TaskRecord WHERE JobID='" + jobID + "'" + " AND TrainCount < MaxTrains",tr,"TrainTasker");
            TrainRecord trn=new TrainRecord();
            for (int i=0;i<oblist.Count;++i) {
                 tr = (TaskRecord) oblist[i];
                trn = (TrainRecord) trn.selectOb("WHERE UserID='"+userID+"'"+ " AND TaskRecordID = "+tr.ID);
                if (trn==null) {
                    trn = new TrainRecord();
                    trn.ResourcePath=tr.ResourcePath;
                    trn.TaskRecordID=tr.ID;
                    trn.RequesterID = tr.RequesterID;
                    trn.JobID = tr.JobID;
                    string s = trn.serialize();
                    s = s.Replace("1/1/1800 12:00:00 AM", "");
                    s = s.Replace("\\", "/");  // \ breaks client json
                    return s;
                }
            }
            return "Error: No Train available";

        }


        public static string reportTrain(string dataString,string jobID,string taskerID)
        {
            TrainRecord returnTrain = new TrainRecord(dataString);
            TaskRecord tr = new TaskRecord();
            tr = (TaskRecord)tr.selectOb("WHERE ID=" + returnTrain.TaskRecordID.ToString());
            if (tr == null) return "Error: ReportTrain TaskRecord not found";
            returnTrain.timeStamp = DateTime.Now;
            returnTrain.UserID = taskerID;
            returnTrain.insertOb();
            tr.TrainCount++;
            tr.JobID = jobID;
            tr.TaskerID = taskerID;
            tr.updateOb();
           // return tr.serialize();
            return "";
        }

        public static string getTrainResultsPerTask(string taskID)
        {
            List<object> oblist = RTDB.selectObList("SELECT * FROM TrainRecord WHERE TaskID=" + taskID, new TrainRecord(), "TrainTasker");
            string s = "";
            for (int i = 0; i < oblist.Count; ++i)
            {
                if (i > 0) s += ",";
                TrainRecord tr = (TrainRecord)oblist[i];
                s += tr.serialize();
            }
            return s;
        }

        public static string getTaskIDsPerJob(string jobID) {
            List<int> list = new List<int>();
            List<object> oblist = RTDB.selectObList("SELECT * FROM TrainRecord WHERE JobID=" + jobID, new TrainRecord(), "TrainTasker");
            string s = "";
            for (int i = 0; i < oblist.Count; ++i)
            {
                TrainRecord tr = (TrainRecord)oblist[i];
                if (!list.Contains(tr.TaskRecordID)) {
                    if (s!="") s += ",";
                    s += tr.serialize();
                    list.Add(tr.TaskRecordID);
                }
            }

            return "["+s+"]";
        }

        public static string getAvailableJobs()
        {
            return "";
            
        }

        public static string removeJobImages(string userID, string jobID)
        {
            string path = "c:\\TrainTasker\\images\\" + userID + "\\" + jobID;
            int fileCount= FileUtil.deleteFIles(path);
            return fileCount + " files deleted.";
        }

    }


}
