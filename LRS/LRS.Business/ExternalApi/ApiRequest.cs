using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace LRS.Business
{
    public class ApiRequest
    {
        public static ApiResponse GetIpdsInfo(string idrNumber)
        {
            ApiResponse rtn = new ApiResponse();
            try
            {
                using (var client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true }))
                {
                    //Post for new ID
                    client.BaseAddress = new Uri(Config.IpdsUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = client.GetAsync($"api/IPDSApi/GetDocumentInfo/{idrNumber}").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var formatters = new List<MediaTypeFormatter>()
                        {
                            new JsonMediaTypeFormatter(),
                            new XmlMediaTypeFormatter()
                        };

                        rtn.IdpsResponse = JsonConvert.DeserializeObject<IpdsResponse>(response.Content.ReadAsAsync<string>(formatters).Result);

                        if (rtn.IdpsResponse != null)
                        {
                            rtn.IdpsResponse.SetupOrcidIds();
                            rtn.Success = true;
                        }
                        else
                        {
                            rtn.Success = false;
                            rtn.ErrorMessage = "No Response was returned from the API.";
                        }
                    }
                    else
                    {
                        rtn.Success = false;
                        rtn.ErrorMessage = $"Status Returned from API was : {response.StatusCode}";
                    }
                }
            }
            catch (Exception ex)
            {
                rtn.Success = false;
                rtn.ErrorMessage = ex.Message;
                rtn.StackTrace = ex.StackTrace;
            }

            return rtn;
        }

        public static ApiResponse ValidateLoiessTrackingNumber(string trackingNumber) //16-050
        {
            ApiResponse rtn = new ApiResponse();
            try
            {
                using (var client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true }))
                {
                    //Post for new ID
                    client.BaseAddress = new Uri(Config.LoiessUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = client.GetAsync($"api/LoiEssApi/ValidateTrackingNumber/{trackingNumber}").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var formatters = new List<MediaTypeFormatter>()
                        {
                            new JsonMediaTypeFormatter(),
                            new XmlMediaTypeFormatter()
                        };

                        rtn.LoiessResponse = JsonConvert.DeserializeObject<LoiessResponse>(response.Content.ReadAsAsync<string>(formatters).Result);

                        if (rtn.LoiessResponse != null)
                        {
                            rtn.Success = true;
                        }
                        else
                        {
                            rtn.Success = false;
                            rtn.ErrorMessage = "No Response was returned from the API.";
                        }
                    }
                    else
                    {
                        rtn.Success = false;
                        rtn.ErrorMessage = $"Status Returned from API was : {response.StatusCode}";
                    }
                }
            }
            catch (Exception ex)
            {
                rtn.Success = false;
                rtn.ErrorMessage = ex.Message;
                rtn.StackTrace = ex.StackTrace;
            }

            return rtn;
        }

        public static ExportResponse UploadArtifactToSort(ArtifactData data)
        {
            ExportResponse rtn = new ExportResponse();
            try
            {
                using (var client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true }))
                {
                    client.BaseAddress = new Uri(Config.SortUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = client.PostAsJsonAsync("api/Artifact/", data).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var formatters = new List<MediaTypeFormatter>()
                        {
                            new JsonMediaTypeFormatter(),
                            new XmlMediaTypeFormatter()
                        };

                        rtn.SortId = response.Content.ReadAsAsync<int?>(formatters).Result;
                        rtn.Success = true;
                    }
                    else
                    {
                        rtn.Success = false;
                        rtn.ErrorMessage = $"Status Returned from API was : {response.StatusCode}";
                    }
                }
            }
            catch (Exception ex)
            {
                rtn.Success = false;
                rtn.ErrorMessage = ex.Message;
                rtn.StackTrace = ex.StackTrace;
            }

            return rtn;
        }


        public static PingResponse PingSort()
        {
            PingResponse rtn = new PingResponse();
            try
            {
                using (var client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true }))
                {
                    client.BaseAddress = new Uri(Config.SortUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = client.PostAsync("api/Ping", null).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        rtn.StatusCode = response.StatusCode.ToString();
                        rtn.ReasonPhrase = response.ReasonPhrase;
                        rtn.RequestMessage = response.RequestMessage.ToString();
                        rtn.Headers = response.Headers.ToString();
                        rtn.Success = true;
                    }
                    else
                    {
                        rtn.StatusCode = response.StatusCode.ToString();
                        rtn.ReasonPhrase = response.ReasonPhrase;
                        rtn.RequestMessage = response.RequestMessage.ToString();
                        rtn.Headers = response.Headers.ToString();
                        rtn.Success = false;
                        rtn.ErrorMessage = $"Status Returned from API was : {response.StatusCode}";
                    }
                }
            }
            catch (Exception ex)
            {
                rtn.Success = false;
                rtn.ErrorMessage = ex.Message;
                rtn.StackTrace = ex.StackTrace;
            }

            return rtn;
        }

        public static ExportResponse RemoveArtifactFromSort(int sortId)
        {
            ExportResponse rtn = new ExportResponse();
            try
            {
                using (var client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true }))
                {
                    //Post for new ID
                    client.BaseAddress = new Uri(Config.SortUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = client.DeleteAsync("api/Artifact?id=" + sortId).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        rtn.Success = true;
                    }
                    else
                    {
                        rtn.Success = false;
                        rtn.ErrorMessage = $"Status Returned from API was : {response.StatusCode}";
                    }
                }

            }
            catch (Exception ex)
            {
                rtn.Success = false;
                rtn.ErrorMessage = ex.Message;
                rtn.StackTrace = ex.StackTrace;
            }

            return rtn;
        }
    }

    public class IpdsResponse
    {
        public string DocketNumber { get; set; }
        public string SnumberAty { get; set; }
        public string SnumberAe { get; set; }
        public string Title { get; set; }

        public string OrcidIdAty { get; set; }

        public string OrcidIdAe { get; set; }

        public void SetupOrcidIds()
        {
            var sp = EmployeeCache.GetEmployee(SnumberAty.PadLeft(6, '0'));
            if (sp != null)
            {
                OrcidIdAty = sp.OrcidId;
            }

            sp = EmployeeCache.GetEmployee(SnumberAe.PadLeft(6, '0'));
            if (sp != null)
            {
                OrcidIdAe = sp.OrcidId;
            }
        }
    }

    public class LoiessResponse
    {
        public string PiSnumber { get; set; }
        public string ProjectNum { get; set; }
        public bool IsValid => !string.IsNullOrWhiteSpace(PiSnumber) && !string.IsNullOrWhiteSpace(ProjectNum);
    }

    public class ApiResponse
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string StackTrace { get; set; }
        public IpdsResponse IdpsResponse { get; set; }
        public LoiessResponse LoiessResponse { get; set; }
    }

    public class PingResponse
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string StackTrace { get; set; }
        public string StatusCode { get; set; }
        public string ReasonPhrase { get; set; }
        public string RequestMessage { get; set; }
        public string Headers { get; set; }
    }

    public class ExportResponse
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string StackTrace { get; set; }
        public int? SortId { get; set; }
    }
}
