﻿using IDontCare.Constants;
using IDontCare.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.CampaignSystem;

namespace IDontCare.Filtering
{
    internal static class FilteringMethods
    {
        internal static bool ShouldPlayerCare(FilterMode filterMode, IEnumerable<IFaction> factionsInvolved)
            => factionsInvolved.Any(faction => ShouldPlayerCare(filterMode, faction));

        internal static bool ShouldPlayerCare(FilterMode filterMode, IFaction factionInvolved)
        {
            if (factionInvolved is null)
                return false;

            bool shouldPlayerCare;

            switch (filterMode)
            {
                case FilterMode.Default:
                    shouldPlayerCare = factionInvolved.IsAtWarOrAlliedWithPlayer();
                    break;
                case FilterMode.FilterAll:
                    shouldPlayerCare = false;
                    break;
                case FilterMode.FilterNothing:
                    shouldPlayerCare = true;
                    break;
                case FilterMode.OnlyMe:
                    shouldPlayerCare = false;
                    break;
                case FilterMode.OnlyMyClan:
                    shouldPlayerCare = factionInvolved.Id == Hero.MainHero.Clan?.Id;
                    break;
                case FilterMode.OnlyMyKingdom:
                    shouldPlayerCare = Hero.MainHero.Clan?.Kingdom is null
                        ? factionInvolved.Id == Hero.MainHero.Clan?.Id
                        : factionInvolved.Id == Hero.MainHero.Clan?.Kingdom?.Id;
                    break;
                default:
                    return false;
            }

            return shouldPlayerCare;
        }

        internal static bool ShouldPlayerCare(FilterMode filterMode, IEnumerable<Hero> heroesInvolved)
            => heroesInvolved.Any(hero => ShouldPlayerCare(filterMode, hero));

        internal static bool ShouldPlayerCare(FilterMode filterMode, Hero heroInvolved)
        {
            if (heroInvolved is null)
                return false;

            bool shouldPlayerCare;

            switch (filterMode)
            {
                case FilterMode.Default:
                    shouldPlayerCare = heroInvolved.MapFaction?.IsAtWarOrAlliedWithPlayer() ?? false;
                    break;
                case FilterMode.FilterAll:
                    shouldPlayerCare = false;
                    break;
                case FilterMode.FilterNothing:
                    shouldPlayerCare = true;
                    break;
                case FilterMode.OnlyMe:
                    shouldPlayerCare = heroInvolved.Id == Hero.MainHero.Id;
                    break;
                case FilterMode.OnlyMyClan:
                    shouldPlayerCare = heroInvolved.Clan?.Id == Hero.MainHero.Clan?.Id;
                    break;
                case FilterMode.OnlyMyKingdom:
                    shouldPlayerCare = Hero.MainHero.Clan?.Kingdom is null
                        ? heroInvolved.Clan?.Id == Hero.MainHero.Clan?.Id
                        : heroInvolved.Clan?.Kingdom?.Id == Hero.MainHero.Clan?.Kingdom?.Id;
                    break;
                default:
                    return false;
            }

            //Markeed Hero.
            if (!shouldPlayerCare && Campaign.Current.EncyclopediaManager.ViewDataTracker.IsEncyclopediaBookmarked(heroInvolved))
                return true;

            return shouldPlayerCare;
        }

        internal static object GetLogEntryPrivateField(object instance, string fieldName)
        {
            var type = instance?.GetType();
            if (type is null)
            {
                return null;
            }

            var bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            var field = type.GetField(fieldName, bindFlags);
            return field?.GetValue(instance);
        }
    }
}
