using System;
using System.IO;
using System.Net;
using RestSharp;
using SignhostApiClientLibrary.Models;
using SignhostClientLibrary.Models;

namespace SignhostApiClientLibrary
{
    public class SignhostClient
    {
        private string _applicationName;
        private string _applicationKey;
        private string _authorizationKey;

        private string ApplicationHeader
        {
            get
            {
                return String.Format("APPKey {0} {1}", _applicationName, _applicationKey);
            }
        }

        private string AuthorizationHeader
        {
            get
            {
                return String.Format("APIKey {0}", _authorizationKey);
            }
        }

        private IRestClient _restClient;

        public SignhostClient(string applicationName, string applicationKey, string authorizationKey)
        {
            Initialize(applicationName, applicationKey, authorizationKey, "https://api.signhost.com/api");
        }

        public SignhostClient(string applicationName, string applicationKey, string authorizationKey, string url)
        {
            Initialize(applicationName, applicationKey, authorizationKey, url);
        }

        private void Initialize(string applicationName, string applicationKey, string authorizationKey, string url)
        {
            _applicationName = applicationName;
            _applicationKey = applicationKey;
            _authorizationKey = authorizationKey;

            _restClient = new RestClient(url);
        }

        private IRestRequest InitializeRestRequest(string resource, Method method)
        {
            var restRequest = new RestRequest(resource, method);
            restRequest.AddHeader("Application", ApplicationHeader);
            restRequest.AddHeader("Authorization", AuthorizationHeader);
            return restRequest;
        }

        public Transaction GetTransaction(Guid id)
        {
            var restRequest = InitializeRestRequest("transaction/{id}", Method.GET);
            restRequest.AddUrlSegment("id", id.ToString());
            var restResponse = _restClient.Execute<Transaction>(restRequest);
            if (restResponse.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(restResponse.ErrorMessage);
            }

            return restResponse.Data;
        }

        public byte[] GetFile(Guid id)
        {
            using (var webClient = new WebClient())
            {
                webClient.Headers.Add("Application", ApplicationHeader);
                webClient.Headers.Add("Authorization", AuthorizationHeader);
                return webClient.DownloadData(String.Format(@"{0}/{1}/{2}/{3}", _restClient.BaseUrl, "file", "document", id));
            }
        }

        public byte[] GetReceipt(Guid id)
        {
            using (var webClient = new WebClient())
            {
                webClient.Headers.Add("Application", ApplicationHeader);
                webClient.Headers.Add("Authorization", AuthorizationHeader);
                return webClient.DownloadData(String.Format(@"{0}/{1}/{2}/{3}", _restClient.BaseUrl, "file", "receipt", id));
            }
        }

        public Transaction AddTransaction(Transaction transaction)
        {
            var restRequest = InitializeRestRequest("transaction", Method.POST);
            restRequest.AddJsonBody(transaction);
            var restResponse = _restClient.Execute<Transaction>(restRequest);

            if(restResponse.StatusCode == HttpStatusCode.BadRequest || restResponse.StatusCode == HttpStatusCode.Unauthorized)
            {
                var error = SimpleJson.DeserializeObject<Error>(restResponse.Content);
                throw new Exception(error.Message);
            }
            else if (restResponse.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(restResponse.ErrorMessage);
            }

            return restResponse.Data;
        }

        public void AddFile(Guid transactionId, byte[] file)
        {
            var webRequest = WebRequest.Create(String.Format(@"{0}/{1}/{2}",_restClient.BaseUrl, "file", transactionId));
            webRequest.Method = "PUT";
            webRequest.Headers.Add("Application", ApplicationHeader);
            webRequest.Headers.Add("Authorization", AuthorizationHeader);
            webRequest.ContentType = "application/pdf";

            using (var requestStream = webRequest.GetRequestStream())
            {
                requestStream.Write(file, 0, file.Length);
            }

            using (var response = (HttpWebResponse)webRequest.GetResponse())
            {
                if (response.StatusCode == HttpStatusCode.OK) return;

                var message = "An error has occurred.";
                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                    {
                        using (var reader = new StreamReader(responseStream))
                        {
                            message = reader.ReadToEnd();
                        }
                    }
                }

                throw new Exception(message);
            }
        }
    }
}
