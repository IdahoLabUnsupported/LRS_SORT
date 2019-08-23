using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Mvc;
using Sort.Business;
using Sort.Mvc.Models;

namespace Sort.Mvc.Controllers
{
    [Authorize]
    public class ArtifactController : Controller
    {
        public ActionResult Index(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new ArtifactModel(id.Value, string.Empty);
            
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Sort Main Id</param>
        /// <param name="st">Selected Tab</param>
        /// <returns></returns>
        public ActionResult Edit(int? id, string st)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new ArtifactModel(id.Value, st);
            if (!model.UserHasWriteAccess)
            {
                return RedirectToAction("Index", "Artifact", new {id});
            }

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Sort Main Id</param>
        /// <param name="alt">Return Location</param>
        /// <returns></returns>
        public ActionResult SetDueDate(int? id, bool? alt)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new DueDateModel(id.Value, alt);
            if (!model.UserHasAccess())
            {
                if (alt.HasValue && alt.Value)
                {
                    return RedirectToAction("Index", "Artifact", new {id});
                }
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SetDueDate(DueDateModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.Save();
            if (model.AltLocation.HasValue && model.AltLocation.Value)
            {
                return RedirectToAction("Index", "Artifact", new { id = model.SortMainId });
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]/*, ValidateAntiForgeryToken]*/
        public ActionResult AddContact(ContactModel model)
        {
            if (ModelState.IsValid && SortMainObject.CheckUserHasWriteAccess(model.SortMainId))
            {
                model.Save();

                return PartialView("Partials/_contactList", ContactObject.GetContacts(model.SortMainId));
            }

            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return null;
        }

        [HttpPost]
        public ActionResult GetContactPartial(int? id)
        {
            if (id.HasValue)
            {
                var contact = ContactObject.GetContact(id.Value);
                if (contact != null)
                {
                    return PartialView("Partials/ContactsPartial", new ContactModel(contact));
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">ContactId</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RemoveContact(int? id)
        {
            int sid = 0;
            if (id.HasValue)
            {
                var contact = ContactObject.GetContact(id.Value);
                if (contact != null)
                {
                    sid = contact.SortMainId;
                    if (SortMainObject.CheckUserHasWriteAccess(sid))
                    {
                        contact.Delete();
                    }
                }
            }

            return PartialView("Partials/_contactList", ContactObject.GetContacts(sid));
        }

        [HttpPost]/*, ValidateAntiForgeryToken]*/
        public ActionResult AddAuthor(AuthorModel model)
        {
            if (ModelState.IsValid && SortMainObject.CheckUserHasWriteAccess(model.SortMainId))
            {
                model.Save();

                return PartialView("Partials/_authorList", AuthorObject.GetAuthors(model.SortMainId));
            }

            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return null;
        }

        [HttpPost]
        public ActionResult GetAuthorPartial(int? id)
        {
            if (id.HasValue)
            {
                var author = AuthorObject.GetAuthor(id.Value);
                if (author != null)
                {
                    return PartialView("Partials/AuthorsPartial", new AuthorModel(author));
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">AuthorId</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RemoveAuthor(int? id)
        {
            int sid = 0;
            if (id.HasValue)
            {
                var author = AuthorObject.GetAuthor(id.Value);
                if (author != null)
                {
                    sid = author.SortMainId;
                    if (SortMainObject.CheckUserHasWriteAccess(sid))
                    {
                        author.Delete();
                    }
                }
            }

            return PartialView("Partials/_authorList", AuthorObject.GetAuthors(sid));
        }

        /// <summary>
        /// Set an author as the primary author
        /// </summary>
        /// <param name="id">Author ID</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SetPrimaryAuthor(int? id)
        {
            int sid = 0;
            if (id.HasValue)
            {
                var author = AuthorObject.GetAuthor(id.Value);
                if (author != null)
                {
                    sid = author.SortMainId;
                    if (SortMainObject.CheckUserHasWriteAccess(sid))
                    {
                        author.SetAsPrimary();
                    }
                }
            }

            return PartialView("Partials/_authorList", AuthorObject.GetAuthors(sid));
        }

        [HttpPost]/*, ValidateAntiForgeryToken]*/
        public ActionResult AddFunding(FundingModel model)
        {
            if (ModelState.IsValid && SortMainObject.CheckUserHasWriteAccess(model.SortMainId))
            {
                model.Save();

                return PartialView("Partials/_fundingList", FundingObject.GetFundings(model.SortMainId));
            }

            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return null;
        }

        [HttpPost]
        public ActionResult GetFundingPartial(int? id)
        {
            if (id.HasValue)
            {
                var funding = FundingObject.GetFunding(id.Value);
                if (funding != null)
                {
                    return PartialView("Partials/FundingPartial", new FundingModel(funding));
                }
            }

            return null;
        }

        [HttpPost]
        public ActionResult GetFundingInfoPartial(int? id)
        {
            if (!id.HasValue)
            {
                return null;
            }

            var model = FundingObject.GetFunding(id.Value);

            return PartialView("Partials/_fundingInfo", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">FundingId</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RemoveFunding(int? id)
        {
            int sid = 0;
            if (id.HasValue)
            {
                var funding = FundingObject.GetFunding(id.Value);
                if (funding != null)
                {
                    sid = funding.SortMainId;
                    if (SortMainObject.CheckUserHasWriteAccess(sid))
                    {
                        funding.Delete();
                    }
                }
            }

            return PartialView("Partials/_fundingList", FundingObject.GetFundings(sid));
        }

        [HttpPost]/*, ValidateAntiForgeryToken]*/
        public ActionResult AddSubject(MetadataModel model)
        {
            if (ModelState.IsValid && SortMainObject.CheckUserHasWriteAccess(model.SortMainId))
            {
                model.Save();

                return PartialView("Partials/_subjectList", SortMetaDataObject.GetSortMetaDatas(model.SortMainId, MetaDataTypeEnum.SubjectCategories));
            }

            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">MetaDataId</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RemoveSubject(int? id)
        {
            int sid = 0;
            if (id.HasValue)
            {
                var meta = SortMetaDataObject.GetSortMetaData(id.Value);
                if (meta != null)
                {
                    sid = meta.SortMainId;
                    if (SortMainObject.CheckUserHasWriteAccess(sid))
                    {
                        meta.Delete();
                    }
                }
            }

            return PartialView("Partials/_subjectList", SortMetaDataObject.GetSortMetaDatas(sid, MetaDataTypeEnum.SubjectCategories));
        }

        #region Core Capabilities

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Main ID</param>
        /// <returns></returns>
        public ActionResult CoreCapabilitiesPartial(int? id)
        {
            var model = new MetadataModel();
            if (id.HasValue)
            {
                model = new MetadataModel(id.Value, MetaDataTypeEnum.CoreCapabilities);
            }

            return PartialView("Partials/CoreCapabilitiesPartial", model);
        }

        [HttpPost]/*, ValidateAntiForgeryToken]*/
        public ActionResult AddCoreCapabilities(MetadataModel model)
        {
            if (ModelState.IsValid && SortMainObject.CheckUserHasWriteAccess(model.SortMainId))
            {
                model.Save();

                return PartialView("Partials/_coreCapabilitiesList", SortMetaDataObject.GetSortMetaDatas(model.SortMainId, MetaDataTypeEnum.CoreCapabilities));
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">MetaDataId</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RemoveCoreCapabilities(int? id)
        {
            int mid = 0;
            if (id.HasValue)
            {
                var meta = SortMetaDataObject.GetSortMetaData(id.Value);
                if (meta != null)
                {
                    mid = meta.SortMainId;
                    if (SortMainObject.CheckUserHasWriteAccess(mid))
                    {
                        meta.Delete();
                    }
                }
            }

            return PartialView("Partials/_coreCapabilitiesList", SortMetaDataObject.GetSortMetaDatas(mid, MetaDataTypeEnum.CoreCapabilities));
        }

        #endregion

        [HttpPost]/*, ValidateAntiForgeryToken]*/
        public ActionResult AddKeyword(MetadataModel model)
        {
            if (ModelState.IsValid && SortMainObject.CheckUserHasWriteAccess(model.SortMainId))
            {
                model.Save();

                return PartialView("Partials/_keywordsList", SortMetaDataObject.GetSortMetaDatas(model.SortMainId, MetaDataTypeEnum.Keywords));
            }

            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">MetadataId</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RemoveKeyword(int? id)
        {
            int sid = 0;
            if (id.HasValue)
            {
                var meta = SortMetaDataObject.GetSortMetaData(id.Value);
                if (meta != null)
                {
                    sid = meta.SortMainId;
                    if (SortMainObject.CheckUserHasWriteAccess(sid))
                    {
                        meta.Delete();
                    }
                }
            }

            return PartialView("Partials/_keywordsList", SortMetaDataObject.GetSortMetaDatas(sid, MetaDataTypeEnum.Keywords));
        }

        [HttpPost]/*, ValidateAntiForgeryToken]*/
        public ActionResult AddSponsor(MetadataModel model)
        {
            if (ModelState.IsValid && SortMainObject.CheckUserHasWriteAccess(model.SortMainId))
            {
                model.Save();

                return PartialView("Partials/_sponsorOrgList", SortMetaDataObject.GetSortMetaDatas(model.SortMainId, MetaDataTypeEnum.SponsoringOrgs));
            }

            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">MetadataId</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RemoveSponsor(int? id)
        {
            int sid = 0;
            if (id.HasValue)
            {
                var meta = SortMetaDataObject.GetSortMetaData(id.Value);
                if (meta != null)
                {
                    sid = meta.SortMainId;
                    if (SortMainObject.CheckUserHasWriteAccess(sid))
                    {
                        meta.Delete();
                    }
                }
            }

            return PartialView("Partials/_sponsorOrgList", SortMetaDataObject.GetSortMetaDatas(sid, MetaDataTypeEnum.SponsoringOrgs));
        }

        [HttpPost]/*, ValidateAntiForgeryToken]*/
        public ActionResult OfficialUseSave(OfficialUseModel model)
        {
            if (ModelState.IsValid && SortMainObject.CheckUserHasWriteAccess(model.SortMainId))
            {
                model.Save();
            }

            return null;
        }

        [HttpPost]/*, ValidateAntiForgeryToken]*/
        public ActionResult TitleSave(TitleModel model)
        {
            if (ModelState.IsValid && SortMainObject.CheckUserHasWriteAccess(model.SortMainId))
            {
                model.Save();
            }

            return null;
        }

        [HttpPost]/*, ValidateAntiForgeryToken]*/
        public ActionResult JorunalSave(JournalModel model)
        {
            if (ModelState.IsValid && SortMainObject.CheckUserHasWriteAccess(model.SortMainId))
            {
                model.Save();
            }

            return null;
        }

        [HttpPost]/*, ValidateAntiForgeryToken]*/
        public ActionResult PublisherSave(PublisherModel model)
        {
            if (ModelState.IsValid && SortMainObject.CheckUserHasWriteAccess(model.SortMainId))
            {
                model.Save();
            }

            return null;
        }

        [HttpPost]/*, ValidateAntiForgeryToken]*/
        public ActionResult ProtectedDataSave(ProtectedDataModel model)
        {
            if (ModelState.IsValid && SortMainObject.CheckUserHasWriteAccess(model.SortMainId))
            {
                model.Save();
            }

            return null;
        }

        [HttpPost]/*, ValidateAntiForgeryToken]*/
        public ActionResult PatentSave(PatentModel model)
        {
            if (ModelState.IsValid && SortMainObject.CheckUserHasWriteAccess(model.SortMainId))
            {
                model.Save();
            }

            return null;
        }

        [HttpPost]/*, ValidateAntiForgeryToken]*/
        public ActionResult OpenNetSave(OpenNetModel model)
        {
            if (ModelState.IsValid && SortMainObject.CheckUserHasWriteAccess(model.SortMainId))
            {
                model.Save();
            }

            return null;
        }

        [HttpPost]/*, ValidateAntiForgeryToken]*/
        public ActionResult ConferenceSave(ConferenceModel model)
        {
            if (ModelState.IsValid && SortMainObject.CheckUserHasWriteAccess(model.SortMainId))
            {
                model.Save();
            }

            return null;
        }

        [HttpPost]/*, ValidateAntiForgeryToken]*/
        public ActionResult AbstractSave(AbstractModel model)
        {
            if (ModelState.IsValid && SortMainObject.CheckUserHasWriteAccess(model.SortMainId))
            {
                model.Save();
            }

            return null;
        }

        #region Attachments
        //[HttpPost]/*, ValidateAntiForgeryToken]*/
        //public ActionResult UploadAttachments(AttachmentModel model)
        //{
        //    if (ModelState.IsValid && SortMainObject.CheckUserHasAccess(model.SortMainId))
        //    {
        //        if (model.DuplicateFinal())
        //        {
        //            TempData["InvalidFile"] = "There can only be one Final Document attached. Delete the old one to replace it.";
        //        }
        //        else if (!model.Save())
        //        {
        //            TempData["InvalidFile"] = "This is not an acceptable file type";
        //        }
        //    }
        //    else
        //    {
        //        TempData["InvalidFile"] = "Unable to Save the Attachment.  Required fields missing or Invalid.";
        //    }

        //    return RedirectToAction("Edit", new { id = model.SortMainId, st = "attachmentsTab" });
        //}

        [HttpPost]/*, ValidateAntiForgeryToken]*/
        public JsonResult AddAttachment(AttachmentModel model)
        {
            if (ModelState.IsValid && SortMainObject.CheckUserHasWriteAccess(model.SortMainId))
            {
                model.Save();

                return Json(model.SortAttachmentId);
            }

            return null;
        }

        /// <summary>
        /// Multi part upload. This is the piece that is called when a part of a file is being uploaded.
        /// </summary>
        /// <param name="id">Attachment Id</param>
        /// <param name="partNum">part of file</param>
        /// <returns></returns>
        [HttpPost]
        public string AttachmentMultiUpload(int id, int partNum)
        {
            var attachment = SortAttachmentObject.GetSortAttachment(id);
            if (attachment != null)
            {
                MemoryStream ms = new MemoryStream();
                Request.InputStream.CopyTo(ms);
                attachment.AddPart(partNum, ms.ToArray());
            }
            return "done";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Sort Attachment Id</param>
        /// <returns></returns>
        public FileResult DownloadAttachment(int? id)
        {
            if (id.HasValue)
            {
                SortAttachmentObject aa = SortAttachmentObject.GetSortAttachment(id.Value);
                if (aa != null && SortMainObject.CheckUserHasReadAccess(aa.SortMainId))
                {
                    var content = aa.Contents;
                    if (content != null)
                    {
                        content.Seek(0, SeekOrigin.Begin);
                        // Need this turned off for large files
                        Response.BufferOutput = false;
                        return File(content, System.Net.Mime.MediaTypeNames.Application.Octet, aa.FileName);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Sort Attachment Id</param>
        /// <returns></returns>
        public ActionResult DeleteAttachment(int? id)
        {
            if (id.HasValue)
            {
                SortAttachmentObject asa = SortAttachmentObject.GetSortAttachment(id.Value);
                asa?.Delete();

                return GetAttachments(asa?.SortMainId);
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Sort Main Id</param>
        /// <returns></returns>
        public ActionResult GetAttachments(int? id)
        {
            var model = new List<SortAttachmentObject>();
            if (id.HasValue && SortMainObject.CheckUserHasReadAccess(id.Value))
            {
                model = SortAttachmentObject.GetSortAttachments(id.Value);
            }

            return PartialView("Partials/_attachmentList", model);
        }

        #endregion

        [HttpPost]
        public ActionResult PublishToOsti(int? id, bool? pr)
        {
            int result = -1;
            string ostiId = null;
            string message = string.Empty;
            string status = string.Empty;
            DateTime? ostiDate = null;

            if (id.HasValue && pr.HasValue)
            {
                var sort = SortMainObject.GetSortMain(id.Value);
                if (sort != null && sort.UserHasWriteAccess())
                {
                    if (sort.UploadToOstiServer(pr.Value))
                    {
                        result = 1;
                        ostiId = sort.OstiId;
                        message = sort.OstiStatusMsg;
                        status = sort.OstiStatus;
                        ostiDate = sort.OstiDate;
                        sort.StatusEnum = StatusEnum.Published;
                    }
                    else
                    {
                        message = sort.OstiStatusMsg;
                        status = sort.OstiStatus;
                        ostiDate = sort.OstiDate;
                    }
                    sort.Save();
                }
            }

            var data = new
            {
                Result = result,
                Id = ostiId,
                Message = message,
                Status = status,
                Date = ostiDate
            };

            return Json(data);
        }

        [HttpPost]
        public ActionResult SendReminderEmail(int? id)
        {
            int result = -1;
            string message = string.Empty;

            if (id.HasValue)
            {
                var model = new DueDateModel(id.Value, null);
                if (model.UserHasAccess())
                {
                    message = model.EmailReminder();
                    if (string.IsNullOrWhiteSpace(message))
                    {
                        result = 1;
                    }
                }
            }

            var data = new
            {
                Result = result,
                Message = message
            };
            return Json(data);
        }
        
        public ActionResult DownloadXmlData(int? id)
        {
            if (id.HasValue)
            {
                var sort = SortMainObject.GetSortMain(id.Value);
                if (sort != null)
                {
                    return File(Encoding.UTF8.GetBytes(sort.GetXml()), System.Net.Mime.MediaTypeNames.Application.Octet, (sort.SortMainId.ToString() + ".txt"));
                }
            }

            return null;
        }

        public ActionResult SetOneYearReminder(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("Index", "Home");
            }
            var reminder = new ReminderModel(id.Value);
            if (!reminder.UserHasWriteAccess)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(reminder);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SetOneYearReminder(ReminderModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.Save();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult SetReleaseDelay(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("Index", "Home");
            }
            var model = new DelayModel(id.Value);
            if (!model.UserHasWriteAccess)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SetReleaseDelay(DelayModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.Save();
            return RedirectToAction("Index", "Artifact", new {id = model.SortMainId});
        }

        public ActionResult ResetReleaseDelay(int? id)
        {
            if (id.HasValue)
            {
                if (SortMainObject.CheckUserHasWriteAccess(id.Value))
                {
                    SortMainObject.ResetDelayToDate(id.Value);
                }
            }

            return RedirectToAction("Index", "Artifact", new{id});
        }



        public ActionResult CreateOstiDocument(int? id, bool? aRtn)
        {
            if (id.HasValue)
            {
                ProcessLogObject.Add(id, "Start CreateOstiDocument");

                if (SortAttachmentObject.GetFinalDocAttachment(id.Value) != null)
                {
                    bool success = false;
                    byte[] file = Config.DigitalLibraryManager.GenerateExportFile(id.Value, SortMainObject.GetSortMain(id.Value).CoverPageRequired, ref success);
                    if (success)
                    {
                        SortAttachmentObject.AddOstiAttachment(id.Value, $"Sort_{id}.pdf", UserObject.CurrentUser.EmployeeId, file.Length, file);
                    }
                    else
                    {
                        TempData.Add("FailMessage", "AdLib Failed to generate the OSTI File! Please try again.");
                    }
                }
                
                ProcessLogObject.Add(id, "End CreateOstiDocument");
            }

            if (aRtn.HasValue && aRtn.Value)
            {
                return RedirectToAction("Index", "Artifact", new {id});
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult DeleteOstiDocument(int? id, bool? aRtn)
        {
            if (id.HasValue)
            {
                if (SortMainObject.CheckUserHasWriteAccess(id.Value))
                {
                    SortAttachmentObject.GetOstiAttachment(id.Value)?.PerminateDelete();
                }
            }

            if (aRtn.HasValue && aRtn.Value)
            {
                return RedirectToAction("Index", "Artifact", new { id });
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult ChangeCoverPageRequired(int? sortMainId, bool? coverPageRequired)
        {
            if (sortMainId.HasValue && coverPageRequired.HasValue)
            {
                SortMainObject.UpdateCoverPageRequired(sortMainId.Value, coverPageRequired.Value);
            }

            return RedirectToAction("Index", "Artifact", new {id = sortMainId});
        }

        public ActionResult ForceEdms(int? id)
        {
            if (id.HasValue)
            {
                SortMainObject.SetForceEdms(id.Value);
            }

            return RedirectToAction("Index", new {id});
        }

        public ActionResult SetOstiNumber(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("Index", new { id });
                
            }

            var model = new SetOstiNumberModel(id.Value);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SetOstiNumber(SetOstiNumberModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.Save();

            return RedirectToAction("Index", new { id = model.SortMainId });
        }

        public ActionResult DeleteArtifact(int? id)
        {
            if (id.HasValue)
            {
                SortMainObject.GetSortMain(id.Value)?.Delete();
            }

            return RedirectToAction("Index", "Home");
        }
    }
}