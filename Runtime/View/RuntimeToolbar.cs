using UnityEngine.UIElements;

namespace EZ.DataTool.View
{
    public interface IToolbar
    {
        void OnClickFile();
        void OnClickHelp();
    }

    public class RuntimeToolbar : VisualElement
    {
        IToolbar _listener;

        public RuntimeToolbar()
        {
            style.flexDirection = FlexDirection.Row;
            var fileButton = new Button(OnClickFile) { text = "File", name = "File" };
            Add(fileButton);

            var toolbarSpacer = new VisualElement();
            toolbarSpacer.style.flexShrink = 1;
            toolbarSpacer.style.flexGrow = 1;
            Add(toolbarSpacer);

            var helpButton = new Button(OnClickHelp) { text = "Help", name = "Help" };
            Add(helpButton);
        }

        public void SetListener(IToolbar l)
        {
            _listener = l;
        }

        #region UIActions

        public void OnClickFile()
        {
            _listener?.OnClickFile();
        }

        public void OnClickHelp()
        {
            _listener?.OnClickHelp();
        }

        #endregion UIActions
    }
}
