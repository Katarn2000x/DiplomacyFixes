using DiplomacyFixes.Extensions;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Localization;

namespace DiplomacyFixes.Patches
{
    [HarmonyPatch(typeof(DefaultClanPoliticsModel))]
    class DefaultClanPoliticsModelPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("CalculateInfluenceChange")]
        public static void CalculateInfluenceChangePatch(ref ExplainedNumber __result, Clan clan, bool includeDescriptions = false)
        {
            if (Settings.Instance.EnableInfluenceBalancing)
            {
                var influenceChange = __result;

                if (Settings.Instance.EnableCorruption)
                {
                    float corruption = clan.GetCorruption();
                    if (corruption > 0)
                        influenceChange.Add(-corruption, new TextObject("{=dUCOV7km}Corruption (too many fiefs)"));
                }

                if (Settings.Instance.EnableInfluenceDecay)
                {
                    int influenceDecayFactor = clan.Influence > Settings.Instance.InfluenceDecayThreshold ? (int)-((clan.Influence - Settings.Instance.InfluenceDecayThreshold) * (Settings.Instance.InfluenceDecayPercentage / 100)) : 0;
                    if (influenceDecayFactor < 0)
                        influenceChange.Add(influenceDecayFactor, new TextObject("{=koTNaZUX}Influence Decay (too much influence)"));
                }

                float maximumInfluenceLoss = Settings.Instance.MaximumInfluenceLoss;
                if (influenceChange.ResultNumber < -maximumInfluenceLoss)
                {
                    influenceChange.Add(-maximumInfluenceLoss, new TextObject("{=uZc8Hg01}Maximum Influence Loss"));
                }

                __result = influenceChange;
            }
        }
    }
}
