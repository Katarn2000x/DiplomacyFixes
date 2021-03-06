using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;

namespace DiplomacyFixes.Patches
{
    [HarmonyPatch(typeof(SiegeAftermathCampaignBehavior))]
    class SiegeAftermathCampaignBehaviorPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("menu_settlement_taken_on_init")]
        public static void menu_settlement_taken_on_initPatch()
        {
            Events.Instance.OnPlayerSettlementTaken(Settlement.CurrentSettlement);
        }
    }
}
