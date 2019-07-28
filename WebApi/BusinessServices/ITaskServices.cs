using BusinessEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessServices
{
    public interface ITaskServices
    {
        TaskEntity GetTaskById(int id);
        IEnumerable<TaskEntity> GetAllTask();
        int CreateTask(TaskEntity taskEntity);
        bool UpdateTask(int id, TaskEntity taskEntity);
        bool EndTask(int id);
        bool DeleteTask(int id);
    }
}
