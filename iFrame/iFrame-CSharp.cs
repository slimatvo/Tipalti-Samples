using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Tipalti.CodeSamples
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //string baseUrl = "https://ui.tipalti.com"; //production
            string baseUrl = "https://ui2.sandbox.tipalti.com"; //integration

            string payerName = "YourPayerNameHere";
            string payeeId = "payeeToOpenHere"; //the unique identifier for the payee
            string privateKey = "FEVUEmSW41JnFY5GukJTM/l74usNpSQiuus3ySeoDGntOOGkqlBrp4Eu8tMrv506"; //The secret key received from Tipalti

            //any additional parameters you may want to pass, according to Tipalti's documentation. Passing unknown parameters will cause an error.
            Dictionary<string,string> additionalParams = new Dictionary<string, string>()
            {
                {"first","John"},
                {"last", "Smith"}
            };

            string iframeUrl = CreateiFrameUrl(baseUrl, payerName, payeeId, privateKey, additionalParams); 
            //Use this URL to open the iFrame
            Console.WriteLine(iframeUrl);
        }

        
        public static string CreateiFrameUrl(string baseUrl, string payerName, string payeeId, string privateKey, Dictionary<string, string> parameters)
        {
            string queryString = CreateQueryString(payerName, payeeId, privateKey, parameters);

            string pageToOpen = "/payeedashboard/home?"; //to open the payment details iFrame. Change this to open a different iframe such as Payment History.

            return string.Format("{0}{1}?{2}", baseUrl, pageToOpen, queryString);
        }

        public static string CreateQueryString(string payerName, string payeeId, string privateKey, Dictionary<string, string> parameters)
        {
            List<string> queryStringPairs = new List<string>();

            queryStringPairs.Add(string.Format("payer={0}", HttpUtility.UrlEncode(payerName)));
            queryStringPairs.Add(string.Format("idap={0}", HttpUtility.UrlEncode(payeeId)));
            queryStringPairs.Add(string.Format("ts={0}", ConvertToUnixTimestamp(DateTime.UtcNow)));

            foreach (var kvp in parameters)
            {
                queryStringPairs.Add(string.Format("{0}={1}", kvp.Key, HttpUtility.UrlEncode(kvp.Value)));
            }

            string combinedQs = string.Join("&", queryStringPairs);
            
            string signature = EncryptFullQueryString(combinedQs, privateKey);
            combinedQs = combinedQs + "&hashkey=" + signature;
            
            return combinedQs;
        }

        public static string EncryptFullQueryString(string queryString, string privateKey)
        {
            using (KeyedHashAlgorithm hash = KeyedHashAlgorithm.Create("HMACSHA256"))
            {
                hash.Key = Encoding.ASCII.GetBytes(privateKey);
                string calculatedAuth = DecodeHex(hash.ComputeHash(Encoding.UTF8.GetBytes(queryString)));
                return calculatedAuth;
            }
        }

        private static double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date - origin;
            return Math.Floor(diff.TotalSeconds);
        }

        static string DecodeHex(byte[] data)
        {
            StringBuilder result = new StringBuilder();
            foreach (char symbol in data) { result.Append(((int)symbol).ToString("x2")); }
            return result.ToString();
        }
    }
}
