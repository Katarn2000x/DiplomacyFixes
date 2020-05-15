using DiplomacyFixes.Messengers;
using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.KingdomDiplomacy;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace DiplomacyFixes.ViewModel
{
    public class KingdomTruceItemVMExtensionVM : KingdomTruceItemVM
    {
        private Action _refreshParent;

        public KingdomTruceItemVMExtensionVM(IFaction faction1, IFaction faction2, Action<KingdomDiplomacyItemVM> onSelection, Action<KingdomTruceItemVM> onAction, Action refreshParent) : base(faction1, faction2, onSelection, onAction)
        {
            this.SendMessengerActionName = new TextObject("{=cXfcwzPp}Send Messenger").ToString();
            this.InfluenceCost = (int)DiplomacyCostCalculator.DetermineInfluenceCostForDeclaringWar(Faction1 as Kingdom);
            this.ActionName = GameTexts.FindText("str_kingdom_declate_war_action", null).ToString();
            this._refreshParent = refreshParent;
            UpdateDiplomacyProperties();
        }

        protected override void UpdateDiplomacyProperties()
        {
            base.UpdateDiplomacyProperties();
            UpdateActionAvailability();
        }
        private void ExecuteExecutiveAction()
        {
            List<TextObject> warExceptions = WarAndPeaceConditions.CanDeclareWarExceptions(this);
            if (warExceptions.IsEmpty())
            {
                float influenceCost = DiplomacyCostCalculator.DetermineInfluenceCostForDeclaringWar(Faction1 as Kingdom);
                DiplomacyCostManager.deductInfluenceFromPlayerClan(influenceCost);
                DeclareWarAction.Apply(Faction1, Faction2);
                this._refreshParent();
            }
            else
            {
                MessageHelper.SendFailedActionMessage(new TextObject("{=v0cDMIcl}Cannot declare war on this kingdom. ").ToString(), warExceptions);
            }
        }

        private void UpdateActionAvailability()
        {
            this.IsMessengerAvailable = MessengerManager.CanSendMessengerWithInfluenceCost(Faction2Leader.Hero, this.SendMessengerInfluenceCost);
            this.IsOptionAvailable = WarAndPeaceConditions.CanDeclareWarExceptions(this).IsEmpty();
        }


        protected override void OnSelect()
        {
            base.OnSelect();
            UpdateActionAvailability();
        }

        protected void SendMessenger()
        {
            Events.Instance.OnMessengerSent(Faction2Leader.Hero);
            this.UpdateDiplomacyProperties();
        }

        [DataSourceProperty]
        public string ActionName { get; }

        [DataSourceProperty]
        public int SendMessengerInfluenceCost { get; } = (int)DiplomacyCostCalculator.DetermineInfluenceCostForSendingMessenger();

        [DataSourceProperty]
        public bool IsMessengerAvailable
        {
            get
            {
                return this._isMessengerAvailable;
            }
            set
            {
                if (value != this._isMessengerAvailable)
                {
                    this._isMessengerAvailable = value;
                    base.OnPropertyChanged("IsMessengerAvailable");
                }
            }
        }

        [DataSourceProperty]
        public int InfluenceCost { get; }

        [DataSourceProperty]
        public bool IsOptionAvailable
        {
            get
            {
                return this._isOptionAvailable;
            }
            set
            {
                if (value != this._isOptionAvailable)
                {
                    this._isOptionAvailable = value;
                    base.OnPropertyChanged("IsOptionAvailable");
                }
            }
        }

        [DataSourceProperty]
        public string SendMessengerActionName { get; private set; }

        [DataSourceProperty]
        public bool IsGoldCostVisible { get; } = false;

        private bool _isOptionAvailable;
        private bool _isMessengerAvailable;
    }
}
