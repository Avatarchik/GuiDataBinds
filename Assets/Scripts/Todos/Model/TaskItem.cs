using GalaSoft.MvvmLight;

namespace Assets.Scripts.Todos.Model
{
    public class TaskItem : ObservableObject
    {
        private string _task;
        public string Task
        {
            get { return _task; }
            set { Set(() => Task, ref _task, value); }
        }

        private bool _done;
        public bool Done
        {
            get { return _done; }
            set { Set(() => Done, ref _done, value); }
        }
    }
}
