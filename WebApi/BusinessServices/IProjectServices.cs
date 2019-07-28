using BusinessEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessServices
{
    public interface IProjectServices
    {
        ProjectEntity GetProjectById(int id);
        IEnumerable<ProjectEntity> GetAllProject();
        int CreateProject(ProjectEntity projectEntity);
        bool UpdateProject(int id, ProjectEntity projectEntity);
        bool DeleteProject(int id);
    }
}
