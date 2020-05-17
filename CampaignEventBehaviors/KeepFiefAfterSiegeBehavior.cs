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
        private List<Settlement> _settlementsToProcess;

        public KeepFiefAfterSiegeBehavior()
        {
            this._settlementsToProcess = new List<Settlement>();
        }

        public override void RegisterEvents()
        {
            CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, AggregateKeepFiefDecisions);
            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, KeepFief);
        }

        private void KeepFief()
        {
            if (_settlementsToProcess.IsEmpty())
            {
                return;
            }

            Kingdom playerKingdom = Clan.PlayerClan.MapFaction as Kingdom;
            if (playerKingdom == null)
            {
                _settlementsToProcess.Clear();
            }

            foreach (Settlement settlement in _settlementsToProcess.ToList())
            {
                _settlementsToProcess.Remove(settlement);
                if (settlement.OwnerClan.Kingdom != playerKingdom)
                {
                    continue;
                }
                ShowKeepFiefInquiry(settlement);
                break;
            }
        }

        private void AggregateKeepFiefDecisions(Settlement settlement, bool openToClaim, Hero newOwner, Hero oldOwner, Hero capturerHero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
        {
            if (detail == ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail.BySiege && openToClaim && capturerHero == Hero.MainHero)
            {
                _settlementsToProcess.Add(settlement);
                settlement.Town.IsOwnerUnassigned = false;
            }
        }

        private void ShowKeepFiefInquiry(Settlement settlement)
        {
            InformationManager.ShowInquiry(
                new InquiryData(
                    new TextObject("{=N06wk0dB}Settlement Captured").ToString(),
                    GetKeepFiefText(settlement).ToString(),
                    true, true,
                    new TextObject("{=Y94H6XnK}Accept").ToString(),
                    new TextObject("{=cOgmdp9e}Decline").ToString(),
                    () =>
                        {
                            ChangeOwnerOfSettlementAction.ApplyByDefault(Hero.MainHero, settlement);
                        },
                    () =>
                        {
                            settlement.Town.IsOwnerUnassigned = true;
                        },
                    ""),
                true);
        }

        private TextObject GetKeepFiefText(Settlement settlement)
        {
            TextObject textObject = new TextObject("{=Zy0yjTha}As the capturer of {SETTLEMENT_NAME}, you have the right of first refusal. Would you like to claim this fief?");
            textObject.SetTextVariable("SETTLEMENT_NAME", settlement.Name);

            return textObject;
        }

        public override void SyncData(IDataStore dataStore) { }
    }
}
