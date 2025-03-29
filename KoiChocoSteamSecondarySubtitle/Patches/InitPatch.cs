using HarmonyLib;

namespace KoiChocoSteamSecondarySubtitle.Patches;

[HarmonyPatch]
public class InitPatch
{
    [HarmonyPatch(typeof(EngineMain), "Start")]
    [HarmonyPrefix]
    public static void RegisterSecLang(EngineMain __instance)
    {
        Prefs.subLangOpt = new OptR(PrefGroup.General, "SecLang", 0);
    }
    
    [HarmonyPatch(typeof(EngineMain), "Start")]
    [HarmonyPostfix]
    public static void InitSecLang(EngineMain __instance)
    {
        SecondaryLanguage.SetSecLang((EngineMain.Language)((Prefs.subLangOpt.value - 1 + 5) % 5));
    }
}