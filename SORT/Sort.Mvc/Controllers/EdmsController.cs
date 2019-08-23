using System.Configuration;
using System.IO;
using System.Web.Mvc;
using GemBox.Document;
using Sort.Business;

namespace Sort.Mvc.Controllers
{
    [Authorize]
    public class EdmsController : Controller
    {
        /// <summary>
        /// Return the Osti generated pdfa document for a Sort Artifact
        /// </summary>
        /// <param name="id">Sort Main Id</param>
        /// <returns></returns>
        [AllowAnonymous]
        public FileResult GetOstiDocument(int? id)
        {
            if (id.HasValue)
            {
                var file = SortAttachmentObject.GetOstiAttachment(id.Value);
                if (file != null)
                {
                    return File(file.Contents, System.Net.Mime.MediaTypeNames.Application.Octet, file.FileName);
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the Final document for a Sort Artifact
        /// </summary>
        /// <param name="id">Sort Main Id</param>
        /// <returns></returns>
        [AllowAnonymous]
        public FileResult GetFinalDocument(int? id)
        {
            if (id.HasValue)
            {
                var file = SortAttachmentObject.GetFinalDocAttachment(id.Value);
                if (file != null)
                {
                    return File(file.Contents, System.Net.Mime.MediaTypeNames.Application.Octet, file.FileName);
                }
            }

            return null;
        }

        /// <summary>
        /// Return the Cover Page for a Sort Artifact
        /// </summary>
        /// <param name="id">Sort Main ID</param>
        /// <returns></returns>
        [AllowAnonymous]
        public FileResult GetCoverPage(int? id)
        {
            if (id.HasValue)
            {
                try
                {
                    GemBox.Document.ComponentInfo.SetLicense(ConfigurationManager.AppSettings["GemBoxLicense"]);
                    Stream cover = PdfCoverPage.GenerateCover(id.Value);
                    DocumentModel doc = DocumentModel.Load(cover, LoadOptions.DocxDefault);
                    Stream output = new MemoryStream();
                    doc.Save(output, SaveOptions.PdfDefault);
                    return File(output, System.Net.Mime.MediaTypeNames.Application.Octet, $"{id}_Cover.pdf");
                }
                catch { }
            }

            return null;
        }

        public ActionResult LrsHome()
        {
            return Redirect(Config.LrsUrl);
        }
    }
}