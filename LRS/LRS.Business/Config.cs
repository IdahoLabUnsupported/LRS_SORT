using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Diagnostics;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Dapper;
using System.Web.Caching;
using EmployeLink;
using LRS.Interfaces;

namespace LRS.Business
{
    public class Config
    {
        #region ConnectionString

        public static string ConnectionString => System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
        public static string EmployeeConnectionString => System.Configuration.ConfigurationManager.ConnectionStrings["EmployeeConnection"].ToString();

        #endregion

        #region Connections

        public static IDbConnection Conn => new SqlConnection(ConnectionString);

        public static IDbConnection EmployeeConn => new SqlConnection(EmployeeConnectionString);

        public static bool IsImport = false;

        #endregion

        #region Employee Manager

        private static IEmployeeManager _employeeManager;

        public static IEmployeeManager EmployeeManager
        {
            get
            {
                if (_employeeManager == null)
                {
                    _employeeManager = new EmployeeManager(EmployeeConnectionString);
                }

                return _employeeManager;
            }
        }

        #endregion

        #region ApplicationMode

        public static ApplicationMode ApplicationMode
        {
            get
            {
                var a = ConfigurationManager.AppSettings["ApplicationMode"];
                if (a == null)
                    return ApplicationMode.Development;
                return (ApplicationMode)Enum.Parse(typeof(ApplicationMode), a);
            }
        }

        public static string IpdsUrl => ConfigurationManager.AppSettings["IpdsUrl"] ?? "https://localhost:46125/";

        public static string LoiessUrl => ConfigurationManager.AppSettings["LoiessUrl"] ?? "http://localhost:46565/";

        public static string DefaultFundingContractNumber => ConfigurationManager.AppSettings["FundContractNumber"] ?? "";

        public static string SiteUrl => ConfigurationManager.AppSettings["SitUrl"] ?? "http://localhost:46202/";

        public static string SortUrl => ConfigurationManager.AppSettings["SortUrl"] ?? "http://localhost:46111/";

        

        private static ExecutionMode? executionMode;
        public static ExecutionMode ExecutionMode
        {
            get
            {
                if (!executionMode.HasValue)
                    executionMode = Process.GetCurrentProcess().ProcessName.ToLower().Contains("iisexpress") ? ExecutionMode.Local : ExecutionMode.Server;
                return executionMode.Value;
            }
        }

        #endregion

        #region Email Parts

        public static string OwnerEmail => ConfigurationManager.AppSettings["OwnerEmail"] ?? "";

        public static string DeveloperEmail => ConfigurationManager.AppSettings["DeveloperEmail"] ?? "";

        public static string FromAddress => ConfigurationManager.AppSettings["FromEmailAddress"] ?? "";

        #endregion

        #region Static Helpers

        private static System.Web.Caching.Cache cache => HttpContext.Current.Cache;
        private static readonly string CONFIGCACHEKEY = "configcache";
        private static object locker = new object();

        private static Dictionary<string, ConfigValue> configCache
        {
            get
            {
                Dictionary<string, ConfigValue> toReturn;
                toReturn = (Dictionary<string, ConfigValue>)cache[CONFIGCACHEKEY] ?? loadCache();
                
                return toReturn;
            }
        }

        private static Dictionary<string, ConfigValue> loadCache()
        {
            Dictionary<string, ConfigValue> freshCache = null;
            lock (locker)
            {
                freshCache = Conn.Query<ConfigSetting>("SELECT ConfigKey AS [Key], ConfigValue AS DefaultValue, Encrypted as Encrypt FROM dat_Config").Select(n => new KeyValuePair<string, ConfigValue>(n.Key, new ConfigValue(n.DefaultValue, n.Encrypt, false))).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                cache.Remove(CONFIGCACHEKEY);
                cache.Insert(CONFIGCACHEKEY, freshCache, null, DateTime.Now.AddMinutes(3), Cache.NoSlidingExpiration);
            }

            return freshCache;
        }

        public static string getValue(ConfigSetting setting)
        {
            string toReturn = setting.DefaultValue;

            ConfigValue stored = null;
            if (configCache.TryGetValue(setting.Key, out stored))
                toReturn = stored.Value;
            else
            {
                setValue(setting);
            }

            return toReturn;
        }

        public static void setValue(ConfigSetting setting)
        {
            lock (locker)
            {
                // remove the existing value then insert the new value
                Config.Conn.Execute(@"
                    DELETE FROM dat_Config WHERE ConfigKey = @ConfigKey;
                    INSERT INTO dat_Config (ConfigKey, ConfigValue, Encrypted) VALUES (@ConfigKey, @ConfigValue, @Encrypted)", new { ConfigKey = setting.Key, ConfigValue = ConfigValue.EncryptString(setting.Encrypt, setting.DefaultValue), Encrypted = setting.Encrypt });
                // add to the cached dictionary without forcing a full reload
                Dictionary<string, ConfigValue> cached = (Dictionary<string, ConfigValue>) cache[CONFIGCACHEKEY];

                if (cached != null)
                {
                    cached.Remove(setting.Key);
                    cached.Add(setting.Key, new ConfigValue(setting.DefaultValue, setting.Encrypt, true));
                }
            }
        }

        #endregion
    }

    internal class ConfigValue
    {
        static readonly string PasswordHash = "@d2sDDAP@99@Sw0YY$dkraa32Pd";
        static readonly string SaltKey = "A3822j_c@1dk!&6kkS@LDEWEadT&K%(Ed)Y";
        static readonly string VIKey = "@(B2c3D#e3F6g7H1";

        private string _value = string.Empty;
        public string Value
        {
            get { return DecryptString(Encrypted, _value); }
            set { _value = value; }
        }
        public bool Encrypted { get; private set; }

        public ConfigValue() { }

        public ConfigValue(string value, bool encrypted, bool encryptValue)
        {
            this.Encrypted = encrypted;
            if (encryptValue)
            {
                this.Value = EncryptString(encrypted, value);
            }
            else
            {
                this.Value = value;
            }
        }

        public static string EncryptString(bool encrypt, string value)
        {
            if (!encrypt)
                return value;

            byte[] plainTextBytes = Encoding.UTF8.GetBytes(value);

            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));

            byte[] cipherTextBytes;

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherTextBytes = memoryStream.ToArray();
                    cryptoStream.Close();
                }
                memoryStream.Close();
            }
            return Convert.ToBase64String(cipherTextBytes);
        }

        public static string DecryptString(bool encrypted, string value)
        {
            if (!encrypted)
                return value;

            byte[] cipherTextBytes = Convert.FromBase64String(value);
            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

            var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));
            var memoryStream = new MemoryStream(cipherTextBytes);
            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
        }
    }

    public class ConfigSetting
    {
        public string Key { get; set; }
        public string DefaultValue { get; set; }
        public bool Encrypt { get; set; }

        public ConfigSetting(string key, string defaultValue) : this( key, defaultValue, false )
        {
        }

        public ConfigSetting(string key, string defaultValue, bool encrypt) 
        {
            this.Key = key;
            this.DefaultValue = defaultValue;
            this.Encrypt = encrypt;
        }

        public ConfigSetting SetValue(string value)
        {
            DefaultValue = value;
            return this;
        }
    }

    public enum ApplicationMode
    {
        Development,
        Acceptance,
        Production,
        CyberScan
    }

    public enum ExecutionMode
    {
        Local,
        Server
    }
}
