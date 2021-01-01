using System;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace DiplomacyFixes.DiplomaticAction.Alliance
{
    class DeclareAllianceAction : AbstractDiplomaticAction<DeclareAllianceAction>
    {
        public override bool PassesConditions(Kingdom kingdom, Kingdom otherKingdom, bool forcePlayerCharacterCosts = false, bool bypassCosts = false)
        {
            return FormAllianceConditions.Instance.CanApply(kingdom, kingdom, forcePlayerCharacterCosts, bypassCosts);
        }

        protected override void ApplyInternal(Kingdom proposingKingdom, Kingdom otherKingdom, float? customDurationInDays)
        {
            DeclareAlliance(proposingKingdom, otherKingdom);
            Events.Instance.OnAllianceFormed(new AllianceEvent(proposingKingdom, otherKingdom));
        }

        protected override void AssessCosts(Kingdom proposingKingdom, Kingdom otherKingdom, bool forcePlayerCharacterCosts)
        {
            DiplomacyCostCalculator.DetermineCostForFormingAlliance(proposingKingdom, otherKingdom, forcePlayerCharacterCosts).ApplyCost();
        }

        protected override void ShowPlayerInquiry(Kingdom proposingKingdom, Action acceptAction)
        {
            TextObject textObject = new TextObject("{=QbOqatd7}{KINGDOM} is proposing an alliance with {PLAYER_KINGDOM}.");
            textObject.SetTextVariable("KINGDOM", proposingKingdom.Name);
            textObject.SetTextVariable("PLAYER_KINGDOM", Clan.PlayerClan.Kingdom.Name);

            InformationManager.ShowInquiry(new InquiryData(
                new TextObject("{=3pbwc8sh}Alliance Proposal").ToString(),
                textObject.ToString(),
                true,
                true,
                new TextObject(StringConstants.Accept).ToString(),
                new TextObject(StringConstants.Decline).ToString(),
                acceptAction,
                null,
                ""), true);
        }

        // Ripped off from TaleWorlds.CampaignSystem.FactionManager.DeclareAlliance which is currently bugged (sets stance to Neutral, not Alliance).
        public static void DeclareAlliance(IFaction faction1, IFaction faction2)
        {
            if (faction1 != faction2 && !faction1.IsBanditFaction && !faction2.IsBanditFaction)
            {
                typeof(FactionManager).GetMethod("SetStance", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { faction1, faction2, 2});
            }
        }
    }
}
