using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using LRS.Business;
using LRS.Mvc.Models;

namespace LRS.Mvc.Controllers
{
    [Authorize]
    public class ArtifactController : Controller
    {
        #region Artifact
        public ActionResult Index(int? id)
        {
            if (id.HasValue && id.Value > 0)
            {
                var model = new ArtifactModel(id.Value, string.Empty);

                if (model.Main != null && 
                    (model.Main.StatusEnum == StatusEnum.New || 
                     model.Main.StatusEnum == StatusEnum.InPeerReview ||
                     model.Main.StatusEnum == StatusEnum.ReviewNotRequired) && model.Main.UserHasWriteAccess())
                {
                    return View("Edit", model);
                }

                return View(model);
            }

            return View("Add", new ArtifactModel());
        }

        /// <summary>
        /// Edit is for Admin or Relase Officer only, otherwise go through Index to 
        /// determain which view they should see.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue || !Current.IsReleaseOfficer)
            {
                return RedirectToAction("Index", new { id });
            }

            var model = new ArtifactModel(id.Value, string.Empty);

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Main Id</param>
        /// <returns></returns>
        public ActionResult Cancel(int? id)
        {
            if (id.HasValue && MainObject.CheckUserHasWriteAccess(id.Value))
            {
                MainObject.GetMain(id.Value)?.Cancel();
            }

            return RedirectToAction("Index", new { id });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Main Id</param>
        /// <returns></returns>
        public ActionResult Restart(int? id)
        {
            if (id.HasValue && MainObject.CheckUserHasWriteAccess(id.Value))
            {
                MainObject.GetMain(id.Value)?.ReStart();
            }

            return RedirectToAction("Index", new {id});
        }

        public ActionResult Delete(int? id)
        {
            if (!id.HasValue || !Current.IsAdmin)
            {
                return RedirectToAction("Index", new { id });
            }

            return View(new DeleteModel(id.Value));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Delete(DeleteModel model)
        {
            if (!Current.IsAdmin)
            {
                return RedirectToAction("Index", new { model.MainId });
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.Save();

            return RedirectToAction("Index", "Home");
        }

        public ActionResult CreateRevision(int? id)
        {
            if (id.HasValue && MainObject.CheckUserHasWriteAccess(id.Value))
            {
                int? newId = MainObject.CreateRevision(id.Value);
                if (newId.HasValue)
                {
                    return RedirectToAction("Index", new { id = newId });
                }
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult FollowOn(int? id)
        {
            if (id.HasValue && MainObject.CheckUserHasWriteAccess(id.Value))
            {
                MainObject.FollowOn(id.Value);

                return RedirectToAction("Index", new {id});
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult MarkCompleted(int? id)
        {
            if (id.HasValue && MainObject.CheckUserHasWriteAccess(id.Value))
            {
                MainObject.GetMain(id.Value)?.MarkReviewComplete();
            }

            return RedirectToAction("Index", new { id });
        }

        public JsonResult ExportArtifactToSort(int? id)
        {
            var main = MainObject.GetMain(id??0);
            if (main == null)
            {
                return Json(
                    new {
                    Status = false,
                    Message = "Unable to find the Artifact to send to SORT"
                    }
                );
            }

            var msg = main.UploadToSort();
            if (string.IsNullOrWhiteSpace(msg))
            {
                return Json(
                    new
                    {
                        Status = true,
                        Message = ""
                    }
                );
            }

            return Json(
                new
                {
                    Status = false,
                    Message = msg
                }
            );
        }

        public ActionResult Claim(int? id)
        {
            if (id.HasValue)
            {
                var main = MainObject.GetMain(id.Value);
                if (main != null)
                {
                    main.ClaimReview();
                }
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult ToggleRush(int? id)
        {
            if (id.HasValue)
            {
                var main = MainObject.GetMain(id.Value);
                if (main != null)
                {
                    main.ToggleRushFlag();
                }
            }

            return RedirectToAction("Index", "Home");
        }
        #endregion;

        #region Attachments

        public ActionResult AttachmentPartial(int? id)
        {
            var model = new AttachmentModel();
            if (id.HasValue && MainObject.CheckUserHasReadAccess(id.Value))
            {
                model = new AttachmentModel(id.Value);
            }

            return PartialView("Partials/AttachmentsPartial", model);
        }

        /// <summary>
        /// Start a new attachment.  This creates the initial attachment record that all the pieces will
        /// use when uploading the file.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]/*, ValidateAntiForgeryToken]*/
        public JsonResult AddAttachment(AttachmentModel model)
        {
            if (ModelState.IsValid && MainObject.CheckUserHasWriteAccess(model.MainId))
            {
                model.Save();

                return Json(model.AttachmentId);
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
            var attachment = AttachmentObject.GetAttachment(id);
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
        /// <param name="id">Attachment Id</param>
        /// <returns></returns>
        public FileResult DownloadAttachment(int? id)
        {
            if (id.HasValue)
            {
                var ao = AttachmentObject.GetAttachment(id.Value);
                if (ao != null && MainObject.CheckUserHasReadAccess(ao.MainId))
                {
                    var content = ao.Contents;
                    if (content != null)
                    {
                        content.Seek(0, SeekOrigin.Begin);
                        // Need this turned off for large files
                        Response.BufferOutput = false;
                        return File(content, System.Net.Mime.MediaTypeNames.Application.Octet, ao.FileName);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Attachment Id</param>
        /// <returns></returns>
        public ActionResult DeleteAttachment(int? id)
        {
            int mid = 0;
            if (id.HasValue)
            {
                var asa = AttachmentObject.GetAttachment(id.Value);
                if (asa != null)
                {
                    mid = asa.MainId;
                    if (MainObject.CheckUserHasWriteAccess(mid))
                    {
                        asa.Delete();
                    }
                }
            }

            return GetAttachments(mid);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Main Id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetAttachments(int? id)
        {
            var model = new List<AttachmentObject>();
            if (id.HasValue && MainObject.CheckUserHasReadAccess(id.Value))
            {
                model = AttachmentObject.GetAttachments(id.Value);
            }

            return PartialView("Partials/_attachmentList", model);
        }

        #endregion

        #region Contact

        public ActionResult ContactNumberPartial()
        {
            return PartialView("Partials/ContactEmployeeIdPartial", new ContactModel());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Main ID</param>
        /// <param name="rid">Contact ID</param>
        /// <returns></returns>
        public ActionResult ContactsPartial(int? id, int? rid)
        {
            var model = new ContactModel();
            if (id.HasValue)
            {
                model = new ContactModel(id.Value, rid);
            }

            return PartialView("Partials/ContactsPartial", model);
        }

        [HttpPost]/*, ValidateAntiForgeryToken]*/
        public ActionResult AddContact(ContactModel model)
        {
            if (ModelState.IsValid && MainObject.CheckUserHasWriteAccess(model.MainId))
            {
                model.Save();

                return PartialView("Partials/_contactList", ContactObject.GetContacts(model.MainId));
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
            int mid = 0;
            if (id.HasValue)
            {
                var contact = ContactObject.GetContact(id.Value);
                if (contact != null)
                {
                    mid = contact.MainId;
                    if (MainObject.CheckUserHasWriteAccess(mid))
                    {
                        contact.Delete();
                    }
                }
            }

            return PartialView("Partials/_contactList", ContactObject.GetContacts(mid));
        }

        [HttpPost]
        public JsonResult AddContactToAuthor(int? id)
        {
            string title = "Error adding Contact as Author";
            string message = "Unable to determain the Contact.";
            bool isAdded = false;
            if (id.HasValue)
            {
                var contact = ContactObject.GetContact(id.Value);
                if (contact != null && MainObject.CheckUserHasWriteAccess(contact.MainId))
                {
                    isAdded = contact.SaveAsAuthor(ref title, ref message);
                }
            }

            return Json(new{title,message,isAdded});
        }
        #endregion

        #region Intellectual Property

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Main Id</param>
        /// <param name="rid">IntellectualId</param>
        /// <returns></returns>
        public ActionResult IntellectualPartial(int? id, int? rid)
        {
            var model = new IntellectualModel();
            if (id.HasValue)
            {
                model = new IntellectualModel(id.Value, rid);
            }

            return PartialView("Partials/IntellectualPartial", model);
        }

        [HttpPost]/*, ValidateAntiForgeryToken]*/
        public ActionResult AddIntellectual(IntellectualModel model)
        {
            if (ModelState.IsValid && MainObject.CheckUserHasWriteAccess(model.MainId))
            {
                model.Save();

                return PartialView("Partials/_intellectualList", IntellectualPropertyObject.GetIntellectualProperties(model.MainId));
            }

            return null;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Intellectual Property Id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RemoveIntellectual(int? id)
        {
            int mid = 0;
            if (id.HasValue)
            {
                var data = IntellectualPropertyObject.GetIntellectualProperty(id.Value);
                if (data != null)
                {
                    mid = data.MainId;
                    if (MainObject.CheckUserHasWriteAccess(mid))
                    {
                        data.Delete();
                    }
                }
            }

            return PartialView("Partials/_intellectualList", IntellectualPropertyObject.GetIntellectualProperties(mid));
        }

        #endregion

        #region Author

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Main ID</param>
        /// <param name="rid">Author ID</param>
        /// <returns></returns>
        public ActionResult AuthorsPartial(int? id, int? rid)
        {
            var model = new AuthorModel();

            if (id.HasValue)
            {
                model = new AuthorModel(id.Value, rid);
            }

            return PartialView("Partials/AuthorsPartial", model);
        }

        [HttpPost]/*, ValidateAntiForgeryToken]*/
        public ActionResult AddAuthor(AuthorModel model)
        {
            if (ModelState.IsValid && MainObject.CheckUserHasWriteAccess(model.MainId))
            {
                model.Save();

                return GetAuthorListPartial(model.MainId);
            }

            return null;
        }

        [HttpPost]
        public ActionResult GetAuthorListPartial(int? id)
        {
            var model = new List<AuthorObject>();
            if (id.HasValue)
            {
                model = AuthorObject.GetAuthors(id.Value);
            }

            return PartialView("Partials/_authorList", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">AuthorId</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RemoveAuthor(int? id)
        {
            int mid = 0;
            if (id.HasValue)
            {
                var author = AuthorObject.GetAuthor(id.Value);
                if (author != null)
                {
                    mid = author.MainId;
                    if (MainObject.CheckUserHasWriteAccess(mid))
                    {
                        author.Delete();
                    }
                }
            }

            return GetAuthorListPartial(mid);
        }

        /// <summary>
        /// Set an author as the primary author
        /// </summary>
        /// <param name="id">Author ID</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SetPrimaryAuthor(int? id)
        {
            int mid = 0;
            if (id.HasValue)
            {
                var author = AuthorObject.GetAuthor(id.Value);
                if (author != null)
                {
                    mid = author.MainId;
                    if (MainObject.CheckUserHasWriteAccess(mid))
                    {
                        author.SetAsPrimary();
                    }
                }
            }

            return GetAuthorListPartial(mid);
        }

        #endregion

        #region Funding

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Main ID</param>
        /// <param name="rid">Funding ID</param>
        /// <returns></returns>
        public ActionResult FundingPartial(int? id, int? rid)
        {
            var model = new FundingModel();
            if (id.HasValue)
            {
                model = new FundingModel(id.Value, rid);
            }

            return PartialView("Partials/FundingPartial", model);
        }

        [HttpPost]/*, ValidateAntiForgeryToken]*/
        public ActionResult AddFunding(FundingModel model)
        {
            if (ModelState.IsValid && MainObject.CheckUserHasWriteAccess(model.MainId))
            {
                model.Save();

                return PartialView("Partials/_fundingList", FundingObject.GetFundings(model.MainId));
            }

            return null;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Funding ID</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetFundingInfoPartial(int? id)
        {
            var model = new FundingObject();
            if (id.HasValue)
            {
                model = FundingObject.GetFunding(id.Value);
            }
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
            int mid = 0;
            if (id.HasValue)
            {
                var funding = FundingObject.GetFunding(id.Value);
                if (funding != null)
                {
                    mid = funding.MainId;
                    if (MainObject.CheckUserHasWriteAccess(mid))
                    {
                        funding.Delete();
                    }
                }
            }

            return PartialView("Partials/_fundingList", FundingObject.GetFundings(mid));
        }

        #endregion

        #region Subject

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Main ID</param>
        /// <returns></returns>
        public ActionResult SubjectPartial(int? id)
        {
            var model = new MetadataModel();
            if (id.HasValue)
            {
                model = new MetadataModel(id.Value, MetaDataTypeEnum.SubjectCategories);
            }

            return PartialView("Partials/SubjectPartial", model);
        }

        [HttpPost]/*, ValidateAntiForgeryToken]*/
        public ActionResult AddSubject(MetadataModel model)
        {
            if (ModelState.IsValid && MainObject.CheckUserHasWriteAccess(model.MainId))
            {
                model.Save();

                return PartialView("Partials/_subjectList", MetaDataObject.GetMetaDatas(model.MainId, MetaDataTypeEnum.SubjectCategories));
            }

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
            int mid = 0;
            if (id.HasValue)
            {
                var meta = MetaDataObject.GetMetaData(id.Value);
                if (meta != null)
                {
                    mid = meta.MainId;
                    if (MainObject.CheckUserHasWriteAccess(mid))
                    {
                        meta.Delete();
                    }
                }
            }

            return PartialView("Partials/_subjectList", MetaDataObject.GetMetaDatas(mid, MetaDataTypeEnum.SubjectCategories));
        }

        #endregion

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
            if (ModelState.IsValid && MainObject.CheckUserHasWriteAccess(model.MainId))
            {
                model.Save();

                return PartialView("Partials/_coreCapabilitiesList", MetaDataObject.GetMetaDatas(model.MainId, MetaDataTypeEnum.CoreCapabilities));
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
                var meta = MetaDataObject.GetMetaData(id.Value);
                if (meta != null)
                {
                    mid = meta.MainId;
                    if (MainObject.CheckUserHasWriteAccess(mid))
                    {
                        meta.Delete();
                    }
                }
            }

            return PartialView("Partials/_coreCapabilitiesList", MetaDataObject.GetMetaDatas(mid, MetaDataTypeEnum.CoreCapabilities));
        }

        #endregion

        #region Keyword

        public ActionResult KeywordPartial(int? id)
        {
            var model = new MetadataModel();
            if (id.HasValue)
            {
                model = new MetadataModel(id.Value, MetaDataTypeEnum.Keywords);
            }

            return PartialView("Partials/KeywordsPartial", model);
        }

        [HttpPost]/*, ValidateAntiForgeryToken]*/
        public ActionResult AddKeyword(MetadataModel model)
        {
            if (ModelState.IsValid && MainObject.CheckUserHasWriteAccess(model.MainId))
            {
                model.Save();

                return PartialView("Partials/_keywordsList", MetaDataObject.GetMetaDatas(model.MainId, MetaDataTypeEnum.Keywords));
            }

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
            int mid = 0;
            if (id.HasValue)
            {
                var meta = MetaDataObject.GetMetaData(id.Value);
                if (meta != null)
                {
                    mid = meta.MainId;
                    if (MainObject.CheckUserHasWriteAccess(mid))
                    {
                        meta.Delete();
                    }
                }
            }

            return PartialView("Partials/_keywordsList", MetaDataObject.GetMetaDatas(mid, MetaDataTypeEnum.Keywords));
        }

        #endregion

        #region Saving Functions
        [HttpPost]/*, ValidateAntiForgeryToken]*/
        public ActionResult AbstractSave(AbstractModel model)
        {
            if (ModelState.IsValid && MainObject.CheckUserHasWriteAccess(model.MainId))
            {
                model.Save();
            }

            return null;
        }

        [HttpPost]/*, ValidateAntiForgeryToken]*/
        public ActionResult TitleSave(TitleModel model)
        {
            if (ModelState.IsValid && model.UserHasAccess)
            {
                model.Save();
            }

            return RedirectToAction("Index", new {id = model.MainId});
        }

        [HttpPost]
        public string GetStiNumber(int? id)
        {
            if (id.HasValue)
            {
                return MainObject.GetMain(id.Value)?.StiNumber;
            }

            return string.Empty;
        }

        [HttpPost]/*, ValidateAntiForgeryToken]*/
        public ActionResult TitleSilentSave(TitleModel model)
        {
            if (ModelState.IsValid && model.UserHasAccess)
            {
                model.Save();
            }

            return null;
        }

        [HttpPost]/*, ValidateAntiForgeryToken]*/
        public ActionResult ConferenceSave(ConferenceModel model)
        {
            if (ModelState.IsValid && MainObject.CheckUserHasWriteAccess(model.MainId))
            {
                model.Save();
            }

            return null;
        }

        [HttpPost]/*, ValidateAntiForgeryToken]*/
        public ActionResult AddSponsor(MetadataModel model)
        {
            if (ModelState.IsValid && MainObject.CheckUserHasWriteAccess(model.MainId))
            {
                model.Save();

                return PartialView("Partials/_sponsorOrgList", MetaDataObject.GetMetaDatas(model.MainId, MetaDataTypeEnum.SponsoringOrgs));
            }

            return null;
        }
        #endregion

        #region Reviewers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Main Id</param>
        /// <param name="rid">Review Id</param>
        /// <returns></returns>
        public ActionResult ReviewersPartial(int? id, int? rid)
        {
            var model = new ReviewerModel();
            if (id.HasValue)
            {
                model = new ReviewerModel(id.Value, rid);
            }

            return PartialView("Partials/ReviewPartial", model);
        }

        [HttpPost]/*, ValidateAntiForgeryToken]*/
        public ActionResult AddReviewer(ReviewerModel model)
        {
            if (ModelState.IsValid && MainObject.CheckUserHasWriteAccess(model.MainId))
            {
                model.Save();

                return GetReviewersList(model.MainId);
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Main ID</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetReviewersList(int? id)
        {
            var model = new List<ReviewObject>();
            if (id.HasValue)
            {
                model = ReviewObject.GetReviews(id.Value);
            }

            return PartialView("Partials/_reviewList", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">ReviewerId</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RemoveReviewer(int? id)
        {
            int mid = 0;
            if (id.HasValue)
            {
                var review = ReviewObject.GetReview(id.Value);
                if (review != null)
                {
                    mid = review.MainId;
                    if (MainObject.CheckUserHasWriteAccess(mid))
                    {
                        review.Cancel();
                    }
                }
            }

            return GetReviewersList(mid);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Review Id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ReactivateReviewer(int? id)
        {
            int mid = 0;
            if (id.HasValue)
            {
                var review = ReviewObject.GetReview(id.Value);
                if (review != null)
                {
                    mid = review.MainId;
                    if (MainObject.CheckUserHasWriteAccess(mid))
                    {
                        review.ReActivate();
                    }
                }
            }

            return GetReviewersList(mid);
        }

        /// <summary>
        /// sends a review reminder and returns a partial view of reviewers
        /// </summary>
        /// <param name="id">Review ID</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SendReminderEmail(int? id)
        {
            if (id.HasValue)
            {
                var review = ReviewObject.GetReview(id.Value);
                if (review != null)
                {
                    review.SendReminderEmail();
                    return GetReviewersList(review.MainId);
                }
            }

            return null;
        }
        
        #endregion

        #region Reviews

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Main Id</param>
        /// <param name="type">Review Type</param>
        /// <returns></returns>
        public ActionResult StartReviews(int? id, string type)
        {
            if (id.HasValue && id.Value > 0)
            {
                var main = MainObject.GetMain(id.Value);
                if (main != null && main.UserHasWriteAccess())
                {
                    main.StartReviews(type);
                }
            }
            return RedirectToAction("Index", new { id });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Main ID</param>
        /// <param name="rid">Review ID</param>
        /// <returns></returns>
        public ActionResult MarkReviewFinished(int? id, int? rid)
        {
            if (id.HasValue && rid.HasValue)
            {
                MainObject.GetMain(id.Value)?.MarkReviewComplete(rid.Value);
            }

            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Main ID</param>
        /// <param name="rid">Review ID</param>
        /// <returns></returns>
        public ActionResult MarkReviewApproved(int? id, int? rid, bool? ouo3, bool? ouo7, string ouo7sn)
        {
            if (id.HasValue && rid.HasValue)
            {
                MainObject.GetMain(id.Value)?.MarkReviewApproved(rid.Value, ouo3, ouo7, ouo7sn);
            }

            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Main ID</param>
        /// <param name="rid">Review ID</param>
        /// <returns></returns>
        public ActionResult MarkNotReviewing(int? id, int? rid)
        {
            if (id.HasValue && rid.HasValue)
            {
                MainObject.GetMain(id.Value)?.MarkNotReviewing(rid.Value);
            }

            return RedirectToAction("Index", "Home");
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Main ID</param>
        /// <param name="rid">Review ID</param>
        /// <returns></returns>
        public ActionResult MarkReviewRejectedPartial(int? id, int? rid)
        {
            if (id.HasValue && rid.HasValue)
            {
                return PartialView("Partials/_rejectComment", new CommentModel(id.Value, rid.Value));
            }

            return null;
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MarkReviewRejected(CommentModel model)
        {
            if (ModelState.IsValid)
            {
                model.SaveRejection();
            }

            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Main ID</param>
        /// <param name="rid">Review ID</param>
        /// <returns></returns>
        public ActionResult AddReviewCommentPartial(int? id, int? rid)
        {
            if (id.HasValue)
            {
                return PartialView("Partials/_addComment", new CommentModel(id.Value, rid));
            }

            return null;
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AddReviewComment(CommentModel model)
        {
            if (ModelState.IsValid)
            {
                model.Save();
            }

            return RedirectToAction("Index", new { id = model.MainId });
        }

        [HttpPost]
        public JsonResult GetReviewComment(int? id)
        {
            if (id.HasValue)
            {
                var cmt = ReviewCommentObject.GetReviewComment(id.Value);
                if (cmt != null)
                {
                    var data = new
                    {
                        id = cmt.ReviewCommentId,
                        reviewId = cmt.ReviewId,
                        comment = cmt.Comment
                    };

                    return Json(data);
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Comment Id</param>
        /// <returns></returns>
        public ActionResult DeleteComment(int? id)
        {
            int mid = 0;
            if (id.HasValue)
            {
                var c = ReviewCommentObject.GetReviewComment(id.Value);
                if (c != null )
                {
                    mid = c.MainId;
                    if (c.EmployeeId == Current.User.EmployeeId || Current.IsAdmin)
                    {
                        c.Delete();
                    }
                }
            }

            return RedirectToAction("Index", new {id = mid});
        }

        #endregion

        #region Admin

        public ActionResult AdminCommentsPartial(int? id)
        {
            if (id.HasValue)
            {
                var model = new AdminCommentsModel(id.Value);

                return PartialView("Partials/AdminCommentPartial", model);
            }

            return null;
        }

        public ActionResult AddAdminCommentsPartial(int? id)
        {
            if (id.HasValue)
            {
                var model = new AdminCommentsModel(id.Value);

                return PartialView("Partials/_addAdminComment", model);
            }

            return null;
        }

        public ActionResult AdminAttachmentsPartial(int? id)
        {
            var model = new AdminAttachmentModel(id);

            return PartialView("Partials/AdminAttachmentPartial", model);
        }

        public ActionResult AddAdminAttachmentsPartial(int? id)
        {
            if (id.HasValue)
            {
                var model = new AttachmentModel(id.Value);

                return PartialView("Partials/_addAdminAttachment", model);
            }

            return null;
        }

        [HttpPost]
        public JsonResult SaveAdminComment(AdminCommentsModel model)
        {
            if (ModelState.IsValid)
            {
                model.Save();

                return Json(model.AdminCommentId);
            }

            return null;
        }

        [HttpPost]
        public ActionResult DeleteAdminComment(int? id)
        {
            if (id.HasValue)
            {
                var cmt = AdminCommentObject.GetAdminComment(id.Value);
                if (cmt != null)
                {
                    cmt.Delete();
                }
            }

            return null;
        }

        [HttpPost]
        public JsonResult SaveAdminAttachment(AttachmentModel model)
        {
            if (ModelState.IsValid)
            {
                model.Save();

                return Json(model.AttachmentId);
            }

            return null;
        }
        #endregion
    }
}