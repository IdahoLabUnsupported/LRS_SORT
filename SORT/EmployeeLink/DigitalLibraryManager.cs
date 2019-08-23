using System;
using System.IO;
using Sort.Interfaces;

namespace EmployeeLink
{
    public class DigitalLibraryManager : IDigitalLibraryManager
    {
        public DigitalLibraryManager(string defaultConnectionString, string employeeConnectionString)
        {
            Config.DefaultConnectionString = defaultConnectionString;
            Config.EmployeeConnectionString = employeeConnectionString;
        }

        public byte[] GenerateExportFile(int sortMainId, bool coverPageRequired, ref bool success)
        {
            throw new NotImplementedException();
        }

        public string GetDigitalLibraryJson(ISort o)
        {
            throw new NotImplementedException();
        }

        public void ReportData(int id, string body)
        {
            throw new NotImplementedException();
        }

        public int UploadFile(string fileName, MemoryStream contents)
        {
            throw new NotImplementedException();
        }
    }
}
