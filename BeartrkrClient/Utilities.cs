using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using RestSharp;

namespace BeartrkrClient
{
    class Utilities
    {
        public static bool ValidateClientKey()
        {
            if (Application.UserAppDataRegistry.GetValue("ClientKey") != null)
            {
                return ValidateClientKey((string)Application.UserAppDataRegistry.GetValue("ClientKey"));
            }
            else return false;
        }

        public static bool ValidateClientKey(string key)
        {
            RestClient rc = new RestClient(GlobalConfig.ApiBase);
            RestRequest rr = new RestRequest("ValidateClientKey", Method.GET);
            rr.AddParameter("key", key);
            return rc.Execute(rr).Content.Contains("success");
        }

        public static void AddToStartup()
        {
            RegistryKey k = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            k.SetValue("Beartrkr Client", Application.ExecutablePath);
        }
    }
}
