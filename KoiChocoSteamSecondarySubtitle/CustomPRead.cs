using System.Text;

namespace KoiChocoSteamSecondarySubtitle;

public class CustomPRead : PRead
{
    private bool _calling = false;
    private const string ConfAppend = """

                                      radiobegin,SecLang
                                      radio,none,uiradio,111,454
                                      radio,en,uiradio,291,454
                                      radio,jp,uiradio,471,454
                                      radio,cn,uiradio,650,454
                                      radio,tc,uiradio,830,454
                                      radioend
                                      
                                      helprect,SecLang,46,445,1000,40
                                      """;

    private const string HelpAppend = """

                                      #SecLang
                                      Change the secondary subtitle language.
                                      セカンダリ字幕の言語を変更します。
                                      切换次要字幕语言。
                                      切換次要字幕語言。
                                      """;
    
    public override byte[] Data(string fn)
    {
        if (_calling) return null;
        if (fn == "def/ui_conf1.csv") return AppendData(fn, ConfAppend);
        if (fn == "def/help.txt") return AppendData(fn, HelpAppend);
        return null;
    }

    private byte[] AppendData(string fn, string appendLine)
    {
        _calling = true;
        var originalData = EngineMain.engine.pkmain.Get(fn); // Get original data
        _calling = false;
        
        var appendData = Encoding.GetEncoding(65001).GetBytes(appendLine);
        var newData = new byte[originalData.Length + appendData.Length];
        originalData.CopyTo(newData, 0);
        appendData.CopyTo(newData, originalData.Length);
        SecondaryLanguage.MyLogger.LogDebug($"Custom PRead for {fn} is called");
        return newData;
    }
}