using StoryMode.StoryModePhases;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;

namespace DiplomacyFixes.DiplomaticAction.WarPeace.Conditions
{
    class SatisfiesQuestConditionsForPeaceCondition : IDiplomacyCondition
    {
        private const string ACTIVE_QUEST = "{=XQFxDr11}There is an active quest preventing it!";
        public bool ApplyCondition(Kingdom kingdom, Kingdom otherKingdom, out TextObject textObject, bool forcePlayerCharacterCosts = false, bool bypassCosts = false)
        {
            textObject = null;

            // check if we actually are in storymode (or sandbox)
            if (StoryMode.StoryMode.Current == null || StoryMode.StoryMode.Current.MainStoryLine == null) 
                return true;

            ThirdPhase thirdPhase = StoryMode.StoryMode.Current.MainStoryLine.ThirdPhase;
            bool isValidQuestState = thirdPhase == null || !thirdPhase.OppositionKingdoms.Contains(otherKingdom);
            if (!isValidQuestState)
            {
                textObject = new TextObject(ACTIVE_QUEST);
            }
            return isValidQuestState;
        }
    }
}
