using LRS.Business;

namespace LRS.Mvc.Models
{
    public class AbstractModel
    {
        #region Properties

        public int MainId { get; set; }
        public string Abstract { get; set; }

        #endregion

        #region Constructor

        public AbstractModel() { }

        #endregion

        #region Functions

        public void Save()
        {
            bool needSave = false;

            if (MainId > 0)
            {
                var o = MainObject.GetMain(MainId);
                if (o != null)
                {
                    if (!string.IsNullOrWhiteSpace(Abstract) && Abstract.Trim().Length > 5000)
                    {
                        Abstract = Abstract.Trim().Substring(0, 5000);
                    }

                    if (Abstract?.Trim() != o.Abstract)
                    {
                        o.Abstract = Abstract?.Trim();
                        needSave = true;
                    }

                    if (needSave)
                    {
                        o.Save();
                    }
                }
            }
        }

        #endregion
    }
}