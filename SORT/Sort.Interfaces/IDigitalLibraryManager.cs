using System.IO;

namespace Sort.Interfaces
{
    public interface IDigitalLibraryManager
    {
        int UploadFile(string fileName, MemoryStream contents);
        void ReportData(int id, string body);
        string GetDigitalLibraryJson(ISort o);
        byte[] GenerateExportFile(int sortMainId, bool coverPageRequired, ref bool success);
    }
}
