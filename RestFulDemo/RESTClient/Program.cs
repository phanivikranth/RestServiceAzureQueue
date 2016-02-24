using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml;

namespace RESTClient
{
    class Program
    {
        internal const string QueueName = "";

        class AzureStorageConstants
        {
            public static string Key = "";
            public static string SharedKeyAuthorizationScheme = "SharedKey";
            public static string Account = "";
            public static string QueueEndPoint = "";            
        }

        class QueueMessage
        {
            public QueueMessage(string messageId, DateTime insertionTime,DateTime expirationTime,int dequeueCount, string popReceipt, DateTime timeNextVisible,VendorProduct messageText)
            {
                this.MessageId = messageId;
                this.InsertionTime = insertionTime;
                this.ExpirationTime = expirationTime;
                this.DequeueCount = dequeueCount;
                this.PopReceipt = popReceipt;
                this.TimeNextVisible = timeNextVisible;
                this.MessageText = messageText;
            }
            public string MessageId { get; set; }
            public DateTime InsertionTime {get; set;}
            public DateTime ExpirationTime { get; set; }
            public int DequeueCount { get; set; }
            public string PopReceipt { get; set; }
            public DateTime TimeNextVisible { get; set; }
            public VendorProduct MessageText { get; set; }
            //public List<QueueMessagesList> Updates { get; set; }
        }

        class VendorProduct
        {
            public VendorProduct(string vendorCode, string productId)
            {
                this.VendorCode = vendorCode;
                this.ProductId = productId;
            }
            public string VendorCode { get; set; }
            public string ProductId { get; set; }
        }

        static void Main(string[] args)
        {

            Program p = new Program();
            p.GetMessage(QueueName);
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("");
            string t = storageAccount.QueueEndpoint.ToString();
            CloudQueueClient queueClient = new CloudQueueClient(storageAccount.QueueStorageUri, storageAccount.Credentials);            
            CloudQueue Queue = queueClient.GetQueueReference(QueueName);

            //var Signature = Convert.ToBase64String(new HMACSHA512(Encoding.UTF8.GetBytes(StringToSign)))
            if (Queue.CreateIfNotExists())
            {
                //HttpWebRequest req = WebRequest.Create()
                Console.WriteLine("Created Queue named: {0}", QueueName);
            }
            else
            {
                Console.WriteLine("Queue {0} already exists", QueueName);
            }
        }

        public static String CreateAuthorizationHeader(String canonicalizedString)
        {
            String signature = String.Empty;
            byte[] storageKey = Convert.FromBase64String(AzureStorageConstants.Key);

            using (HMACSHA256 hmacSha256 = new HMACSHA256(storageKey))
            {
                Byte[] dataToHmac = System.Text.Encoding.UTF8.GetBytes(canonicalizedString);
                signature = Convert.ToBase64String(hmacSha256.ComputeHash(dataToHmac));
            }

            String authorizationHeader = String.Format(
                CultureInfo.InvariantCulture,
                "{0} {1}:{2}",
                AzureStorageConstants.SharedKeyAuthorizationScheme,
                AzureStorageConstants.Account,
                signature
            );

            return authorizationHeader;
        }

        public void GetMessage(String queueName)
        {

            //myaccount.queue.core.windows.net/myqueue/messages
            string requestMethod = "GET";

            String urlPath = String.Format("{0}/messages", queueName);

            String storageServiceVersion = "2011-08-18";
            String dateInRfc1123Format = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);
            String canonicalizedHeaders = String.Format(
                    "x-ms-date:{0}\nx-ms-version:{1}",
                    dateInRfc1123Format,
                    storageServiceVersion);
            String canonicalizedResource = String.Format("/{0}/{1}", AzureStorageConstants.Account, urlPath);
            String stringToSign = String.Format(
                    "{0}\n\n\n\n\n\n\n\n\n\n\n\n{1}\n{2}",
                    requestMethod,
                    canonicalizedHeaders,
                    canonicalizedResource);
            String authorizationHeader = CreateAuthorizationHeader(stringToSign);

            Uri uri = new Uri(AzureStorageConstants.QueueEndPoint + urlPath);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = requestMethod;
            request.Headers.Add("x-ms-date",dateInRfc1123Format);
        
            request.Headers.Add("x-ms-version",storageServiceVersion);
        
            request.Headers.Add("Authorization",authorizationHeader);
        
            request.Accept = "application/atom+xml,application/xml";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                XmlDocument doc = new XmlDocument();                
                Stream dataStream = response.GetResponseStream();
                doc.Load(dataStream);
                byte[] data = Convert.FromBase64String(doc["QueueMessagesList"]["QueueMessage"]["MessageText"].InnerText);
                string decodedString = Encoding.UTF8.GetString(data);
                Console.WriteLine(decodedString);
                StreamReader str = new StreamReader(dataStream);
                string st = str.ReadToEnd();
                Console.WriteLine(st);
                
                Console.ReadKey();                              
            }
        }



  //      {
  //"Updates": [
  //  {
  //    "VendorCode": "TRK",
  //    "ProductId": "74f347d3-3725-4da5-83b2-7a9d639222cd"
  //  },
  //  {
  //    "VendorCode": "BTN",
  //    "ProductId": "2ff736f5-f671-4b5c-8879-d2fe635d7d85"
  //  }
  //]
        ////static void Main(string[] args)
        ////{
        ////    do
        ////    {
        ////        try
        ////        {
        ////            string content;
        ////            Console.WriteLine("Enter Method:");
        ////            string Method = Console.ReadLine();

        ////            Console.WriteLine("Enter URI:");
        ////            string uri = Console.ReadLine();

        ////            HttpWebRequest req = WebRequest.Create(uri) as HttpWebRequest;
        ////            req.KeepAlive = false;
        ////            req.Method = Method.ToUpper();

        ////            if (("POST,PUT").Split(',').Contains(Method.ToUpper()))
        ////            {
        ////                Console.WriteLine("Enter XML FilePath:");
        ////                string FilePath = Console.ReadLine();
        ////                content = (File.OpenText(@FilePath)).ReadToEnd();

        ////                byte[] buffer = Encoding.ASCII.GetBytes(content);
        ////                req.ContentLength = buffer.Length;
        ////                req.ContentType = "text/xml";
        ////                Stream PostData = req.GetRequestStream();
        ////                PostData.Write(buffer, 0, buffer.Length);
        ////                PostData.Close();

        ////            }

        ////            HttpWebResponse resp = req.GetResponse() as HttpWebResponse;


        ////            Encoding enc = System.Text.Encoding.GetEncoding(57005);
        ////            StreamReader loResponseStream = new StreamReader(resp.GetResponseStream(), enc);

        ////            string Response = loResponseStream.ReadToEnd();

        ////            loResponseStream.Close();
        ////            resp.Close();
        ////            Console.WriteLine(Response);
        ////        }
        ////        catch (Exception ex)
        ////        {
        ////            Console.WriteLine(ex.Message.ToString());
        ////        }

        ////        Console.WriteLine();
        ////        Console.WriteLine("Do you want to continue?");

        ////    } while (Console.ReadLine().ToUpper() == "Y");
        //}
    }
}
