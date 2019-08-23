using Dapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using FluentFTP;

namespace Sort.Business
{
    public class OstiUploadObject
    {
        #region Properties

        public int OstiUploadId { get; set; }
        public int SortMainId { get; set; }
        public DateTime RecordDate { get; set; }
        public string RequestXml { get; set; }
        public string Response { get; set; }
        public string Status { get; set; }

        #endregion

        #region Constructor

        public OstiUploadObject() { }

        #endregion

        #region Repository

        private static IOstiUploadRepository repo => new OstiUploadRepository();

        #endregion

        #region Static Methods

        public static List<OstiUploadObject> GetOstiUploads(DateTime startDate) => repo.GetOstiUploads(startDate);

        public static List<OstiUploadObject> GetOstiUploads(int sortMainId, DateTime startDate) => repo.GetOstiUploads(sortMainId, startDate);

        public static OstiUploadObject GetOstiUpload(int ostiUploadId) => repo.GetOstiUpload(ostiUploadId);

        public static void AddLogRecord(int sortMainId, string xml, string response, string status)
        {
            try
            {
                OstiUploadObject o = new OstiUploadObject();
                o.SortMainId = sortMainId;
                o.RequestXml = xml;
                o.Response = response;
                o.Status = status;
                o.RecordDate = DateTime.Now;
                o.Save();
            }
            catch (Exception ex)
            {
                ErrorLogObject.LogError("OstiUploadObject::AddLogRecord", ex);
            }
        }

        public static bool UploadToOsti(SortMainObject sort)
        {
            string xml = string.Empty;
            try
            {
                // upload to OSTI
                HttpWebRequest webreq = null;
                HttpWebResponse webres = null;
                string res = string.Empty;
                xml = sort.GetXml();

                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                webreq = WebRequest.Create(Config.OstiUrl) as HttpWebRequest;
                webreq.Credentials = new NetworkCredential(Config.OstiUserName, Config.OstiPassword);
                webreq.PreAuthenticate = true;

                webreq.ContentType = "application/xml";
                webreq.Method = "POST";
                webreq.Timeout = 10000000;

                Stream postData = null; 
                byte[] buffer = Encoding.UTF8.GetBytes(xml);
                try
                {
                    postData = webreq.GetRequestStream();
                }
                catch (Exception ex)
                {
                    ErrorLogObject.LogError("OstiUploadObject::UploadToOsti1", ex);
                    sort.OstiStatusMsg = $"Exception Caught while connecting to OSTI API: {ex.Message}";
                    AddLogRecord(sort.SortMainId.Value, xml, sort.OstiStatusMsg, "FAILED - Exception Caught while connecting to OSTI API");
                    return false;
                }

                if (postData != null && buffer != null)
                {
                    postData.Write(buffer, 0, buffer.Length);
                    postData.Close();
                }
                
                WebResponse xres = null;
                try
                {
                    xres = webreq.GetResponse();
                }
                catch (WebException ex)
                {
                    ErrorLogObject.LogError("OstiUploadObject::UploadToOsti", ex);
                    if (ex.Response != null)
                    {
                        using (var stream = ex.Response.GetResponseStream())
                        using (var reader = new StreamReader(stream))
                        {
                            res = reader.ReadToEnd();
                        }
                    }
                    else
                    {
                        sort.OstiStatusMsg = $"Error returned:{ex.Message} => Could be that the password for OSTI site has expired.";
                        AddLogRecord(sort.SortMainId.Value, xml, sort.OstiStatusMsg, "FAILED - webreq.GetResponse");
                        return false;
                    }
                }

                if (xres is HttpWebResponse)
                {
                    if (((HttpWebResponse)xres).StatusCode != HttpStatusCode.OK)
                    {
                        sort.OstiStatusMsg = "Status Code returned was: " + ((HttpWebResponse)xres).StatusDescription;
                        AddLogRecord(sort.SortMainId.Value, xml, sort.OstiStatusMsg, "FAILED - StatusCode");
                        return false;
                    }

                    using (webres = xres as HttpWebResponse)
                    {
                        StreamReader reader = new StreamReader(webres.GetResponseStream());
                        res = reader.ReadToEnd();
                    }
                    xres.Close();

                }

                if (!string.IsNullOrWhiteSpace(res))
                {
                    try
                    {
                        OstiRecords record = null;
                        if (!string.IsNullOrWhiteSpace(res))
                        {
                            using (TextReader reader = new StringReader(res))
                            {
                                XmlSerializer serializer = new XmlSerializer(typeof(OstiRecords));
                                record = (OstiRecords)serializer.Deserialize(reader);
                            }
                        }

                        if (record != null && record.Record != null && record.Record.Length > 0)
                        {
                            OstiRecord rec = record.Record[0];
                            sort.OstiId = rec.Id.HasValue ? (rec.Id.Value > 0 ? rec.Id.Value.ToString() : sort.OstiId) : sort.OstiId;
                            sort.OstiStatus = record.Record[0].Status;
                            sort.OstiStatusMsg = record.Record[0].StatusMessage;
                            sort.OstiDate = DateTime.Now;
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorLogObject.LogError("OstiUploadObject::UploadToOsti2", ex);
                        using (XmlReader rdr = XmlReader.Create(new StringReader(res)))
                        {
                            rdr.ReadToFollowing(Extensions.GetPropertyShortName(() => sort.OstiId));
                            sort.OstiId = rdr.ReadElementContentAsString();

                            rdr.ReadToFollowing(Extensions.GetPropertyShortName(() => sort.OstiStatus));
                            sort.OstiStatus = rdr.ReadElementContentAsString();

                            rdr.ReadToFollowing(Extensions.GetPropertyShortName(() => sort.OstiStatusMsg));
                            if (rdr.NodeType == XmlNodeType.Element)
                            {
                                sort.OstiStatusMsg = rdr.ReadElementContentAsString();
                            }

                            sort.OstiDate = DateTime.Now;
                        }
                    }

                    AddLogRecord(sort.SortMainId.Value, xml, res, sort.OstiStatus);

                    if (!string.IsNullOrWhiteSpace(sort.OstiId) && sort.OstiStatus.Equals("success", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!sort.PublicRelease)
                        {
                            string error = string.Empty;
                            if (!UploadFilesFtpS(sort.SortMainId.Value, sort.OstiId, ref error))
                            {
                                sort.OstiStatusMsg = $"FAILED - MetaData Uploaded, but FTPS Failed to upload the document to OSTI: {error}";
                                sort.OstiStatus = "FAILED";
                                AddLogRecord(sort.SortMainId.Value, xml, sort.OstiStatusMsg, "FAILED - FTPS Exception Caught");
                                return false;
                            }
                        }
                        else
                        {
                            // Update Digital Library for file that was uploaded to them.
                            string jsonStr = Config.DigitalLibraryManager.GetDigitalLibraryJson(sort);
                            Config.DigitalLibraryManager.ReportData(sort.StiSpId.Value, jsonStr);
                        }

                        //Report all LDRD Funding to LOI-ESS
                        if (sort.SharePointId.HasValue) // SharePoint ID is also the LRS ID
                        {
                            try
                            {
                                StimsData.SendToLoiess(sort);
                            }
                            catch (Exception ex)
                            {
                                ErrorLogObject.LogError("OstiUploadObject::UploadToOsti3", ex);
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    sort.OstiStatusMsg = "No data returned from OSTI, Unable to complete the transfer.";
                    AddLogRecord(sort.SortMainId.Value, xml, sort.OstiStatusMsg, "FAILED - Return Data");
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorLogObject.LogError("OstiUploadObject::UploadToOsti4", ex);
                sort.OstiStatusMsg = ex.Message;
                AddLogRecord(sort.SortMainId.Value, xml, sort.OstiStatusMsg, "FAILED - Exception Caught");
                return false;
            }

            return true;
        }

        private static bool UploadFilesFtpS(int sortMainId, string ostiId, ref string error)
        {
            bool uploaded = false;
            //// https://github.com/hgupta9/FluentFTP
            try
            {
                var file = SortAttachmentObject.GetOstiAttachment(sortMainId);
                if (file != null)
                {
                    using (FtpClient client = new FtpClient(Config.OstiFtpSite, Config.OstiFtpPort, Config.OstiFtpUsername, Config.OstiFtpPassword))
                    {
                        client.EncryptionMode = FtpEncryptionMode.Explicit;
                        client.SslProtocols = SslProtocols.Tls12;
                        client.ValidateCertificate += new FtpSslValidation(OnValidateCertificate);
                        client.Connect();

                        if (client.IsConnected)
                        {
                            string fileName = $"{ostiId}{Path.GetExtension(file.FileName)}";
                            Stream stream = file.Contents;
                            stream.Seek(0, SeekOrigin.Begin);

                            uploaded = client.Upload(stream, $"{Config.OstiFtpDirectory}{fileName}");

                            client.Disconnect();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogObject.LogError("OstiUploadObject::UploadFilesFtpS", ex);
                error = ex.Message;
            }

            return uploaded;
        }

        private static void OnValidateCertificate(FtpClient control, FtpSslValidationEventArgs e)
        {
            e.Accept = true;
        }
        #endregion

        #region Object Methods

        public void Save()
        {
            repo.SaveOstiUpload(this);
        }
        #endregion
    }

    public interface IOstiUploadRepository
    {
        List<OstiUploadObject> GetOstiUploads(DateTime startDate);
        List<OstiUploadObject> GetOstiUploads(int sortMainId, DateTime startDate);
        OstiUploadObject GetOstiUpload(int ostiUploadId);
        OstiUploadObject SaveOstiUpload(OstiUploadObject ostiUpload);
    }

    public class OstiUploadRepository : IOstiUploadRepository
    {
        public List<OstiUploadObject> GetOstiUploads(DateTime startDate) => Config.Conn.Query<OstiUploadObject>("SELECT * FROM dat_OstiUpload WHERE RecordDate > @StartDate", new { StartDate = startDate }).ToList();

        public List<OstiUploadObject> GetOstiUploads(int sortMainId, DateTime startDate) => Config.Conn.Query<OstiUploadObject>("SELECT * FROM dat_OstiUpload WHERE SortMainId = @SortMainId AND RecordDate > @StartDate", new { SortMainId = sortMainId, StartDate = startDate }).ToList();

        public OstiUploadObject GetOstiUpload(int ostiUploadId) => Config.Conn.Query<OstiUploadObject>("SELECT * FROM dat_OstiUpload WHERE OstiUploadId = @OstiUploadId", new { OstiUploadId = ostiUploadId }).FirstOrDefault();

        public OstiUploadObject SaveOstiUpload(OstiUploadObject ostiUpload)
        {
            // Only save new records.
            if (ostiUpload.OstiUploadId == 0)
            {
                string sql = @"
                    INSERT INTO dat_OstiUpload (
                        SortMainId,
                        RecordDate,
                        RequestXml,
                        Response,
                        Status
                    )
                    VALUES (
                        @SortMainId,
                        @RecordDate,
                        @RequestXml,
                        @Response,
                        @Status
                    )
                    SELECT CAST(SCOPE_IDENTITY() AS INT)";
                ostiUpload.OstiUploadId = Config.Conn.Query<int>(sql, ostiUpload).Single();
            }
            return ostiUpload;
        }
        
    }
}
