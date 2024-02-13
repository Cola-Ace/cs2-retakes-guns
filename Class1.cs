using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using Dapper;
using Microsoft.Data.Sqlite;

using RetakesGuns.Modules;

namespace RetakesGuns;

public class PlayerWeaponsList
{
    public string CTPrimary = "m4a1";
    public string TPrimary = "ak";
    public string CTSecondary = "usp";
    public string TSecondary = "glock";
    public bool AWP = false;
}

public class RetakesGuns : BasePlugin
{
    public override string ModuleName => "Retakes - Guns";
    public override string ModuleAuthor => "Xc_ace";
    public override string ModuleVersion => "1.0";
    public override string ModuleDescription => "";

    private Dictionary<string, Dictionary<string, string>> WeaponsList = new Dictionary<string, Dictionary<string, string>>();

    private Dictionary<string, PlayerWeaponsList> PlayerWeapons = new Dictionary<string, PlayerWeaponsList>();

    private SqliteConnection _connection = null;

    public override void Load(bool hotReload)
    {
        // init weapons
        // string[] SameNameWeapon = new string[] { "galiar", "glock", "tec9", "aug", "famas", "p2000", "fn57", "cz75", "p250", "deagle" };
        // foreach (string weapon in SameNameWeapon)
        // {
        //     WeaponsList.Add(weapon, new Dictionary<string, string>{ { "classname", $"weapon_{weapon}" } });
        // }

        // p -> primary , s -> secondary , g -> generic
        WeaponsList.Add("ak", new Dictionary<string, string> { { "classname", "weapon_ak47" }, { "team", "t" }, { "type", "p" } });
        WeaponsList.Add("sg553", new Dictionary<string, string> { { "classname", "weapon_sg556" }, { "team", "t" }, { "type", "p" } });
        WeaponsList.Add("m4a4", new Dictionary<string, string> { { "classname", "weapon_m4a1" }, { "team", "ct" }, { "type", "p" } });
        WeaponsList.Add("m4a1", new Dictionary<string, string> { { "classname", "weapon_m4a1_sliencer" }, { "team", "ct" }, { "type", "p" } });
        WeaponsList.Add("usp", new Dictionary<string, string> { { "classname", "weapon_usp_sliencer" }, { "team", "ct" }, { "type", "s" } });
        WeaponsList.Add("r8", new Dictionary<string, string> { { "classname", "weapon_revolver" }, { "team", "g" }, { "type", "s" } });

        WeaponsList.Add("galiar", new Dictionary<string, string> { { "classname", "weapon_galiar" }, { "team", "t" }, { "type", "p" } });
        WeaponsList.Add("glock", new Dictionary<string, string> { { "classname", "weapon_glock" }, { "team", "t" }, { "type", "s" } });
        WeaponsList.Add("tec9", new Dictionary<string, string> { { "classname", "weapon_tec9" }, { "team", "t" }, { "type", "s" } });
        WeaponsList.Add("aug", new Dictionary<string, string> { { "classname", "weapon_aug" }, { "team", "ct" }, { "type", "p" } });
        WeaponsList.Add("famas", new Dictionary<string, string> { { "classname", "weapon_famas" }, { "team", "ct" }, { "type", "p" } });
        WeaponsList.Add("p2000", new Dictionary<string, string> { { "classname", "weapon_p2000" }, { "team", "ct" }, { "type", "s" } });
        WeaponsList.Add("fn57", new Dictionary<string, string> { { "classname", "weapon_fn57" }, { "team", "ct" }, { "type", "s" } });
        WeaponsList.Add("cz75", new Dictionary<string, string> { { "classname", "weapon_cz75" }, { "team", "g" }, { "type", "s" } });
        WeaponsList.Add("p250", new Dictionary<string, string> { { "classname", "weapon_p250" }, { "team", "g" }, { "type", "s" } });
        WeaponsList.Add("deagle", new Dictionary<string, string> { { "classname", "weapon_deagle" }, { "team", "g" }, { "type", "s" } });

        // SQLite
        string sql = @"
            CREATE TABLE IF NOT EXISTS `weapons` (
              `steamid` text(32) NOT NULL,
              `ct_primary` TEXT(32) NOT NULL DEFAULT m4a4,
              `t_primary` TEXT(32) NOT NULL DEFAULT ak,
              `ct_secondary` TEXT(32) NOT NULL DEFAULT usp,
              `t_secondary` TEXT(32) NOT NULL DEFAULT glock,
              `awp` integer(1) NOT NULL DEFAULT 0,
              PRIMARY KEY (`steamid`)
            );
        ";

        _connection = new SqliteConnection($"Data Source={Path.Join(ModuleDirectory, "retakes.db")}");
        _connection.Open();

        Task.Run(async () =>
        {
            await _connection.ExecuteAsync(sql); // init database
        });


    }

    // Guns Settings
    [ConsoleCommand("css_guns", "Show guns")]
    [ConsoleCommand("css_gun", "Show guns")]
    [CommandHelper(whoCanExecute: CommandUsage.CLIENT_ONLY)]
    public void OnGunsCalled(CCSPlayerController? caller, CommandInfo info)
    {
        if (caller == null) return;

        Output.PrintToChat(caller, "输入指令 [GREEN]!weapon <武器名>[DEFAULT] 并将 [GREEN]<武器名>[DEFAULT] 替换为以下字符");
        Output.PrintToChat(caller, "可在对应回合使用你选择的武器:");
        Output.PrintToChat(caller, "[GREEN]T[DEFAULT]: ak galiar sg553");
        Output.PrintToChat(caller, "[GREEN]T[DEFAULT]: glock tec9");
        Output.PrintToChat(caller, "[GREEN]CT[DEFAULT]: m4a4 m4a1 aug famas");
        Output.PrintToChat(caller, "[GREEN]CT[DEFAULT]: usp p2000 fn57");
        Output.PrintToChat(caller, "[GREEN]通用武器[DEFAULT]: cz75 p250 deagle r8");
        Output.PrintToChat(caller, "输入 [GREEN]!awp[DEFAULT] 有机会获取AWP");
    }

    [ConsoleCommand("css_weapon", "Change Weapon")]
    [CommandHelper(minArgs: 1, usage: "[weapon]", whoCanExecute: CommandUsage.CLIENT_ONLY)]
    public void ChangeWeapon(CCSPlayerController? caller, CommandInfo info)
    {
        if (caller == null) return;

        string weapon = info.GetArg(1);
        if (!WeaponsList.ContainsKey(weapon))
        {
            Output.PrintToChat(caller, "输入的武器并不存在");
            return;
        }

        PlayerWeapons[caller.AuthorizedSteamID.SteamId3].AWP = !PlayerWeapons[caller.AuthorizedSteamID.SteamId3].AWP;
        Output.PrintToChat(caller, $"下回合开始[GREEN]你将{(PlayerWeapons[caller.AuthorizedSteamID.SteamId3].AWP ? "有机会获得AWP":"不再获得AWP")}[DEFAULT]");
    }

    [ConsoleCommand("css_awp", "Switch AWP")]
    [CommandHelper(whoCanExecute: CommandUsage.CLIENT_ONLY)]
    public void SwitchAWP(CCSPlayerController? caller, CommandInfo info)
    {
        if (caller == null) return;


    }

    [GameEventHandler]
    public HookResult OnRoundPostStart(EventRoundPoststart @event, GameEventInfo info)
    {
        if (Utils.IsWarmup()) return HookResult.Continue;

        var PistolRound = 10;

        Random random = new Random();
        int RoundType = random.Next(1, 101) <= 10 ? 0 : 1; // 0 为手枪局, 1 为全起局
        int Bombsite = random.Next(0, 2); // 0 为 A, 1 为 B

        foreach (var player in Utilities.GetPlayers())
        {
            if (player == null) continue;

            var itemServices = new CCSPlayer_ItemServices(player.PlayerPawn.Value.ItemServices.Handle);

            player.RemoveWeapons();

            var steamid = player.AuthorizedSteamID.SteamId3;

            CsTeam team = player.Team;

            switch (team)
            {
                case CsTeam.Terrorist:
                    {
                        player.GiveNamedItem(PlayerWeapons[steamid].TSecondary);

                        break;
                    }

                case CsTeam.CounterTerrorist:
                    {
                        player.GiveNamedItem(PlayerWeapons[steamid].CTSecondary);

                        break;
                    }
            }

            switch (RoundType)
            {
                case 0: // 手枪局
                    {
                        if ((team == CsTeam.CounterTerrorist && PlayerWeapons[steamid].CTSecondary == "weapon_usp_sliencer")
                            || (team == CsTeam.Terrorist && PlayerWeapons[steamid].TSecondary == "weapon_glock")) 
                            itemServices.HasHelmet = true;

                        break;
                    }

                case 1: // 全起局
                    {
                        itemServices.HasHeavyArmor = true;
                        if (team == CsTeam.CounterTerrorist) itemServices.HasDefuser = true;
                        player.GiveNamedItem(PlayerWeapons[steamid].CTPrimary);

                        break;
                    }
            }
        }

        return HookResult.Continue;
    }

    [GameEventHandler]
    public HookResult OnPlayerConnectFull(EventPlayerConnectFull @event, GameEventInfo info)
    {
        var player = @event.Userid;

        if (player == null) return HookResult.Continue;

        string steamid = player.AuthorizedSteamID.SteamId3;

        string sql = $"SELECT `*` FROM `weapons` WHERE `steamid`=`${steamid}`";

        PlayerWeapons[steamid] = new PlayerWeaponsList();

        Task.Run(async () =>
        {
            var result = await _connection.QueryFirstOrDefaultAsync(sql);

            Server.NextFrame(async () =>
            {
                if (result == null) // player not exist
                {
                    sql = $"INSERT INTO `weapons` (`steamid`) VALUES ({steamid})";
                    await _connection.ExecuteAsync(sql);
                    return;
                }

                PlayerWeapons[steamid].CTPrimary = result.ct_primary;
                PlayerWeapons[steamid].TPrimary = result.t_primary;
                PlayerWeapons[steamid].CTSecondary = result.ct_secondary;
                PlayerWeapons[steamid].TSecondary = result.t_secondary;
                PlayerWeapons[steamid].AWP = result.awp == 1 ? true : false;
            });
        });

        return HookResult.Continue;
    }

    [GameEventHandler]
    public HookResult OnPlayerDisconnected(EventPlayerDisconnect @event, GameEventInfo info)
    {
        var player = @event.Userid;

        PlayerWeapons.Remove(@event.Userid.AuthorizedSteamID.SteamId3);

        return HookResult.Continue;
    }
}