#if UNITY_EDITOR
using UnityEditor.UIElements;

namespace EZ.DataTool.View
{
    public class EditorToolbar : Toolbar
    {
        IToolbar _listener;

        public EditorToolbar()
        {
            var fileButton = new ToolbarButton(OnClickFile) { text = "File", name = "File" };
            Add(fileButton);

            Add(new ToolbarSpacer() { flex = true });

            var helpButton = new ToolbarButton(OnClickHelp) { text = "Help", name = "Help" };
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

#endif