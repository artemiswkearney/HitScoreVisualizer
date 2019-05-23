using Harmony;
using IllusionPlugin;
using System;
using System.Reflection;
using UnityEngine.SceneManagement;

namespace HitScoreVisualizer
{
    public class Plugin : IPlugin
    {
        public string Name => "HitScoreVisualizer";
        public string Version => "2.2.0";

        internal const int majorVersion = 2;
        internal const int minorVersion = 2;
        internal const int patchVersion = 0;

        public void OnApplicationStart()
        {
            SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
            try
            {
                var harmony = HarmonyInstance.Create("com.arti.BeatSaber.HitScoreVisualizer");
                harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception e)
            {
                Console.WriteLine("[HitScoreVisualizer] This plugin requires Harmony. Make sure you " +
                    "installed the plugin properly, as the Harmony DLL should have been installed with it.");
                Console.WriteLine(e);
            }
            Config.load();
        }

        private void SceneManagerOnActiveSceneChanged(Scene arg0, Scene arg1)
        {
        }

        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
        }

        public void OnApplicationQuit()
        {
            SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
            SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
        }

        public void OnLevelWasLoaded(int level)
        {

        }

        public void OnLevelWasInitialized(int level)
        {
        }

        public void OnUpdate()
        {
        }

        public void OnFixedUpdate()
        {
        }

        internal static void log(object message)
        {
#if DEBUG
            Console.WriteLine("[HitScoreVisualizer] " + message);
#endif
        }
    }
}
