using System.Linq;
using Assets.Polarhigh.GuiDataBindings;
using Assets.Polarhigh.GuiDataBindings.Collections;
using Assets.Scripts.Todos.Model;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.Todos
{
    public class TaskViewModel : MonoBehaviour
    {
        [SerializeField]
        private GuiComponentsCollection _guiTasksList;

        [SerializeField]
        private GuiComponent _guiTasksCommonInfo;

        [SerializeField]
        private InputField _newTask;

        // используйте Zenject или другой ioc фреймворк)
        private readonly ITasksDataService _tasksDataService = new SimpleTaskDataService();


        private ObservableCollection<TaskItem> _taskItems;
        private readonly TasksCommonInfo _tasksCommonInfo = new TasksCommonInfo();

        public void OnNewTaskSubmit()
        {
            if (_newTask.text.Length > 0)
            {
                _taskItems.Add(new TaskItem { Task = _newTask.text });
                UpdateCommonTasksInfo();

                // очистить поле ввода и установить на него фокус
                _newTask.text = string.Empty;
                EventSystem.current.SetSelectedGameObject(_newTask.gameObject, null);
            }
        }

        public void OnClearCompletedTasks()
        {
            _taskItems.RemoveAll(item => item.Done);

            UpdateCommonTasksInfo();
        }

        private void UpdateCommonTasksInfo()
        {
            _tasksCommonInfo.ActiveTasksCount = _taskItems.Count(item => !item.Done);
            _tasksCommonInfo.TotalTasksCount = _taskItems.Count;
            _tasksCommonInfo.ComplitedTasksCount = _tasksCommonInfo.TotalTasksCount - _tasksCommonInfo.ActiveTasksCount;
        }

        private void Start()
        {
            _tasksDataService.GetTasks(items => _taskItems = items);

            // 
            _guiTasksCommonInfo.BindDataSource(_tasksCommonInfo);

            //
            _guiTasksList.BindDataSource(_taskItems);
            _guiTasksList.AddEventListener("Done", "onValueChanged", (data, args) => UpdateCommonTasksInfo());
            _guiTasksList.AddEventListener("Remove", "onClick", (data, args) =>
            {
                _taskItems.RemoveAt(data.Index);
                UpdateCommonTasksInfo();
            });
        }
    }
}
