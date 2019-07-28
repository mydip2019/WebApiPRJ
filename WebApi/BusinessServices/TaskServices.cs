using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using AutoMapper;
using BusinessEntities;
using DataModel;
using DataModel.UnitOfWork;

namespace BusinessServices
{
    /// <summary>
    /// Offers services for task specific CRUD operations
    /// </summary>
    public class TaskServices : ITaskServices
    {
        private readonly UnitOfWork _unitOfWork;

        /// <summary>
        /// Public constructor.
        /// </summary>
        public TaskServices(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private string GetMangerName(int id)
        {
            var contact = _unitOfWork.ContactRepository.GetByID(id);
            if (contact != null && contact.Id > 0)
            {
                return string.Format("{0} {1}", contact.Firstname, contact.Lastname);
            }
            return null;
        }
        private string GetProjectName(int id)
        {
            var project = _unitOfWork.ProjectRepository.GetByID(id);
            if (project != null && project.Id > 0)
            {
                return project.ProjectName;
            }
            return null;

        }

        /// <summary>
        /// Fetches Task details by id
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public BusinessEntities.TaskEntity GetTaskById(int id)
        {
            var task = _unitOfWork.TaskRepository.GetByID(id);
            if (task != null)
            {
                Mapper.CreateMap<Task, TaskEntity>();
                var taskModel = Mapper.Map<Task, TaskEntity>(task);
                if (taskModel.contactId.HasValue && taskModel.contactId > 0)
                    taskModel.contactName = GetMangerName(taskModel.contactId.Value);

                if (taskModel.projectId.HasValue && taskModel.projectId > 0)
                    taskModel.projectName = GetProjectName(taskModel.projectId.Value);

                return taskModel;
            }
            return null;
        }

        /// <summary>
        /// Fetches all the tasks.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BusinessEntities.TaskEntity> GetAllTask()
        {
            var tasks = _unitOfWork.TaskRepository.GetAll().ToList();
            if (tasks.Any())
            {
                Mapper.CreateMap<Task, TaskEntity>();
                var tasksModel = Mapper.Map<List<Task>, List<TaskEntity>>(tasks);
                foreach (var item in tasksModel)
                {
                    if (item.contactId.HasValue && item.contactId > 0)
                        item.contactName = GetMangerName(item.contactId.Value);

                    if (item.projectId.HasValue && item.projectId > 0)
                        item.projectName = GetProjectName(item.projectId.Value);

                }

                return tasksModel;
            }
            return null;
        }

        /// <summary>
        /// Creates a task
        /// </summary>
        /// <param name="productEntity"></param>
        /// <returns></returns>
        public int CreateTask(BusinessEntities.TaskEntity taskEntity)
        {
            using (var scope = new TransactionScope())
            {
                var task = new Task
                {
                    ContactId = taskEntity.contactId,
                    EndDate = taskEntity.endDate,
                    IsParent = taskEntity.isParent,
                    ParentTaskId = taskEntity.parentTaskId,
                     ProjectId = taskEntity.projectId,
                    Priority = taskEntity.priority,
                    TaskName = taskEntity.taskName,
                    StartDate = taskEntity.startDate,
                    Status =0
                };
                _unitOfWork.TaskRepository.Insert(task);
                _unitOfWork.Save();
                scope.Complete();
                return task.Id;
            }
        }

        public bool EndTask(int id)
        {
            var success = false;
            if (id > 0)
            {
                using (var scope = new TransactionScope())
                {
                    var task = _unitOfWork.TaskRepository.GetByID(id);
                    if (task != null)
                    {
                        task.Status = 1;
                        _unitOfWork.TaskRepository.Update(task);
                        _unitOfWork.Save();
                        scope.Complete();
                        success = true;
                    }
                }
            }
            return success;
        }

        /// <summary>
        /// Updates a task
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="taskEntity"></param>
        /// <returns></returns>
        public bool UpdateTask(int id, BusinessEntities.TaskEntity taskEntity)
        {
            var success = false;
            if (taskEntity != null)
            {
                using (var scope = new TransactionScope())
                {
                    var task = _unitOfWork.TaskRepository.GetByID(id);
                    if (task != null)
                    {
                        task.ContactId = taskEntity.contactId;
                        task.EndDate = taskEntity.endDate;
                        task.IsParent = taskEntity.isParent;
                        task.ParentTaskId = taskEntity.parentTaskId;
                        task.ProjectId = taskEntity.projectId;
                        task.Priority = taskEntity.priority;
                        task.TaskName = taskEntity.taskName;
                        task.StartDate = taskEntity.startDate;
                        _unitOfWork.TaskRepository.Update(task);
                        _unitOfWork.Save();
                        scope.Complete();
                        success = true;
                    }
                }
            }
            return success;
        }

        /// <summary>
        /// Deletes a particular task
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteTask(int id)
        {
            var success = false;
            if (id > 0)
            {
                using (var scope = new TransactionScope())
                {
                    var task = _unitOfWork.TaskRepository.GetByID(id);
                    if (task != null)
                    {

                        _unitOfWork.TaskRepository.Delete(task);
                        _unitOfWork.Save();
                        scope.Complete();
                        success = true;
                    }
                }
            }
            return success;
        }


    }
}
