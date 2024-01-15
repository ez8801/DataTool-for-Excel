using UnityEngine;
using UnityEngine.UIElements;
using EZ.DataTool;
using EZ.DataTool.View;

public class UIDocumentBehaviour : MonoBehaviour, ICloser
{
    void Start()
    {
        var document = GetComponent<UIDocument>();
        var rootView = new RootView(document.rootVisualElement, true);
        var rootViewController = new RootViewController(rootView, this);
        rootViewController.SetProgressBar(new ProgressBar());
        rootViewController.Refresh(true);
    }

    public void Close()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    public class ProgressBar : IProgressBar
    {
        public void ClearProgressBar()
        {
            
        }

        public void DisplayProgressBar()
        {
            
        }

        public void DisplayProgressBar(float progress)
        {
            
        }
    }
}
