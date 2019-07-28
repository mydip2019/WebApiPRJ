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
    [RoutePrefix("v1/Tasks")]
    public class TaskController : ApiController
    {
        #region Private variable.

        private readonly ITaskServices _taskServices;

        #endregion

        #region Public Constructor

        /// <summary>
        /// Public constructor to initialize Task service instance
        /// </summary>
        public TaskController(ITaskServices taskServices)
        {
            _taskServices = taskServices;
        }

        #endregion

        // GET api/Task
        [GET("allTasks")]
        [GET("All")]
        public HttpResponseMessage Get()
        {
            var tasks = _taskServices.GetAllTask();
            var contacEntities = tasks as List<TaskEntity> ?? tasks.ToList();
            if (contacEntities.Any())
                return Request.CreateResponse(HttpStatusCode.OK, contacEntities);
            throw new ApiDataException(1000, "Task not found", HttpStatusCode.NotFound);
        }

        // GET api/task/5

        [GET("Task/{id?}")]
        [GET("particulartask/{id?}")]
        [GET("mytask/{id:range(1, 3)}")]
        public HttpResponseMessage Get(int id)
        {
            if (id != null && id > 0)
            {
                var task = _taskServices.GetTaskById(id);
                if (task != null)
                    return Request.CreateResponse(HttpStatusCode.OK, task);

                throw new ApiDataException(1001, "No task found for this id.", HttpStatusCode.NotFound);
            }
            throw new ApiException() { ErrorCode = (int)HttpStatusCode.BadRequest, ErrorDescription = "Bad Request..." };
        }

        // POST api/task
        [POST("Create")]
        [POST("Register")]
        public int Post([FromBody] TaskEntity taskEntity)
        {
            return _taskServices.CreateTask(taskEntity);
        }

        // PUT api/task/5
        [PUT("Update/{id}")]
        [PUT("Modify/taskid/{id}")]
        public bool Put(int id, [FromBody] TaskEntity taskEntity)
        {
            if (id > 0)
            {
                return _taskServices.UpdateTask(id, taskEntity);
            }
            return false;
        }

        [POST("EndTask/{id}")]
         public bool EndTask(int id)        {
            if (id > 0)
            {
                return _taskServices.EndTask(id);
            }
            return false;
        }

        // DELETE api/task/5
        [DELETE("Remove/{id}")]
        [DELETE("clear/taskid/{id}")]
        [PUT("Delete/{id}")]
        public bool Delete(int id)
        {
            if (id != null && id > 0)
            {
                var isSuccess = _taskServices.DeleteTask(id);
                if (isSuccess)
                {
                    return isSuccess;
                }
                throw new ApiDataException(1002, "task is already deleted or not exist in system.", HttpStatusCode.NoContent);
            }
            throw new ApiException() { ErrorCode = (int)HttpStatusCode.BadRequest, ErrorDescription = "Bad Request..." };
        }
    }
}
