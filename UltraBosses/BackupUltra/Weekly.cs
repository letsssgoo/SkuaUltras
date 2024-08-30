//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Army/CoreArmyLite.cs
//cs_include Scripts/Farm/BuyScrolls.cs
//cs_include Scripts/Army/UltraBosses/BackupUltra/CoreUltra.cs
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Skua.Core.Interfaces;
using Skua.Core.Models;
using Skua.Core.Models.Items;
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class Weekly
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private CoreAdvanced Adv = new();
    private static CoreArmyLite sArmy = new();
    private static CoreUltra cUltra = new();

    public string OptionsStorage = cUltra.OptionsStorage;
    public List<IOption> Options = cUltra.StartOptions;

    public void ScriptMain(IScriptInterface Bot)
    {
        // Core.BankingBlackList.Add("Engineer Insignia");
        Core.SetOptions(disableClassSwap: true);

        cUltra.InitUltra();

        //cUltra.ChampionDrakath();
        //cUltra.UltraDage();
        //cUltra.UltraNulgath();
        //cUltra.UltraDrago();
        cUltra.UltraSpeaker();
        cUltra.UltraDarkon();

        // Core.SetOptions(false);
    }
}