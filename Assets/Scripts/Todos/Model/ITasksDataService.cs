using System;
using Assets.Polarhigh.GuiDataBindings.Collections;

namespace Assets.Scripts.Todos.Model
{
    public interface ITasksDataService
    {
        void GetTasks(Action<ObservableCollection<TaskItem>> callback);
    }
}