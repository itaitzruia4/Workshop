using System;
using System.Net;
using System.Text;
using System.IO;

namespace Workshop.ServiceLayer
{
    public class ExternalSystem : IExternalSystem
    {
        private readonly string _SERVER_URL = "https://cs-bgu-wsep.herokuapp.com/";

        public int Cancel_Pay(int transaction_id)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_SERVER_URL);

            string postData = "action_type=" + Uri.EscapeDataString("cancel_pay");
                postData += "transaction_id=" + Uri.EscapeDataString(transaction_id.ToString());
            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var encoding = Encoding.GetEncoding(response.CharacterSet);
                int res;
                using (var responseStream = response.GetResponseStream())
                using (var reader = new StreamReader(responseStream, encoding))
                    return int.TryParse(reader.ReadToEnd(), out res) ? res : -1;
            }
            return -1;
        }

        public int Cancel_Supply(int transaction_id)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_SERVER_URL);

            string postData = "action_type=" + Uri.EscapeDataString("cancel_supply");
            postData += "transaction_id=" + Uri.EscapeDataString(transaction_id.ToString());
            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var encoding = Encoding.GetEncoding(response.CharacterSet);
                int res;
                using (var responseStream = response.GetResponseStream())
                using (var reader = new StreamReader(responseStream, encoding))
                    return int.TryParse(reader.ReadToEnd(), out res) ? res : -1;
            }
            return -1;
        }

        public bool IsExternalSystemOnline()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_SERVER_URL);

            var postData = "action_type=" + Uri.EscapeDataString("handshake");
            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();      
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var encoding = Encoding.GetEncoding(response.CharacterSet);
                using (var responseStream = response.GetResponseStream())
                using (var reader = new StreamReader(responseStream, encoding))
                    return reader.ReadToEnd().Equals("OK");
            }
            return false; 
        }

        public int Pay(string card_number, string month, string year, string holder, string ccv, string id)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_SERVER_URL);

            string postData = "action_type=" + Uri.EscapeDataString("pay");
            postData += "card_number=" + Uri.EscapeDataString(card_number);
            postData += "month=" + Uri.EscapeDataString(month);
            postData += "year=" + Uri.EscapeDataString(year);
            postData += "holder=" + Uri.EscapeDataString(holder);
            postData += "ccv=" + Uri.EscapeDataString(ccv);
            postData += "id=" + Uri.EscapeDataString(id);
            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var encoding = Encoding.GetEncoding(response.CharacterSet);
                int res;
                using (var responseStream = response.GetResponseStream())
                using (var reader = new StreamReader(responseStream, encoding))
                    return int.TryParse(reader.ReadToEnd(), out res) ? res : -1;
            }
            return -1;
        }

        public int Supply(string name, string address, string city, string country, string zip)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_SERVER_URL);

            string postData = "action_type=" + Uri.EscapeDataString("pay");
            postData += "name=" + Uri.EscapeDataString(name);
            postData += "address=" + Uri.EscapeDataString(address);
            postData += "city=" + Uri.EscapeDataString(city);
            postData += "country=" + Uri.EscapeDataString(country);
            postData += "zip=" + Uri.EscapeDataString(zip);
            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var encoding = Encoding.GetEncoding(response.CharacterSet);
                int res;
                using (var responseStream = response.GetResponseStream())
                using (var reader = new StreamReader(responseStream, encoding))
                    return int.TryParse(reader.ReadToEnd(), out res) ? res : -1;
            }
            return -1;
        }
    }
}
