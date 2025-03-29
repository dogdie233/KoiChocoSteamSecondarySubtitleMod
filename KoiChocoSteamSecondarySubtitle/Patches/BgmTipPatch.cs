using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

namespace KoiChocoSteamSecondarySubtitle.Patches;

[HarmonyPatch]
public class BgmTipPatch
{
    [HarmonyPatch(typeof(ScriptOps), nameof(ScriptOps.Op_bgm))]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> TranspileScriptOpsOpBgm(IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        SecondaryLanguage.MyLogger.LogDebug($"Start transpiling {original.Name} in {original.DeclaringType?.Name}");
        var bgmTitleField = AccessTools.Field(typeof(ScriptOps), nameof(ScriptOps.bgmtitle));
        var engineLangField = AccessTools.Field(typeof(EngineMain), "lang");
        var getBgmTipMethod = AccessTools.Method(typeof(BgmTipPatch), nameof(GetBgmTip));
        
        // Find the instructions that load the engineLang field
        var newInstructions = new List<CodeInstruction>(instructions);
        var begin = newInstructions.FindIndex(inst => inst.Is(OpCodes.Ldsfld, engineLangField));
        if (begin == -1) return newInstructions;
        var end = newInstructions.FindIndex(begin, inst => inst.opcode == OpCodes.Ldelem_Ref);
        if (end == -1) return newInstructions;
        
        SecondaryLanguage.MyLogger.LogDebug($"find engineLang field at {begin} and end at {end}");
        if (end - begin < 3) return newInstructions;

        // Nop the instructions between begin and end
        for (var i = begin; i <= end; i++)
            newInstructions[i].opcode = OpCodes.Nop;

        // Call our method to get the tip instead
        newInstructions[end - 3] = new CodeInstruction(OpCodes.Ldarg_0);
        newInstructions[end - 2] = new CodeInstruction(OpCodes.Ldfld, bgmTitleField);
        newInstructions[end - 1] = new CodeInstruction(OpCodes.Ldarg_1);
        newInstructions[end] = new CodeInstruction(OpCodes.Call, getBgmTipMethod);

        SecondaryLanguage.MyLogger.LogDebug($"Transpiled {original.Name} in {original.DeclaringType?.Name}");
        return newInstructions;
    }
    
    private static string GetBgmTip(Dictionary<string, string[]> bgmTitle, string bgmName)
    {
        SecondaryLanguage.MyLogger.LogDebug($"Get bgm tip for {bgmName}");
        var primaryLang = EngineMain.lang;
        var secondaryLang = SecondaryLanguage.SecLang;
        var names = bgmTitle[bgmName];
        if (secondaryLang == EngineMain.Language.none)
            return names[(int)primaryLang];
        return $"{names[(int)primaryLang]}  /  {names[(int)secondaryLang]}";
    }
}