using System;
using System.Collections.Generic;

namespace WSP.Entities.Processing
{
    public partial class ProcessingAddress
    {
        public List<string> Ping(string hostName)
        {
            hostName = MathcAddress(hostName, _httpwebAddress, 0);

            System.Net.IPHostEntry entry;
            List<string> list = new List<string>();
            try
            {
                entry = PingSite(hostName);
            }
            catch (Exception ex)
            {
                list.Add(ex.Message);
                return list;
            }
            list.Add("IP addresses:");
            string temp = "";
            foreach (System.Net.IPAddress address in entry.AddressList)
            {
                list.Add(address.ToString());
                temp = PingInfo(address.ToString());
                if (!string.IsNullOrEmpty(temp))
                    list.Add(temp);

            }

            if (entry.Aliases.Length > 0)
            {
                list.Add("Aliases:");
                foreach (string name in entry.Aliases)
                {
                    list.Add(name);
                }
            }
            list.Add("HostName:");
            list.Add(entry.HostName);

            return list;
        }
        private string PingInfo(string p)
        {

            System.Net.NetworkInformation.Ping pingSender = new System.Net.NetworkInformation.Ping();
            System.Net.NetworkInformation.PingOptions options = new System.Net.NetworkInformation.PingOptions();

            options.DontFragment = true;

            string data = new string('a', 32);
            byte[] buffer = System.Text.Encoding.ASCII.GetBytes(data);
            int timeout = 120;

            System.Net.NetworkInformation.PingReply reply = pingSender.Send(p, timeout, buffer, options);

            if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
            {
                return string.Format("\t-> Round Trip time: {1} ms, Time to live: {2}, Buffer size: {4}; ", reply.Address.ToString(), reply.RoundtripTime, reply.Options.Ttl, reply.Options.DontFragment, reply.Buffer.Length);
            }
            return "";
        }
        private System.Net.IPHostEntry PingSite(string hostName)
        {
            string match = MathcAddress(hostName, _webAddress, 2);

            if (string.IsNullOrEmpty(match))
            {
                throw new System.Exception("Do net corectly web address");
            }

            try
            {
                return System.Net.Dns.GetHostEntry(match);
            }
            catch (Exception ex)
            {
                throw ex;// new System.Exception("Do net corectly web address");
            }
        }

        private List<string> SiteIPAddress(string hostName)
        {
            string match = MathcAddress(hostName, _webAddress, 2);

            if (string.IsNullOrEmpty(match)) return new List<string>();

            System.Net.IPHostEntry entry;

            try
            {
                entry = System.Net.Dns.GetHostEntry(match);
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                return new List<string>();
            }

            List<string> list = new List<string>();

            foreach (System.Net.IPAddress address in entry.AddressList)
                list.Add(address.ToString());

            return list;
        }

        private string RequestByUrl(string hostName)
        {
            try
            {
                var request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(new Uri(hostName));

                request.KeepAlive = false;
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.103 Safari/537.36";
                return new System.IO.StreamReader(request.GetResponse().GetResponseStream()).ReadToEnd();
            }
            catch (System.Net.WebException wex)
            {
                if (wex.Response != null)
                {
                    return new System.IO.StreamReader(wex.Response.GetResponseStream()).ReadToEnd();
                }
                else
                {
                    return "";
                }
            }
            catch (System.UriFormatException ufe)
            {
                return "";
            }
            catch (System.IO.IOException ioe)
            {
                return "";
            }

        }






    }
}