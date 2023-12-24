using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LCNoPropsLost.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCNoPropsLost
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class LCNoPropsLostPlugin : BaseUnityPlugin
    {
        private const string modGUID = "LCNoPropsLost";
        private const string modName = "LC No Props Lost";
        private const string modVersion = "1.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        private static LCNoPropsLostPlugin instance;

        public static ManualLogSource Logger { get; private set; }

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }

            Logger = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            harmony.PatchAll(typeof(LCNoPropsLostPlugin));
            harmony.PatchAll(typeof(RoundManagerPatch));
        }
    }
}
