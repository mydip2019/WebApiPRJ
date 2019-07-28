using AttributeRouting;
using AttributeRouting.Web.Http;
using BusinessEntities;
using BusinessServices;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApi.ActionFilters;
using WebApi.ErrorHelper;

namespace WebApi.Controllers
{
    [AuthorizationRequired]
    [RoutePrefix("v1/Projects")]
    public class ProjectController : ApiController
    {
        #region Private variable.

        private readonly IProjectServices _projectServices;

        #endregion

        #region Public Constructor

        /// <summary>
        /// Public constructor to initialize Project service instance
        /// </summary>
        public ProjectController(IProjectServices projectServices)
        {
            _projectServices = projectServices;
        }

        #endregion

        // GET api/Project
        [GET("allProjects")]
        [GET("All")]
        public HttpResponseMessage Get()
        {
            var projects = _projectServices.GetAllProject();
            var contacEntities = projects as List<ProjectEntity> ?? projects.ToList();
            if (contacEntities.Any())
                return Request.CreateResponse(HttpStatusCode.OK, contacEntities);
            throw new ApiDataException(1000, "Project not found", HttpStatusCode.NotFound);
        }

        // GET api/project/5

        [GET("Project/{id?}")]
        [GET("particularproject/{id?}")]
        [GET("myproject/{id:range(1, 3)}")]
        public HttpResponseMessage Get(int id)
        {
            if (id != null && id > 0)
            {
                var project = _projectServices.GetProjectById(id);
                if (project != null)
                    return Request.CreateResponse(HttpStatusCode.OK, project);

                throw new ApiDataException(1001, "No project found for this id.", HttpStatusCode.NotFound);
            }
            throw new ApiException() { ErrorCode = (int)HttpStatusCode.BadRequest, ErrorDescription = "Bad Request..." };
        }

        // POST api/project
        [POST("Create")]
        [POST("Register")]
        public int Post([FromBody] ProjectEntity projectEntity)
        {
            return _projectServices.CreateProject(projectEntity);
        }

        // PUT api/project/5
        [PUT("Update/{id}")]
        [PUT("Modify/projectid/{id}")]
        public bool Put(int id, [FromBody] ProjectEntity projectEntity)
        {
            if (id > 0)
            {
                return _projectServices.UpdateProject(id, projectEntity);
            }
            return false;
        }

        // DELETE api/project/5
        [DELETE("Remove/{id}")]
        [DELETE("clear/projectid/{id}")]
        [PUT("Delete/{id}")]
        public bool Delete(int id)
        {
            if (id != null && id > 0)
            {
                var isSuccess = _projectServices.DeleteProject(id);
                if (isSuccess)
                {
                    return isSuccess;
                }
                throw new ApiDataException(1002, "project is already deleted or not exist in system.", HttpStatusCode.NoContent);
            }
            throw new ApiException() { ErrorCode = (int)HttpStatusCode.BadRequest, ErrorDescription = "Bad Request..." };
        }
    }
}
