using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;

namespace DiplomacyFixes.DiplomaticAction.GenericConditions
{
    class HasAuthorityCondition : IDiplomacyCondition
    {
        public bool ApplyCondition(Kingdom kingdom, Kingdom otherKingdom, out TextObject textObject, bool forcePlayerCharacterCosts = false, bool bypassCosts = false)
        {
            textObject = null;
            var authority = false;

            authority |= kingdom != null && Clan.PlayerClan.Kingdom != kingdom;
            authority |= kingdom != null && kingdom.Leader.IsHumanPlayerCharacter;
            authority |= !forcePlayerCharacterCosts;
            authority |= Settings.Instance.PlayerDiplomacyControl;

            if (!authority)
            {
                textObject = new TextObject("{=lQAaLeSy}You don't have the authority to perform this action.");
            }

            return authority;
        }
    }
}
