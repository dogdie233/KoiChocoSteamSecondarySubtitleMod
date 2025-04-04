﻿using HarmonyLib;

using TMPro;

using UnityEngine;

namespace KoiChocoSteamSecondarySubtitle.Patches;

[HarmonyPatch]
public class AdvChoicePatch
{
    [HarmonyPatch(typeof(AdvChoice), MethodType.Constructor)]
    [HarmonyPostfix]
    public static void AdvChoiceConstructorPostfix(AdvChoice __instance, GameObject ___chgrp)
    {
        for (var i = 0; i < ___chgrp.transform.childCount; i++)
        {
            var child = ___chgrp.transform.GetChild(i);
            if (!child.gameObject.name.StartsWith("choice")) continue;
            var tmp = child.GetComponentInChildren<TextMeshProUGUI>();
            tmp.lineSpacing = 0f;
        }
    }
    
    [HarmonyPatch(typeof(AdvChoice), nameof(AdvChoice.ShowMenu))]
    [HarmonyPrefix]
    public static void ShowMenuPrefix(List<string>[] mplist, ref List<string> __state)
    {
        if (mplist.Length <= 1 || SecondaryLanguage.SecLang == EngineMain.Language.none) return;
        var primaryLangChoices = mplist[(int)EngineMain.lang];
        var secondaryLangChoices = mplist[(int)SecondaryLanguage.SecLang];
        var newList = new List<string>(primaryLangChoices);  // 复制一个新的列表
        for (var i = 0; i < newList.Count && i < primaryLangChoices.Count; i++)
            newList[i] = $"<size=18>{primaryLangChoices[i]}</size>\n<size=14>{secondaryLangChoices[i]}</size>";
        __state = primaryLangChoices;
        mplist[(int)EngineMain.lang] = newList;
    }
    
    [HarmonyPatch(typeof(AdvChoice), nameof(AdvChoice.ShowMenu))]
    [HarmonyPostfix]
    public static void ShowMenuPostfix(List<string>[] mplist, ref List<string> __state)
    {
        if (__state == null || mplist.Length <= 1 || SecondaryLanguage.SecLang == EngineMain.Language.none) return;
        mplist[(int)EngineMain.lang] = __state;
    }
}