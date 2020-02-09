using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;

namespace WebServiceRestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string baseUrl = ConfigurationManager.AppSettings["BaseUrl"].ToString();
                string username = ConfigurationManager.AppSettings["Username"].ToString();
                string apiKey = ConfigurationManager.AppSettings["ApiKey"].ToString();


                //Get Folders...
                GetFolders(baseUrl, username, apiKey);
                

                /*
                //Get Templates...
                GetTemplates(baseUrl, username, apiKey);
                */

                /*
                //Process a document...               
                ProcessDocument(baseUrl, username, apiKey);
                */                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:" + ex.Message);
            }
            Console.ReadKey();
        }

        private static void GetFolders(string baseUrl, string username, string apiKey)
        {
            HttpWebRequest request = CreateHttpRequest($"{baseUrl}/GetFolders", username, apiKey, "GET");
            request.ContentLength = 0;
            try
            {
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    string strJSON = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    Console.WriteLine(strJSON);
                }
            }
            catch (WebException wex)
            {
                Console.WriteLine(string.Format("API Error. HTTPCode:{0}", ((HttpWebResponse)wex.Response).StatusCode));
            }
        }

        private static void GetTemplates(string baseUrl, string username, string apiKey)
        {
            HttpWebRequest request = CreateHttpRequest($"{baseUrl}/GetTemplates", username, apiKey, "GET");
            request.ContentLength = 0;
            try
            {
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    string strJSON = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    Console.WriteLine(strJSON);
                }
            }
            catch (WebException wex)
            {
                Console.WriteLine(string.Format("API Error. HTTPCode:{0}", ((HttpWebResponse)wex.Response).StatusCode));
            }
        }
        
        private static void ProcessDocument(string baseUrl, string username, string apiKey)
        {
            string filepath = @"C:\img.jpg";//set file path here...

            var fileParams = new FileParam
            {
                FileName = Path.GetFileName(filepath),
                TemplateId = "53zlkxgpfn45htnge1hxh53r",//one of the template ids received from "GetTemplates"...
                FileBytes = GetUploadedFile(filepath),
                FolderId = "7fb0e3a343f0"               //if we do not set FolderId here, the root folder will be selected by default...
            };

            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(fileParams);
            byte[] bytesToSend = Encoding.ASCII.GetBytes(jsonString);

            HttpWebRequest request = CreateHttpRequest($"{baseUrl}/ProcessDocument", username, apiKey, "POST");
            request.ContentLength = bytesToSend.Length;
            using (Stream post = request.GetRequestStream())
            {
                post.Write(bytesToSend, 0, bytesToSend.Length);
            }

            try
            {
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    string strJSON = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    Console.WriteLine(strJSON);
                }
            }
            catch (WebException wex)
            {
                Console.WriteLine(string.Format("API Error. HTTPCode:{0}", ((HttpWebResponse)wex.Response).StatusCode));
            }
        }
        private static byte[] GetUploadedFile(string file_name)
        {
            FileStream streamContent = new FileStream(file_name, FileMode.Open, FileAccess.Read);
            byte[] inData = new byte[streamContent.Length];
            streamContent.Read(inData, 0, (int)streamContent.Length);
            return inData;
        }

        private static HttpWebRequest CreateHttpRequest(string apiUrl, string username, string apiKey, string httpMethod)
        {
            Uri address = new Uri(apiUrl);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(address);
            byte[] authBytes = Encoding.UTF8.GetBytes(string.Format("{0}:{1}", username, apiKey).ToCharArray());
            request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(authBytes);
            request.Method = httpMethod;
            request.Timeout = 600000;
            request.ContentType = "application/json";
            return request;
        }

    }

    class FileParam
    {
        public string FileName { get; set; }
        public string FolderId { get; set; }
        public string TemplateId { get; set; }
        public byte[] FileBytes { get; set; }
    }
}
