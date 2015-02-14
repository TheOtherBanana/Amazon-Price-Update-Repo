using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace AWSProjects.CommonUtils
{
    public class XmlUtils
    {
        public static string SerializeObjectToString<T>(T obj)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            string objString = string.Empty;

            using (StringWriter stringWriter = new StringWriter())
            {
                xmlSerializer.Serialize(stringWriter, obj);
                objString = stringWriter.ToString();
            }

            return objString;
        }

        public static T DeserializeStringToObject<T>(string objectString)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            T obj;

            using(StringReader stringReader = new StringReader(objectString))
            {
                //XmlReader xmlReader = XmlReader.Create(stringReader);
                obj = (T)xmlSerializer.Deserialize(stringReader);
            }

            return obj;
        }
    }

    //TODO: Make it functional. 
    public class RestApiUtils
    {
        public static string ExecuteRestApi(string requestUri)
        {
            HttpWebRequest request = WebRequest.Create(requestUri) as HttpWebRequest;
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;

            string responseBody = null;

            //get the response status 
            if (response != null)
            {
                //get the response body
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    responseBody = reader.ReadToEnd();
                }
            }

            return responseBody;

        }

        public static HttpWebResponse GetHttpResponse(string requestUri)
        {
            HttpWebRequest request = WebRequest.Create(requestUri) as HttpWebRequest;
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;

            return response;
        }
    }
}
