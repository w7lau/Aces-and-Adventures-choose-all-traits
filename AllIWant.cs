using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace AllIWant
{
    [BepInPlugin("mods.baozhen.AcesAndAdventures.AllIWant", "选择特质时全选", "1.0.0.0")]
    public class AllIWant : BaseUnityPlugin
    {
        void Start()
        {
            Harmony.CreateAndPatchAll(typeof(AllIWant));
        }

        [HarmonyPatch(typeof(GameStepLevelUp), "_OnAbilityClick")]
        static bool Prefix(HeroDeckPile pile, Ability card, GameStepLevelUp __instance)
        {
            if (card.isTrait)
            {
            }
            else
            {
                return true;
            }
            var t = Traverse.Create(__instance);
            var state = t.Property("state");
            var heroDeck = t.Property("heroDeck");
            //Player player = state.GetValue<Player>();
            var player = state.Property("player");


            if (pile != HeroDeckPile.SelectionHand || __instance.finished)
            {
                return false;
            }
            if (card.view.hasOffset)
            {
                Debug.LogWarning("需要has offset");
                return false;
            }
            t.Method("_ClearGlows").GetValue();
            __instance.view.ClearMessage();
            player.GetValue<Player>().abilityDeck.Transfer(card, new Ability.Pile?(card.actPile), null);
            //t.Method("_ClearSelectionHand").GetValue();
            player.GetValue<Player>().AddTrait(card);
            //}
            if (heroDeck.GetValue<IdDeck<HeroDeckPile, Ability>>().GetCards(HeroDeckPile.SelectionHand).ToList<Ability>().Count < 1)
            {
                __instance.finished = true;
            }
            else if (heroDeck.GetValue<IdDeck<HeroDeckPile, Ability>>().GetCards(HeroDeckPile.SelectionHand).ToList<Ability>()[0].view.hasOffset)
            {
                __instance.finished = true;
            }
            return false;
        }
    }
}
