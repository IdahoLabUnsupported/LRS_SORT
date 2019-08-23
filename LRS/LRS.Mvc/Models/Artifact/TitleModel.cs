using LRS.Business;

namespace LRS.Mvc.Models
{
    public class TitleModel
    {
        #region Properties

        public int? MainId { get; set; }
        public string Title { get; set; }
        public string DocumentType { get; set; }
        public string JournalName { get; set; }
        public bool ContainsSciInfo { get; set; }
        public bool ContainsTechData { get; set; }
        public bool TechDataPublic { get; set; }
        public bool? Ouo3 { get; set; }
        public bool Ouo3b { get; set; }
        public bool Ouo4 { get; set; }
        public bool Ouo5 { get; set; }
        public bool Ouo6 { get; set; }
        public bool Ouo7 { get; set; }
        public string Ouo7EmployeeId { get; set; }
        public string LimitedExp { get; set; }
        #endregion

        #region Extended Properties

        public bool UserHasAccess => !MainId.HasValue || (MainObject.GetMain(MainId ?? 0)?.UserHasWriteAccess() ?? false);

        #endregion

        #region Constructor

        public TitleModel() { }

        #endregion

        #region Functions
        
        public void Save()
        {
            bool needSave = false;
            bool needPoc = false;

            var o = MainObject.GetMain(MainId??0) ?? new MainObject();
            if (o != null)
            {
                if (!o.MainId.HasValue)
                {
                    o.DocumentType = DocumentType;
                    needSave = true;
                    needPoc = true;
                }

                if (Title?.Trim() != o.Title)
                {
                    o.Title = Title?.Trim();
                    needSave = true;
                }

                if (JournalName?.Trim() != o.JournalName)
                {
                    o.JournalName = JournalName?.Trim();
                    needSave = true;
                }

                if (ContainsSciInfo != o.ContainsSciInfo)
                {
                    o.ContainsSciInfo = ContainsSciInfo;
                    needSave = true;
                }

                if (ContainsTechData != o.ContainsTechData)
                {
                    o.ContainsTechData = ContainsTechData;
                    needSave = true;
                }

                if (TechDataPublic != o.TechDataPublic)
                {
                    o.TechDataPublic = TechDataPublic;
                    needSave = true;
                }

                if (Ouo3.HasValue && Ouo3.Value != o.Ouo3)
                {
                    o.Ouo3 = Ouo3.Value;
                    needSave = true;
                }

                if (Ouo3b != o.Ouo3b)
                {
                    o.Ouo3b = Ouo3b;
                    needSave = true;
                }

                if (Ouo4 != o.Ouo4)
                {
                    o.Ouo4 = Ouo4;
                    needSave = true;
                }

                if (Ouo5 != o.Ouo5)
                {
                    o.Ouo5 = Ouo5;
                    needSave = true;
                }

                if (Ouo6 != o.Ouo6)
                {
                    o.Ouo6 = Ouo6;
                    needSave = true;
                }

                if (Ouo7 != o.Ouo7)
                {
                    o.Ouo7 = Ouo7;
                    needSave = true;
                }

                if (Ouo7EmployeeId != o.Ouo7EmployeeId)
                {
                    o.Ouo7EmployeeId = Ouo7EmployeeId;
                    needSave = true;
                }

                if (DocumentType != o.DocumentType)
                {
                    o.DocumentType = DocumentType;
                    needSave = true;
                }

                if (LimitedExp != o.LimitedExp)
                {
                    o.LimitedExp = LimitedExp;
                    needSave = true;
                }

                if (needSave)
                {
                    o.Save();
                    MainId = o.MainId;
                }

                // Add owner as contact if this was the first time saving.
                if (needPoc && MainId.HasValue)
                {
                    ContactObject.Add(MainId.Value, Current.User.EmployeeId);
                }
            }
        }
        #endregion
    }
}