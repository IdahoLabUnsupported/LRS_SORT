using System;
using System.IO;
using System.Linq;
using System.Web;
using Novacode;

namespace Sort.Business
{
    public class PdfCoverPage
    {
        private static readonly string ExtCoverPath = HttpContext.Current.Server.MapPath("~/Documents/EXT_Cover.docx");
        private static readonly string JouCoverPath = HttpContext.Current.Server.MapPath("~/Documents/JOU_Cover.docx");
        private static Stream _extCoverStream = Stream.Null;
        private static Stream _jouCoverStream = Stream.Null;

        private static DocX GetExtCover()
        {
            if (_extCoverStream == Stream.Null)
                _extCoverStream = new MemoryStream(File.ReadAllBytes(ExtCoverPath));

            return DocX.Load(_extCoverStream);
        }

        private static DocX GetJouCover()
        {
            if (_jouCoverStream == Stream.Null)
                _jouCoverStream = new MemoryStream(File.ReadAllBytes(JouCoverPath));

            return DocX.Load(_jouCoverStream);
        }

        public static Stream GenerateCover(int sortMainId)
        {
            var sort = SortMainObject.GetSortMain(sortMainId);
            if (sort != null)
            {
                var toReturn = new MemoryStream();
                DocX doc = null;

                if (sort.Title.Trim().Substring(0, 7).Equals("INL-CON", StringComparison.OrdinalIgnoreCase) ||
                    sort.Title.Trim().Substring(0, 7).Equals("INL-JOU", StringComparison.OrdinalIgnoreCase))
                {
                    doc = GetJouCover();
                }
                else
                {
                    doc = GetExtCover();
                }

                doc.ReplaceText("{publicationdate}", sort.PublicationDate?.ToString("MMMM yyyy") ?? string.Empty);
                doc.ReplaceText("{stititle}", sort.Title?.RemoveNonAscii() ?? string.Empty);
                doc.ReplaceText("{publishtitle}", sort.PublishTitle?.RemoveNonAscii() ?? string.Empty);
                doc.ReplaceText("{authors}", string.Join(", ", sort.Authors.Select(n => n.FullName))?.RemoveNonAscii() ?? string.Empty);
                doc.ReplaceText("{doctype}", sort.ProductTypeDisplayName?.RemoveNonAscii() ?? string.Empty);
                doc.ReplaceText("{contractnumber}", string.Join(", ", sort.Funding.Select(n => n.ContractNumber))?.RemoveNonAscii() ?? string.Empty);
                doc.ReplaceText("{fundingoffice}", string.Join(", ", sort.Funding.Select(n => n.TitleFundingSourceName))?.RemoveNonAscii() ?? string.Empty);

                doc.SaveAs(toReturn);
                return toReturn;
            }

            return null;
        }
    }
}