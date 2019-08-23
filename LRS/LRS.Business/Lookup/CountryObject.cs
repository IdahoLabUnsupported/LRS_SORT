using Dapper;
using System.Collections.Generic;
using System.Linq;

namespace LRS.Business
{
    public class CountryObject
    {
        #region Properties

        public int CountryId { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public bool Active { get; set; }

        #endregion

        #region Constructor

        public CountryObject() { }

        #endregion

        #region Repository

        private static ICountryRepository repo => new CountryRepository();

        #endregion

        #region Static Methods

        internal static List<CountryObject> GetCountries() => repo.GetCountries();

        internal static CountryObject GetCountry(int countryId) => repo.GetCountry(countryId);

        #endregion

        #region Object Methods

        public void Save()
        {
            repo.SaveCountry(this);
            MemoryCache.ClearCountries();
        }

        public void Delete()
        {
            repo.DeleteCountry(this);
            MemoryCache.ClearCountries();
        }

        #endregion
    }

    public interface ICountryRepository
    {
        List<CountryObject> GetCountries();
        CountryObject GetCountry(int countryId);
        CountryObject SaveCountry(CountryObject country);
        bool DeleteCountry(CountryObject country);
    }

    public class CountryRepository : ICountryRepository
    {
        public List<CountryObject> GetCountries() => Config.Conn.Query<CountryObject>("SELECT * FROM lu_Country").ToList();

        public CountryObject GetCountry(int countryId) => Config.Conn.Query<CountryObject>("SELECT * FROM lu_Country WHERE CountryId = @CountryId", new { CountryId = countryId }).FirstOrDefault();

        public CountryObject SaveCountry(CountryObject country)
        {
            if (country.CountryId > 0) // Update
            {
                string sql = @"
                    UPDATE  lu_Country
                    SET     Country = @Country,
                            CountryCode = @CountryCode,
                            Active = @Active
                    WHERE   CountryId = @CountryId";
                Config.Conn.Execute(sql, country);
            }
            else
            {
                string sql = @"
                    INSERT INTO lu_Country (
                        Country,
                        CountryCode,
                        Active
                    )
                    VALUES (
                        @Country,
                        @CountryCode,
                        @Active
                    )
                    SELECT CAST(SCOPE_IDENTITY() AS INT)";
                country.CountryId = Config.Conn.Query<int>(sql, country).Single();
            }
            return country;
        }

        public bool DeleteCountry(CountryObject country)
        {
            try
            {
                Config.Conn.Execute("DELETE FROM lu_Country WHERE CountryId = @CountryId", country);
            }
            catch { return false; }
            return true;
        }
    }
}
