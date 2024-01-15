using System.IO;
using UnityEditor;

namespace EZ.DataTool
{
    public class SpecialFolders
    {
        /// <summary>
        /// Project Folder (Root)
        /// </summary>
        public static string Project;

        /// <summary>
        /// Assets
        /// </summary>
        public static string Assets;

        /// <summary>
        /// Assets/Plugins
        /// </summary>
        public static string Plugins;

        /// <summary>
        /// Assets/Resources
        /// </summary>
        public static string Resources;

        /// <summary>
        /// Assets/HiddenAssets
        /// </summary>
        public static string HiddenAssets;

        /// <summary>
        /// Assets/Scripts
        /// </summary>
        public static string Scripts;

        /// <summary>
        /// Assets/StreamingAssets
        /// </summary>
        public static string StreamingAssets;

        // [InitializeOnLoadMethod]
        [UnityEngine.RuntimeInitializeOnLoadMethod]
        public static void Initialize()
        {
            InitWith(System.Environment.CurrentDirectory);
        }

        public static void InitWith(string projectPath)
        {
            Project = projectPath;
            Assets = Path.Combine(Project, "Assets");

            // Assets Children
            Plugins = Path.Combine(Assets, "Plugins");
            Resources = Path.Combine(Assets, "Resources");
            Scripts = Path.Combine(Assets, "Scripts");
            HiddenAssets = Path.Combine(Assets, "HiddenAssets");
            StreamingAssets = Path.Combine(Assets, "StreamingAssets");

            //UnityEngine.Debug.Log("Project: " + Project);
            //UnityEngine.Debug.Log("Assets: " + Assets);
            //UnityEngine.Debug.Log("Plugins: " + Plugins);
            //UnityEngine.Debug.Log("Resources: " + Resources);
            //UnityEngine.Debug.Log("Scripts: " + Scripts);
            //UnityEngine.Debug.Log("HiddenAssets: " + HiddenAssets);
            //UnityEngine.Debug.Log("StreamingAssets: " + StreamingAssets);
        }
    }
}