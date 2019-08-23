using System.IO;
using System.Reflection;
using System.Web.Mvc;

namespace Sort.EmployeeHelper.Controllers
{
    public class HelperController : Controller
    {
        private FileResult getFile(string path)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(path))
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return File(ms.ToArray(), mimeType(path));
            }
        }

        [OutputCache(Duration = 3600, VaryByParam = "file")]
        public ActionResult Css(string file)
        {
            return getFile($"Sort.EmployeeHelper.Dependencies.Css.{file}");
        }

        [OutputCache(Duration = 3600, VaryByParam = "file")]
        public ActionResult Script(string file)
        {
            return getFile($"Sort.EmployeeHelper.Dependencies.Script.{file}");
        }

        private static string mimeType(string file)
        {
            switch (System.IO.Path.GetExtension(file).ToLower())
            {
                case ".js":
                    return "text/javascript";
                case ".css":
                    return "text/css";
                case ".png":
                    return "image/png";
                case ".jpg":
                    return "image/jpeg";
                default:
                    return "text/unknown";
            }
        }
    }
}
