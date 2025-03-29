using HarmonyLib;

using KoiChocoSteamSecondarySubtitle.Behaviours;

namespace KoiChocoSteamSecondarySubtitle.Patches;

[HarmonyPatch]
public class UIAdvPatch
{
    [HarmonyPatch(typeof(UIAdv), "Start")]
    [HarmonyPostfix]
    public static void InjectSecUIAdv(UIAdv __instance)
    {
        __instance.gameObject.AddComponent<SecondaryUIAdv>().Init();
    }
    
    [HarmonyPatch(typeof(UIAdv), nameof(UIAdv.ShowText))]
    [HarmonyPrefix]
    public static void ShowTextMirror(UIAdv __instance, string txin, string dispnamein, bool updateonly)
    {
        var secUIAdv = __instance.GetComponent<SecondaryUIAdv>();
        secUIAdv.ShowText(txin, dispnamein, updateonly);
    }
    
    [HarmonyPatch(typeof(UIAdv), nameof(UIAdv.AdvTextShadow_Update))]
    [HarmonyPostfix]
    public static void AdvTextShadowUpdateMirror(UIAdv __instance)
    {
        var secUIAdv = __instance.GetComponent<SecondaryUIAdv>();
        secUIAdv.AdvTextShadow_Update();
    }

    [HarmonyPatch(typeof(UIAdv), "ReinitLangFont")]
    [HarmonyPrefix]
    public static void ReinitLangFontPrefix(ref bool __runOriginal, UIAdv __instance)
    {
        __runOriginal = SecondaryLanguage.SecLang == EngineMain.Language.none;
        if (__runOriginal) return;

        var advText = __instance.advtext;
        advText.fontSize = 20f;
        advText.characterSpacing = 0f;
        advText.lineSpacing = 54f;
        
        var pos = advText.transform.localPosition;
        pos.y = -730;
        advText.transform.localPosition = pos;
    }
}