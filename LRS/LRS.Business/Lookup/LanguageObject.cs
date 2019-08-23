using Dapper;
using System.Collections.Generic;
using System.Linq;

namespace LRS.Business
{
    public class LanguageObject
    {
        #region Properties

        public int LanguageId { get; set; }
        public string Language { get; set; }
        public bool Active { get; set; }

        #endregion

        #region Constructor

        public LanguageObject() { }

        #endregion

        #region Repository

        private static ILanguageRepository repo => new LanguageRepository();

        #endregion

        #region Static Methods

        internal static List<LanguageObject> GetLanguages() => repo.GetLanguages();

        internal static LanguageObject GetLanguage(int languageId) => repo.GetLanguage(languageId);

        #endregion

        #region Object Methods

        public void Save()
        {
            repo.SaveLanguage(this);
            MemoryCache.ClearLanguages();
        }

        public void Delete()
        {
            repo.DeleteLanguage(this);
            MemoryCache.ClearLanguages();
        }

        #endregion
    }

    public interface ILanguageRepository
    {
        List<LanguageObject> GetLanguages();
        LanguageObject GetLanguage(int languageId);
        LanguageObject SaveLanguage(LanguageObject language);
        bool DeleteLanguage(LanguageObject language);
    }

    public class LanguageRepository : ILanguageRepository
    {
        public List<LanguageObject> GetLanguages() => Config.Conn.Query<LanguageObject>("SELECT * FROM lu_Language").ToList();

        public LanguageObject GetLanguage(int languageId) => Config.Conn.Query<LanguageObject>("SELECT * FROM lu_Language WHERE LanguageId = @LanguageId", new { LanguageId = languageId }).FirstOrDefault();

        public LanguageObject SaveLanguage(LanguageObject language)
        {
            if (language.LanguageId > 0) // Update
            {
                string sql = @"
                    UPDATE  lu_Language
                    SET     Language = @Language,
                            Active = @Active
                    WHERE   LanguageId = @LanguageId";
                Config.Conn.Execute(sql, language);
            }
            else
            {
                string sql = @"
                    INSERT INTO lu_Language (
                        Language,
                        Active
                    )
                    VALUES (
                        @Language,
                        @Active
                    )
                    SELECT CAST(SCOPE_IDENTITY() AS INT)";
                language.LanguageId = Config.Conn.Query<int>(sql, language).Single();
            }
            return language;
        }

        public bool DeleteLanguage(LanguageObject language)
        {
            try
            {
                Config.Conn.Execute("DELETE FROM lu_Language WHERE LanguageId = @LanguageId", language);
            }
            catch { return false; }
            return true;
        }
    }
}
