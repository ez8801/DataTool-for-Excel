using UnityEngine;
using UnityEditor;
using EZ.DataTool.View;

namespace EZ.DataTool
{
	public class DataToolWindow : ReloadableWindow<DataToolWindow>, 
		IEditorWindow, IProgressBar, ICloser
	{
		private RootView _rootView;
		private RootViewController _rootViewController;
		
		public override string WindowName => "Data Tool";

        [MenuItem("Tools/Data Tool", false)]
		static public void Open()
		{
			s_window = CreateWindow();
			if (s_window != null)
				s_window.Initialize(false);
		}

		static public DataToolWindow CreateWindow()
        {
			var window = GetWindow<DataToolWindow>(false, "Data Tool", true);
			window.minSize = new Vector2(945f, 420f);
			return window;
		}

		public object lockObject = new object();

		public override void Initialize()
        {
			Initialize(false);
		}

		public void Initialize(bool forceUpdateManifest)
		{
			s_window = this;

			CreateViewElements();

			_rootViewController = new RootViewController(_rootView, this);
			_rootViewController.SetProgressBar(this);
			_rootViewController.Refresh(forceUpdateManifest);
		}

		public void DisplayProgressBar(float progress)
		{
#if UNITY_EDITOR
			EditorUtility.DisplayProgressBar("Processing...", string.Empty, progress);
#endif
		}

		public void DisplayProgressBar()
		{
#if UNITY_EDITOR
			EditorUtility.DisplayProgressBar("Processing...", string.Empty, 0.5f);
#endif
		}

		public void ClearProgressBar()
        {
#if UNITY_EDITOR
			EditorUtility.ClearProgressBar();
#endif
		}

        public void CreateViewElements()
		{
			if (_rootView == null)
			{
				_rootView = new RootView(rootVisualElement, false);				
			}
		}
	}
}