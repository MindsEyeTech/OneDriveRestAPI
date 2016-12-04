using System;
using System.Collections.Generic;
using System.Linq;


namespace OneDriveRestAPI.Util
{
    public static class NameValueCollectionExtensions
    {
        public static string ToQueryString(this Dictionary<string,string> nvc, bool includePrefix = true)
        {
           
            List<string> list = new List<string>();
             foreach (KeyValuePair<string, string> keyValuePair in nvc)
            {
                var str = string.Format("{0}={1}", Uri.EscapeDataString(keyValuePair.Key),
                    Uri.EscapeDataString(keyValuePair.Value));
                  list.Add(str);
            }

            var array = list.ToArray<string>();
            return (includePrefix ? "?" : "") + string.Join("&", array);
        }

        public static Dictionary<string, string> ToNameValueCollection(this string queryString)
        {
            var nvc = new Dictionary<string, string>();

            foreach (string x in queryString.Split('&'))
            {
                string[] kvp = x.Split('=');
                nvc[kvp[0]] = kvp[1];
            }

            return nvc;
        }
    }
}
