using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

namespace KoiChocoSteamSecondarySubtitle.Patches;

[HarmonyPatch]
public class UIBacklogPatch
{
    [HarmonyPatch(typeof(UIBacklog), "RefreshUI")]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> TranspileUIBacklogRefreshUI(IEnumerable<CodeInstruction> instructions, MethodBase original)
    { 
        var hijack = AccessTools.Method(typeof(UIBacklogPatch), nameof(SplitLangStr));
        
        foreach (var instruction in instructions)
        {
            // We only need to compare the signature of the called function with the signature of the function we want to replace
            if ((instruction.opcode == OpCodes.Call || instruction.opcode == OpCodes.Callvirt)
                && instruction.operand is MethodInfo methodInfo
                && methodInfo.ToString() == hijack.ToString())
            {
                yield return new CodeInstruction(OpCodes.Call, hijack);
                SecondaryLanguage.MyLogger.LogDebug($"Hijacking method {original.Name} in {original.DeclaringType?.Name}");
                continue;
            }
            yield return instruction;
        }
    }

    private static void SplitLangStr(string n, string t, out string usename, out string uestext)
    {
        usename = Utils.SplitLangStr(n, EngineMain.lang);
        if (t.Contains('\u2402'))
        {
            var primary = Utils.SplitLangStr(t, EngineMain.lang);
            var secondary = Utils.SplitLangStr(t, SecondaryLanguage.SecLang);
            uestext = $"<line-height=70%><size=20>{primary}</size>\n<line-height=60%><size=16>{secondary}</size>";
            return;
        }
        uestext = t;
    }
}