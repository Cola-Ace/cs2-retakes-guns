using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace RetakesGuns.Modules;

public static class Utils
{
    public static bool IsValidPlayer(CCSPlayerController? player)
    {
        return player != null && player.IsValid;
    }

    public static bool IsPlayer(CCSPlayerController? player)
    {
        return IsValidPlayer(player) && !player.IsBot;
    }

    public static bool IsWarmup()
    {
        var GameRulesEntities = Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules");
        var GameRules = GameRulesEntities.First().GameRules;

        if (GameRules == null) throw new Exception("[Retakes] Game rule not found!");

        return GameRules.WarmupPeriod;
    }

    public static int GetPlayerCounts(bool includeSpectator = false)
    {
        var players = Utilities.GetPlayers();
        int count = 0;

        foreach (var player in players)
        {
            if (!IsPlayer(player) || player.Team == CsTeam.None || includeSpectator ? false : player.Team == CsTeam.Spectator) continue;

            count++;
        }

        return count;
    }
}