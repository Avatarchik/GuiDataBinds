using System;
using Assets.Polarhigh.GuiDataBindings.Collections;

namespace Assets.Scripts.Todos.Model
{
    public class SimpleTaskDataService : ITasksDataService
    {
        private readonly ObservableCollection<TaskItem> _taskItems = new ObservableCollection<TaskItem>();

        public void GetTasks(Action<ObservableCollection<TaskItem>> callback)
        {
            callback(_taskItems);
        }
    }
}