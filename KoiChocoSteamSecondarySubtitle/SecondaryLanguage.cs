using BepInEx;
using BepInEx.Logging;

using HarmonyLib;

using KoiChocoSteamSecondarySubtitle.Behaviours;

namespace KoiChocoSteamSecondarySubtitle
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class SecondaryLanguage : BaseUnityPlugin
    {
        internal static ManualLogSource MyLogger { get; private set; }
        public static EngineMain.Language SecLang { get; internal set; }
        
        private void Awake()
        {
            MyLogger = Logger;
            // Plugin startup logic
            Harmony.CreateAndPatchAll(typeof(SecondaryLanguage).Assembly);
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        internal static void SetSecLang(EngineMain.Language lang)
        {
            if (SecLang == lang) return;
            
            SecLang = lang;
            
            // UIAdvSec
            UIDefContainer.ReloadAll();
            Prefs.subLangOpt.Set(lang == EngineMain.Language.none ? 0 : (int)lang + 1);
            EngineMain.engine.advmain.GetComponent<SecondaryUIAdv>().ReinitUI();
            MyLogger.LogInfo($"Secondary language is: {lang}");
        }
    }
}