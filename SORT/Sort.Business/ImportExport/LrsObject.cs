using System.Collections.Generic;
using System.Linq;
using Dapper;

namespace Sort.Business
{
    public class LrsObject
    {
        
        public static void ImportLatest()
        {
            string sql = @"select 
                                m.MainId as LrsId,
                                m.OwnerEmployeeId as OwnerEmployeeId, 
                                'Approved' as ReviewStatus,
                                100 as ReviewProgress,
                                m.DocumentType,
                                m.Title,
                                m.StiNumber,
                                m.Revision,
                                m.CreateDate,
                                m.ActivityDate as ModifiedDate,
                                m.ApprovalDate as ApprovedDate,
                                m.Abstract,
                                m.ConferenceName,
                                m.ConferenceSponsor,
                                m.ConferenceLocation,
                                m.COnferenceBeginDate,
                                m.ConferenceEndDate,
                                m.JournalName,
                                m.RelatedSti
                            from dat_Main m
                            where m.[Status] = 'Completed'
                            and m.DocumentTYpe != 'DoeId'
                            and m.ContainsSciInfo = 1
                            and m.SortId is null
                            and m.OstiId is null
                            and m.ApprovalDate > '8/1/2018'";

            var artifacts = Config.LrsConn.Query<ArtifactData>(sql).ToList();

            foreach (var ad in artifacts)
            {
                var obj = SortMainObject.GetSortMainForStiNumber(ad.StiNumber, ad.Revision);
                if (obj == null)
                {
                    ad.Contacts = GetContacts(ad.LrsId);
                    ad.Authors = GetAuthors(ad.LrsId);
                    ad.Fundings = GetFunding(ad.LrsId);
                    ad.Reviewers = GetReviewers(ad.LrsId);
                    ad.Subjects = GetMetaData(ad.LrsId, "SubjectCategories");
                    ad.Keywords = GetMetaData(ad.LrsId, "Keywords");
                    ad.CoreCapabilities = GetMetaData(ad.LrsId, "CoreCapabilities");
                    int? id = ad.Import();
                    if (id.HasValue)
                    {
                        UpdateLrsSortId(ad.LrsId, id.Value);
                    }
                }
                else
                {
                    UpdateLrsSortId(ad.LrsId, obj.SortMainId.Value);
                }
            }
        }

        public static List<ContactData> GetContacts(int lrsId)
        {
            string sql = @"select 
                                c.Phone,
                                c.Location,
                                c.EmployeeId,
                                c.WorkOrg,
                                c.OrcidId
                            from dat_Contact c
                            where c.MainId = @MainId";

            return Config.LrsConn.Query<ContactData>(sql, new { MainId  = lrsId }).ToList();
        }

        public static List<AuthorData> GetAuthors(int lrsId)
        {
            string sql = @"select 
                                a.AuthorId,
                                a.[Name],
                                a.Affiliation,
                                a.Email,
                                a.OrcidId,
                                a.IsPrimary,
                                a.EmployeeId,
                                a.AffiliationType,
                                a.WorkOrg,
                                c.CountryCode,
                                s.ShortName as StateCode
                            from dat_Author a
                            left join lu_Country c on c.CountryId = a.CountryId
                            left join lu_State s on s.StateId = a.StateId
                            where a.MainId = @MainId";

            return Config.LrsConn.Query<AuthorData>(sql, new { MainId = lrsId }).ToList();
        }

        public static List<FundingData> GetFunding(int lrsId)
        {
            string sql = @"select 
                                f.[Year],
                                f.FundingTypeId,
                                f.Org,
                                f.ContractNumber,
                                f.[Percent],
                                f.DoeFundingCategoryId,
                                f.GrantNumber,
                                f.TrackingNumber,
                                f.SppCategoryId,
                                f.SppApproved,
                                f.FederalAgencyId,
                                f.ApproveNoReason,
                                f.OtherDescription,
                                f.CountryId,
                                f.AdditionalInfo,
                                f.ProjectArea,
                                f.ProjectNumber,
                                f.PrincipalInvEmployeeId,
                                f.MilestoneTrackingNumber
                            from dat_Funding f
                            where f.MainId = @MainId";

            return Config.LrsConn.Query<FundingData>(sql, new { MainId = lrsId }).ToList();
        }

        public static List<ReviewerData> GetReviewers(int lrsId)
        {
            string sql = @"select 
                                r.ReviewerEmployeeId,
                                r.ReviewerType,
                                r.ReviewDate,
                                r.[Status]
                            from dat_Review r
                            where r.MainId = @MainId";

            return Config.LrsConn.Query<ReviewerData>(sql, new { MainId = lrsId }).ToList();
        }

        public static List<string> GetMetaData(int lrsId, string type) => Config.LrsConn.Query<string>("select [data] from dat_MetaData where MainId = @MainId and MetaDataType = @MetaDataType", new { MainId = lrsId, MetaDataType = type }).ToList();

        public static void UpdateLrsSortId(int lrsId, int sortId)
        {
            string sql = @"";

            Config.LrsConn.Execute("UPDATE dat_Main SET SortId = @SortId WHERE MainId = @MainId", new { SortId = sortId, MainId = lrsId });
        }
    }
}
