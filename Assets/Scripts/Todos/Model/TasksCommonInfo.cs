using GalaSoft.MvvmLight;

namespace Assets.Scripts.Todos.Model
{
    public class TasksCommonInfo : ObservableObject
    {
        private int _activeTasksCount;
        public int ActiveTasksCount
        {
            get { return _activeTasksCount; }
            set { Set(() => ActiveTasksCount, ref _activeTasksCount, value); }
        }

        private int _complitedTasksCount;
        public int ComplitedTasksCount
        {
            get { return _complitedTasksCount; }
            set { Set(() => ComplitedTasksCount, ref _complitedTasksCount, value); }
        }

        private int _totalTasksCount;
        public int TotalTasksCount
        {
            get { return _totalTasksCount; }
            set { Set(() => TotalTasksCount, ref _totalTasksCount, value); }
        }
    }
}