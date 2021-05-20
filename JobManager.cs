using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;


namespace AphAsyncHandler
{
    public class JobManager
    {
        public List<Job> activeJobs = new List<Job>();
        List<Job> newJobs = new List<Job>();
        public string baseDir = "c:\\traintasker\\jobs";
        bool busy = false;
        bool dirty = true;

        public static JobManager instance = null;

        public JobManager()
        {
            instance = this;

        }

        public void saveJob(string data)
        {
            JobRecord job = new JobRecord(data);
            job.insertOb();
            //if (job.name == "") job.name = "Default";
            //saveJob(job);
        }

        public void saveJob(JobRecord job) 
        {
            job.insertOb();
        }

        public string getActiveJobs()
        {
            List<object> oblist = RTDB.selectObList("SELECT * FROM JobRecord WHERE Active=1 ",new JobRecord(),"TrainTasker");

            string s = "";
            for (int i = 0; i < oblist.Count; ++i)
            {
                JobRecord job = (JobRecord)oblist[i];
                if (i > 0) s += ",";
                s += job.serialize();
            }
            return "[" + s + "]";

        }

        public string getUserJobs(string userID)
        {
            List<object> oblist = RTDB.selectObList("SELECT * FROM JobRecord WHERE userID= '" + userID + "'", new JobRecord(), "TrainTasker");

            string s = "";
            for (int i = 0; i < oblist.Count; ++i)
            {
                JobRecord job = (JobRecord)oblist[i];
                if (i > 0) s += ",";
                s += job.serialize();
            }
            return "[" + s + "]";

        }

        //string serializeActiveJobs()
        //{
        //    string s = "";
        //    for (int i = 0; i < activeJobs.Count; ++i)
        //    {
        //        Job job = activeJobs[i];
        //        if (job.start < DateTime.Now && job.end > DateTime.Now && job.active)
        //        {
        //            string js = job.serialize();
        //            if (s != "") s += ",";
        //            s += js;
        //        }
        //    }
        //    s = "{" + s + "}";
        //    return s;

        //}

        public string getAllJobs()
        {
            List<object> oblist = RTDB.selectObList("SELECT * FROM JobRecord", new JobRecord(), "TrainTasker");

            string s = "";
            for (int i = 0; i < oblist.Count; ++i)
            {
                JobRecord job = (JobRecord)oblist[i];
                if (i > 0) s += ",";
                s += job.serialize();
            }
            return "{" + s + "}";
        }

        //string serializeAllJobs()
        //{
        //    string s = "";
        //    for (int i = 0; i < activeJobs.Count; ++i)
        //    {
        //        Job job = activeJobs[i];

        //        string js = job.serialize();
        //        if (s != "") s += ",";
        //        s += js;

        //    }
        //    s = "{" + s + "}";
        //    return s;

        //}

        //public void scanJobs()
        //{
        //    Thread thread = new Thread(lowScanJobs);
        //    thread.Start();
        //}

        //void lowScanJobs()
        //{
        //    if (!busy)
        //    {
        //        if (dirty)
        //        {
        //            loadJobs();
        //            dirty = false;
        //        }
        //    }
        //    Thread.Sleep(1000 * 30);
        //}


        //public void loadJobs()
        //{
        //    newJobs.Clear();
        //    string[] dirs = Directory.GetDirectories(baseDir);
        //    for (int i = 0; i < dirs.Length; ++i)
        //    {
        //        loadDIr(dirs[i]);
        //    }
        //    activeJobs = newJobs;
        //}

        //public void loadDIr(string dir)
        //{
     
        //    string[] files = Directory.GetFiles(dir);
        //    for (int i = 0; i < files.Length; ++i)
        //    {
        //        string data = File.ReadAllText(files[i]);
        //        Job job = new Job(data);
        //     //   if (job.start < DateTime.Now && job.end > DateTime.Now && job.active)
        //    //    {
        //            newJobs.Add(job);
        //    //    }
        //    }
        //}
    }
}
