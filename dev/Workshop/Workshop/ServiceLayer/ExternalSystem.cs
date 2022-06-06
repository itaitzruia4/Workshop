using System;
using System.Net;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Net.Http;
using System.Collections.Generic;

namespace Workshop.ServiceLayer
{
    public class ExternalSystem : IExternalSystem
    {
        private readonly string _SERVER_URL = "https://cs-bgu-wsep.herokuapp.com/";
        public int Cancel_Pay(int transaction_id)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_SERVER_URL);
            var data = $"action_type=handshake&transaction_id={transaction_id}";
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            using (var stream = new StreamWriter(request.GetRequestStream()))
            {
                stream.Write(data);
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var encoding = Encoding.GetEncoding(response.CharacterSet);
                using (var responseStream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(responseStream, encoding))
                    {
                        int res;
                        string responseData = reader.ReadToEnd();
                        return int.TryParse(responseData, out res) ? res : -1;
                    }
                }
            }
            return -1;
        }

        public int Cancel_Supply(int transaction_id)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_SERVER_URL);
            var data = $"action_type=handshake&transaction_id={transaction_id}";
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            using (var stream = new StreamWriter(request.GetRequestStream()))
            {
                stream.Write(data);
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var encoding = Encoding.GetEncoding(response.CharacterSet);
                using (var responseStream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(responseStream, encoding))
                    {
                        int res;
                        string responseData = reader.ReadToEnd();
                        return int.TryParse(responseData, out res) ? res : -1;
                    }
                }
            }
            return -1;
        }

        public bool IsExternalSystemOnline()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_SERVER_URL);
            var data = "action_type=handshake";
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            using (var stream = new StreamWriter(request.GetRequestStream()))
            {
                stream.Write(data);
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var encoding = Encoding.GetEncoding(response.CharacterSet);
                using (var responseStream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(responseStream, encoding))
                    {
                        string responseData = reader.ReadToEnd();
                        return responseData.Equals("OK");
                    }
                }
            }
            return false;
        }

        public int Pay(string card_number, string month, string year, string holder, string ccv, string id)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_SERVER_URL);
            string data = $"action_type=pay&card_number={card_number}&month={month}&year={year}&holder={holder}&ccv={ccv}&id={id}";
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            using (var stream = new StreamWriter(request.GetRequestStream()))
            {
                stream.Write(data);
            }

            var response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var encoding = Encoding.GetEncoding(response.CharacterSet);
                using (var responseStream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(responseStream, encoding))
                    {
                        int res;
                        string responseData = reader.ReadToEnd();
                        return int.TryParse(responseData, out res) ? res : -1;
                    }
                }
            }
            return -1;
        }

        public int Supply(string name, string address, string city, string country, string zip)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_SERVER_URL);
            string data = $"action_type=supply&name={name}&address={address}&city={city}&country={country}&zip={zip}";
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            using (var stream = new StreamWriter(request.GetRequestStream()))
            {
                stream.Write(data);
            }

            var response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var encoding = Encoding.GetEncoding(response.CharacterSet);
                using (var responseStream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(responseStream, encoding))
                    {
                        int res;
                        string responseData = reader.ReadToEnd();
                        return int.TryParse(responseData, out res) ? res : -1;
                    }
                }
            }
            return -1;
        }
    }
}
