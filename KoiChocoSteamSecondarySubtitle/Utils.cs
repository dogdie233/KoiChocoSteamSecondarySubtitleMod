using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

using MonoMod.RuntimeDetour;
using MonoMod.Utils;

using TMPro;

namespace KoiChocoSteamSecondarySubtitle;

public static class Utils
{
    private static Action<FontLoader, TextMeshProUGUI> _applyFontLoaderSecondaryTo;
    
    public static string SplitLangStr(string str, EngineMain.Language lang)
    {
        if (str == null) return string.Empty;
        var start = 0;
        var end = str.IndexOf('\u2402');
        if (end == -1) return str;
        while ((int)lang != 0)
        {
            start = end + 1;
            end = str.IndexOf('\u2402', start);
            if (end == -1) return str.Substring(start);
            lang--;
        }
        
        return str.Substring(start, end - start);
    }

    public static void ApplyFontTo(FontLoader fontLoader, TextMeshProUGUI text)
    {
        if (_applyFontLoaderSecondaryTo == null)
        {
            var originalMethod = AccessTools.Method(typeof(FontLoader), nameof(FontLoader.ApplyTo));
            var patchMethod = AccessTools.Method(typeof(Utils), nameof(PatchFontLoaderApplyTo));
            var harmony = new Harmony("FontLoaderCopier");
            var newMethodInfo = harmony.Patch(originalMethod, transpiler: new HarmonyMethod(patchMethod));
            var fontLoaderExpression = Expression.Parameter(typeof(FontLoader), "fontLoader");
            var textExpression = Expression.Parameter(typeof(TextMeshProUGUI), "text");
            var callExpression = Expression.Call(newMethodInfo, [fontLoaderExpression, textExpression]);
            _applyFontLoaderSecondaryTo = Expression.Lambda<Action<FontLoader, TextMeshProUGUI>>(callExpression, fontLoaderExpression, textExpression).Compile();
        }
        
        _applyFontLoaderSecondaryTo(fontLoader, text);
    }
    
    private static IEnumerable<CodeInstruction> PatchFontLoaderApplyTo(IEnumerable<CodeInstruction> instructions)
    {
        var langField = AccessTools.Field(typeof(EngineMain), nameof(EngineMain.lang));
        var secLangField = AccessTools.Field(typeof(SecondaryLanguage), $"<{nameof(SecondaryLanguage.SecLang)}>k__BackingField");
        foreach (var instruction in instructions)
        {
            if (instruction.opcode == OpCodes.Ldsfld && instruction.operand is FieldInfo fieldInfo && fieldInfo.ToString() == langField.ToString())
                yield return new CodeInstruction(OpCodes.Ldsfld, secLangField);
            yield return instruction;
        }
    }
}