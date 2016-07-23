using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Todos
{
    public class GuiTaskItemController : MonoBehaviour
    {
        [SerializeField]
        private Text _taskText;

        [SerializeField]
        private Toggle _doneToggle;

        [SerializeField]
        private GameObject _doneOverlayPanel;

        public void DoneChanged()
        {
            if (_doneToggle.isOn)
                _taskText.color = new Color(_taskText.color.r, _taskText.color.g, _taskText.color.b, 0.33f);
            else
                _taskText.color = new Color(_taskText.color.r, _taskText.color.g, _taskText.color.b);

            _doneOverlayPanel.SetActive(_doneToggle.isOn);
        }

    }
}
