using System;
using System.Collections.Generic;
using System.Linq;
using Sort.Business;

namespace Sort.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            Write("Starting");
            Config.IsConsoleMode = true;
            
            try
            {
                
                ImportFromLrs();
            }
            catch (Exception ex)
            {
                ErrorLogObject.LogError("Console:Main", ex);
                ProcessLogObject.Add("Failed in Main ImportFromLrs", ex.Message);
            }

            try
            {
                ProcessOneYearReminders();
            }
            catch (Exception ex)
            {
                ErrorLogObject.LogError("Console:Main", ex);
                ProcessLogObject.Add("Failed in Main ProcessOneYearReminders", ex.Message);
            }

            try
            {
                ProcessDelayedReminders();
            }
            catch (Exception ex)
            {
                ErrorLogObject.LogError("Console:Main", ex);
                ProcessLogObject.Add("Failed in Main ProcessDelayedReminders", ex.Message);
            }

            try
            {
                ProcessAdLibFileGen();
            }
            catch (Exception ex)
            {
                ErrorLogObject.LogError("Console:Main", ex);
                ProcessLogObject.Add("Failed in Main ProcessAdLibFileGen", ex.Message);
            }

            Config.LastUpdateTime = DateTime.Now;

            Write("Done");
//#if DEBUG
//            System.Console.ReadKey();
//#endif
        }

        private static List<SubjectCategoryObject> _subjectCategories=null;
        private static List<SubjectCategoryObject> SubjectCategories
        {
            get
            {
                if (_subjectCategories == null)
                {
                    _subjectCategories = SubjectCategoryObject.GetSubjectCategories();
                }

                return _subjectCategories;
            }
        }

        private static void ImportFromLrs()
        {
            ProcessLogObject.Add("Retriving LRS Records to Process");
            LrsObject.ImportLatest();
            ProcessLogObject.Add("LRS Successfully Processed.");
        }

        private static void ProcessOneYearReminders()
        {
            var sorts = SortMainObject.GetOneYearReminders();
            if (sorts != null)
            {
                foreach (var sort in sorts)
                {
                    try
                    {
                        if (Email.SendEmail(sort, EmailTypeEnum.FirstYearReminder))
                        {
                            sort.OneYearReminderSent = true;
                            sort.StatusEnum = StatusEnum.Complete;
                            sort.Save();
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorLogObject.LogError("Console:ProcessOneYearReminders", ex);
                    }
                }
            }
        }

        private static void ProcessDelayedReminders()
        {
            var sorts = SortMainObject.GetDeleyedReminders();
            if (sorts != null)
            {
                foreach (var sort in sorts)
                {
                    try
                    {
                        if (Email.SendEmail(sort, EmailTypeEnum.DelayedReminder))
                        {
                            sort.DelayReminderSent = true;
                            sort.Save();
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorLogObject.LogError("Console:ProcessDelayedReminders", ex);
                    }
                }
            }
        }

        private static void ProcessAdLibFileGen()
        {
            ProcessLogObject.Add("Processing AdLib File Generation");
            var sorts = SortMainObject.GetSortNeedingAdlibDocument();
            if (sorts != null && sorts.Count > 0)
            {
                foreach (var sort in sorts)
                {
                    if (SortAttachmentObject.GetFinalDocAttachment(sort.SortMainId.Value) != null)
                    {
                        ProcessLogObject.Add(sort.SortMainId, $"Processing AdLib File");

                        bool success = false;
                        byte[] file = Config.DigitalLibraryManager.GenerateExportFile(sort.SortMainId.Value, sort.CoverPageRequired, ref success);
                        if (success)
                        {
                            SortAttachmentObject.AddOstiAttachment(sort.SortMainId.Value, $"Sort_{sort.SortMainId}.pdf", "System", file.Length, file);
                            ProcessLogObject.Add(sort.SortMainId, $"AdLib File Generation was Successful");
                        }
                        else
                        {
                            ProcessLogObject.Add(sort.SortMainId, $"AdLib File Generation Failed");
                        }
                    }
                }
            }
        }

        private static void Write(string text)
        {
#if DEBUG
            System.Console.WriteLine(text);
#endif
        }
    }
}
