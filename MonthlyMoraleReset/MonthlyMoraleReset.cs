using Harmony;
using BattleTech;
using System.Reflection;


namespace MonthlyMoraleReset
{
    public static class Control
    {
        public static void Start()
        {
            var harmony = HarmonyInstance.Create("QuarterlyResetMorale.Control.ResetMorale");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        #region Harmony Patch
        [HarmonyPatch(typeof(SimGameState), "OnNewQuarterBegin")]
        public static class OnNewQuarterBeginSimGameStateBattleTechPatch
        {

            public static void Postfix(SimGameState __instance)
            {
                __instance.ResetMorale();
            }
        }
        #endregion

        #region ResetMorale
        public static void ResetMorale(this SimGameState sim)
        {
            int MoraleModifier = 0;
            if (sim.CurDropship == DropshipType.Argo)
            {
                foreach (ShipModuleUpgrade shipModuleUpgrade in sim.ShipUpgrades)
                {
                    foreach (SimGameStat companyStat in shipModuleUpgrade.Stats)
                    {
                        bool isNumeric = false;
                        int modifier = 0;
                        if (companyStat.name == "Morale")
                        {
                            isNumeric = int.TryParse(companyStat.value, out modifier);
                            if (isNumeric)
                            {
                                MoraleModifier += modifier;
                            }
                        }
                    }
                }
            }
            int NewMoraleRating = sim.Constants.Story.StartingMorale + MoraleModifier;
            sim.CompanyStats.ModifyStat<int>("Mission", 0, "COMPANY_MonthlyStartingMorale", StatCollection.StatOperation.Set, NewMoraleRating, -1, true);
            sim.CompanyStats.ModifyStat<int>("Mission", 0, "Morale", StatCollection.StatOperation.Set, NewMoraleRating, -1, true);
        }
        #endregion
    }
}
