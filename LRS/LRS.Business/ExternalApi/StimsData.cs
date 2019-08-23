using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LRS.Business
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
    }
}
