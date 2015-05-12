using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NLBLib.Misc
{
    class HttpRequestProcessor
    {
        private HttpClient _client;

        public HttpRequestProcessor()
        {
            _client = new HttpClient();
        }

        public bool IsHttpAccessible(Uri url)
        {
            try
            {
                _client.Timeout = TimeSpan.FromSeconds(30);
                HttpResponseMessage response = _client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).Result;

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Failed to get URL {0} with message {1}", url.ToString(), ex.Message));
                return false;
            }
        }
    }
}
