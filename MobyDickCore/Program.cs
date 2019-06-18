using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;

namespace MobyDickCore
{
    class Program
    {
        public static Dictionary<string, int> WordList;

        static void Main(string[] args)
        {
            FileStream filestream = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + "MobyDick.txt", FileMode.Create);
            var streamwriter = new StreamWriter(filestream);
            streamwriter.AutoFlush = true;
            Console.SetOut(streamwriter);
            Console.SetError(streamwriter);

            Thread[] array = new Thread[2];

            for (int i = 0; i < 1; i++)
            {
                array[i] = new Thread(new ThreadStart(Metod));
                array[i].Name = i + "";
                array[i].Start();
            }
            //Console.ReadKey();
        }

        static void Metod()
        {
            for (int i = 0; i < 1; i++)
            {
                postReq();
            }
        }

        static void postReq()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            WebRequest request = WebRequest.Create("http://www.gutenberg.org/files/2701/2701-0.txt");
            request.Method = "POST";
            string postString = "";
            byte[] byteArray = Encoding.UTF8.GetBytes(postString);
            // Set the ContentType property of the WebRequest.  
            request.ContentType = "application/x-www-form-urlencoded";
            // Set the ContentLength property of the WebRequest.  
            request.ContentLength = byteArray.Length;
            // Get the request stream.  
            Stream dataStream = request.GetRequestStream();
            // Write the data to the request stream.  
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.  
            dataStream.Close();
            // Get the response.  
            WebResponse response = request.GetResponse();
            // Display the status.  
            //Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.  
            dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.  
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.  
            string responseFromServer = reader.ReadToEnd();
            // Display the content.  
            //Console.WriteLine("Name: "+Thread.CurrentThread.Name);
            //Console.WriteLine("id: "+id);
            //Console.WriteLine("response: "+responseFromServer);
            //Console.WriteLine(Thread.CurrentThread.Name+" "+id);
            //Console.WriteLine("response: " + responseFromServer);
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine(responseFromServer);
            //Console.WriteLine("----");
            // Clean up the streams.  
            reader.Close();
            dataStream.Close();
            response.Close();

            WordList = new Dictionary<string, int>();
            WordCounter(responseFromServer);
            WriteToXML("MobyDick");

        }
        public static void WriteToXML(string xml)
        {
            XmlWriter xmlWriter = XmlWriter.Create(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + "MobyDick.xml", null);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("words");

            foreach (KeyValuePair<string, int> entry in WordList)
            {
                xmlWriter.WriteStartElement("word");
                xmlWriter.WriteAttributeString("count", entry.Value + "");
                xmlWriter.WriteAttributeString("text", entry.Key);
                xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
        }

        public static void WordCounter(string input)
        {
            Regex rgx = new Regex("[^a-zA-Z -]");
            input = rgx.Replace(input, " ");
            string[] words = input.Split(' ');

            foreach (string word in words)
            {
                word.Replace('\r', ' ');
                if (word.Length > 1)
                {
                    if (WordList.ContainsKey(word.ToLower()))
                    {
                        WordList[word.ToLower()] = WordList[word.ToLower()] + 1;
                    }
                    else
                    {
                        WordList.Add(word.ToLower(), 1);
                    }
                }
            }
        }
    }
}
