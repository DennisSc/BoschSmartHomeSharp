﻿using System;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using RestSharp;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Globalization;


public class BoschSmartHomeSharp
{
 
    public class ApiClient
    {
        public string IPaddress { get; set; }
        string url;
        private static string certFile;
        private X509Certificate2 certificate;
        public ApiClient(string ip)
        {
            IPaddress = ip;
            certFile = Path.Join(Directory.GetCurrentDirectory(), "client_pfx.pfx");
            certificate = new X509Certificate2(certFile, "12345");
            url = "https://" + IPaddress + ":8444/smarthome";



        }

        public ApiClient(string ip, string certFilePath, string certPassword)
        {
            IPaddress = ip;
            certificate = new X509Certificate2(certFilePath, certPassword);
            url = "https://" + IPaddress + ":8444/smarthome";



        }

        public ApiClient(string ip, string certFilePath)
        {
            IPaddress = ip;
            certificate = new X509Certificate2(certFilePath);
            url = "https://" + IPaddress + ":8444/smarthome";



        }


        public int getDeviceOnOffState(device mydevice)
        {
            RestSharp.RestClient client;

            if (mydevice.deviceModel == "BSM")
                client = new RestSharp.RestClient(url + "/devices/" + mydevice.id + "/services/PowerSwitch/state");
            else
                client = new RestSharp.RestClient(url + "/devices/" + mydevice.id + "/services/BinarySwitch/state");


            client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            client.ClientCertificates = new X509CertificateCollection() { certificate };
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("api-version", "2.1");

            IRestResponse response = client.Execute(request);
            //Debug.WriteLine(response.Content);
            Debug.WriteLine(((int)response.StatusCode));

            Debug.WriteLine(response.Content);

            string payloadJson;

            JToken token = JObject.Parse(response.Content);
            if (mydevice.deviceModel == "BSM")
                payloadJson = token["switchState"].ToString();
            else
                payloadJson = token["on"].ToString();

            Debug.WriteLine(payloadJson);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                if (payloadJson == "True" || payloadJson == "ON")
                    return 1;
                else if (payloadJson == "False" || payloadJson == "OFF")
                    return 0;
                else
                    return -1;
            }
            else
                return -3;
        }


        public int getDeviceLevelState(device mydevice)
        {
            RestSharp.RestClient client = new RestSharp.RestClient(url + "/devices/" + mydevice.id + "/services/MultiLevelSwitch/state");


            client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            client.ClientCertificates = new X509CertificateCollection() { certificate };
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("api-version", "2.1");
            request.AddParameter("application/json", "", ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);
            Debug.WriteLine(((int)response.StatusCode));
            Debug.WriteLine(response.Content);


            JToken token = JObject.Parse(response.Content);
            string payloadJson = token["level"].ToString();

            Debug.WriteLine(payloadJson);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {

                return Int32.Parse(payloadJson);

            }
            else
                return -3;
        }



        public float getShutterLevelState(device mydevice)
        {
            RestSharp.RestClient client = new RestSharp.RestClient(url + "/devices/" + mydevice.id + "/services/ShutterControl/state");


            client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            client.ClientCertificates = new X509CertificateCollection() { certificate };
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("api-version", "2.1");
            request.AddParameter("application/json", "", ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);
            Debug.WriteLine(((int)response.StatusCode));
            Debug.WriteLine(response.Content);


            JToken token = JObject.Parse(response.Content);
            string payloadJson = token["level"].ToString();

            Debug.WriteLine(payloadJson);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {

                return float.Parse(payloadJson);

            }
            else
                return 666;
        }



        public bool switchOnLightDevice(device mydevice)
        {
            RestSharp.RestClient client;

            if (mydevice.deviceModel == "BSM")
                client = new RestSharp.RestClient(url + "/devices/" + mydevice.id + "/services/PowerSwitch/state");
            else
                client = new RestSharp.RestClient(url + "/devices/" + mydevice.id + "/services/BinarySwitch/state");

            client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            client.ClientCertificates = new X509CertificateCollection() { certificate };
            client.Timeout = -1;
            var request = new RestRequest(Method.PUT);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("api-version", "2.1");
            if (mydevice.deviceModel == "BSM")
                request.AddParameter("application/json", "{\r\n    \"@type\": \"powerSwitchState\",\r\n    \"switchState\": \"ON\"\r\n}", ParameterType.RequestBody);
            else
                request.AddParameter("application/json", "{\r\n    \"@type\": \"binarySwitchState\",\r\n    \"on\": true\r\n}", ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);
            //Debug.WriteLine(response.Content);
            Debug.WriteLine(((int)response.StatusCode));

            Debug.WriteLine(response.Content);

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return true;
            else
                return false;
        }

        public bool switchOffLightDevice(device mydevice)
        {
            RestSharp.RestClient client;

            if (mydevice.deviceModel == "BSM")
                client = new RestSharp.RestClient(url + "/devices/" + mydevice.id + "/services/PowerSwitch/state");
            else
                client = new RestSharp.RestClient(url + "/devices/" + mydevice.id + "/services/BinarySwitch/state");

            client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            client.ClientCertificates = new X509CertificateCollection() { certificate };
            client.Timeout = -1;
            var request = new RestRequest(Method.PUT);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("api-version", "2.1");
            if (mydevice.deviceModel == "BSM")
                request.AddParameter("application/json", "{\r\n    \"@type\": \"powerSwitchState\",\r\n    \"switchState\": \"OFF\"\r\n}", ParameterType.RequestBody);
            else
                request.AddParameter("application/json", "{\r\n    \"@type\": \"binarySwitchState\",\r\n    \"on\": false\r\n}", ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);
            //Debug.WriteLine(response.Content);
            Debug.WriteLine(((int)response.StatusCode));

            Debug.WriteLine(response.Content);

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return true;
            else
                return false;
        }


        public bool setDeviceLevelState(device mydevice, int level)
        {
            RestSharp.RestClient client;

            client = new RestSharp.RestClient(url + "/devices/" + mydevice.id + "/services/MultiLevelSwitch/state");


            client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            client.ClientCertificates = new X509CertificateCollection() { certificate };
            client.Timeout = -1;
            var request = new RestRequest(Method.PUT);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("api-version", "2.1");
            request.AddParameter("application/json", "{\r\n    \"@type\": \"multiLevelSwitchState\",\r\n    \"level\": " + level.ToString() + "\r\n}", ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);
            //Debug.WriteLine(response.Content);
            Debug.WriteLine(((int)response.StatusCode));

            Debug.WriteLine(response.Content);



            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {

                return true;

            }
            else
                return false;
        }


        public bool setShutterLevelState(device mydevice, float level)
        {
            RestSharp.RestClient client;

            client = new RestSharp.RestClient(url + "/devices/" + mydevice.id + "/services/ShutterControl/state");


            client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            client.ClientCertificates = new X509CertificateCollection() { certificate };
            client.Timeout = -1;
            var request = new RestRequest(Method.PUT);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("api-version", "2.1");
            Debug.WriteLine(level.ToString("0.00", new CultureInfo("en-US", false)));
            request.AddParameter("application/json", "{\r\n    \"@type\": \"shutterControlState\",\r\n    \"level\": " + level.ToString("0.00", new CultureInfo("en-US", false)) + "\r\n}", ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);
            //Debug.WriteLine(response.Content);
            Debug.WriteLine(((int)response.StatusCode));

            Debug.WriteLine(response.Content);



            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {

                return true;

            }
            else
                return false;
        }
            
            
        public string subscribeLongPoll()
        {
            RestSharp.RestClient client;
            client = new RestSharp.RestClient("https://" + IPaddress + ":8444/remote/json-rpc");
            client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            client.ClientCertificates = new X509CertificateCollection() { certificate };
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("api-version", "2.1");
            request.AddParameter("application/json", "[\n    {\n        \"jsonrpc\":\"2.0\",\n        \"method\":\"RE/subscribe\",\n        \"params\": [\"com/bosch/sh/remote/*\", null]\n    }\n]", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            string jsonResponse = "{\"answer\":" + response.Content + "}";
            JToken token = JObject.Parse(jsonResponse);
            string pollId = token["answer"][0]["result"].ToString();
            Debug.WriteLine("sub pollId: " + pollId);
            return pollId;
        }

        public bool unsubscribeLongPoll(string pollId)
        {
            try
            {
                RestSharp.RestClient client;
                client = new RestSharp.RestClient("https://" + IPaddress + ":8444/remote/json-rpc");
                client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
                client.ClientCertificates = new X509CertificateCollection() { certificate };
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("api-version", "2.1");
                request.AddParameter("application/json", "[\n    {\n        \"jsonrpc\":\"2.0\",\n        \"method\":\"RE/unsubscribe\",\n        \"params\": [\"" + pollId + "\"]\n    }\n]", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);

                string jsonResponse2 = "{\"answer\":" + response.Content + "}";
                JToken token = JObject.Parse(jsonResponse2);
                var payloadJson = token["answer"][0]["result"].ToString();
                Debug.WriteLine("unsubscribed. " + payloadJson);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return false;
            }
        }


        public IEnumerable<JToken> longPoll(string pollId)
        {
            RestSharp.RestClient client;
            client = new RestSharp.RestClient("https://" + IPaddress + ":8444/remote/json-rpc");
            client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            client.ClientCertificates = new X509CertificateCollection() { certificate };
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("api-version", "2.1");
            request.AddParameter("application/json", "[\n    {\n        \"jsonrpc\":\"2.0\",\n        \"method\":\"RE/longPoll\",\n        \"params\": [\"" + pollId + "\", 30]\n    }\n]", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            var JArray = Newtonsoft.Json.Linq.JArray.Parse(response.Content);
            return JArray.Children();
            
        }



        public List<device> getDevices()
        {
            RestSharp.RestClient client = new RestSharp.RestClient(url + "/devices");
            client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            client.ClientCertificates = new X509CertificateCollection() { certificate };
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("api-version", "2.1");
            IRestResponse response = client.Execute(request);
            //Debug.WriteLine(response.Content);
            string jsonResponse2 = "{\"devices\":" + response.Content + "}";
            JToken token = JObject.Parse(jsonResponse2);
            var payloadJson = token["devices"].ToString();
            return JsonConvert.DeserializeObject<List<device>>(payloadJson);
        }

        public List<client> getClients()
        {
            RestSharp.RestClient client = new RestSharp.RestClient(url + "/clients");
            client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            client.ClientCertificates = new X509CertificateCollection() { certificate };
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("api-version", "2.1");
            IRestResponse response = client.Execute(request);
            string jsonResponse2 = "{\"clients\":" + response.Content + "}";
            JToken token = JObject.Parse(jsonResponse2);
            var payloadJson = token["clients"].ToString();
            return JsonConvert.DeserializeObject<List<client>>(payloadJson);
        }


        public bool registerDevice(string devicePwdBase64, string cert, string clientname)
        {

            RestSharp.RestClient client = new RestSharp.RestClient("https://" + IPaddress + ":8443/smarthome/clients");
            client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            client.ClientCertificates = new X509CertificateCollection() { certificate };
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Systempassword", devicePwdBase64);
            request.AddParameter("application/json", "{\r\n  \"@type\": \"client\",\r\n  \"id\": \"oss_" + clientname + "\",\r\n  \"name\": \"OSS " + clientname + "\",\r\n  \"primaryRole\": \"ROLE_RESTRICTED_CLIENT\",\r\n  \"certificate\": " + cert + "\r\n}\r\n", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            Debug.WriteLine(((int)response.StatusCode));

            Debug.WriteLine(response.Content);

            if (response.StatusCode == System.Net.HttpStatusCode.Created)
                return true;
            else
                return false;


        }


    }
}


public class device
{
    public string id { get; set; }
    public string deviceModel { get; set; }
    public string name { get; set; }
    public string status { get; set; }
    public string roomId { get; set; }
    public string rootDeviceId { get; set; }
    public string manufacturer { get; set; }
    public string[] deviceServiceIds { get; set; }
    public string parentDeviceId { get; set; }
    public string[] childDeviceIds { get; set; }
    public string profile { get; set; }





}

public class client
{
    public string id { get; set; }
    public string clientType { get; set; }
    public string name { get; set; }
    public string primaryRole { get; set; }
    public string[] roles { get; set; }


}

public class roles
{
    public const string ROLE_DEVICES_WRITE = "ROLE_DEVICES_WRITE";
    public const string ROLE_CLIENTS_WRITE = "ROLE_CLIENTS_WRITE";
    public const string ROLE_SCENARIOS_WRITE = "ROLE_SCENARIOS_WRITE";
    public const string ROLE_RESTRICTED_CLIENT = "ROLE_RESTRICTED_CLIENT";
    public const string ROLE_CLOUD_CAMERA_RW = "ROLE_CLOUD_CAMERA_RW";
    public const string ROLE_AUTOMATION_RULES_WRITE = "ROLE_AUTOMATION_RULES_WRITE";
};




