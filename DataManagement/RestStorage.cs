using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace DataManagement
{
    public class RestStorage
    {

        public static RestClient GetClient()
        {
            var rest =  new RestClient("http://localhost:5000");
            rest.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            rest.AddDefaultHeader("SECRET_KEY", "6efb4653-a124-4941-af02-a26d4853aec7");
            return rest;
        }

        public static RestRequest GetRequest(string path, Method type)
        {
            return new RestRequest("api/v1/" + path, type, RestSharp.DataFormat.Json);
        }

        public static T JsonToObj<T>(string json)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        }
    }
}
