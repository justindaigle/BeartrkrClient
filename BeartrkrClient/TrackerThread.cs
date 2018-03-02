using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net;
using Newtonsoft.Json;
using RestSharp;
using SpotifyAPI.Local;
using Microsoft.VisualBasic.Devices;

namespace BeartrkrClient
{
    class TrackerThread
    {
        Timer trackTimer, updateTimer;
        ProcessList pList = new ProcessList();

        HttpListener webListener;

        public void DoAllTheThings()
        {
            // update current tracking info every second
            trackTimer = new Timer(1000);
            trackTimer.Elapsed += new ElapsedEventHandler(Track);
            trackTimer.Start();

            // send and clear tracking info every minute
            updateTimer = new Timer(60000);
            updateTimer.Elapsed += new ElapsedEventHandler(Update);
            updateTimer.Start();

            // listen for the web tracker
            webListener = new HttpListener();
            webListener.Prefixes.Add("http://localhost:11112/");
            webListener.Start();
            webListener.BeginGetContext(new AsyncCallback(ProcessRequest), null);
        }

        private void Track(object source, ElapsedEventArgs e)
        {
            foreach (Process p in Process.GetProcesses())
            {
                //Console.WriteLine("Debug, process name: " + p.ProcessName);
                pList.AddOrIncrement(p.ProcessName);
            }
        }

        private void Update(object source, ElapsedEventArgs e)
        {
            System.Threading.Thread senderThread = new System.Threading.Thread(SendData);
            senderThread.Start();
        }

        void SendData()
        {
            ClientDataModel cdm = new ClientDataModel();
            try
            {
                // DEBUG: Get platform
                ComputerInfo ci = new ComputerInfo();
                Console.WriteLine("Platform: " + ci.OSFullName);
                // prepare data to send
                cdm.clientKey = (string)System.Windows.Forms.Application.UserAppDataRegistry.GetValue("ClientKey");
                List<AppTimePair> atpList = new List<AppTimePair>();
                foreach (ProcessItem pi in pList.GetProcesses()) atpList.Add(new AppTimePair() { appName = pi.Name, time = (int)pi.Time, platform = pi.Web == true ? "Web" : ci.OSFullName });
                cdm.appTimes = atpList.ToArray();
                if (SpotifyLocalAPI.IsSpotifyRunning() && SpotifyLocalAPI.IsSpotifyWebHelperRunning())
                {
                    SpotifyLocalAPI sApi = new SpotifyLocalAPI();
                    sApi.Connect();
                    cdm.currentSong = sApi.GetStatus().Track.ArtistResource.Name + " - " + sApi.GetStatus().Track.TrackResource.Name;
                    cdm.currentSongUrl = sApi.GetStatus().Track.TrackResource.Uri;
                }

                // create a request and send it
                RestClient rc = new RestClient(GlobalConfig.ApiBase);
                RestRequest rr = new RestRequest("InsertTrackingData", Method.POST);
                rr.AddParameter("data", JsonConvert.SerializeObject(cdm));
                IRestResponse resp = rc.Execute(rr);
                Console.WriteLine(resp.Content);

                // clear times
                pList.Clear();
            }
            catch
            {
                Console.WriteLine("Something went wrong sending data. Invalid client key maybe?");
            }
        }

        // shamelessly copied from the old client
        // 'cause i honestly don't remember wtf any of this does
        void ProcessRequest(IAsyncResult result)
        {
            HttpListenerContext context = webListener.EndGetContext(result);
            HttpListenerRequest request = context.Request;
            string content = String.Empty;
            while (true)
            {
                int c = request.InputStream.ReadByte();
                if (c == -1) break;
                content += Convert.ToChar(c);
            }
            try
            {
                content = content.Substring(17, content.Length - 18);
                Console.WriteLine("Received sites: " + content);
                string[] sites = content.Split('+');
                foreach (String s in sites) pList.AddOrIncrement(s, true);
            }
            catch
            {
                Console.WriteLine("Something went wrong while trying to parse input from the web tracker.");
            }
            // Console.WriteLine(content);
            // let's just restart the whole damn thing?
            webListener.Abort();
            webListener = new HttpListener();
            webListener.Prefixes.Add("http://localhost:11112/");
            webListener.Start();
            webListener.BeginGetContext(new AsyncCallback(ProcessRequest), null);
        }
    }
}
