//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/Army/CoreArmyLite.cs
//cs_include Scripts/Farm/BuyScrolls.cs
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Skua.Core.Interfaces;
using Skua.Core.Models;
using Skua.Core.Models.Items;
using Skua.Core.Scripts;


public class CoreUltra
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private CoreAdvanced Adv = new();
    private static CoreArmyLite sArmy = new();
    private BuyScrolls Scroll = new();
    private int[] skillList;

    public string OptionsStorage = "CoreUltraOptionsStorage";
    private string[] PartyMembers;
    private string player1;
    private string player2;
    private string player3;
    private string player4;
    private string monsPriorityID;
    private DateTime targetTime;
    private bool waitTaunt = false;
    Dictionary<string, int> playersHP = new Dictionary<string, int>(); 

    public List<IOption> StartOptions = new List<IOption>()
    {
        sArmy.player1,
        sArmy.player2,
        sArmy.player3,
        sArmy.player4,
        new Option<string>(
            " ",
            "Mode Explanation [player1]",
            "player1:\r\nclass: LR, LC\r\nweap: Arcana, Valiance, Dauntless, Elysium\r\ncape: Vainglory, Penitence, Lament\r\nhelm: Wizard, Forge",
            "click here"
        ),
        new Option<string>(
            " ",
            "Mode Explanation [player2]",
            "player2:\r\nclass: QCM, SC, CaV, VDK\r\nweap: Elysium, Valiance, Lacerate, Dauntless\r\ncape: Lament, Absolution, Penitence, Vainglory\r\nhelm: Wizard, Forge, Luck, Anima",
            "click here"
        ),
        new Option<string>(
            " ",
            "Mode Explanation [player3]",
            "player3:\r\nclass: LoO, CaV\r\nweap: AweBlast, Dauntless, Valiance\r\ncape: Avarice, Penitence, Absolution, Lament\r\nhelm: Luck, Forge",
            "click here"
        ),
        new Option<string>(
            " ",
            "Mode Explanation [player4]",
            "player4:\r\nclass: AP, VDK, LR\r\nweap: Ravenous, Arcana, Dauntless, Valiance, Lacerate\r\ncape: Avarice, Penitence, Lament\r\nhelm: Luck, Forge, Pneuma",
            "click here"
        ),
        new Option<string>("SafePot", "your safe pot", "yeah safe pot wont be used", "Vigil"),
        new Option<string>(
            "SafeClass",
            "your safe class",
            "any class that not used in this bot",
            "Shaman"
        ),
        // weapons
        new Option<string>("Dauntless", "weap Dauntless", "insert name of your Dauntless weap", ""),
        new Option<string>("Valiance", "weap Valiance", "insert name of your Valiance weap", ""),
        new Option<string>("Arcana", "weap Arcana", "insert name of your Arcana weap", ""),
        new Option<string>("AweBlast", "weap AweBlast", "insert name of your AweBlast weap", ""),
        new Option<string>("Ravenous", "weap Ravenous", "insert name of your Ravenous weap", ""),
        new Option<string>("Elysium", "weap Elysium", "insert name of your Elysium weap", ""),
        new Option<string>("Lacerate", "weap Lacerate", "insert name of your Lacerate weap", ""),
        // helm
        new Option<string>("WizHelm", "helm WizHelm", "insert name of your WizHelm helm", ""),
        new Option<string>("LuckHelm", "helm LuckHelm", "insert name of your LuckHelm helm", ""),
        new Option<string>("ForgeHelm", "helm ForgeHelm", "insert name of your ForgeHelm helm", ""),
        new Option<string>("AnimaHelm", "helm AnimaHelm", "insert name of your AnimaHelm helm", ""),
        new Option<string>(
            "PneumaHelm",
            "helm PneumaHelm",
            "insert name of your PneumaHelm helm",
            ""
        ),
        // cape
        new Option<string>(
            "Absolution",
            "cape Absolution",
            "insert name of your Absolution cape",
            ""
        ),
        new Option<string>("Avarice", "cape Avarice", "insert name of your Avarice cape", ""),
        new Option<string>("Penitence", "cape Penitence", "insert name of your Penitence cape", ""),
        new Option<string>("Vainglory", "cape Vainglory", "insert name of your Vainglory cape", ""),
        new Option<string>("Lament", "cape Lament", "insert name of your Lament cape", ""),
    };

    public void InitUltra()
    {
        Core.PrivateRooms = true;
        Core.PrivateRoomNumber = sArmy.getRoomNr();
        sArmy.initArmy();

        sArmy.setLogName($"{OptionsStorage}Logging");
        setPlayerName();
        PartyMembers = new[] { player1, player2, player3, player4 };

        Bot.Events.ExtensionPacketReceived -= PlayersHP;
        Bot.Events.ExtensionPacketReceived += PlayersHP;

        Bot.Events.ScriptStopping += Events_ScriptStopping;

        bool Events_ScriptStopping(Exception? e)
        {
            Bot.Events.ExtensionPacketReceived -= PlayersHP;
            return true;
        }

    }


    public void scrollOfLifeSteal(){
        if (!Core.CheckInventory("Scroll of Life Steal", 60))
            Core.BuyItem("arcangrove", 211, "Scroll of Life Steal", 99);
    }

    public void UltraTyndarius()
    {
        string bossId = "2";
        string cellMsg = "inTyndariusEnterCell";
        string clientName = "Ultra Tyndarius";
        string mapName = "ultratyndarius";
        string waitIn = "Enter";
        string doneMsg = "tyndariusdone";
        string bossCell = "Boss";
        string bossPad = "Left";
        string item = "Ultra Avatar Tyndarius Defeated";
        int questId = 8245;

        initClass(UltraBosses.UltraTyndarius);

        string classUsed = Bot.Player.CurrentClass!.Name;
        Bot.Events.PlayerAFK += PlayerAFK;

        scrollOfLifeSteal();

        Core.EnsureAccept(questId);

        // initClass(whatUltra);

        string[] monsList = monsPriorityID.Split(',');

        setClient(clientName);
        sArmy.ClearLogFile();

        Core.Join(mapName);
        // waitForParty(waitIn, "Spawn");
        sArmy.waitForPartyCell("Enter", "Spawn");
        waitForSignal(cellMsg, waitIn);
        sArmy.registerMessage(doneMsg, false);

        // string classUsed = Bot.Player.CurrentClass!.Name;

        Bot.Skills.UseSkill(1);
        Bot.Sleep(1000);
        Bot.Skills.UseSkill(2);
        Bot.Sleep(1000);
        Bot.Skills.UseSkill(3);

        if (Bot.Map.Name.ToLower() == mapName)
        {
            if (Bot.Player.CurrentClass!.Name != "ArchPaladin")
                Bot.Sleep(200);
            Core.Jump(bossCell, bossPad);
            Bot.Skills.UseSkill(2);
            Bot.Player.SetSpawnPoint();
        }

        bool needSendDone = true;
        int countCheck = 0;
        Core.Logger($"starting {mapName}");
        bool fight = true;
        int countRestart = 0;

        while (!Bot.ShouldExit && fight)
        {
            try
            {
                if (!Bot.Player.Alive)
                {
                    Bot.Sleep(500);
                    continue;
                }
                if ((Bot.TempInv.Contains(item) || Bot.Inventory.Contains(item)) && needSendDone)
                {
                    if (sArmy.sendDone())
                        needSendDone = false;
                }
                if (!needSendDone && sArmy.isDone() && countCheck % 5 == 0)
                {
                    break;
                }
                countCheck++;

                doPriorityAttack(monsList);
                if (Bot.Player.HasTarget)
                {
                    for (int i = 0; i < skillList.Length; i++)
                    {
                        if(Bot.Skills.CanUseSkill(skillList[i]))
                            Bot.Skills.UseSkill(skillList[i]);
                        if (classUsed != "Lord Of Order")
                        {
                            if (!Bot.Target.HasActiveAura("Focus"))
                                Bot.Skills.UseSkill(5);
                        }
                        // if (
                        //     (classUsed == "Lord Of Order" || classUsed == "ArchPaladin")
                        //     && belowHealthPercentage(70)
                        // )
                        // {
                        //     Bot.Skills.UseSkill(2);
                        //     Bot.Skills.UseSkill(5);
                        // }
                    }
                }
            }
            catch { }
        }
        Core.Jump(waitIn);
        waitForSignal($"{cellMsg}Done", waitIn);
        Core.Logger(doneMsg);
        Bot.Events.PlayerAFK -= PlayerAFK;
        Bot.Sleep(1000);

        // killWithArmy(
        //     UltraBosses.UltraTyndarius,
        //     bossId,
        //     cellMsg,
        //     clientName,
        //     mapName,
        //     waitIn,
        //     doneMsg,
        //     bossCell,
        //     bossPad,
        //     questId,
        //     tempInv,
        //     skillAction
        // );

        // void skillAction()
        // {
        //     for (int i = 0; i < skillList.Length; i++)
        //     {
        //         Bot.Skills.UseSkill(skillList[i]);
        //         if (classUsed != "Lord Of Order")
        //         {
        //             if (!Bot.Target.HasActiveAura("Focus"))
        //                 Bot.Skills.UseSkill(5);
        //         }
        //         if (
        //             (classUsed == "Lord Of Order" || classUsed == "ArchPaladin")
        //             && belowHealthPercentage(70)
        //         )
        //         {
        //             Bot.Skills.UseSkill(2);
        //             Bot.Skills.UseSkill(5);
        //         }
        //     }
        // }
    }

    public void UltraWarden()
    {
        string bossId = "1";
        string cellMsg = "inWardenEnterCell";
        string clientName = "Ultra Warden";
        string mapName = "ultrawarden";
        string waitIn = "Enter";
        string doneMsg = "wardendone";
        string bossCell = "r2";
        string bossPad = "Left";
        string item = "Ultra Warden Defeated";
        int questId = 8153;

        initClass(UltraBosses.UltraWarden);

        string classUsed = Bot.Player.CurrentClass!.Name;
        Bot.Events.PlayerAFK += PlayerAFK;

        scrollOfLifeSteal();

        Core.EnsureAccept(questId);

        // initClass(whatUltra);

        string[] monsList = monsPriorityID.Split(',');

        setClient(clientName);
        sArmy.ClearLogFile();

        Core.Join(mapName);
        // waitForParty(waitIn, "Spawn");
        sArmy.waitForPartyCell("Enter", "Spawn");
        waitForSignal(cellMsg, waitIn);
        sArmy.registerMessage(doneMsg, false);

        // string classUsed = Bot.Player.CurrentClass!.Name;

        Bot.Skills.UseSkill(1);
        Bot.Sleep(1000);
        Bot.Skills.UseSkill(2);
        Bot.Sleep(1000);
        Bot.Skills.UseSkill(3);

        if (Bot.Map.Name.ToLower() == mapName)
        {
            if (Bot.Player.CurrentClass!.Name != "ArchPaladin")
                Bot.Sleep(200);
            Core.Jump(bossCell, bossPad);
            Bot.Skills.UseSkill(2);
            Bot.Player.SetSpawnPoint();
        }

        bool needSendDone = true;
        int countCheck = 0;
        Core.Logger($"starting {mapName}");
        bool fight = true;
        int countRestart = 0;

        while (!Bot.ShouldExit && fight)
        {
            try
            {
                if (!Bot.Player.Alive)
                {
                    Bot.Sleep(500);
                    continue;
                }
                if ((Bot.TempInv.Contains(item) || Bot.Inventory.Contains(item)) && needSendDone)
                {
                    if (sArmy.sendDone())
                        needSendDone = false;
                }
                if (!needSendDone && sArmy.isDone() && countCheck % 5 == 0)
                {
                    break;
                }
                countCheck++;

                doPriorityAttack(monsList);
                if (Bot.Player.HasTarget)
                {
                    for (int i = 0; i < skillList.Length; i++)
                    {
                        if(Bot.Skills.CanUseSkill(skillList[i]))
                            Bot.Skills.UseSkill(skillList[i]);
                        if (classUsed == "Legion Revenant" || classUsed == "LightCaster")
                        {
                            if (!Bot.Target.HasActiveAura("Focus"))
                                Bot.Skills.UseSkill(5);
                            Bot.Skills.UseSkill(3);
                        }
                        // if (
                        //     (classUsed == "Lord Of Order" || classUsed == "ArchPaladin")
                        //     && belowHealthPercentage(70)
                        // )
                        // {
                        //     Bot.Skills.UseSkill(2);
                        //     Bot.Skills.UseSkill(5);
                        // }
                    }
                }
            }
            catch { }
        }
        Core.Jump(waitIn);
        waitForSignal($"{cellMsg}Done", waitIn);
        Core.Logger(doneMsg);
        Bot.Events.PlayerAFK -= PlayerAFK;
        Bot.Sleep(1000);

        // killWithArmy(
        //     UltraBosses.UltraWarden,
        //     bossId,
        //     cellMsg,
        //     clientName,
        //     mapName,
        //     waitIn,
        //     doneMsg,
        //     bossCell,
        //     bossPad,
        //     questId,
        //     tempInv,
        //     skillAction
        // );

        // void skillAction()
        // {
        //     for (int i = 0; i < skillList.Length; i++)
        //     {
        //         Bot.Skills.UseSkill(skillList[i]);
        //         if (classUsed == "Legion Revenant" || classUsed == "LightCaster")
        //         {
        //             if (!Bot.Target.HasActiveAura("Focus"))
        //                 Bot.Skills.UseSkill(5);
        //             Bot.Skills.UseSkill(3);
        //         }
        //         if (
        //             (classUsed == "Lord Of Order" || classUsed == "ArchPaladin")
        //             && belowHealthPercentage(70)
        //         )
        //         {
        //             Bot.Skills.UseSkill(2);
        //             Bot.Skills.UseSkill(5);
        //         }
        //     }
        // }
    }

    private bool shouldAttack = true;

    public void UltraEzrajal()
    {
        string bossId = "1";
        string cellMsg = "inEzrajalEnterCell";
        string clientName = "Ultra Ezrajal";
        string mapName = "ultraezrajal";
        string waitIn = "Enter";
        string doneMsg = "ezrajaldone";
        string bossCell = "r2";
        string bossPad = "Left";
        string item = "Ultra Ezrajal Defeated";
        int questId = 8152;

        initClass(UltraBosses.UltraEzrajal);

        string classUsed = Bot.Player.CurrentClass!.Name;

        Bot.Events.CounterAttack += _KillEzrajal;


        Bot.Events.PlayerAFK += PlayerAFK;

        scrollOfLifeSteal();

        Core.EnsureAccept(questId);

        // initClass(whatUltra);

        string[] monsList = monsPriorityID.Split(',');

        setClient(clientName);
        sArmy.ClearLogFile();

        Core.Join(mapName);
        // waitForParty(waitIn, "Spawn");
        sArmy.waitForPartyCell("Enter", "Spawn");
        waitForSignal(cellMsg, waitIn);
        sArmy.registerMessage(doneMsg, false);

        // string classUsed = Bot.Player.CurrentClass!.Name;

        Bot.Skills.UseSkill(1);
        Bot.Sleep(1000);
        Bot.Skills.UseSkill(2);
        Bot.Sleep(1000);
        Bot.Skills.UseSkill(3);

        if (Bot.Map.Name.ToLower() == mapName)
        {
            if (Bot.Player.CurrentClass!.Name != "ArchPaladin")
                Bot.Sleep(200);
            Core.Jump(bossCell, bossPad);
            Bot.Skills.UseSkill(2);
            Bot.Player.SetSpawnPoint();
        }

        bool needSendDone = true;
        int countCheck = 0;
        Core.Logger($"starting {mapName}");
        bool fight = true;
        int countRestart = 0;

        while (!Bot.ShouldExit && fight)
        {
            try
            {
                if (!Bot.Player.Alive)
                {
                    Bot.Sleep(500);
                    continue;
                }
                if ((Bot.TempInv.Contains(item) || Bot.Inventory.Contains(item)) && needSendDone)
                {
                    if (sArmy.sendDone())
                        needSendDone = false;
                }
                if (!needSendDone && sArmy.isDone() && countCheck % 5 == 0)
                {
                    break;
                }
                countCheck++;

                doPriorityAttack(monsList);
                if (Bot.Player.HasTarget)
                {
                    for (int i = 0; i < skillList.Length; i++)
                    {
                        if (!shouldAttack)
                        {
                            if (
                                (classUsed == "Lord Of Order" || classUsed == "ArchPaladin")
                                && belowHealthPercentage(70)
                            )
                            {
                                Bot.Skills.UseSkill(2);
                            }
                            continue;
                        }
                        if(Bot.Skills.CanUseSkill(skillList[i]))
                            Bot.Skills.UseSkill(skillList[i]);
                        if (classUsed == "ArchPaladin" || classUsed == "Legion Revenant")
                        {
                            if (!Bot.Target.HasActiveAura("Focus"))
                                Bot.Skills.UseSkill(5);
                        }

                        // if (
                        //     (classUsed == "Lord Of Order" || classUsed == "ArchPaladin")
                        //     && belowHealthPercentage(70)
                        // )
                        // {
                        //     Bot.Skills.UseSkill(2);
                        //     Bot.Skills.UseSkill(5);
                        // }
                    }
                }
            }
            catch { }
        }
        Core.Jump(waitIn);
        waitForSignal($"{cellMsg}Done", waitIn);
        Core.Logger(doneMsg);
        Bot.Events.PlayerAFK -= PlayerAFK;
        Bot.Sleep(1000);
        Bot.Events.CounterAttack -= _KillEzrajal;

        // killWithArmy(
        //     UltraBosses.UltraEzrajal,
        //     bossId,
        //     cellMsg,
        //     clientName,
        //     mapName,
        //     waitIn,
        //     doneMsg,
        //     bossCell,
        //     bossPad,
        //     questId,
        //     tempInv,
        //     skillAction
        // );
        // Bot.Events.CounterAttack -= _KillEzrajal;
        // void skillAction()
        // {
        //     for (int i = 0; i < skillList.Length; i++)
        //     {
        //         if (!shouldAttack)
        //         {
        //             if (
        //                 (classUsed == "Lord Of Order" || classUsed == "ArchPaladin")
        //                 && belowHealthPercentage(70)
        //             )
        //             {
        //                 Bot.Skills.UseSkill(2);
        //             }
        //             continue;
        //         }
        //         Bot.Skills.UseSkill(skillList[i]);
        //         if (classUsed == "ArchPaladin" || classUsed == "Legion Revenant")
        //         {
        //             if (!Bot.Target.HasActiveAura("Focus"))
        //                 Bot.Skills.UseSkill(5);
        //         }

        //         if (
        //             (classUsed == "Lord Of Order" || classUsed == "ArchPaladin")
        //             && belowHealthPercentage(70)
        //         )
        //         {
        //             Bot.Skills.UseSkill(2);
        //             Bot.Skills.UseSkill(5);
        //         }
        //     }
        // }

        void _KillEzrajal(bool faded)
        {
            if (!faded)
            {
                shouldAttack = false;
                Bot.Combat.CancelAutoAttack();
                Bot.Combat.CancelTarget();
            }
            else
            {
                Bot.Combat.Attack("Ultra Ezrajal");
                shouldAttack = true;
            }
        }
    }

    public void UltraEngineer()
    {
        string bossId = "3";
        string cellMsg = "inEngineerEnterCell";
        string clientName = "Ultra Engineer";
        string mapName = "ultraengineer";
        string waitIn = "Enter";
        string doneMsg = "engineerdone";
        string bossCell = "r2";
        string bossPad = "Left";
        string item = "Ultra Engineer Defeated";
        int questId = 8154;

        initClass(UltraBosses.UltraEngineer);

        string classUsed = Bot.Player.CurrentClass!.Name;

        Bot.Events.PlayerAFK += PlayerAFK;

        scrollOfLifeSteal();

        Core.EnsureAccept(questId);

        // initClass(whatUltra);

        string[] monsList = monsPriorityID.Split(',');

        setClient(clientName);
        sArmy.ClearLogFile();

        Core.Join(mapName);
        // waitForParty(waitIn, "Spawn");
        sArmy.waitForPartyCell("Enter", "Spawn");
        waitForSignal(cellMsg, waitIn);
        sArmy.registerMessage(doneMsg, false);

        // string classUsed = Bot.Player.CurrentClass!.Name;

        Bot.Skills.UseSkill(1);
        Bot.Sleep(1000);
        Bot.Skills.UseSkill(2);
        Bot.Sleep(1000);
        Bot.Skills.UseSkill(3);

        if (Bot.Map.Name.ToLower() == mapName)
        {
            if (Bot.Player.CurrentClass!.Name != "ArchPaladin")
                Bot.Sleep(200);
            Core.Jump(bossCell, bossPad);
            Bot.Skills.UseSkill(2);
            Bot.Player.SetSpawnPoint();
        }

        bool needSendDone = true;
        int countCheck = 0;
        Core.Logger($"starting {mapName}");
        bool fight = true;
        int countRestart = 0;

        while (!Bot.ShouldExit && fight)
        {
            try
            {
                if (!Bot.Player.Alive)
                {
                    Bot.Sleep(500);
                    continue;
                }
                if ((Bot.TempInv.Contains(item) || Bot.Inventory.Contains(item)) && needSendDone)
                {
                    if (sArmy.sendDone())
                        needSendDone = false;
                }
                if (!needSendDone && sArmy.isDone() && countCheck % 5 == 0)
                {
                    break;
                }
                countCheck++;

                doPriorityAttack(monsList);
                if (Bot.Player.HasTarget)
                {
                    for (int i = 0; i < skillList.Length; i++)
                    {
                        if(Bot.Skills.CanUseSkill(skillList[i]))
                            Bot.Skills.UseSkill(skillList[i]);
                        if (classUsed == "Legion Revenant" || classUsed == "ArchPaladin")
                        {
                            if (!Bot.Target.HasActiveAura("Focus"))
                                Bot.Skills.UseSkill(5);
                        }
                        // if (
                        //     (classUsed == "Lord Of Order" || classUsed == "ArchPaladin")
                        //     && belowHealthPercentage(70)
                        // )
                        // {
                        //     Bot.Skills.UseSkill(2);
                        //     Bot.Skills.UseSkill(5);
                        // }
                    }
                }
            }
            catch { }
        }
        Core.Jump(waitIn);
        waitForSignal($"{cellMsg}Done", waitIn);
        Core.Logger(doneMsg);
        Bot.Events.PlayerAFK -= PlayerAFK;
        Bot.Sleep(1000);

        // killWithArmy(
        //     UltraBosses.UltraEngineer,
        //     bossId,
        //     cellMsg,
        //     clientName,
        //     mapName,
        //     waitIn,
        //     doneMsg,
        //     bossCell,
        //     bossPad,
        //     questId,
        //     tempInv,
        //     skillAction
        // );
        // void skillAction()
        // {
        //     for (int i = 0; i < skillList.Length; i++)
        //     {
        //         Bot.Skills.UseSkill(skillList[i]);
        //         if (classUsed == "Legion Revenant" || classUsed == "ArchPaladin")
        //         {
        //             if (!Bot.Target.HasActiveAura("Focus"))
        //                 Bot.Skills.UseSkill(5);
        //         }
        //         if (
        //             (classUsed == "Lord Of Order" || classUsed == "ArchPaladin")
        //             && belowHealthPercentage(70)
        //         )
        //         {
        //             Bot.Skills.UseSkill(2);
        //             Bot.Skills.UseSkill(5);
        //         }
        //     }
        // }
    }

    public void ChampionDrakath()
    {
        string bossId = "1";
        string cellMsg = "inDrakathEnterCell";
        string clientName = "Champion Drakath";
        string mapName = "championdrakath";
        string waitIn = "Enter";
        string doneMsg = "championdrakathdone";
        string bossCell = "r2";
        string bossPad = "Left";
        string item = "Champion Drakath Defeated";
        int questId = 8300;

        initClass(UltraBosses.Championdrakath);

        string classUsed = Bot.Player.CurrentClass!.Name;

        Bot.Events.PlayerAFK += PlayerAFK;

        scrollOfLifeSteal();

        Core.EnsureAccept(questId);

        // initClass(whatUltra);

        string[] monsList = monsPriorityID.Split(',');

        setClient(clientName);
        sArmy.ClearLogFile();

        Core.Join(mapName);
        // waitForParty(waitIn, "Spawn");
        sArmy.waitForPartyCell("Enter", "Spawn");
        waitForSignal(cellMsg, waitIn);
        sArmy.registerMessage(doneMsg, false);

        // string classUsed = Bot.Player.CurrentClass!.Name;

        Bot.Skills.UseSkill(1);
        Bot.Sleep(1000);
        Bot.Skills.UseSkill(2);
        Bot.Sleep(1000);
        Bot.Skills.UseSkill(3);

        if (Bot.Map.Name.ToLower() == mapName)
        {
            if (Bot.Player.CurrentClass!.Name != "ArchPaladin")
                Bot.Sleep(200);
            Core.Jump(bossCell, bossPad);
            Bot.Skills.UseSkill(2);
            Bot.Player.SetSpawnPoint();
        }

        bool needSendDone = true;
        int countCheck = 0;
        Core.Logger($"starting {mapName}");
        bool fight = true;
        int countRestart = 0;

        while (!Bot.ShouldExit && fight)
        {
            try
            {
                if (!Bot.Player.Alive)
                {
                    Bot.Sleep(500);
                    continue;
                }
                if ((Bot.TempInv.Contains(item) || Bot.Inventory.Contains(item)) && needSendDone)
                {
                    if (sArmy.sendDone())
                        needSendDone = false;
                }
                if (!needSendDone && sArmy.isDone() && countCheck % 5 == 0)
                {
                    break;
                }
                countCheck++;

                doPriorityAttack(monsList);
                if (Bot.Player.HasTarget)
                {
                    for (int i = 0; i < skillList.Length; i++)
                    {
                        if(Bot.Skills.CanUseSkill(skillList[i]))
                            Bot.Skills.UseSkill(skillList[i]);
                        if (classUsed == "Legion Revenant" && drakathHealthCheck(GetMonsterHP("1")))
                        {
                            Bot.Skills.UseSkill(5);
                            Bot.Skills.UseSkill(5);
                            Bot.Skills.UseSkill(5);
                        }
                        // if ((classUsed != "Legion Revenant") && belowHealthPercentage(70))
                        // {
                        //     Bot.Skills.UseSkill(2);
                        //     Bot.Skills.UseSkill(5);
                        // }
                    }
                }
            }
            catch { }
        }
        Core.Jump(waitIn);
        waitForSignal($"{cellMsg}Done", waitIn);
        Core.Logger(doneMsg);
        Bot.Events.PlayerAFK -= PlayerAFK;
        Bot.Sleep(1000);

        // killWithArmy(
        //     UltraBosses.Championdrakath,
        //     bossId,
        //     cellMsg,
        //     clientName,
        //     mapName,
        //     waitIn,
        //     doneMsg,
        //     bossCell,
        //     bossPad,
        //     questId,
        //     tempInv,
        //     skillAction
        // );

        // void skillAction()
        // {
        //     for (int i = 0; i < skillList.Length; i++)
        //     {
        //         Bot.Skills.UseSkill(skillList[i]);
        //         if (classUsed == "Legion Revenant" && drakathHealthCheck(GetMonsterHP("1")))
        //         {
        //             Bot.Skills.UseSkill(5);
        //             Bot.Skills.UseSkill(5);
        //             Bot.Skills.UseSkill(5);
        //         }
        //         if ((classUsed != "Legion Revenant") && belowHealthPercentage(70))
        //         {
        //             Bot.Skills.UseSkill(2);
        //             Bot.Skills.UseSkill(5);
        //         }
        //     }
        // }
    }

    public void UltraDage()
    {
        string bossId = "1";
        string cellMsg = "inDageEnterCell";
        string clientName = "Ultra Dage";
        string mapName = "ultradage";
        string waitIn = "Enter";
        string doneMsg = "ultradagedone";
        string bossCell = "Boss";
        string bossPad = "Right";
        string item = "Dage the Dark Lord Defeated";
        int questId = 8547;

        initClass(UltraBosses.UltraDage);

        string classUsed = Bot.Player.CurrentClass!.Name;
        Bot.Events.RunToArea += Event_RunToArea;

        Bot.Events.PlayerAFK += PlayerAFK;

        scrollOfLifeSteal();

        Core.EnsureAccept(questId);

        // initClass(whatUltra);

        string[] monsList = monsPriorityID.Split(',');

        setClient(clientName);
        sArmy.ClearLogFile();

        Core.Join(mapName);
        // waitForParty(waitIn, "Spawn");
        sArmy.waitForPartyCell("Enter", "Spawn");
        waitForSignal(cellMsg, waitIn);
        sArmy.registerMessage(doneMsg, false);

        // string classUsed = Bot.Player.CurrentClass!.Name;

        Bot.Skills.UseSkill(1);
        Bot.Sleep(1000);
        Bot.Skills.UseSkill(2);
        Bot.Sleep(1000);
        Bot.Skills.UseSkill(3);

        if (Bot.Map.Name.ToLower() == mapName)
        {
            if (Bot.Player.CurrentClass!.Name != "ArchPaladin")
                Bot.Sleep(200);
            Core.Jump(bossCell, bossPad);
            Bot.Skills.UseSkill(2);
            Bot.Player.SetSpawnPoint();
        }

        bool needSendDone = true;
        int countCheck = 0;
        Core.Logger($"starting {mapName}");
        bool fight = true;
        int countRestart = 0;

        while (!Bot.ShouldExit && fight)
        {
            try
            {
                if (!Bot.Player.Alive)
                {
                    Bot.Sleep(500);
                    continue;
                }
                if ((Bot.TempInv.Contains(item) || Bot.Inventory.Contains(item)) && needSendDone)
                {
                    if (sArmy.sendDone())
                        needSendDone = false;
                }
                if (!needSendDone && sArmy.isDone() && countCheck % 5 == 0)
                {
                    break;
                }
                countCheck++;

                doPriorityAttack(monsList);
                if (Bot.Player.HasTarget)
                {
                    for (int i = 0; i < skillList.Length; i++)
                    {
                        if(Bot.Skills.CanUseSkill(skillList[i]))
                            Bot.Skills.UseSkill(skillList[i]);
                        if (classUsed == "Legion Revenant" && !Bot.Target.HasActiveAura("Focus"))
                        {
                            Bot.Skills.UseSkill(5);
                        }
                        // if (
                        //     (classUsed == "Verus DoomKnight" || classUsed == "Quantum Chronomancer")
                        //     && belowHealthPercentage(70)
                        // )
                        // {
                        //     Bot.Skills.UseSkill(2);
                        //     Bot.Skills.UseSkill(5);
                        // }
                    }
                }
            }
            catch { }
        }
        Core.Jump(waitIn);
        waitForSignal($"{cellMsg}Done", waitIn);
        Core.Logger(doneMsg);
        Bot.Events.PlayerAFK -= PlayerAFK;
        Bot.Events.RunToArea -= Event_RunToArea;
        Bot.Sleep(1000);

        void Event_RunToArea(string zone)
        {
            switch (zone.ToLower())
            {
                case "a":
                    //Move to the left
                    Bot.Flash.Call(
                        "walkTo",
                        Bot.Random.Next(40, 175),
                        Bot.Random.Next(400, 410),
                        8
                    );
                    // Bot.Player.WalkTo(Bot.Random.Next(40, 175), Bot.Random.Next(400, 410), speed: 8);
                    break;
                case "b":
                    //Move to the right
                    Bot.Flash.Call(
                        "walkTo",
                        Bot.Random.Next(760, 930),
                        Bot.Random.Next(410, 415),
                        8
                    );
                    // Bot.Player.WalkTo(Bot.Random.Next(760, 930), Bot.Random.Next(410, 415), speed: 8);
                    break;
                default:
                    //Move to the center
                    Bot.Sleep(500);
                    Bot.Flash.Call(
                        "walkTo",
                        Bot.Random.Next(480, 500),
                        Bot.Random.Next(300, 420),
                        8
                    );
                    // Bot.Player.WalkTo(Bot.Random.Next(480, 500), Bot.Random.Next(300, 420), speed: 8);
                    break;
            }
        }

        // killWithArmy(
        //     UltraBosses.UltraDage,
        //     bossId,
        //     cellMsg,
        //     clientName,
        //     mapName,
        //     waitIn,
        //     doneMsg,
        //     bossCell,
        //     bossPad,
        //     questId,
        //     tempInv,
        //     skillAction
        // );
        


        // void skillAction()
        // {
        //     for (int i = 0; i < skillList.Length; i++)
        //     {
        //         Bot.Skills.UseSkill(skillList[i]);
        //         if (classUsed == "Legion Revenant" && !Bot.Target.HasActiveAura("Focus"))
        //         {
        //             Bot.Skills.UseSkill(5);
        //         }
        //         if (
        //             (classUsed == "Verus DoomKnight" || classUsed == "Quantum Chronomancer")
        //             && belowHealthPercentage(70)
        //         )
        //         {
        //             Bot.Skills.UseSkill(2);
        //             Bot.Skills.UseSkill(5);
        //         }
        //     }
        // }
    }

    public void UltraNulgath()
    {
        string bossId = "2";
        string cellMsg = "inNulgathEnterCell";
        string clientName = "Ultra Nulgath";
        string mapName = "ultranulgath";
        string waitIn = "Enter";
        string doneMsg = "ultranulgathdone";
        string bossCell = "Boss";
        string bossPad = "Right";
        string item = "Nulgath the Archfiend Defeated?";
        int questId = 8692;

        initClass(UltraBosses.UltraNulgath);

        string classUsed = Bot.Player.CurrentClass!.Name;

        Bot.Events.PlayerAFK += PlayerAFK;

        scrollOfLifeSteal();

        Core.EnsureAccept(questId);

        // initClass(whatUltra);

        string[] monsList = monsPriorityID.Split(',');

        setClient(clientName);
        sArmy.ClearLogFile();

        Core.Join(mapName);
        // waitForParty(waitIn, "Spawn");
        sArmy.waitForPartyCell("Enter", "Spawn");
        waitForSignal(cellMsg, waitIn);
        sArmy.registerMessage(doneMsg, false);

        // string classUsed = Bot.Player.CurrentClass!.Name;

        Bot.Skills.UseSkill(1);
        Bot.Sleep(1000);
        Bot.Skills.UseSkill(2);
        Bot.Sleep(1000);
        Bot.Skills.UseSkill(3);

        if (Bot.Map.Name.ToLower() == mapName)
        {
            if (Bot.Player.CurrentClass!.Name != "ArchPaladin")
                Bot.Sleep(200);
            Core.Jump(bossCell, bossPad);
            Bot.Skills.UseSkill(2);
            Bot.Player.SetSpawnPoint();
        }

        bool needSendDone = true;
        int countCheck = 0;
        Core.Logger($"starting {mapName}");
        bool fight = true;
        int countRestart = 0;

        while (!Bot.ShouldExit && fight)
        {
            try
            {
                if (!Bot.Player.Alive)
                {
                    Bot.Sleep(500);
                    continue;
                }
                if ((Bot.TempInv.Contains(item) || Bot.Inventory.Contains(item)) && needSendDone)
                {
                    if (sArmy.sendDone())
                        needSendDone = false;
                }
                if (!needSendDone && sArmy.isDone() && countCheck % 5 == 0)
                {
                    break;
                }
                countCheck++;

                doPriorityAttack(monsList);
                if (Bot.Player.HasTarget)
                {
                    for (int i = 0; i < skillList.Length; i++)
                    {
                        if(Bot.Skills.CanUseSkill(skillList[i]))
                            Bot.Skills.UseSkill(skillList[i]);
                        if (
                            (classUsed == "Legion Revenant" || classUsed == "ArchPaladin")
                            && !Bot.Target.HasActiveAura("Focus")
                        )
                            Bot.Skills.UseSkill(5);
                        // if ((classUsed != "Legion Revenant") && belowHealthPercentage(70))
                        // {
                        //     Bot.Skills.UseSkill(2);
                        //     Bot.Skills.UseSkill(5);
                        // }
                    }
                }
            }
            catch { }
        }
        Core.Jump(waitIn);
        waitForSignal($"{cellMsg}Done", waitIn);
        Core.Logger(doneMsg);
        Bot.Events.PlayerAFK -= PlayerAFK;
        Bot.Sleep(1000);


        // killWithArmy(
        //     UltraBosses.UltraNulgath,
        //     bossId,
        //     cellMsg,
        //     clientName,
        //     mapName,
        //     waitIn,
        //     doneMsg,
        //     bossCell,
        //     bossPad,
        //     questId,
        //     tempInv,
        //     skillAction
        // );

        // void skillAction()
        // {
        //     for (int i = 0; i < skillList.Length; i++)
        //     {
        //         Bot.Skills.UseSkill(skillList[i]);
        //         if (
        //             (classUsed == "Legion Revenant" || classUsed == "ArchPaladin")
        //             && !Bot.Target.HasActiveAura("Focus")
        //         )
        //             Bot.Skills.UseSkill(5);
        //         if ((classUsed != "Legion Revenant") && belowHealthPercentage(70))
        //         {
        //             Bot.Skills.UseSkill(2);
        //             Bot.Skills.UseSkill(5);
        //         }
        //     }
        // }
    }

    public void UltraDrago()
    {
        string bossId = "2";
        string cellMsg = "inDragoEnterCell";
        string clientName = "Ultra Drago";
        string mapName = "ultradrago";
        string waitIn = "Enter";
        string doneMsg = "ultradragodone";
        string bossCell = "Boss";
        string bossPad = "Right";
        string item = "Drago Dethroned";
        int questId = 8397;

        initClass(UltraBosses.UltraDrago);

        string classUsed = Bot.Player.CurrentClass!.Name;

        Bot.Events.PlayerAFK += PlayerAFK;

        scrollOfLifeSteal();

        Core.EnsureAccept(questId);

        // initClass(whatUltra);

        string[] monsList = monsPriorityID.Split(',');

        setClient(clientName);
        sArmy.ClearLogFile();

        Core.Join(mapName);
        // waitForParty(waitIn, "Spawn");
        sArmy.waitForPartyCell("Enter", "Spawn");
        waitForSignal(cellMsg, waitIn);
        sArmy.registerMessage(doneMsg, false);

        // string classUsed = Bot.Player.CurrentClass!.Name;

        Bot.Skills.UseSkill(1);
        Bot.Sleep(1000);
        Bot.Skills.UseSkill(2);
        Bot.Sleep(1000);
        Bot.Skills.UseSkill(3);

        if (Bot.Map.Name.ToLower() == mapName)
        {
            if (Bot.Player.CurrentClass!.Name != "ArchPaladin")
                Bot.Sleep(200);
            Core.Jump(bossCell, bossPad);
            Bot.Skills.UseSkill(2);
            Bot.Player.SetSpawnPoint();
        }

        bool needSendDone = true;
        int countCheck = 0;
        Core.Logger($"starting {mapName}");
        bool fight = true;
        int countRestart = 0;

        while (!Bot.ShouldExit && fight)
        {
            try
            {
                if (!Bot.Player.Alive)
                {
                    Bot.Sleep(500);
                    continue;
                }
                if ((Bot.TempInv.Contains(item) || Bot.Inventory.Contains(item)) && needSendDone)
                {
                    if (sArmy.sendDone())
                        needSendDone = false;
                }
                if (!needSendDone && sArmy.isDone() && countCheck % 5 == 0)
                {
                    break;
                }
                countCheck++;

                doPriorityAttack(monsList);
                if (Bot.Player.HasTarget)
                {
                    for (int i = 0; i < skillList.Length; i++)
                    {
                        if(Bot.Skills.CanUseSkill(skillList[i]))
                            Bot.Skills.UseSkill(skillList[i]);
                        if (
                            (classUsed == "Legion Revenant" || classUsed == "ArchPaladin")
                            && !Bot.Target.HasActiveAura("Focus")
                        )
                            Bot.Skills.UseSkill(5);
                        // if ((classUsed != "Legion Revenant") && belowHealthPercentage(70))
                        // {
                        //     Bot.Skills.UseSkill(2);
                        //     Bot.Skills.UseSkill(5);
                        // }
                    }
                }
            }
            catch { }
        }
        Core.Jump(waitIn);
        waitForSignal($"{cellMsg}Done", waitIn);
        Core.Logger(doneMsg);
        Bot.Events.PlayerAFK -= PlayerAFK;
        Bot.Sleep(1000);


        // killWithArmy(
        //     UltraBosses.UltraDrago,
        //     bossId,
        //     cellMsg,
        //     clientName,
        //     mapName,
        //     waitIn,
        //     doneMsg,
        //     bossCell,
        //     bossPad,
        //     questId,
        //     tempInv,
        //     skillAction
        // );

        // void skillAction()
        // {
        //     for (int i = 0; i < skillList.Length; i++)
        //     {
        //         Bot.Skills.UseSkill(skillList[i]);
        //         if (
        //             (classUsed == "Legion Revenant" || classUsed == "ArchPaladin")
        //             && !Bot.Target.HasActiveAura("Focus")
        //         )
        //             Bot.Skills.UseSkill(5);
        //         if ((classUsed != "Legion Revenant") && belowHealthPercentage(70))
        //         {
        //             Bot.Skills.UseSkill(2);
        //             Bot.Skills.UseSkill(5);
        //         }
        //     }
        // }
    }

    public void UltraSpeaker()
    {
        string bossId = "1";
        string cellMsg = "inSpeakerEnterCell";
        string clientName = "Ultra Speaker";
        string mapName = "ultraspeaker";
        string waitIn = "Enter";
        string doneMsg = "ultraspeakerdone";
        string bossCell = "Boss";
        string bossPad = "Left";
        string item = "The First Speaker Silenced";
        int questId = 9173;

        initClass(UltraBosses.UltraSpeaker);

        string classUsed = Bot.Player.CurrentClass!.Name;
        //Bot.Events.ExtensionPacketReceived -= UltraSpeakerTodo;
        //Bot.Events.ExtensionPacketReceived += UltraSpeakerTodo;
        
        Bot.Events.PlayerAFK += PlayerAFK;

        scrollOfLifeSteal();

        Core.EnsureAccept(questId);

        // initClass(whatUltra);

        string[] monsList = monsPriorityID.Split(',');

        setClient(clientName);
        sArmy.ClearLogFile();

        Core.Join(mapName);
        // waitForParty(waitIn, "Spawn");
        sArmy.waitForPartyCell("Enter", "Spawn");
        waitForSignal(cellMsg, waitIn);
        sArmy.registerMessage(doneMsg, false);

        // string classUsed = Bot.Player.CurrentClass!.Name;

        Bot.Skills.UseSkill(1);
        Bot.Sleep(1000);
        Bot.Skills.UseSkill(2);
        Bot.Sleep(1000);
        Bot.Skills.UseSkill(3);

        if (Bot.Map.Name.ToLower() == mapName)
        {
            if (Bot.Player.CurrentClass!.Name != "ArchPaladin")
                Bot.Sleep(200);
            Core.Jump(bossCell, bossPad);
            Bot.Skills.UseSkill(2);
            Bot.Player.SetSpawnPoint();
        }

        bool needSendDone = true;
        int countCheck = 0;
        Core.Logger($"starting {mapName}");
        bool fight = true;
        int countRestart = 0;

        while (!Bot.ShouldExit && fight)
        {
            try
            {
                if (!Bot.Player.Alive)
                {
                    Bot.Sleep(500);
                    continue;
                }
                int playerPositionX = Bot.Player.X;
                if (
                    playerPositionX != 100
                    && Bot.Map.Name.ToLower() == "ultraspeaker"
                )
                {
                    Bot.Flash.Call("walkTo", 100, 321, 8);
                }
                if ((Bot.TempInv.Contains(item) || Bot.Inventory.Contains(item)) && needSendDone)
                {
                    if (sArmy.sendDone())
                        needSendDone = false;
                }
                if (!needSendDone && sArmy.isDone() && countCheck % 5 == 0)
                {
                    break;
                }
                countCheck++;

                doPriorityAttack(monsList);
                // if(forceSkill){
                //     while (!Bot.ShouldExit && !Bot.Skills.CanUseSkill(skillToForce))
                //     {
                //         // await Task.Delay(10);
                //         Bot.Sleep(5);
                //     }
                //     Bot.Skills.UseSkill(skillToForce);
                //     Bot.Skills.UseSkill(skillToForce);
                //     Bot.Skills.UseSkill(skillToForce);
                //     forceSkill = false;
                // }
                
                if (Bot.Player.HasTarget)
                {
                    if (GetMonsterHP("1") >= 9900000){
                        speakerCounter = 0;
                        forceSkill = false;
                    }
                    for (int i = 0; i < skillList.Length; i++)
                    {
                        if(Bot.Skills.CanUseSkill(skillList[i]))
                            Bot.Skills.UseSkill(skillList[i]);
                    }
                    Bot.Skills.UseSkill(2);
                }
            }
            catch { }
        }
        Core.Jump(waitIn);
        waitForSignal($"{cellMsg}Done", waitIn);
        Core.Logger(doneMsg);
        Bot.Events.PlayerAFK -= PlayerAFK;
        Bot.Sleep(1000);

        //Bot.Events.ExtensionPacketReceived -= UltraSpeakerTodo;

        
        // killWithArmy(
        //     UltraBosses.UltraSpeaker,
        //     bossId,
        //     cellMsg,
        //     clientName,
        //     mapName,
        //     waitIn,
        //     doneMsg,
        //     bossCell,
        //     bossPad,
        //     questId,
        //     inv,
        //     skillAction
        // );

        // void skillAction()
        // {
        //     if (GetMonsterHP("1") >= 9900000)
        //         speakerCounter = 0;
        //     for (int i = 0; i < skillList.Length; i++)
        //     {
        //         Bot.Skills.UseSkill(skillList[i]);
        //     }
        //     Bot.Skills.UseSkill(2);
        // }
    }

    public void UltraDarkon()
    {
        string bossId = "1";
        string cellMsg = "inDarkonEnterCell";
        string clientName = "Ultra Darkon";
        string mapName = "ultradarkon";
        string waitIn = "Enter";
        string doneMsg = "ultradarkondone";
        string bossCell = "r2";
        string bossPad = "Left";
        string item = "Darkon the Conductor Defeated";
        int questId = 8746;

        // Bot.Events.ExtensionPacketReceived -= DarkonHandler;
        // Bot.Events.ExtensionPacketReceived += DarkonHandler;
        initClass(UltraBosses.UltraDarkon);
        sArmy.ClearLogFile();
        string classUsed = Bot.Player.CurrentClass!.Name;

        Bot.Events.PlayerAFK += PlayerAFK;

        if (!Core.CheckInventory("Scroll of Life Steal", 60))
            Core.BuyItem("arcangrove", 211, "Scroll of Life Steal", 99);

        Core.EnsureAccept(questId);

        // initClass(whatUltra);

        string[] monsList = monsPriorityID.Split(',');

        setClient(clientName);

        Core.Join(mapName);
        // waitForParty(waitIn, "Spawn");
        sArmy.waitForPartyCell("Enter", "Spawn");
        waitForSignal(cellMsg, waitIn);
        sArmy.registerMessage(doneMsg, false);

        // string classUsed = Bot.Player.CurrentClass!.Name;

        Bot.Skills.UseSkill(1);
        Bot.Sleep(1000);
        Bot.Skills.UseSkill(2);
        Bot.Sleep(1000);
        Bot.Skills.UseSkill(3);

        if (Bot.Map.Name.ToLower() == mapName)
        {
            if (Bot.Player.CurrentClass!.Name != "ArchPaladin")
                Bot.Sleep(200);
            Core.Jump(bossCell, bossPad);
            Bot.Skills.UseSkill(2);
            Bot.Player.SetSpawnPoint();
        }

        bool needSendDone = true;
        int countCheck = 0;
        Core.Logger($"starting {mapName}");
        bool fight = true;
        int countRestart = 0;

        while (!Bot.ShouldExit && fight)
        {
            try
            {
                if (!Bot.Player.Alive)
                {
                    Bot.Sleep(500);
                    continue;
                }
                if ((Bot.TempInv.Contains(item) || Bot.Inventory.Contains(item)) && needSendDone)
                {
                    if (sArmy.sendDone())
                        needSendDone = false;
                }
                if (!needSendDone && sArmy.isDone() && countCheck % 5 == 0)
                {
                    break;
                }
                countCheck++;

                doPriorityAttack(monsList);
                if (Bot.Player.HasTarget)
                {
                    int monsHP = GetMonsterHP("1");

                    for (int i = 0; i < skillList.Length; i++)
                    {
                        if (classUsed == "Lord Of Order" && (monsHP <= 4700000 && monsHP >= 4400000))
                            Bot.Skills.UseSkill(4);
                        if (Bot.Target.HasActiveAura("Focus"))
                            Bot.Skills.UseSkill(5);
                        if(Bot.Skills.CanUseSkill(skillList[i]))
                            Bot.Skills.UseSkill(skillList[i]);
                        Bot.Skills.UseSkill(2);

                    }
                }
            }
            catch { }
        }
        Core.Jump(waitIn);
        waitForSignal($"{cellMsg}Done", waitIn);
        Core.Logger(doneMsg);
        Bot.Events.PlayerAFK -= PlayerAFK;
        Bot.Sleep(1000);
    }

    private void killWithArmy(
        UltraBosses whatUltra,
        string bossId,
        string cellMsg,
        string clientName,
        string mapName,
        string waitIn,
        string doneMsg,
        string bossCell,
        string bossPad,
        int questId,
        string item,
        Action skillAction
    )
    {
        Bot.Events.PlayerAFK += PlayerAFK;

        if (!Core.CheckInventory("Scroll of Life Steal", 60))
            Core.BuyItem("arcangrove", 211, "Scroll of Life Steal", 99);

        Core.EnsureAccept(questId);

        // initClass(whatUltra);

        string[] monsList = monsPriorityID.Split(',');

        setClient(clientName);
        sArmy.ClearLogFile();

        Core.Join(mapName);
        // waitForParty(waitIn, "Spawn");
        sArmy.waitForPartyCell("Enter", "Spawn");
        waitForSignal(cellMsg, waitIn);
        sArmy.registerMessage(doneMsg, false);

        // string classUsed = Bot.Player.CurrentClass!.Name;

        Bot.Skills.UseSkill(1);
        Bot.Sleep(1000);
        Bot.Skills.UseSkill(2);
        Bot.Sleep(1000);
        Bot.Skills.UseSkill(3);

        if (Bot.Map.Name.ToLower() == mapName)
        {
            if (Bot.Player.CurrentClass!.Name != "ArchPaladin")
                Bot.Sleep(200);
            Core.Jump(bossCell, bossPad);
            Bot.Skills.UseSkill(2);
            Bot.Player.SetSpawnPoint();
        }

        bool needSendDone = true;
        int countCheck = 0;
        Core.Logger($"starting {mapName}");
        bool fight = true;
        int countRestart = 0;

        while (!Bot.ShouldExit && fight)
        {
            try
            {
                if (!Bot.Player.Alive)
                {
                    Bot.Sleep(500);
                    continue;
                }
                if ((Bot.TempInv.Contains(item) || Bot.Inventory.Contains(item)) && needSendDone)
                {
                    if (sArmy.sendDone())
                        needSendDone = false;
                }
                if (!needSendDone && sArmy.isDone() && countCheck % 5 == 0)
                {
                    break;
                }
                countCheck++;

                doPriorityAttack(monsList);
                if (Bot.Player.HasTarget)
                {
                    skillAction();
                }
            }
            catch { }
        }
        Core.Jump(waitIn);
        waitForSignal($"{cellMsg}Done", waitIn);
        Core.Logger(doneMsg);
        Bot.Events.PlayerAFK -= PlayerAFK;
        Bot.Sleep(1000);
    }

    private bool zoneA;
    private bool zoneB;

    public void QueenIona()
    {
        string bossId = "1";
        string cellMsg = "inQueenIonaCell";
        string clientName = "Queen Iona";
        string mapName = "queeniona";
        string waitIn = "Enter";
        string bossCell = "r2";
        string bossPad = "Left";
        string item = "Lightning Diadem";
        int questId = 9852;

        Bot.Events.RunToArea += moveQueenIona;

        Bot.Events.PlayerAFK += PlayerAFK;

        Core.EnsureAccept(questId);

        // initClass(whatUltra);

        string[] monsList = bossId.Split(',');

        setClient(clientName);
        equipAllNeeded(
            "Void Highlord",
            Bot.Config.Get<string>("Dauntless"),
            Bot.Config.Get<string>("Vainglory"),
            Bot.Config.Get<string>("AnimaHelm")
        );
        Core.PrivateRoomNumber = 999999;
        Core.Join(mapName);
        Core.Jump(bossCell, bossPad);
        Core.Logger($"starting {mapName}");
        
        Bot.Skills.StartAdvanced("1 | 2 | 4");

        while (!Bot.ShouldExit)
        {
            try
            {
                if (!Bot.Player.Alive)
                {
                    Bot.Sleep(500);
                    continue;
                }
                if ((Bot.TempInv.Contains(item) || Bot.Inventory.Contains(item)))
                {
                    break;
                }

                doPriorityAttack(monsList);
                if (Bot.Player.HasTarget)
                {
                    Bot.Combat.Attack("*");
                    bool posCharge = Bot.Self.HasActiveAura("Positive Charge");
                    bool posChargeQ = Bot.Self.HasActiveAura("Positive Charge?");
                    bool negCharge = Bot.Self.HasActiveAura("Negative Charge");
                    bool negChargeQ = Bot.Self.HasActiveAura("Negative Charge?");
                    if (zoneA){

                        if (posCharge || negChargeQ){
                            _walkFlash((591, 736), (360, 372));
                            zoneA = false;
                        }
                        if (negCharge || posChargeQ){
                            _walkFlash((203, 364), (380, 420));
                            zoneA = false;
                        }
                    }
                    if (zoneB){

                        if (posCharge || negChargeQ){
                            _walkFlash((203, 364), (380, 420));
                            zoneB = false;
                        }
                        if (negCharge || posChargeQ){
                            _walkFlash((591, 736), (360, 372));
                            zoneB = false;
                        }
                    }
                }
            }
            catch { }
        }
        Core.Jump("Enter");
        Core.Logger("Queen Iona done");
        Bot.Events.PlayerAFK -= PlayerAFK;
        Bot.Events.RunToArea -= moveQueenIona;
        Bot.Sleep(1000);

    }

    void _walkFlash((int, int) X, (int, int) Y) => Bot.Flash.Call("walkTo", Bot.Random.Next(X.Item1, X.Item2), Bot.Random.Next(Y.Item1, Y.Item2), 8);

    void moveQueenIona(string zone){
            switch(zone.ToLower()){
                case "a":
                    zoneA = true;
                    zoneB = false;
                    
                    break;
                case "b":
                    zoneA = false;
                    zoneB = true;
                    
                    break;
                default:
                    zoneA = false;
                    zoneB = false;
                    break;
                
            }
        }


    public void waitForParty(string? cell = null, string? pad = null)
    {
        int i = 0;
        if (cell != null)
            Core.Jump(cell, pad != null ? pad : "Left");
        int PartySize = PartyMembers.Length;
        while (
            !Bot.ShouldExit
            && (
                cell != null && Bot.Map.CellPlayers != null && Bot.Map.CellPlayers.Count() > 0
                    ? Bot.Map.CellPlayers.Count()
                    : Bot.Map.PlayerCount
            ) != PartySize
        )
        {
            Bot.Sleep(500);
            i++;

            if (i >= 4)
            {
                if (cell != null && Bot.Map.CellPlayers != null && Bot.Map.CellPlayers.Count() > 0)
                {
                    var missingPlayers = PartyMembers
                        .Where(x => !Bot.Map.CellPlayers.Select(y => y.Name).Contains(x))
                        .ToList();
                    if (missingPlayers.Count == 1 && missingPlayers[0] == Bot.Player.Username)
                    {
                        Core.Logger("Bugged lobby, we were the only one missing?");
                        break;
                    }
                    Core.Logger(
                        $"[{Bot.Map.CellPlayers.Count()}/{PartySize}] Waiting for {String.Join(" & ", missingPlayers)}"
                    );
                }
                else if (Bot.Map.PlayerNames != null && Bot.Map.PlayerNames.Count() > 0)
                {
                    var missingPlayers = PartyMembers
                        .Where(x => !Bot.Map.PlayerNames.Contains(x))
                        .ToList();
                    if (missingPlayers.Count == 1 && missingPlayers[0] == Bot.Player.Username)
                    {
                        Core.Logger("Bugged lobby, we were the only one missing?");
                        break;
                    }
                    Core.Logger(
                        $"[{Bot.Map.PlayerNames.Count()}/{PartySize}] Waiting for {String.Join(" & ", missingPlayers)}"
                    );
                }
                else
                {
                    Core.Logger(
                        $"[{Bot.Map.PlayerCount}/{PartySize}] Waiting for the rest of the party"
                    );
                }
                i = 0;
            }
        }
    }

    private void waitForSignal(string message, string waitIn = "Enter", bool delPrevMsg = false)
    {
        sArmy.registerMessage(message, delPrevMsg);
        while (!Bot.ShouldExit && Bot.Player.Cell == waitIn)
        {
            sArmy.sendDone();
            if (sArmy.isAlreadyInLog(new string[] { Bot.Player.Username.ToLower() }))
                break;
            Bot.Sleep(500);
        }
        while (!Bot.ShouldExit)
        {
            if (sArmy.isDone())
                break;
            Bot.Sleep(500);
        }
    }

    private bool doneHandler(string waitIn, string bossCell, string bossPad, string tempInv)
    {
        bool needSendDone = true;
        int countCheck = 0;
        Core.Jump(waitIn);
        bool outputReturn = true;
        while (!Bot.ShouldExit)
        {
            if (countCheck >= 100)
            {
                countCheck = 0;
                // Core.Jump(bossCell, bossPad);
                outputReturn = true;
                break;
            }
            if (Bot.TempInv.Contains(tempInv) && needSendDone)
            {
                // Bot.Sleep(500);
                if (sArmy.sendDone())
                    needSendDone = false;
            }
            if (!needSendDone && sArmy.isDone() && countCheck % 5 == 0)
            {
                // Bot.Sleep(500);
                outputReturn = false;
                break;
            }
            countCheck++;
            // if (countCheck > 5)
            //     countCheck = 0;
        }
        return outputReturn;
    }

    private bool forceSkill = false;
    private int skillToForce;
    private void ForceSkill(int skill, int wait = 0)
    {
        // forceSkill = true;
        // skillToForce = skill;
        if (!Bot.Player.Alive)
            return;
        if (Bot.Player.HasTarget)
        {
            while (!Bot.ShouldExit && !Bot.Skills.CanUseSkill(skill))
            {
                // await Task.Delay(10);
                Bot.Sleep(5);
            }
            Bot.Skills.UseSkill(skill);
            Bot.Skills.UseSkill(skill);
            Bot.Skills.UseSkill(skill);
        }

    }

    private int healHealthLessThan;
    private int skillHeal;

    private void PlayersHP(dynamic packet)
    {
        string type = packet["params"].type;
        dynamic data = packet["params"].dataObj;
        if (type is not null and "json")
        {
            string cmd = data.cmd;
            switch (cmd)
            {
                case "ct":
                    var p = data.p;
                    foreach (var item in p)
                    {
                        string username = item.Name;
                        var userInfo = item.Value;
                        if (userInfo.intHP != null)
                        {
                            int intHP = userInfo.intHP;
                            if (playersHP.ContainsKey(username))
                            {
                                playersHP[username] = intHP; // Update the value if the key exists
                            }
                            else
                            {
                                playersHP.Add(username, intHP); // Add a new entry if the key does not exist
                            }
                            
                            // if (intHP < healHealthLessThan){
                            //     Core.Logger($"Username: {username}, intHP: {intHP}");
                            //         Bot.Skills.UseSkill(skillHeal);
                            //         Bot.Skills.UseSkill(skillHeal);
                            //         Bot.Skills.UseSkill(skillHeal);
                            //         Bot.Skills.UseSkill(skillHeal);
                            // }
                        }
                    }
                    foreach(KeyValuePair<string, int> ele in playersHP)
                        {
                            if(ele.Value <= healHealthLessThan){
                                if(Bot.Skills.CanUseSkill(skillHeal))
                                    Bot.Skills.UseSkill(skillHeal);
                            }
                        }
                    break;
            }
        }
    }

    private int speakerCounter = 0;
    private bool inZone = false;

    private (string, string, string, int, bool) whatAction()
    {
        // who taunt, who zone, in/out, wait skill, need prax
        switch (speakerCounter)
        {
            case 0:
                return ("Legion Revenant", null, null, 0, false);
            case 1:
                return ("Lord Of Order", "Verus DoomKnight", "IN", 0, false);
            case 2:
                return ("ArchPaladin", "Verus DoomKnight", "OUT", 0, false);
            case 3:
                return ("Lord Of Order", null, null, 500, false);
            case 4: // LR PRAXIS
                return (null, "Legion Revenant", "IN", 0, true);
            case 5:
                return ("Legion Revenant", "Legion Revenant", "OUT", 500, false);
            case 7: // LR PRAXIS
                return ("Lord Of Order", null, null, 500, true);
            case 8:
                return (null, "ArchPaladin", "IN", 0, false);
            case 9:
                return ("ArchPaladin", "ArchPaladin", "OUT", 0, false);
            case 10:
                return ("Legion Revenant", null, null, 500, false);
            case 11: // LR PRAXIS
                return (null, "Lord Of Order", "IN", 0, true);
            case 12:
                return ("Lord Of Order", "Lord Of Order", "OUT", 500, false);
            case 14: // LR PRAXIS
                return ("Legion Revenant", null, null, 0, true);
            case 15:
                speakerCounter = 1;
                return ("Lord Of Order", "Verus DoomKnight", "IN", 500, false);
        }
        return (null, null, null, 0, false);
    }

    private void UltraSpeakerTodo(dynamic packet)
    {
        string type = packet["params"].type;
        dynamic data = packet["params"].dataObj;
        if (type is not null and "json")
        {
            string cmd = data.cmd;
            switch (cmd)
            {
                case "event":
                    string zone = data.args?["zoneSet"]!;
                    string currentClass2 = Bot.Player.CurrentClass?.Name;
                    if (zone is not null && zone == "A")
                    {
                        if (Bot.Player.CurrentClass?.Name == "LegionRevenant")
                        {
                            ForceSkill(1);
                        }
                    }
                    break;

                case "ct":
                    int playerPositionX = Bot.Player.X;
                    string? currentClass = Bot.Player.CurrentClass?.Name;
                    if (!inZone)
                    {
                        if (playerPositionX != 100 && Bot.Map.Name.ToLower() == "ultraspeaker")
                        {
                            Bot.Flash.Call("walkTo", 100, 321, 8);
                        }
                    }
                    dynamic anims = data.anims?[0]!;
                    if (anims is not null)
                    {
                        string msg = anims["msg"];

                        if (
                            msg is not null
                            && (msg.ToLower().Contains("truth") || msg.ToLower().Contains("listen"))
                        )
                        {
                            var act = whatAction();

                            speakerCounter++;

                            if (currentClass == act.Item2)
                            {
                                if (act.Item3 == "IN")
                                {
                                    inZone = true;
                                    if (
                                        playerPositionX != 203
                                        && Bot.Map.Name.ToLower() == "ultraspeaker"
                                    )
                                    {
                                        Bot.Flash.Call("walkTo", 203, 301, 8);
                                    }
                                }
                                if (act.Item3 == "OUT")
                                {
                                    inZone = false;
                                    if (
                                        playerPositionX != 100
                                        && Bot.Map.Name.ToLower() == "ultraspeaker"
                                    )
                                    {
                                        Bot.Flash.Call("walkTo", 100, 321, 8);
                                    }
                                }
                            }
                            if (currentClass == act.Item1)
                            {
                                ForceSkill(5, act.Item4);
                            }
                        }
                    }
                    break;
            }
        }
    }


    private bool drakathHealthCheck(int HP)
    {
        if (HP >= 18020000 && HP <= 18250000)
            return true;
        if (HP >= 16020000 && HP <= 16250000)
            return true;
        if (HP >= 14020000 && HP <= 14250000)
            return true;
        if (HP >= 12020000 && HP <= 12250000)
            return true;
        if (HP >= 10020000 && HP <= 10150000)
            return true;
        if (HP >= 8020000 && HP <= 8200000)
            return true;
        if (HP >= 6020000 && HP <= 6200000)
            return true;
        if (HP >= 4020000 && HP <= 4200000)
            return true;
        if (HP >= 2020000 && HP <= 2200000)
            return true;
        return false;
    }

    private void doPriorityAttack(string[] monsterList)
    {
        for (int i = 0; i < monsterList.Length; i++)
        {
            if (isMonsterAlive(monsterList[i]))
            {
                int x = 0;
                if (Int32.TryParse(monsterList[i], out x))
                {
                    Bot.Combat.Attack(x);
                    return;
                }
            }
        }
    }

    private bool monsterAvail(string[] monsterList)
    {
        for (int i = 0; i < monsterList.Length; i++)
        {
            if (isMonsterAlive(monsterList[i]))
            {
                return true;
            }
        }
        return false;
    }

    private bool isMonsterAlive(string monMapID)
    {
        try
        {
            string jsonData = Bot.Flash.Call("availableMonsters");
            var monsters = JArray.Parse(jsonData);
            if (monsters.Count == 0)
            {
                return false;
            }

            foreach (var monster in monsters)
            {
                if (monster["MonMapID"]?.ToString() == monMapID)
                {
                    var intState = monster["intState"]?.ToString();

                    if (string.IsNullOrEmpty(intState) || intState == "1" || intState == "2")
                    {
                        return true;
                    }
                    else if (intState == "0")
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        catch
        {
            return true;
        }
    }

    private int GetMonsterHP(string monMapID)
    {
        try
        {
            string jsonData = Bot.Flash.Call("availableMonsters");
            var monsters = JArray.Parse(jsonData);

            foreach (var mon in monsters)
            {
                if (mon["MonMapID"]?.ToString() == monMapID)
                {
                    return mon["intHP"]?.ToObject<int>() ?? 0;
                }
            }
            return 0;
        }
        catch
        {
            return 0;
        }
    }

    private void initClass(UltraBosses whatUltra)
    {
        switch (whatUltra)
        {
            case UltraBosses.UltraEngineer:
                if (player1.ToLower() == Bot.Player.Username.ToLower())
                    ultraEngineerClass(1);
                else if (player2.ToLower() == Bot.Player.Username.ToLower())
                    ultraEngineerClass(2);
                else if (player3.ToLower() == Bot.Player.Username.ToLower())
                    ultraEngineerClass(3);
                else if (player4.ToLower() == Bot.Player.Username.ToLower())
                    ultraEngineerClass(4);
                break;
            case UltraBosses.UltraTyndarius:
                if (player1.ToLower() == Bot.Player.Username.ToLower())
                    ultraTyndariusClass(1);
                else if (player2.ToLower() == Bot.Player.Username.ToLower())
                    ultraTyndariusClass(2);
                else if (player3.ToLower() == Bot.Player.Username.ToLower())
                    ultraTyndariusClass(3);
                else if (player4.ToLower() == Bot.Player.Username.ToLower())
                    ultraTyndariusClass(4);
                break;
            case UltraBosses.UltraWarden:
                if (player1.ToLower() == Bot.Player.Username.ToLower())
                    ultraWardenClass(1);
                else if (player2.ToLower() == Bot.Player.Username.ToLower())
                    ultraWardenClass(2);
                else if (player3.ToLower() == Bot.Player.Username.ToLower())
                    ultraWardenClass(3);
                else if (player4.ToLower() == Bot.Player.Username.ToLower())
                    ultraWardenClass(4);
                break;
            case UltraBosses.UltraEzrajal:
                if (player1.ToLower() == Bot.Player.Username.ToLower())
                    ultraEzrajalClass(1);
                else if (player2.ToLower() == Bot.Player.Username.ToLower())
                    ultraEzrajalClass(2);
                else if (player3.ToLower() == Bot.Player.Username.ToLower())
                    ultraEzrajalClass(3);
                else if (player4.ToLower() == Bot.Player.Username.ToLower())
                    ultraEzrajalClass(4);
                break;
            case UltraBosses.Championdrakath:
                if (player1.ToLower() == Bot.Player.Username.ToLower())
                    championDrakthClass(1);
                else if (player2.ToLower() == Bot.Player.Username.ToLower())
                    championDrakthClass(2);
                else if (player3.ToLower() == Bot.Player.Username.ToLower())
                    championDrakthClass(3);
                else if (player4.ToLower() == Bot.Player.Username.ToLower())
                    championDrakthClass(4);
                break;
            case UltraBosses.UltraDage:
                if (player1.ToLower() == Bot.Player.Username.ToLower())
                    ultraDageClass(1);
                else if (player2.ToLower() == Bot.Player.Username.ToLower())
                    ultraDageClass(2);
                else if (player3.ToLower() == Bot.Player.Username.ToLower())
                    ultraDageClass(3);
                else if (player4.ToLower() == Bot.Player.Username.ToLower())
                    ultraDageClass(4);
                break;
            case UltraBosses.UltraNulgath:
                if (player1.ToLower() == Bot.Player.Username.ToLower())
                    ultraNulgathClass(1);
                else if (player2.ToLower() == Bot.Player.Username.ToLower())
                    ultraNulgathClass(2);
                else if (player3.ToLower() == Bot.Player.Username.ToLower())
                    ultraNulgathClass(3);
                else if (player4.ToLower() == Bot.Player.Username.ToLower())
                    ultraNulgathClass(4);
                break;
            case UltraBosses.UltraDrago:
                if (player1.ToLower() == Bot.Player.Username.ToLower())
                    ultraDragoClass(1);
                else if (player2.ToLower() == Bot.Player.Username.ToLower())
                    ultraDragoClass(2);
                else if (player3.ToLower() == Bot.Player.Username.ToLower())
                    ultraDragoClass(3);
                else if (player4.ToLower() == Bot.Player.Username.ToLower())
                    ultraDragoClass(4);
                break;
            case UltraBosses.UltraSpeaker:
                if (player1.ToLower() == Bot.Player.Username.ToLower())
                    ultraSpeakerClass(1);
                else if (player2.ToLower() == Bot.Player.Username.ToLower())
                    ultraSpeakerClass(2);
                else if (player3.ToLower() == Bot.Player.Username.ToLower())
                    ultraSpeakerClass(3);
                else if (player4.ToLower() == Bot.Player.Username.ToLower())
                    ultraSpeakerClass(4);
                break;
            case UltraBosses.UltraDarkon:
                if (player1.ToLower() == Bot.Player.Username.ToLower())
                    ultraDarkonClass(1);
                else if (player2.ToLower() == Bot.Player.Username.ToLower())
                    ultraDarkonClass(2);
                else if (player3.ToLower() == Bot.Player.Username.ToLower())
                    ultraDarkonClass(3);
                else if (player4.ToLower() == Bot.Player.Username.ToLower())
                    ultraDarkonClass(4);
                break;
        }
    }

    private void ultraWardenClass(int optionClass)
    {
        monsPriorityID = "1";
        switch (optionClass)
        {
            case 1:
                skillList = new int[] { 1, 2, 3, 4, 5 };
                equipAllNeeded(
                    "Legion Revenant",
                    Bot.Config.Get<string>("Valiance"),
                    Bot.Config.Get<string>("Penitence"),
                    Bot.Config.Get<string>("WizHelm"),
                    "Scroll of Enrage"
                );
                // skillList = new int[] { 1, 2, 3, 4 };
                // equipAllNeeded(
                //     "Legion Revenant",
                //     Bot.Config.Get<string>("Arcana"),
                //     Bot.Config.Get<string>("Vainglory"),
                //     Bot.Config.Get<string>("WizHelm"),
                //     "Scroll of Enrage"
                // );
                skillHeal = 2;
                healHealthLessThan = 0;
                break;
            case 2:
                skillList = new int[] { 1, 2, 3, 4, 5 };
                // equipAllNeeded(
                //     "Verus DoomKnight",
                //     Bot.Config.Get<string>("Dauntless"),
                //     Bot.Config.Get<string>("Lament"),
                //     Bot.Config.Get<string>("WizHelm"),
                //     "Scroll of Enrage"
                // );
                equipAllNeeded(
                    "Chaos Avenger",
                    Bot.Config.Get<string>("Valiance"),
                    Bot.Config.Get<string>("Penitence"),
                    Bot.Config.Get<string>("LuckHelm"),
                    "Scroll of Enrage"
                );
                // equipAllNeeded(
                //     "Verus DoomKnight",
                //     Bot.Config.Get<string>("Dauntless"),
                //     Bot.Config.Get<string>("Vainglory"),
                //     Bot.Config.Get<string>("AnimaHelm"),
                //     "Scroll of Enrage"
                // );
                skillHeal = 2;
                healHealthLessThan = 0;
                break;
            case 3:
                skillList = new int[] { 1, 3, 4, 2}; // AweBlast
                equipAllNeeded(
                    "Lord Of Order",
                    Bot.Config.Get<string>("Valiance"),
                    Bot.Config.Get<string>("Penitence"), // try penitence/vain
                    Bot.Config.Get<string>("LuckHelm"),
                    "Scroll of Life Steal"
                );
                // skillList = new int[] { 1, 3, 4 }; // AweBlast
                // equipAllNeeded(
                //     "Lord Of Order",
                //     Bot.Config.Get<string>("Valiance"),
                //     Bot.Config.Get<string>("Avarice"), // try penitence/vain
                //     Bot.Config.Get<string>("LuckHelm"),
                //     "Scroll of Life Steal"
                // );
                skillHeal = 2;
                healHealthLessThan = 2500;
                break;
            case 4:
                skillList = new int[] { 1, 2, 3, 4 }; // ravenous
                equipAllNeeded(
                    "ArchPaladin",
                    Bot.Config.Get<string>("Valiance"),
                    Bot.Config.Get<string>("Penitence"), // try penitence/vain
                    Bot.Config.Get<string>("LuckHelm"),
                    "Scroll of Enrage"
                );
                // skillList = new int[] { 1, 2, 3, 4 }; // ravenous
                // equipAllNeeded(
                //     "ArchPaladin",
                //     Bot.Config.Get<string>("Valiance"),
                //     Bot.Config.Get<string>("Avarice"), // try penitence/vain
                //     Bot.Config.Get<string>("LuckHelm"),
                //     "Scroll of Life Steal"
                // );
                skillHeal = 2;
                healHealthLessThan = 2000;
                break;
        }
    }

    private void ultraTyndariusClass(int optionClass)
    {
        switch (optionClass)
        {
            case 1:
                monsPriorityID = "1,3,2";
                // skillList = new int[] { 1, 2, 3, 4 };
                // equipAllNeeded(
                //     "Legion Revenant",
                //     Bot.Config.Get<string>("Arcana"),
                //     Bot.Config.Get<string>("Vainglory"),
                //     Bot.Config.Get<string>("ForgeHelm"),
                //     "Scroll of Enrage"
                // );
                skillList = new int[] { 1, 2, 3, 4, 5 };
                equipAllNeeded(
                    "Legion Revenant",
                    Bot.Config.Get<string>("Valiance"),
                    Bot.Config.Get<string>("Penitence"),
                    Bot.Config.Get<string>("WizHelm"),
                    "Scroll of Enrage"
                );
                skillHeal = 2;
                healHealthLessThan = 0;
                break;
            case 2:
                monsPriorityID = "3,1,2";
                // skillList = new int[] { 1, 2, 3, 4 };
                // equipAllNeeded(
                //     "Quantum Chronomancer",
                //     Bot.Config.Get<string>("Valiance"),
                //     Bot.Config.Get<string>("Lament"),
                //     Bot.Config.Get<string>("ForgeHelm"),
                //     "Scroll of Enrage"
                // );
                skillList = new int[] { 1, 3, 4, 2}; 
                equipAllNeeded(
                    "Chaos Avenger",
                    Bot.Config.Get<string>("Valiance"),
                    Bot.Config.Get<string>("Penitence"),
                    Bot.Config.Get<string>("LuckHelm"),
                    "Scroll of Enrage"
                );
                skillHeal = 2;
                healHealthLessThan = 0;
                break;
            case 3:
                monsPriorityID = "2,1,3";
                // skillList = new int[] { 1, 3, 4 };
                // equipAllNeeded(
                //     "Lord Of Order",
                //     Bot.Config.Get<string>("AweBlast"),
                //     Bot.Config.Get<string>("Penitence"),
                //     Bot.Config.Get<string>("LuckHelm"),
                //     "Scroll of Life Steal"
                // );
                skillList = new int[] { 1, 3, 4, 2}; 
                equipAllNeeded(
                    "Lord Of Order",
                    Bot.Config.Get<string>("Valiance"),
                    Bot.Config.Get<string>("Penitence"),
                    Bot.Config.Get<string>("LuckHelm"),
                    "Scroll of Life Steal"
                );
                skillHeal = 2;
                healHealthLessThan = 2500;
                break;
            case 4:
                monsPriorityID = "2,1,3";
                // skillList = new int[] { 1, 3, 4 }; // ravenous
                // equipAllNeeded(
                //     "ArchPaladin",
                //     Bot.Config.Get<string>("Valiance"),
                //     Bot.Config.Get<string>("Penitence"),
                //     Bot.Config.Get<string>("LuckHelm"),
                //     "Scroll of Enrage"
                // );
                skillList = new int[] { 1, 2, 3, 4 };
                equipAllNeeded(
                    "ArchPaladin",
                    Bot.Config.Get<string>("Valiance"),
                    Bot.Config.Get<string>("Penitence"),
                    Bot.Config.Get<string>("LuckHelm"),
                    "Scroll of Enrage"
                );
                skillHeal = 2;
                healHealthLessThan = 2500;
                break;
        }
    }

    private void ultraEngineerClass(int optionClass)
    {
        monsPriorityID = "1,2,3";
        switch (optionClass)
        {
            case 1:
                // skillList = new int[] { 1, 2, 3, 4 };
                // equipAllNeeded(
                //     "Legion Revenant",
                //     Bot.Config.Get<string>("Arcana"),
                //     Bot.Config.Get<string>("Vainglory"),
                //     Bot.Config.Get<string>("ForgeHelm"),
                //     "Scroll of Enrage"
                // );
                skillList = new int[] { 1, 2, 3, 4, 5 };
                equipAllNeeded(
                    "Legion Revenant",
                    Bot.Config.Get<string>("Valiance"),
                    Bot.Config.Get<string>("Penitence"),
                    Bot.Config.Get<string>("WizHelm"),
                    "Scroll of Enrage"
                );
                skillHeal = 2;
                healHealthLessThan = 0;
                break;
            case 2:
                // skillList = new int[] { 1, 2, 3, 4 };
                // equipAllNeeded(
                //     "Quantum Chronomancer",
                //     Bot.Config.Get<string>("Valiance"),
                //     Bot.Config.Get<string>("Lament"),
                //     Bot.Config.Get<string>("ForgeHelm"),
                //     "Scroll of Life Steal"
                // );
                skillList = new int[] { 1, 3, 4, 2}; 
                equipAllNeeded(
                    "Chaos Avenger",
                    Bot.Config.Get<string>("Valiance"),
                    Bot.Config.Get<string>("Penitence"),
                    Bot.Config.Get<string>("LuckHelm"),
                    "Scroll of Enrage"
                );
                skillHeal = 2;
                healHealthLessThan = 0;
                break;
            case 3:
                // skillList = new int[] { 1, 3, 4 };
                // equipAllNeeded(
                //     "Lord Of Order",
                //     Bot.Config.Get<string>("AweBlast"),
                //     Bot.Config.Get<string>("Penitence"),
                //     Bot.Config.Get<string>("LuckHelm"),
                //     "Scroll of Life Steal"
                // );
                skillList = new int[] { 1, 3, 4, 2}; 
                equipAllNeeded(
                    "Lord Of Order",
                    Bot.Config.Get<string>("Valiance"),
                    Bot.Config.Get<string>("Penitence"),
                    Bot.Config.Get<string>("LuckHelm"),
                    "Scroll of Life Steal"
                );
                skillHeal = 2;
                healHealthLessThan = 2500;
                break;
            case 4:
                // skillList = new int[] { 1, 3, 4 }; // ravenous
                // equipAllNeeded(
                //     "ArchPaladin",
                //     Bot.Config.Get<string>("Valiance"),
                //     Bot.Config.Get<string>("Penitence"),
                //     Bot.Config.Get<string>("LuckHelm"),
                //     "Scroll of Enrage"
                // );
                skillList = new int[] { 1, 2, 3, 4 };
                equipAllNeeded(
                    "ArchPaladin",
                    Bot.Config.Get<string>("Valiance"),
                    Bot.Config.Get<string>("Penitence"),
                    Bot.Config.Get<string>("LuckHelm"),
                    "Scroll of Enrage"
                );
                skillHeal = 2;
                healHealthLessThan = 2500;
                break;
        }
    }

    private void ultraEzrajalClass(int optionClass)
    {
        monsPriorityID = "1";
        switch (optionClass)
        {
            case 1:
                // skillList = new int[] { 1, 2, 3, 4 };
                // equipAllNeeded(
                //     "Legion Revenant",
                //     Bot.Config.Get<string>("Arcana"),
                //     Bot.Config.Get<string>("Vainglory"),
                //     Bot.Config.Get<string>("ForgeHelm"),
                //     "Scroll of Enrage"
                // );
                skillList = new int[] { 1, 2, 3, 4, 5 };
                equipAllNeeded(
                    "Legion Revenant",
                    Bot.Config.Get<string>("Valiance"),
                    Bot.Config.Get<string>("Penitence"),
                    Bot.Config.Get<string>("WizHelm"),
                    "Scroll of Enrage"
                );
                skillHeal = 2;
                healHealthLessThan = 0;
                break;
            case 2:
                // skillList = new int[] { 1, 2, 3, 4 };
                // equipAllNeeded(
                //     "Quantum Chronomancer",
                //     Bot.Config.Get<string>("Valiance"),
                //     Bot.Config.Get<string>("Lament"),
                //     Bot.Config.Get<string>("ForgeHelm"),
                //     "Scroll of Life Steal"
                // );
                skillList = new int[] { 1, 3, 4, 2}; 
                equipAllNeeded(
                    "Chaos Avenger",
                    Bot.Config.Get<string>("Valiance"),
                    Bot.Config.Get<string>("Penitence"),
                    Bot.Config.Get<string>("LuckHelm"),
                    "Scroll of Enrage"
                );
                skillHeal = 2;
                healHealthLessThan = 0;
                break;
            case 3:
                // skillList = new int[] { 1, 3, 4 };
                // equipAllNeeded(
                //     "Lord Of Order",
                //     Bot.Config.Get<string>("AweBlast"),
                //     Bot.Config.Get<string>("Penitence"),
                //     Bot.Config.Get<string>("ForgeHelm"),
                //     "Scroll of Life Steal"
                // );
                skillList = new int[] { 1, 3, 4, 2}; 
                equipAllNeeded(
                    "Lord Of Order",
                    Bot.Config.Get<string>("Valiance"),
                    Bot.Config.Get<string>("Penitence"),
                    Bot.Config.Get<string>("LuckHelm"),
                    "Scroll of Life Steal"
                );
                skillHeal = 2;
                healHealthLessThan = 2500;
                break;
            case 4:
                // skillList = new int[] { 1, 3, 4 }; // ravenous
                // equipAllNeeded(
                //     "ArchPaladin",
                //     Bot.Config.Get<string>("Valiance"),
                //     Bot.Config.Get<string>("Penitence"),
                //     Bot.Config.Get<string>("LuckHelm"),
                //     "Scroll of Enrage"
                // );
                skillList = new int[] { 1, 2, 3, 4 };
                equipAllNeeded(
                    "ArchPaladin",
                    Bot.Config.Get<string>("Valiance"),
                    Bot.Config.Get<string>("Penitence"),
                    Bot.Config.Get<string>("LuckHelm"),
                    "Scroll of Enrage"
                );
                skillHeal = 2;
                healHealthLessThan = 2500;
                break;
        }
    }

    private void championDrakthClass(int optionClass)
    {
        monsPriorityID = "1";
        switch (optionClass)
        {
            case 1:
                skillList = new int[] { 1, 2, 3, 4 };
                equipAllNeeded(
                    "Legion Revenant",
                    Bot.Config.Get<string>("Valiance"),
                    Bot.Config.Get<string>("Vainglory"),
                    Bot.Config.Get<string>("WizHelm"),
                    "Scroll of Enrage",
                    new string[] {"Sage Tonic", "Potent Malevolence Elixir"}
                );
                skillHeal = 2;
                healHealthLessThan = 0;
                break;
            case 2:
                skillList = new int[] { 1, 2, 3, 4 };
                equipAllNeeded(
                    "StoneCrusher",
                    Bot.Config.Get<string>("Lacerate"),
                    Bot.Config.Get<string>("Absolution"),
                    Bot.Config.Get<string>("WizHelm"),
                    "Scroll of Life Steal",
                    new string[] {"Sage Tonic", "Potent Malevolence Elixir"}
                );
                skillHeal = 2;
                healHealthLessThan = 0;
                break;
            case 3:
                skillList = new int[] { 1, 3, 4 };
                equipAllNeeded(
                    "Lord Of Order",
                    Bot.Config.Get<string>("AweBlast"),
                    Bot.Config.Get<string>("Absolution"),
                    Bot.Config.Get<string>("LuckHelm"),
                    "Scroll of Life Steal",
                    new string[] {"Fate Tonic", "Potent Battle Elixir"}
                );
                skillHeal = 2;
                healHealthLessThan = 2500;
                break;
            case 4:
                skillList = new int[] { 1, 3, 4 };
                equipAllNeeded(
                    "ArchPaladin",
                    Bot.Config.Get<string>("Arcana"),
                    Bot.Config.Get<string>("Penitence"),
                    Bot.Config.Get<string>("LuckHelm"),
                    "Scroll of Life Steal",
                    new string[] {"Fate Tonic", "Potent Battle Elixir"}
                );
                skillHeal = 2;
                healHealthLessThan = 2000;
                break;
        }
    }

    private void ultraDageClass(int optionClass)
    {
        monsPriorityID = "1";
        switch (optionClass)
        {
            case 1:
                skillList = new int[] { 1, 2, 3, 4 };
                equipAllNeeded(
                    "Legion Revenant",
                    Bot.Config.Get<string>("Dauntless"),
                    Bot.Config.Get<string>("Penitence"),
                    Bot.Config.Get<string>("WizHelm"),
                    "Scroll of Enrage"
                );
                skillHeal = 2;
                healHealthLessThan = 0;
                break;
            case 2:
                skillList = new int[] { 1, 2, 3, 4 };
                equipAllNeeded(
                    "Quantum Chronomancer",
                    Bot.Config.Get<string>("Dauntless"),
                    Bot.Config.Get<string>("Lament"),
                    Bot.Config.Get<string>("ForgeHelm"),
                    "Scroll of Life Steal"
                );
                skillHeal = 2;
                healHealthLessThan = 0;
                break;
            case 3:
                skillList = new int[] { 1, 2, 3, 4, 5 };
                equipAllNeeded(
                    "Chaos Avenger",
                    Bot.Config.Get<string>("Dauntless"),
                    Bot.Config.Get<string>("Lament"),
                    Bot.Config.Get<string>("ForgeHelm"),
                    "Scroll of Enrage"
                );
                skillHeal = 2;
                healHealthLessThan = 0;
                break;
            case 4:
                skillList = new int[] { 1, 2, 3, 4 };
                equipAllNeeded(
                    "Verus DoomKnight",
                    Bot.Config.Get<string>("Dauntless"),
                    Bot.Config.Get<string>("Lament"),
                    Bot.Config.Get<string>("ForgeHelm"),
                    "Scroll of Life Steal"
                );
                skillHeal = 2;
                healHealthLessThan = 0;
                break;
        }
    }

    private void ultraNulgathClass(int optionClass)
    {
        switch (optionClass)
        {
            case 1:
                monsPriorityID = "2,1";
                skillList = new int[] { 1, 2, 3, 4 };
                equipAllNeeded(
                    "Legion Revenant",
                    Bot.Config.Get<string>("Arcana"),
                    Bot.Config.Get<string>("Vainglory"),
                    Bot.Config.Get<string>("ForgeHelm"),
                    "Scroll of Enrage"
                );
                skillHeal = 2;
                healHealthLessThan = 0;
                break;
            case 2:
                monsPriorityID = "1,2";
                skillList = new int[] { 1, 2, 3, 4 };
                equipAllNeeded(
                    "Quantum Chronomancer",
                    Bot.Config.Get<string>("Valiance"),
                    Bot.Config.Get<string>("Lament"),
                    Bot.Config.Get<string>("ForgeHelm"),
                    "Scroll of Life Steal"
                );
                skillHeal = 2;
                healHealthLessThan = 0;
                break;
            case 3:
                monsPriorityID = "2,1";
                skillList = new int[] { 1, 3, 4 };
                equipAllNeeded(
                    "Lord Of Order",
                    Bot.Config.Get<string>("AweBlast"),
                    Bot.Config.Get<string>("Penitence"),
                    Bot.Config.Get<string>("LuckHelm"),
                    "Scroll of Enrage"
                );
                skillHeal = 2;
                healHealthLessThan = 2500;
                break;
            case 4:
                monsPriorityID = "2,1";
                skillList = new int[] { 1, 3, 4 };
                equipAllNeeded(
                    "ArchPaladin",
                    Bot.Config.Get<string>("Valiance"),
                    Bot.Config.Get<string>("Penitence"),
                    Bot.Config.Get<string>("LuckHelm"),
                    "Scroll of Enrage"
                );
                skillHeal = 2;
                healHealthLessThan = 2000;
                break;
        }
    }

    private void ultraDragoClass(int optionClass)
    {
        switch (optionClass)
        {
            case 1:
                monsPriorityID = "3,1,2";
                skillList = new int[] { 1, 2, 3, 4 };
                equipAllNeeded(
                    "Legion Revenant",
                    Bot.Config.Get<string>("Arcana"),
                    Bot.Config.Get<string>("Vainglory"),
                    Bot.Config.Get<string>("ForgeHelm"),
                    "Scroll of Enrage"
                );
                skillHeal = 2;
                healHealthLessThan = 0;
                break;
            case 2:
                monsPriorityID = "3,1,2";
                skillList = new int[] { 1, 2, 3, 4 };
                equipAllNeeded(
                    "Chaos Avenger",
                    Bot.Config.Get<string>("Dauntless"),
                    Bot.Config.Get<string>("Lament"),
                    Bot.Config.Get<string>("ForgeHelm"),
                    "Scroll of Enrage"
                );
                skillHeal = 2;
                healHealthLessThan = 0;
                break;
            case 3:
                monsPriorityID = "3,1,2";
                skillList = new int[] { 1, 2, 3, 4 };
                equipAllNeeded(
                    "Lord Of Order",
                    Bot.Config.Get<string>("AweBlast"),
                    Bot.Config.Get<string>("Penitence"),
                    Bot.Config.Get<string>("LuckHelm"),
                    "Scroll of Life Steal"
                );
                skillHeal = 2;
                healHealthLessThan = 0;
                break;
            case 4:
                monsPriorityID = "1,3,2";
                skillList = new int[] { 1, 2, 3, 4 };
                equipAllNeeded(
                    "ArchPaladin",
                    Bot.Config.Get<string>("Valiance"),
                    Bot.Config.Get<string>("Penitence"),
                    Bot.Config.Get<string>("LuckHelm"),
                    "Scroll of Enrage"
                );
                skillHeal = 2;
                healHealthLessThan = 0;
                break;
        }
    }

    private void ultraSpeakerClass(int optionClass)
    {
        monsPriorityID = "1";
        switch (optionClass)
        {
            case 1:
                // skillList = new int[] { 2, 3, 4 };
				skillList = new int[] { 1, 2, 3, 4, 5 };
                equipAllNeeded(
                    "Legion Revenant",
                    Bot.Config.Get<string>("Arcana"),
                    Bot.Config.Get<string>("Penitence"),
                    Bot.Config.Get<string>("WizHelm"),
                    "Scroll of Enrage"
                );
                
                skillHeal = 3;
                healHealthLessThan = 2500;
                break;
            case 2:
                skillList = new int[] { 1, 2, 3, 4, 5 };
                equipAllNeeded(
                    "Verus DoomKnight",
                    Bot.Config.Get<string>("Dauntless"),
                    Bot.Config.Get<string>("Penitence"),
                    Bot.Config.Get<string>("LuckHelm"),
                    "Scroll of Enrage"
                );
                skillHeal = 2;
                healHealthLessThan = 0;
                break;
            case 3:
                skillList = new int[] { 1, 3, 4, 5, 2 };
                equipAllNeeded(
                    "Lord Of Order",
                    Bot.Config.Get<string>("Valiance"),
                    Bot.Config.Get<string>("Penitence"),
                    Bot.Config.Get<string>("LuckHelm"),
                    "Scroll of Enrage"
                );
                skillHeal = 2;
                healHealthLessThan = 2500;
                break;
            case 4:
                skillList = new int[] { 1, 3, 4, 5, 2  };
                equipAllNeeded(
                    "ArchPaladin",
                    Bot.Config.Get<string>("Lacerate"),
                    Bot.Config.Get<string>("Penitence"),
                    Bot.Config.Get<string>("LuckHelm"),
                    "Scroll of Enrage"
                );
                skillHeal = 2;
                healHealthLessThan = 2000;
                break;
        }
    }

    private void ultraDarkonClass(int optionClass)
    {
        monsPriorityID = "1";
        string[] pots;
        switch (optionClass)
        {
            case 1:
                skillList = new int[] { 1, 2, 5, 4, 3 };
                // equipAllNeeded(
                //     "LightCaster",
                //     Bot.Config.Get<string>("Elysium"),
                //     Bot.Config.Get<string>("Lament"),
                //     Bot.Config.Get<string>("WizHelm"),
                //     "Scroll of Enrage"
                // );
                equipAllNeeded(
                    "Legion Revenant",
                    Bot.Config.Get<string>("Arcana"),
                    Bot.Config.Get<string>("Lament"),
                    Bot.Config.Get<string>("WizHelm"),
                    "Scroll of Enrage",
                    new string[] {"Sage Tonic", "Potent Malevolence Elixir"}
                );
                skillHeal = 2;
                healHealthLessThan = 0;
                break;
            case 2:
                skillList = new int[] { 1, 2, 4, 5 };
                // equipAllNeeded(
                //     "Verus DoomKnight",
                //     Bot.Config.Get<string>("Dauntless"),
                //     Bot.Config.Get<string>("Vainglory"),
                //     Bot.Config.Get<string>("AnimaHelm"),
                //     "Potent Honor Potion"
                // );
                equipAllNeeded(
                    "LightCaster",
                    Bot.Config.Get<string>("Elysium"),
                    Bot.Config.Get<string>("Lament"),
                    Bot.Config.Get<string>("WizHelm"),
                    "Scroll of Enrage",
                    new string[] {"Sage Tonic", "Potent Malevolence Elixir"}
                );
                skillHeal = 3;
                healHealthLessThan = 2000;
                break;
            case 3:
                skillList = new int[] { 1, 3, 4, 5 };
                // equipAllNeeded(
                //     "Lord Of Order",
                //     Bot.Config.Get<string>("Valiance"),
                //     Bot.Config.Get<string>("Absolution"),
                //     Bot.Config.Get<string>("LuckHelm"),
                //     "Scroll of Enrage"
                // );
                equipAllNeeded(
                    "Lord Of Order",
                    Bot.Config.Get<string>("AweBlast"),
                    Bot.Config.Get<string>("Absolution"),
                    Bot.Config.Get<string>("LuckHelm"),
                    "Scroll of Life Steal",
                    new string[] {"Fate Tonic", "Potent Battle Elixir"}
                );
                skillHeal = 2;
                healHealthLessThan = 2500;
                break;
            case 4:
                skillList = new int[] { 1, 5, 3, 4, 2 };
                // equipAllNeeded(
                //     "Legion Revenant",
                //     Bot.Config.Get<string>("Arcana"),
                //     Bot.Config.Get<string>("Avarice"),
                //     Bot.Config.Get<string>("PneumaHelm"),
                //     "Scroll of Enrage"
                // );
                // skillList = new int[] { 1, 2, 3, 2, 5 };
                // equipAllNeeded(
                //     "Frostval Barbarian",
                //     Bot.Config.Get<string>("Valiance"),
                //     Bot.Config.Get<string>("Absolution"),
                //     Bot.Config.Get<string>("AnimaHelm"),
                //     "Scroll of Enrage"
                // );
                // skillList = new int[] { 1, 2, 3, 2, 4 };
                equipAllNeeded(
                    "StoneCrusher",
                    Bot.Config.Get<string>("Valiance"),
                    Bot.Config.Get<string>("Absolution"),
                    Bot.Config.Get<string>("WizHelm"),
                    "Scroll of Enrage",
                    new string[] {"Sage Tonic", "Potent Malevolence Elixir"}
                );
                skillHeal = 2;
                healHealthLessThan = 0;
                break;
        }
    }

    private void equipAllNeeded(
        string className,
        string weapon,
        string cape,
        string helm,
        string scroll = null,
        string[] pots = null
    )
    {
        Core.Equip(Bot.Config.Get<string>("SafeClass"));
        Core.Equip(Bot.Config.Get<string>("SafePot"));
        Core.Equip(className);
        Core.Equip(Bot.Config.Get<string>("SafePot"));
        Core.Equip(weapon);
        Core.Equip(cape);
        Core.Equip(helm);
        if (pots != null){
            foreach (string pot in pots){
                Bot.Sleep(1000);
                _equipAndDrinkConsumable(pot);
            }
        }
        Bot.Sleep(1000);
        if (scroll != null)
            Core.Equip(scroll);
    }

    public void UsePotion()
    {
        var skill = Bot.Flash.GetArrayObject<dynamic>("world.actions.active", 5);
        if (skill == null)
            return;
        Bot.Flash.CallGameFunction(
            "world.testAction",
            JsonConvert.DeserializeObject<ExpandoObject>(JsonConvert.SerializeObject(skill))!
        );
    }

    private void _equipAndDrinkConsumable(string consumableName)
    {
        var item = Bot.Inventory.Items.Find(i =>
            i.Name == consumableName && i.Category == ItemCategory.Item
        );
        if (item == null)
            return;
        if (!item.Equipped)
        {
            Core.Equip(consumableName);
            Bot.Sleep(1000);
        }
        UsePotion();
        Bot.Sleep(500);
    }

    private bool belowHealthPercentage(int percentage)
    {
        int percent = (Bot.Player.Health / Bot.Player.MaxHealth) * 100;
        return percent <= percentage;
    }

    private void setPlayerName()
    {
        player1 = Bot.Config.Get<string>("player1");
        player2 = Bot.Config.Get<string>("player2");
        player3 = Bot.Config.Get<string>("player3");
        player4 = Bot.Config.Get<string>("player4");
    }

    private void setTargetTime(int time)
    {
        DateTime startTime = DateTime.Now;
        targetTime = startTime.AddSeconds(time);
    }
    private void setWaitTaunt(bool wait){
        waitTaunt = wait;
    }
    private bool getWaitTaunt(){
        return waitTaunt;
    }

    private void setClient(string customName)
    {
        Bot.Options.InfiniteRange = true;
        Bot.Options.Magnetise = true;
        Bot.Options.LagKiller = true;
        // Bot.Options.HidePlayers = true;
        // Bot.Options.CustomName = customName;

        // Bot.Flash.SetGameObject("stage.frameRate", 30);
        Bot.Options.SetFPS = 60;
        if (!Bot.Flash.GetGameObject<bool>("ui.monsterIcon.redX.visible"))
            Bot.Flash.CallGameFunction("world.toggleMonsters");
    }

    private void PlayerAFK()
    {
        Core.Logger("Anti-AFK engaged");
        Core.Sleep(1500);
        Bot.Send.Packet("%xt%zm%afk%1%false%");
    }

    public enum UltraBosses
    {
        UltraEngineer = 1,
        UltraTyndarius = 2,
        UltraWarden = 3,
        UltraEzrajal = 4,
        Championdrakath = 5,
        UltraDage = 6,
        UltraNulgath = 7,
        UltraDrago = 8,
        UltraSpeaker = 9,
        UltraDarkon = 10
    }
}
