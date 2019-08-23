using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Script.Serialization;
using System.Web.Services.Description;
using Sort.Business;

namespace Sort.Mvc.Controllers
{
    /// <summary>
    /// REST API for Artifacts
    /// </summary>
    [AllowAnonymous]
    public class ArtifactApiController : ApiController
    {
        /// <summary>
        /// Gets the OSTI information for all the Artifacts
        /// NOTE: Not Implamented yet
        /// </summary>
        /// <returns>JSON string of the OSTI data</returns>
        [Route("~/api/Artifact/"), HttpGet]
        public string Get()
        {
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();

            var results = new
            {
                OstiId = string.Empty,
                OstiDate = string.Empty,
                OstiStatus = string.Empty,
                OstiStatusMsg = string.Empty,
                DoiNumber = string.Empty,
                Status = "InvalidRequest"
            };

            return jsonSerializer.Serialize(results);
        }

        /// <summary>
        /// Get the OSTI information for a given Artifact Id
        /// </summary>
        /// <param name="id">Artifact Id</param>
        /// <returns>JSON serialized OSTI data</returns>
        [Route("~/api/Artifact/{id:int}"), HttpGet]
        public string Get(int id)
        {
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            var sort = SortMainObject.GetSortMainForSharpointId(id);

            var results = new
            {
                OstiId = sort?.OstiId ?? string.Empty,
                OstiDate = sort?.OstiDate?.ToString("MM/dd/yyyy hh:mm:ss tt") ?? string.Empty,
                OstiStatus = sort?.OstiStatus ?? string.Empty,
                OstiStatusMsg = sort?.OstiStatusMsg ?? string.Empty,
                DoiNumber = sort?.DoiNum ?? string.Empty,
                Status = sort?.StatusEnum.ToString() ?? "NotFound"
            };

            return jsonSerializer.Serialize(results);
        }

        /// <summary>
        /// Add a new Artifact to the system for processing
        /// </summary>
        /// <param name="data">Artifact Data</param>
        /// <returns>Sort ID</returns>
        /// <response code="200">OK</response>
        /// <response code="404">The information was not found.</response>
        /// <response code="500">System Exception.</response>
        [ResponseType(typeof(int?)), Route("~/api/Artifact/"), HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] ArtifactData data)
        {
            return await Task.FromResult(AddNewArtifact(data));
        }


        [ResponseType(typeof(bool)), Route("~/api/Ping/"), HttpPost]
        public async Task<IHttpActionResult> Ping()
        {
            return Ok(true);
        }

        /// <summary>
        /// Marks a Artifact as Cancelled.
        /// </summary>
        /// <param name="id">Sort Id</param>
        /// <returns>bool</returns>
        [ResponseType(typeof(bool)), Route("~/api/Artifact/{id:int}"), HttpDelete]
        public async Task<IHttpActionResult> Delete(int id)
        {
            return await Task.FromResult(DeleteArtifact(id));
        }

        #region Private Functions

        private IHttpActionResult AddNewArtifact(ArtifactData artifact)
        {
            int? sortId = null;
            if (artifact != null)
            {
                var sort = SortMainObject.GetSortMainForLrsId(artifact.LrsId);
                if (sort == null)
                {
                    sortId = artifact.Import();
                }
                else if (sort.StatusEnum == StatusEnum.Cancelled)
                {
                    sort.StatusEnum = StatusEnum.Imported;
                    sort.Save();
                    sortId = sort.SortMainId;
                }
            }
            
            return Ok(sortId);
        }

        private IHttpActionResult DeleteArtifact(int sortId)
        {
            bool result = false;

            var sort = SortMainObject.GetSortMain(sortId);
        
            if (sort != null && sort.StatusEnum != StatusEnum.Cancelled)
            {
                ProcessLogObject.Add(sort.SortMainId.Value, "Removed Entry");
                sort.StatusEnum = StatusEnum.Cancelled;
                sort.Save();
                result = true;
            }

            return Ok(result);
        }

        #endregion

    }
}
