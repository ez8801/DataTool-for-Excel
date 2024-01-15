using UnityEditor;

namespace EZ.DataTool.View
{
    public interface IEditorWindow
    {
        void Initialize();
    }

    public class ReloadableWindow<T> : EditorWindow where T : EditorWindow, IEditorWindow
    {
        protected static T s_window;

        public static T Window => s_window;

        public virtual string WindowName => string.Empty;

        public virtual void Initialize()
        {

        }

        public virtual void Reload()
        {
            s_window = GetWindow<T>(false, WindowName, true);
            s_window.Initialize();
        }

        public void Update()
        {
            if (s_window == null)
            {
                Reload();
                return;
            }
        }

        //[InitializeOnLoadMethod]
        //private static void Restore()
        //{
        //    if (HasOpenInstances<T>())
        //    {
        //        FocusWindowIfItsOpen<T>();
        //        var window = focusedWindow as T;
        //        s_window = window;
        //        s_window.Initialize();
        //    }
        //}
    }
}