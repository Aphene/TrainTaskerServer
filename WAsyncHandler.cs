using System;
using System.Collections.Generic;
using System.Web;
using System.Threading;
using System.IO;
using System.Drawing;
using System.Text;



namespace AphAsyncHandler
{




    public class WAsyncHandler : IHttpAsyncHandler
    {



        public bool IsReusable { get { return false; } }

        public static Dictionary<string, string> configs = null;

        public WAsyncHandler()
        {

        }
        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, Object extraData)
        {
         //   context.Response.Write("<p>Begin IsThreadPoolThread is " + Thread.CurrentThread.IsThreadPoolThread + "</p>\r\n");
            AsynchOperation asynch = new AsynchOperation(cb, context, extraData);
            asynch.StartAsyncWork();
            return asynch;
        }

        public void EndProcessRequest(IAsyncResult result)
        {
        }

        public void ProcessRequest(HttpContext context)
        {
            throw new InvalidOperationException();
        }

        public static void log(string text)
        {

        }
    }

    class AsynchOperation : IAsyncResult
    {
        private bool _completed;
        private Object _state;
        private AsyncCallback _callback;
        private HttpContext _context;

        bool IAsyncResult.IsCompleted { get { return _completed; } }
        WaitHandle IAsyncResult.AsyncWaitHandle { get { return null; } }
        Object IAsyncResult.AsyncState { get { return _state; } }
        bool IAsyncResult.CompletedSynchronously { get { return false; } }

        byte[] bytes;

        public AsynchOperation(AsyncCallback callback, HttpContext context, Object state)
        {
            _callback = callback;
            _context = context;
            _state = state;
            _completed = false;
        }

        public void StartAsyncWork()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(StartAsyncTask), null);
        }

        private void StartAsyncTask(Object workItemState)
        {
            HttpWorkerRequest worker = GetWorkerRequest(_context);
            string method = worker.GetHttpVerbName();


            string reply = "Ok";
            string body = "";
            string url = _context.Request.RawUrl;


            if (JobManager.instance == null) JobManager.instance = new JobManager();


            Dictionary<string,string> parms = UrlUtil.parseUrl(_context.Request.RawUrl,"");

            bool done = false;

     

            try
            {
                if (method == "GET")
                {
                    if (url.ToLower().Contains("/getcoins")) {

                        reply = Coins.GetCoins();
                        done=true;
                    }
                    if (url.ToLower().Contains("/storyboard"))
                    {
                        if (StoryBoard.instance == null)
                        {
                            StoryBoard storyboard = new StoryBoard();
                        }
                        reply = StoryBoard.instance.processRequest(parms);
                        done = true;
                    }

                    if (!done)
                    {
                        if (parms.ContainsKey("Command"))
                        {
                            switch (parms["Command"])
                            {
                                case "Files":
                                    string path = "";
                                    if (parms.ContainsKey("Path")) path = parms["Path"];
                                    reply = FileUtil.getAllFiles(path);
                                    break;
                                case "DownloadToon":
                                    path = parms["Path"];
                                    path = path.Replace("~", "\\");
                                    reply = File.ReadAllText(path);
                                    break;
                                case "LoadGraph":
                                    string coin = parms["Coin"];
                                    reply = Coins.LoadGraph(coin);
                                    break;
                                case "LoadPage":
                                    string page = parms["Page"];
                                    string project = parms["Project"];
                                    reply = Layout.loadPage(page,project);
                                    break;
                                case "Uploader":
                                    reply = TrainTasker.getUploader(parms["UserID"],parms["JobID"]);
                                    break;

                                case "GetAllJobs":
                                    reply = JobManager.instance.getAllJobs();
                                    break;

                                case "GetUserJobs":
                                    reply = JobManager.instance.getUserJobs(parms["UserID"]);
                                    break;

                                case "GetActiveJobs":
                                    reply = JobManager.instance.getActiveJobs();
                                    break;

                                case "TaskerLogon":
                                    reply = TrainTasker.taskerLogon(parms["Email"],parms["Password"]);
                                    break;

                                case "TaskerRegistration":
                                    reply = TrainTasker.taskerRegistration(parms["Email"], parms["Password"]);
                                    break;

                               case "RequesterLogon":
                                    reply = TrainTasker.requesterLogon(parms["Email"],parms["Password"]);
                                    break;

                               case "RequesterRegistration":
                                    reply = TrainTasker.requesterRegistration(parms["Email"], parms["Password"]);
                                    break;

                               case "RemoveJobImages":
                                    reply = TrainTasker.removeJobImages(parms["JobID"], parms["UserID"]);
                                    break;

                               case "GetNexAvailableTrain":
                                reply = TrainTasker.requestNextAvailableTrain(parms["JobID"],parms["UserID"]);
                                break;  

                                case "GetTaskImage":
                                    reply = TrainTasker.getTaskImage(parms["TaskID"]);
                                    break;

                                case "GetTrainResultsPerTask":
                                    reply = TrainTasker.getTrainResultsPerTask(parms["TaskID"]);
                                    break;

                                case "GetTasksPerJob":
                                    reply = TrainTasker.getTaskIDsPerJob(parms["JobID"]);
                                    break;

                            }

                     

                            done = true;
                        }
                    }
                }
            }
            catch (Exception err)
            {
                reply = "Error: " + err.Message;
            }








            if (_context.Request.Files.Count > 0)
            {
  
            }

            if (method == "POST")
            {

                try
                {
                    string data = "";
                    MemoryStream memstream = new MemoryStream();
                    _context.Request.InputStream.CopyTo(memstream);
                    memstream.Position = 0;
                    if (url.Contains("Binary"))
                    {
                        Thread.Sleep(600);
                        using (StreamReader reader = new StreamReader(memstream))
                        {
                            var buffer = new byte[1000000];
                            var bytesRead = default(int);
                            while ((bytesRead = reader.BaseStream.Read(buffer, 0, buffer.Length)) > 0)
                                memstream.Write(buffer, 0, bytesRead);
                            bytes = memstream.ToArray();
     
                        }
                    }
                    else
                    {
                        using (StreamReader reader = new StreamReader(memstream))
                        {
                            data = reader.ReadToEnd();
                        }
                    }

                    if (parms.ContainsKey("Command"))
                    {
                        switch (parms["Command"])
                        {

                            case "UploadToon":
                                string path = parms["Path"];
                                path = path.Replace("~", "\\");
                                File.WriteAllText(path, data);
                                reply = "Ok";
                                break;

                            case "UploadImage":
                                path = parms["Path"];
                                path = path.Replace("~", "\\");
                                Bitmap bitmap = stringToPng(data);
                                bitmap.Save(path);
                                reply = "Ok";
                                break;
                            case "UploadImageBinary":
                                string userID = parms["UserID"];
                                string jobID = parms["JobID"];                        
                                string filename= trimFormBoundry();  // remove formwebkit header and footer https://stackoverflow.com/questions/61264628/remove-the-webkitformboundary-in-c-sharp
                                path = "c:\\TrainTasker\\images\\" + userID+"\\"+jobID;
                                Directory.CreateDirectory(path);
                                File.WriteAllBytes(path+"\\"+filename,bytes);
                                reply="Ok";
                                break;

                            case "PublishJob1":
                                reply = TrainTasker.publishJob1(parms["UserID"], parms["Name"],data);
                                break;

                            case "PublishJob2":
                                reply = TrainTasker.publishJob2(parms["UserID"], parms["Name"], data);
                                break;

                            case "UpdateJob":
                                reply = TrainTasker.updateJob(parms["UserID"], parms["Name"], data);
                                break;

                            case "ReportTrain":
                                reply = TrainTasker.reportTrain(data, parms["JobID"], parms["UserID"]);
                                break;





                        }
                        done = true;
                    }
                }
                catch (Exception err)
                {
                    reply = err.Message;
                    if (!reply.Contains("Error:")) {
                        reply = "Error: " + reply;
                    }
                }

            }  // end POST
            else
            {

            }

            if (reply.Contains("__ReturnFile__") )
            {
                try
                {
                    reply = reply.Replace("__ReturnFile__", "");
                    _context.Response.WriteFile(reply);
                }
                catch (Exception err)
                {
                    _context.Response.Write("Error: Image path not found");
                }
            }
            else {
                _context.Response.Write(reply);
            }
           

            _completed = true;
            _callback(this);
        }

        protected HttpWorkerRequest GetWorkerRequest(HttpContext context)
        {
            IServiceProvider provider = (IServiceProvider)context;
            return (HttpWorkerRequest)provider.GetService(typeof(HttpWorkerRequest));
        }

        public string trimFormBoundry()
        {
            List<byte> list = new List<byte>();
            byte b=0;
            bool startFound = false;
            List<byte> header = new List<byte>();
            for (int i = 0; i < bytes.Length-48; ++i)
            {
                b = bytes[i];
                if (b == 0x89 || b==0xFF ) startFound = true;  // 89 png, ff jpg
                if (startFound) list.Add(b);
                else header.Add(b);
            }
            string head = System.Text.Encoding.UTF8.GetString(header.ToArray());
            string[] segments = head.Split(new char[] { '\"' });
            string filename = head;
            if (segments.Length > 3) filename = segments[3];
            bytes = list.ToArray();
            return filename;
        }


        public string pngToString(Bitmap bitmap)
        {
            string s = "";
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                s = Convert.ToBase64String(stream.GetBuffer());
            }
            return s;
        }

        public Bitmap stringToPng(string pngStr)
        {
            byte[] bytes = Convert.FromBase64String(pngStr);
            Image image;
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                image = Image.FromStream(ms);
            }
            return (Bitmap)image;
        }
    }
}