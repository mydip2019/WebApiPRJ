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
    /// Offers services for project specific CRUD operations
    /// </summary>
    public class ProjectServices : IProjectServices
    {
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Public constructor.
        /// </summary>
        public ProjectServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private string GetMangerName(int id)
        {
            if (id > 0)
            {            
                var contact = _unitOfWork.ContactRepository.GetByID(id);
                if (contact != null && contact.Id > 0)
                {
                    return string.Format("{0} {1}", contact.Firstname, contact.Lastname);
                }
            }
            return null;
        }

        /// <summary>
        /// Fetches Project details by id
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public BusinessEntities.ProjectEntity GetProjectById(int id)
        {
            var project = _unitOfWork.ProjectRepository.GetByID(id);
            if (project != null)
            {
                Mapper.CreateMap<Project, ProjectEntity>();
                var projectModel = Mapper.Map<Project, ProjectEntity>(project);
                if (projectModel.contactId.HasValue && projectModel.contactId > 0)
                    projectModel.projectManager = GetMangerName(projectModel.contactId.Value);
                
                return projectModel;
            }
            return null;
        }

        /// <summary>
        /// Fetches all the projects.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BusinessEntities.ProjectEntity> GetAllProject()
        {
            var projects = _unitOfWork.ProjectRepository.GetAll().ToList();
            if (projects.Any())
            {
                Mapper.CreateMap<Project, ProjectEntity>();
                var projectsModel = Mapper.Map<List<Project>, List<ProjectEntity>>(projects);
                foreach (var item in projectsModel)
                {
                    if (item.contactId.HasValue && item.contactId > 0)
                        item.projectManager = GetMangerName(item.contactId.Value);

                }

                return projectsModel;
            }
            return null;
        }

        /// <summary>
        /// Creates a project
        /// </summary>
        /// <param name="productEntity"></param>
        /// <returns></returns>
        public int CreateProject(BusinessEntities.ProjectEntity projectEntity)
        {
            using (var scope = new TransactionScope())
            {
                var project = new Project
                {
                    ContactId = projectEntity.contactId,
                    EndDate = projectEntity.endDate,
                    IsSetDate = projectEntity.isSetDate,
                    Priority = projectEntity.priority,
                    ProjectName = projectEntity.projectName,
                    StartDate = projectEntity.startDate
                };
                _unitOfWork.ProjectRepository.Insert(project);
                _unitOfWork.Save();
                scope.Complete();
                return project.Id;
            }
        }

        /// <summary>
        /// Updates a project
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="projectEntity"></param>
        /// <returns></returns>
        public bool UpdateProject(int id, BusinessEntities.ProjectEntity projectEntity)
        {
            var success = false;
            if (projectEntity != null)
            {
                using (var scope = new TransactionScope())
                {
                    var project = _unitOfWork.ProjectRepository.GetByID(id);
                    if (project != null)
                    {
                        project.ContactId = projectEntity.contactId;
                        project.EndDate = projectEntity.endDate;
                        project.IsSetDate = projectEntity.isSetDate;
                        project.Priority = projectEntity.priority;
                        project.ProjectName = projectEntity.projectName;
                        project.StartDate = projectEntity.startDate;

                        _unitOfWork.ProjectRepository.Update(project);
                        _unitOfWork.Save();
                        scope.Complete();
                        success = true;
                    }
                }
            }
            return success;
        }

        /// <summary>
        /// Deletes a particular project
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteProject(int id)
        {
            var success = false;
            if (id > 0)
            {
                using (var scope = new TransactionScope())
                {
                    var project = _unitOfWork.ProjectRepository.GetByID(id);
                    if (project != null)
                    {

                        _unitOfWork.ProjectRepository.Delete(project);
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
