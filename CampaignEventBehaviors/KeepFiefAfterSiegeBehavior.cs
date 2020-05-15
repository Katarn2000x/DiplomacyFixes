using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace DiplomacyFixes.CampaignEventBehaviors
{
    class KeepFiefAfterSiegeBehavior : CampaignBehaviorBase
    {
        private List<SettlementClaimantDecision> _decisionsToProcess;

        public KeepFiefAfterSiegeBehavior()
        {
            this._decisionsToProcess = new List<SettlementClaimantDecision>();
        }

        public override void RegisterEvents()
        {
            CampaignEvents.KingdomDecisionAdded.AddNonSerializedListener(this, AggregateKeepFiefDecisions);
            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, KeepFief);
        }

        private void KeepFief()
        {
            if (_decisionsToProcess.IsEmpty())
            {
                return;
            }

            Kingdom playerKingdom;
            if ((playerKingdom = Clan.PlayerClan.Kingdom) != null)
            {
                RemoveExpiredDecisions(playerKingdom);
            }
            else
            {
                _decisionsToProcess.Clear();
            }

            SettlementClaimantDecision processedDecision = null;
            foreach (SettlementClaimantDecision decision in _decisionsToProcess)
            {
                processedDecision = decision;
                ShowKeepFiefInquiry(decision, playerKingdom);
                break;
            }
            if (processedDecision != null)
            {
                _decisionsToProcess.Remove(processedDecision);
            }
        }

        private void RemoveExpiredDecisions(Kingdom playerKingdom)
        {

            IEnumerable<SettlementClaimantDecision> expiredDecisions =
                _decisionsToProcess.Where(decisionToProcess => !playerKingdom.UnresolvedDecisions.Contains(decisionToProcess));

            if (expiredDecisions.Any())
            {
                _decisionsToProcess.RemoveAll(decision => expiredDecisions.Contains(decision));
            }
        }

        private void AggregateKeepFiefDecisions(KingdomDecision kingdomDecision, bool isPlayerInvolved)
        {
            SettlementClaimantDecision settlementClaimantDecision = kingdomDecision as SettlementClaimantDecision;
            if (isPlayerInvolved && (settlementClaimantDecision?.Settlement?.LastAttackerParty?.LeaderHero?.IsHumanPlayerCharacter ?? false))
            {
                _decisionsToProcess.Add(settlementClaimantDecision);
            }
        }

        private void ShowKeepFiefInquiry(SettlementClaimantDecision settlementClaimantDecision, Kingdom playerKingdom)
        {
            InformationManager.ShowInquiry(
                new InquiryData(
                    new TextObject("{=N06wk0dB}Settlement Captured").ToString(),
                    GetKeepFiefText(settlementClaimantDecision).ToString(),
                    true, true,
                    new TextObject("{=Y94H6XnK}Accept").ToString(),
                    new TextObject("{=cOgmdp9e}Decline").ToString(),
                    () =>
                        {
                            ChangeOwnerOfSettlementAction.ApplyByDefault(Hero.MainHero, settlementClaimantDecision.Settlement);
                            playerKingdom.RemoveDecision(settlementClaimantDecision);
                        },
                    null,
                    ""),
                true);
        }

        private TextObject GetKeepFiefText(SettlementClaimantDecision settlementClaimantDecision)
        {
            TextObject textObject = new TextObject("{=Zy0yjTha}As the capturer of {SETTLEMENT_NAME}, you have the right of first refusal. Would you like to claim this fief?");
            textObject.SetTextVariable("SETTLEMENT_NAME", settlementClaimantDecision.Settlement.Name);

            return textObject;
        }

        public override void SyncData(IDataStore dataStore) { }
    }
}
