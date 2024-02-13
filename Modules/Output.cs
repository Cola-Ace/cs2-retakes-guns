using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace RetakesGuns.Modules;

public static class Output
{
    private const string Prefix = "[[GREEN]Retakes[DEFAULT]]";
    public static void PrintToChat(CCSPlayerController client, string text)
    {
        if (client == null) return;

        client.PrintToChat(ConvertColor($"{Prefix} {text}"));
    }

    public static void PrintToChatAll(string text)
    {
        Server.PrintToChatAll(ConvertColor($"{Prefix} {text}"));
    }

    public static void PrintToServer(string text)
    {
        Console.WriteLine($"[Retakes] {text}");
    }

    private static string ConvertColor(string text)
    {
        return text
            .Replace("[BLUE]", ChatColors.Blue.ToString())
            .Replace("[BLUEGREY]", ChatColors.BlueGrey.ToString())
            .Replace("[DARKBLUE]", ChatColors.DarkBlue.ToString())
            .Replace("[DARKRED]", ChatColors.DarkRed.ToString())
            .Replace("[DEFAULT]", ChatColors.Default.ToString())
            .Replace("[GOLD]", ChatColors.Gold.ToString())
            .Replace("[GREEN]", ChatColors.Green.ToString())
            .Replace("[GREY]", ChatColors.Grey.ToString())
            .Replace("[LIGHTBLUE]", ChatColors.LightBlue.ToString())
            .Replace("[LIGHTPURPLE]", ChatColors.LightPurple.ToString())
            .Replace("[LIGHTRED]", ChatColors.LightRed.ToString())
            .Replace("[LIGHTYELLOW]", ChatColors.LightYellow.ToString())
            .Replace("[LIME]", ChatColors.Lime.ToString())
            .Replace("[MAGENTA]", ChatColors.Magenta.ToString())
            .Replace("[OLIVE]", ChatColors.Olive.ToString())
            .Replace("[ORANGE]", ChatColors.Orange.ToString())
            .Replace("[PURPLE]", ChatColors.Purple.ToString())
            .Replace("[RED]", ChatColors.Red.ToString())
            .Replace("[SILVER]", ChatColors.Silver.ToString())
            .Replace("[WHITE]", ChatColors.White.ToString())
            .Replace("[YELLOW]", ChatColors.Yellow.ToString());
    }
}