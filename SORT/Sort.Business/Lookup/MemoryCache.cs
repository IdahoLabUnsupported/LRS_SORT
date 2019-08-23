using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;

namespace Sort.Business
{
    public class MemoryCache
    {
        #region Users

        private static string usersKey = "users";

        public static List<UserObject> GetUsers()
        {
            var cache = HttpContext.Current.Cache;
            List<UserObject> users = (List<UserObject>)cache[usersKey];
            if (users == null)
            {
                users = UserObject.GetUsers();
                cache.Insert(usersKey, users, null, DateTime.Now.AddMinutes(10), Cache.NoSlidingExpiration);
            }
            return users;
        }

        public static SelectList UsersList()
        {
            return new SelectList(GetUsers(), "EmployeeId", "FullName");
        }

        public static UserObject GetUser(string employeeId)
        {
            var users = GetUsers();
            return users.FirstOrDefault(x => x.EmployeeId.Equals(employeeId, StringComparison.InvariantCultureIgnoreCase));
        }

        public static void ClearUsers()
        {
            var cache = HttpContext.Current.Cache;
            cache.Remove(usersKey);
        }

        #endregion

        #region DOE Funding Category

        private static string DoeFundingKey = "doefunding";

        public static List<DoeFundingCategoryObject> GetDoeFunding(bool enabledOnly = false, int? requiredId = null)
        {
            var cache = HttpContext.Current.Cache;
            List<DoeFundingCategoryObject> data = (List<DoeFundingCategoryObject>)cache[DoeFundingKey];
            if (data == null)
            {
                data = DoeFundingCategoryObject.GetDoeFundingCategories();
                cache.Insert(DoeFundingKey, data, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration);
            }
            return data.Where(n => (!enabledOnly || n.Active || (requiredId.HasValue && n.DoeFundingCategoryId == requiredId.Value))).ToList();
        }

        public static SelectList DoeFundingList(bool enabledOnly = true, int? requiredId = null)
        {
            return new SelectList(GetDoeFunding(enabledOnly, requiredId).OrderBy(n => n.Description), "DoeFundingCategoryId", "Description");
        }

        public static DoeFundingCategoryObject GetDoeFunding(int id)
        {
            return GetDoeFunding()?.FirstOrDefault(x => x.DoeFundingCategoryId == id);
        }

        public static DoeFundingCategoryObject GetDoeFunding(string category)
        {
            return GetDoeFunding()?.FirstOrDefault(x => x.Category == category);
        }

        public static void ClearDoeFunding()
        {
            var cache = HttpContext.Current.Cache;
            cache.Remove(DoeFundingKey);
        }

        #endregion

        #region SPP Funding Category

        private static string SppFundingKey = "sppfunding";

        public static List<SppCategoryObject> GetSppFunding(bool enabledOnly = false, int? requiredId = null)
        {
            var cache = HttpContext.Current.Cache;
            List<SppCategoryObject> data = (List<SppCategoryObject>)cache[SppFundingKey];
            if (data == null)
            {
                data = SppCategoryObject.GetSppCategories();
                cache.Insert(SppFundingKey, data, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration);
            }
            return data.Where(n => (!enabledOnly || n.Active || (requiredId.HasValue && n.SppCategoryId == requiredId.Value))).ToList();
        }

        public static SelectList SppFundingList(bool enabledOnly = true, int? requiredId = null)
        {
            return new SelectList(GetSppFunding(enabledOnly, requiredId).OrderBy(n => n.Category), "SppCategoryId", "Category");
        }

        public static SppCategoryObject GetSppFunding(int id)
        {
            return GetSppFunding()?.FirstOrDefault(x => x.SppCategoryId == id);
        }

        public static SppCategoryObject GetSppFunding(string categoryCode)
        {
            return GetSppFunding()?.FirstOrDefault(x => x.CategoryCode == categoryCode);
        }

        public static void ClearSppFunding()
        {
            var cache = HttpContext.Current.Cache;
            cache.Remove(SppFundingKey);
        }

        #endregion

        #region SPP Funding Category Federal Agency

        private static string SppFundingFedAgKey = "sppfundingfedag";

        public static List<SppCategoryFederalAgencyObject> GetSppFundingFederalAgencies(bool enabledOnly = false, int? requiredId = null)
        {
            var cache = HttpContext.Current.Cache;
            List<SppCategoryFederalAgencyObject> data = (List<SppCategoryFederalAgencyObject>)cache[SppFundingFedAgKey];
            if (data == null)
            {
                data = SppCategoryFederalAgencyObject.GetSppCategoryFederalAgencies();
                cache.Insert(SppFundingFedAgKey, data, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration);
            }
            return data.Where(n => (!enabledOnly || n.Active || (requiredId.HasValue && n.SppCategoryFederalAgencyId == requiredId.Value))).ToList();
        }

        public static SelectList SppFundingFederalAgenciesList(bool enabledOnly = true, int? requiredFacilityId = null)
        {
            return new SelectList(GetSppFundingFederalAgencies(enabledOnly, requiredFacilityId).OrderBy(n => n.FederalAgency), "SppCategoryFederalAgencyId", "FederalAgency");
        }

        public static SppCategoryFederalAgencyObject GetSppFundingFederalAgency(int id)
        {
            return GetSppFundingFederalAgencies()?.FirstOrDefault(x => x.SppCategoryFederalAgencyId == id);
        }

        public static void ClearSppFundingFederalAgencies()
        {
            var cache = HttpContext.Current.Cache;
            cache.Remove(SppFundingFedAgKey);
        }

        #endregion

        #region Funding Type

        private static string FundingTypeKey = "fundingtype";

        public static List<FundingTypeObject> GetFundingTypes(bool enabledOnly = false, int? requiredId = null)
        {
            var cache = HttpContext.Current.Cache;
            List<FundingTypeObject> data = (List<FundingTypeObject>)cache[FundingTypeKey];
            if (data == null)
            {
                data = FundingTypeObject.GetFundingTypes();
                cache.Insert(FundingTypeKey, data, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration);
            }
            return data.Where(n => (!enabledOnly || n.Active || (requiredId.HasValue && n.FundingTypeId == requiredId.Value))).ToList();
        }

        public static SelectList FundingTypeList(bool enabledOnly = true, int? requiredId = null)
        {
            return new SelectList(GetFundingTypes(enabledOnly, requiredId).OrderBy(n => n.Description), "FundingTypeId", "Description");
        }

        public static FundingTypeObject GetFundingType(int id)
        {
            return GetFundingTypes()?.FirstOrDefault(x => x.FundingTypeId == id);
        }

        public static FundingTypeObject GetFundingType(string fundingType)
        {
            return GetFundingTypes()?.FirstOrDefault(x => x.FundingType == fundingType);
        }

        public static void ClearFundingTypes()
        {
            var cache = HttpContext.Current.Cache;
            cache.Remove(FundingTypeKey);
        }

        #endregion

        #region Country

        private static string CountryKey = "country";

        public static List<CountryObject> GetCountries(bool enabledOnly = false, int? requiredId = null)
        {
            try
            {
                var cache = HttpContext.Current.Cache;
                List<CountryObject> data = (List<CountryObject>) cache[CountryKey];
                if (data == null)
                {
                    data = CountryObject.GetCountries();
                    cache.Insert(CountryKey, data, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration);
                }

                return data.Where(n => (!enabledOnly || n.Active || (requiredId.HasValue && n.CountryId == requiredId.Value))).ToList();
            }
            catch { }

            return CountryObject.GetCountries().Where(n => (!enabledOnly || n.Active || (requiredId.HasValue && n.CountryId == requiredId.Value))).ToList();
        }

        public static SelectList CountryList(bool enabledOnly = true, int? requiredId = null) => new SelectList(GetCountries(enabledOnly, requiredId).OrderBy(n => n.Country), "CountryId", "Country");

        public static SelectList CountryNameList(bool enabledOnly = true, int? requiredId = null) => new SelectList(GetCountries(enabledOnly, requiredId).OrderBy(n => n.Country), "Country", "Country");

        public static SelectList CountryCodeList(bool enabledOnly = true, int? requiredId = null) => new SelectList(GetCountries(enabledOnly, requiredId).OrderBy(n => n.Country), "CountryCode", "Country");

        public static CountryObject GetCountry(int id) => GetCountries()?.FirstOrDefault(x => x.CountryId == id);

        public static CountryObject GetCountry(string country) => GetCountries()?.FirstOrDefault(x => x.Country == country);

        public static CountryObject GetCountryByCode(string countryCode) => GetCountries()?.FirstOrDefault(n => n.CountryCode.Equals(countryCode, StringComparison.OrdinalIgnoreCase));

        public static void ClearCountries() => HttpContext.Current.Cache.Remove(CountryKey);

        #endregion

        #region State

        private static string StateKey = "statesK";

        public static List<StateObject> GetStates(bool enabledOnly = false, int? requiredId = null)
        {
            try
            {
                var cache = HttpContext.Current.Cache;
                List<StateObject> data = (List<StateObject>) cache[StateKey];
                if (data == null)
                {
                    data = StateObject.GetStates();
                    cache.Insert(StateKey, data, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration);
                }

                return data.Where(n => (!enabledOnly || n.Active || (requiredId.HasValue && n.StateId == requiredId.Value))).ToList();
            }
            catch { }

            return StateObject.GetStates().Where(n => (!enabledOnly || n.Active || (requiredId.HasValue && n.StateId == requiredId.Value))).ToList();
        }

        public static SelectList StateList(bool enabledOnly = true, int? requiredId = null) => new SelectList(GetStates(enabledOnly, requiredId).OrderBy(n => n.StateName), "StateId", "StateName");

        public static SelectList StateShortNameList(bool enabledOnly = true, int? requiredId = null) => new SelectList(GetStates(enabledOnly, requiredId).OrderBy(n => n.StateName), "StateId", "ShortName");

        public static StateObject GetState(int id) => GetStates()?.FirstOrDefault(x => x.StateId == id);

        public static StateObject GetStateByShortName(string shortName) => GetStates()?.FirstOrDefault(x => x.ShortName == shortName);

        public static void ClearStates() => HttpContext.Current.Cache.Remove(StateKey);

        #endregion

        #region CoreCapabilities

        private static string CoreCapabilitiesKey = "CoreCapabilities";

        public static List<CoreCapabilitiesObject> GetCoreCapabilities(bool enabledOnly = false, int? requiredId = null)
        {
            var cache = HttpContext.Current.Cache;
            List<CoreCapabilitiesObject> data = (List<CoreCapabilitiesObject>)cache[CoreCapabilitiesKey];
            if (data == null)
            {
                data = CoreCapabilitiesObject.GetCoreCapabilitiess();
                cache.Insert(CoreCapabilitiesKey, data, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration);
            }
            return data.Where(n => (!enabledOnly || n.Active || (requiredId.HasValue && n.CoreCapabilitiesId == requiredId.Value))).ToList();
        }

        public static SelectList CoreCapabilitiesList(bool enabledOnly = true, int? requiredId = null) => new SelectList(GetCoreCapabilities(enabledOnly, requiredId).OrderBy(n => n.Name), "CoreCapabilitiesId", "Name");

        public static CoreCapabilitiesObject GetCoreCapability(int id) => GetCoreCapabilities()?.FirstOrDefault(x => x.CoreCapabilitiesId == id);

        public static void ClearCoreCapabilities() => HttpContext.Current.Cache.Remove(CoreCapabilitiesKey);

        #endregion

        #region Language

        private static string LanguageKey = "language";

        public static List<LanguageObject> GetLanguages(bool enabledOnly = false, int? requiredId = null)
        {
            var cache = HttpContext.Current.Cache;
            List<LanguageObject> data = (List<LanguageObject>)cache[LanguageKey];
            if (data == null)
            {
                data = LanguageObject.GetLanguages();
                cache.Insert(LanguageKey, data, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration);
            }
            return data.Where(n => (!enabledOnly || n.Active || (requiredId.HasValue && n.LanguageId == requiredId.Value))).ToList();
        }

        public static SelectList LanguageList(bool enabledOnly = true, int? requiredId = null)
        {
            return new SelectList(GetLanguages(enabledOnly, requiredId).OrderBy(n => n.Language), "LanguageId", "Language");
        }

        public static SelectList LanguageNameList(bool enabledOnly = true, int? requiredId = null)
        {
            return new SelectList(GetLanguages(enabledOnly, requiredId).OrderBy(n => n.Language), "Language", "Language");
        }

        public static LanguageObject GetLanguage(int id)
        {
            return GetLanguages()?.FirstOrDefault(x => x.LanguageId == id);
        }

        public static LanguageObject GetLanguage(string language)
        {
            return GetLanguages()?.FirstOrDefault(x => x.Language == language);
        }

        public static void ClearLanguages()
        {
            var cache = HttpContext.Current.Cache;
            cache.Remove(LanguageKey);
        }

        #endregion

        #region Subject Category

        private static string SubjectCategoryKey = "subjectcategory";

        public static List<SubjectCategoryObject> GetSubjectCategories(bool enabledOnly = false, int? requiredId = null)
        {
            var cache = HttpContext.Current.Cache;
            List<SubjectCategoryObject> data = (List<SubjectCategoryObject>)cache[SubjectCategoryKey];
            if (data == null)
            {
                data = SubjectCategoryObject.GetSubjectCategories();
                cache.Insert(SubjectCategoryKey, data, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration);
            }
            return data.Where(n => (!enabledOnly || n.Active || (requiredId.HasValue && n.SubjectCategoryId == requiredId.Value))).ToList();
        }

        public static SelectList SubjectCategoryList(bool enabledOnly = true, int? requiredId = null)
        {
            return new SelectList(GetSubjectCategories(enabledOnly, requiredId).OrderBy(n => n.FullSubject), "SubjectCategoryId", "FullSubject");
        }

        public static SelectList SubjectCategoryNameList(bool enabledOnly = true, int? requiredId = null)
        {
            return new SelectList(GetSubjectCategories(enabledOnly, requiredId).OrderBy(n => n.FullSubject), "FullSubject", "FullSubject");
        }

        public static SubjectCategoryObject GetSubjectCategory(int id)
        {
            return GetSubjectCategories()?.FirstOrDefault(x => x.SubjectCategoryId == id);
        }

        public static SubjectCategoryObject GetSubjectCategory(string fullSubject)
        {
            return GetSubjectCategories()?.FirstOrDefault(x => x.FullSubject == fullSubject);
        }

        public static void ClearSubjectCategories()
        {
            var cache = HttpContext.Current.Cache;
            cache.Remove(SubjectCategoryKey);
        }

        #endregion

        #region Journal Suggestion

        private static string JournalKey = "journalsuggestion";

        public static List<JournalObject> GetJournalSuggestions(bool enabledOnly = false, int? requiredId = null)
        {
            var cache = HttpContext.Current.Cache;
            List<JournalObject> data = (List<JournalObject>)cache[JournalKey];
            if (data == null)
            {
                data = JournalObject.GetJournals();
                cache.Insert(JournalKey, data, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration);
            }
            return data.Where(n => (!enabledOnly || n.Active || (requiredId.HasValue && n.JournalId == requiredId.Value))).ToList();
        }

        public static SelectList JournalSuggestionList(bool enabledOnly = true, int? requiredId = null)
        {
            return new SelectList(GetJournalSuggestions(enabledOnly, requiredId).OrderBy(n => n.JournalName), "JournalId", "JournalName");
        }

        public static SelectList JournalSuggestionNameList(bool enabledOnly = true, int? requiredId = null)
        {
            return new SelectList(GetJournalSuggestions(enabledOnly, requiredId).OrderBy(n => n.JournalName), "JournalName", "JournalName");
        }

        public static JournalObject GetJournalSuggestion(int id)
        {
            return GetJournalSuggestions()?.FirstOrDefault(x => x.JournalId == id);
        }

        public static JournalObject GetJournalSuggestion(string journalName)
        {
            return GetJournalSuggestions()?.FirstOrDefault(x => x.JournalName == journalName);
        }

        public static void ClearJournalSuggestion()
        {
            var cache = HttpContext.Current.Cache;
            cache.Remove(JournalKey);
        }

        #endregion

        #region Sponsoring Organizations

        private static string SponsorOrgKey = "sponsororgs";

        public static List<SponsorOrgObject> GetSponsoringOrgs(bool enabledOnly = false, int? requiredId = null)
        {
            var cache = HttpContext.Current.Cache;
            List<SponsorOrgObject> data = (List<SponsorOrgObject>)cache[SponsorOrgKey];
            if (data == null)
            {
                data = SponsorOrgObject.GetSponsorOrgs();
                cache.Insert(SponsorOrgKey, data, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration);
            }
            return data.Where(n => (!enabledOnly || n.Active || (requiredId.HasValue && n.SponsorOrgId == requiredId.Value))).ToList();
        }

        public static SelectList SponsoringOrgsList(bool enabledOnly = true, int? requiredId = null)
        {
            return new SelectList(GetSponsoringOrgs(enabledOnly, requiredId).OrderBy(n => n.Name), "SponsorOrgId", "Name");
        }

        public static SelectList SponsoringOrgsNameList(bool enabledOnly = true, int? requiredId = null)
        {
            return new SelectList(GetSponsoringOrgs(enabledOnly, requiredId).OrderBy(n => n.Name), "Name", "Name");
        }

        public static SelectList SponsoringOrgsCodeList(bool enabledOnly = true, int? requiredId = null)
        {
            return new SelectList(GetSponsoringOrgs(enabledOnly, requiredId).OrderBy(n => n.Name), "Code", "Name");
        }

        public static SponsorOrgObject GetSponsoringOrgs(int id)
        {
            return GetSponsoringOrgs()?.FirstOrDefault(x => x.SponsorOrgId == id);
        }

        public static SponsorOrgObject GetSponsoringOrgs(string name)
        {
            return GetSponsoringOrgs()?.FirstOrDefault(x => x.Name == name);
        }

        public static void ClearSponsoringOrgs()
        {
            var cache = HttpContext.Current.Cache;
            cache.Remove(SponsorOrgKey);
        }

        #endregion
    }
}
