using HarmonyLib;

using UnityEngine.EventSystems;

namespace KoiChocoSteamSecondarySubtitle.Patches;

[HarmonyPatch]
public class ConfVisualPatch
{
    [HarmonyPatch(typeof(EngineMain), "InitPGroups")]
    [HarmonyPostfix]
    private static void InjectMyConfExtData(EngineMain __instance)
    {
        var group = (List<PRead>)AccessTools.Field(typeof(PkMain), "group").GetValue(__instance.pkmain);
        group.Insert(0, new CustomPRead());
        SecondaryLanguage.MyLogger.LogDebug($"Injected custom PRead");
    }

    [HarmonyPatch(typeof(UIConfVisual), nameof(UIConfVisual.InitUI))]
    [HarmonyPostfix]
    private static void InitConfVisualUIBinding(UIConfVisual __instance, UIDefine ___uid)
    {
        SecondaryLanguage.MyLogger.LogDebug($"Binding UIConfVisual UI elements");
        foreach (var e in ___uid.subobj)
        {
            var text = e.type == UIDefine.UIElement.elemtype.radio ? e.obj.transform.parent.name : e.Name;
            if (text != "SecLang") continue;
            
            e.AssignTriggerClick(ChangeSecLang);
        }

        return;

        static void ChangeSecLang(BaseEventData evbase)
        {
            var pointerEventData = (PointerEventData)evbase;
            var pointerPress = pointerEventData.pointerPress;
            var name = pointerPress.name;
            var language = name switch
            {
                "en" => EngineMain.Language.en,
                "jp" => EngineMain.Language.jp,
                "cn" => EngineMain.Language.cn,
                "tc" => EngineMain.Language.tc,
                "none" => EngineMain.Language.none,
                _ => EngineMain.Language.none
            };
            SecondaryLanguage.SetSecLang(language);
        }
    }
}