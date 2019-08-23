using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Runtime.Serialization;

namespace Sort.Business
{
    [DataContract(Namespace = "Stims")]
    public class StimsData
    {
        #region Properties
        [DataMember] public int SourceId { get; set; }
        [DataMember] public string TrackingNumber { get; set; }
        [DataMember] public string StimsNumber { get; set; }
        [DataMember] public string StimsType { get; set; }
        [DataMember] public string Title { get; set; }
        [DataMember] public string JournalName { get; set; }
        [DataMember] public DateTime? PublicationDate { get; set; }
        [DataMember] public string JournalVolume { get; set; }
        [DataMember] public string JournalNumber { get; set; }
        [DataMember] public string OstiId { get; set; }
        [DataMember] public DateTime OstiSaveDate { get; set; }
        [DataMember] public string DoiNum { get; set; }
        [DataMember] public string AuthorNames { get; set; }
        [DataMember] public string FirstInlAuthor { get; set; }
        #endregion

        #region Constructor

        public StimsData()
        {
        }

        #endregion

        public static void SendToLoiess(SortMainObject sort)
        {
            var funding = sort.Funding.Where(n => n.FundingTypeId == 2 && !string.IsNullOrWhiteSpace(n.TrackingNumber)).ToList();
            if (funding != null && funding.Count > 0)
            {
                foreach (var f in funding)
                {
                    var stims = new StimsData();
                    stims.SourceId = sort.SortMainId.Value;
                    stims.TrackingNumber = f.TrackingNumber;
                    stims.StimsNumber = sort.TitleStr;
                    stims.StimsType = sort.ProductTypeDisplayName;
                    stims.Title = sort.PublishTitle;
                    stims.JournalName = sort.JournalName;
                    stims.PublicationDate = sort.PublicationDate;
                    stims.JournalVolume = sort.JournalVolume;
                    stims.JournalNumber = sort.JournalSerial;
                    stims.OstiId = sort.OstiId;
                    stims.OstiSaveDate = sort.OstiDate.Value;
                    stims.DoiNum = sort.DoiNum;
                    stims.AuthorNames = string.Join("|", sort.Authors.Select(n => n.FullName));
                    stims.FirstInlAuthor = sort.Authors.FirstOrDefault(n => n.IsPrimary)?.FullName;

                    UploadDataToLoiEss(stims);
                }
            }
        }

        public static ExportResponse UploadDataToLoiEss(StimsData data)
        {
            ExportResponse rtn = new ExportResponse();
            try
            {
                using (var client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true }))
                {
                    client.BaseAddress = new Uri(Config.LoiessUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = client.PostAsJsonAsync("api/Stims/", data).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var formatters = new List<MediaTypeFormatter>()
                        {
                            new JsonMediaTypeFormatter(),
                            new XmlMediaTypeFormatter()
                        };

                        rtn.Success = response.Content.ReadAsAsync<bool>(formatters).Result;
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

    public class ExportResponse
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string StackTrace { get; set; }
    }
}
