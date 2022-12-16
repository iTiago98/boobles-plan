using CardGame.Cards.DataModel.Effects;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CardGame.Cards.DataModel
{
    [CustomEditor(typeof(CardsDataContainer))]
    public class CardsDataEditor : Editor
    {
        SerializedProperty cardsData;
        static CardsDataContainer cardsDataContainer;
        static List<CardsData> cards;
        static List<bool> showContent;

        static CardsData currentCard = null;

        private const string cardSeparator = "%";
        private const string attributeSeparator = "/";

        private void OnEnable()
        {
            cardsData = serializedObject.FindProperty("cards");
            cardsDataContainer = (CardsDataContainer)cardsData.serializedObject.targetObject;
            cards = cardsDataContainer.cards;
            if (cards == null) cards = new List<CardsData>();

            showContent = new List<bool>();
            for (int i = 0; i < cards?.Count; i++)
            {
                showContent.Add(true);
            }
        }

        private Sprite GetSpriteFromName(string rootPath, string name)
        {
            return Resources.Load<Sprite>(rootPath + "/" + name);
        }

        public override void OnInspectorGUI()
        {
            cardsDataContainer.resourcesPath = EditorGUILayout.TextField("Sprites path: ", cardsDataContainer.resourcesPath);

            if (GUILayout.Button("Deck options"))
            {
                OpenDeckOptions();
            }

            for (int i = 0; i < cards.Count; i++)
            {
                CardsData card = cards[i];
                showContent[i] = EditorGUILayout.BeginFoldoutHeaderGroup(showContent[i], card.name);

                if (showContent[i])
                {
                    card.name = EditorGUILayout.TextField("Name: ", card.name);

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.PrefixLabel("Sprite: ");
                    card.sprite = GetSpriteFromName(cardsDataContainer.resourcesPath, card.name);
                    card.sprite = (Sprite)EditorGUILayout.ObjectField(card.sprite, typeof(Sprite), allowSceneObjects: true);

                    EditorGUILayout.EndHorizontal();

                    card.cost = EditorGUILayout.IntField("Cost: ", card.cost);
                    card.type = (CardType)EditorGUILayout.EnumPopup("Type", card.type);

                    if (card.type == CardType.ARGUMENT)
                    {
                        card.strength = EditorGUILayout.IntField("Strength: ", card.strength);
                        card.defense = EditorGUILayout.IntField("Defense: ", card.defense);
                    }
                    else
                    {
                        card.strength = 0;
                        card.defense = 0;
                    }

                    if (card.effects.Count > 0)
                    {
                        GUI.enabled = false;

                        EditorGUILayout.IntField("Effects:", card.effects.Count);

                        GUI.enabled = true;
                    }

                    //card.effects.Clear();

                    for (int j = 0; j < card.effects.Count; j++)
                    {
                        CardEffect cardEffect = card.effects[j];

                        ShowEffect(card, cardEffect);
                    }

                    GUILayout.Space(10f);

                    if (GUILayout.Button("Options"))
                    {
                        OpenCardOptions(card);
                    }
                }

                EditorGUILayout.EndFoldoutHeaderGroup();
                EditorGUILayout.Space(10);
            }

            if (GUILayout.Button("Deck options"))
            {
                OpenDeckOptions();
            }
        }

        private void ShowEffect(CardsData card, CardEffect cardEffect)
        {
            switch (cardEffect.type)
            {
                case EffectType.NONE:
                    break;
                case EffectType.DEFENSIVE:

                    DefensiveType defensive = (DefensiveType)Enum.Parse(typeof(DefensiveType), cardEffect.subType.ToString());
                    cardEffect.SetSubTypeFromChild((DefensiveType)EditorGUILayout.EnumPopup("SubType", defensive));

                    switch (cardEffect.subType)
                    {
                        case SubType.NONE:
                            break;
                        case SubType.RESTORE_LIFE:
                        case SubType.INCREASE_MAX_MANA:
                            cardEffect.intParameter1 = EditorGUILayout.IntField("Mana amount: ", cardEffect.intParameter1);
                            break;
                    }

                    break;
                case EffectType.OFFENSIVE:

                    OffensiveType offensive = (OffensiveType)Enum.Parse(typeof(OffensiveType), cardEffect.subType.ToString());
                    cardEffect.SetSubTypeFromChild((OffensiveType)EditorGUILayout.EnumPopup("SubType", offensive));

                    switch (cardEffect.subType)
                    {
                        case SubType.NONE:
                        case SubType.DESTROY_CARD:
                            break;
                        case SubType.DEAL_DAMAGE:
                            cardEffect.intParameter1 = EditorGUILayout.IntField("Damage: ", cardEffect.intParameter1);
                            break;
                        case SubType.DECREASE_MANA:
                            cardEffect.intParameter1 = EditorGUILayout.IntField("Mana amount: ", cardEffect.intParameter1);
                            break;
                    }

                    break;
                case EffectType.BOOST:

                    BoostType boost = (BoostType)Enum.Parse(typeof(BoostType), cardEffect.subType.ToString());
                    cardEffect.SetSubTypeFromChild((BoostType)EditorGUILayout.EnumPopup("SubType", boost));

                    switch (cardEffect.subType)
                    {
                        case SubType.NONE:
                        case SubType.LIFELINK:
                        case SubType.REBOUND:
                        case SubType.TRAMPLE:
                        case SubType.GUARD:
                            break;
                        case SubType.STAT_BOOST:
                            cardEffect.intParameter1 = EditorGUILayout.IntField("Strength boost: ", cardEffect.intParameter1);
                            cardEffect.intParameter2 = EditorGUILayout.IntField("Defense boost: ", cardEffect.intParameter2);
                            break;
                        case SubType.ADD_EFFECT:
                            cardEffect.cardParameter_Effect = (SubType)EditorGUILayout.EnumPopup("Effect:", cardEffect.cardParameter_Effect);
                            break;
                    }

                    break;
                case EffectType.TACTICAL:

                    TacticalType tactical = (TacticalType)Enum.Parse(typeof(TacticalType), cardEffect.subType.ToString());
                    cardEffect.SetSubTypeFromChild((TacticalType)EditorGUILayout.EnumPopup("SubType", tactical));

                    switch (cardEffect.subType)
                    {
                        case SubType.NONE:
                        case SubType.SWAP_POSITION:
                        case SubType.SWAP_CONTENDER:
                        case SubType.DUPLICATE_CARD:
                        case SubType.RETURN_CARD:
                            break;
                        case SubType.CREATE_CARD:
                            if (cardEffect.cardParameter == null)
                            {
                                if (GUILayout.Button("Create")) cardEffect.cardParameter = new CardsDataSimple();
                            }
                            else
                            {
                                cardEffect.cardParameter.name = EditorGUILayout.TextField("Card name:", cardEffect.cardParameter.name);

                                EditorGUILayout.BeginHorizontal();

                                EditorGUILayout.PrefixLabel("Card sprite: ");
                                cardEffect.cardParameter.sprite = GetSpriteFromName(cardsDataContainer.resourcesPath, cardEffect.cardParameter.name);
                                cardEffect.cardParameter.sprite = (Sprite)EditorGUILayout.ObjectField(cardEffect.cardParameter.sprite, typeof(Sprite), allowSceneObjects: true);

                                EditorGUILayout.EndHorizontal();

                                cardEffect.cardParameter.strength = EditorGUILayout.IntField("Card strength:", cardEffect.cardParameter.strength);
                                cardEffect.cardParameter.defense = EditorGUILayout.IntField("Card defense:", cardEffect.cardParameter.defense);

                                cardEffect.cardParameter_Effect = (SubType)EditorGUILayout.EnumPopup("Card effect:", cardEffect.cardParameter_Effect);
                            }
                            break;
                        case SubType.DRAW_CARD:
                        case SubType.DISCARD_CARD:
                        case SubType.STEAL_CARD_FROM_HAND:
                        case SubType.STEAL_CARD_FROM_DECK:
                            cardEffect.intParameter1 = EditorGUILayout.IntField("Num cards: ", cardEffect.intParameter1);
                            break;
                    }

                    break;
                case EffectType.ALTERNATE_WIN_CONDITION:
                    AlternateWinConditionType alternateWinCondition = (AlternateWinConditionType)Enum.Parse(typeof(AlternateWinConditionType), cardEffect.subType.ToString());
                    cardEffect.SetSubTypeFromChild((AlternateWinConditionType)EditorGUILayout.EnumPopup("SubType", alternateWinCondition));
                    break;
            }

            if (cardEffect.type != EffectType.ALTERNATE_WIN_CONDITION)
            {
                if (cardEffect.applyTime != ApplyTime.COMBAT)
                {
                    switch (cardEffect.subType)
                    {
                        case SubType.NONE:
                        case SubType.RESTORE_LIFE:
                        case SubType.INCREASE_MAX_MANA:
                        case SubType.DECREASE_MANA:
                        case SubType.LIFELINK:
                        case SubType.REBOUND:
                        case SubType.TRAMPLE:
                        case SubType.COMPARTMENTALIZE:
                        case SubType.CREATE_CARD:
                        case SubType.DRAW_CARD:
                        case SubType.DISCARD_CARD:
                        case SubType.FREE_MANA:
                        case SubType.WHEEL:
                        case SubType.GUARD:
                        case SubType.SPONGE:
                        case SubType.SKIP_COMBAT:
                        case SubType.STEAL_CARD_FROM_HAND:
                        case SubType.MIRROR:
                        case SubType.STEAL_MANA:
                        case SubType.STEAL_REWARD:
                        case SubType.STEAL_CARD_FROM_DECK:
                            cardEffect.targetType = Target.NONE;
                            break;
                        default:
                            cardEffect.targetType = (Target)EditorGUILayout.EnumPopup("Target", cardEffect.targetType);
                            break;
                    }
                }

                if (card.type == CardType.ACTION)
                {
                    cardEffect.applyTime = ApplyTime.ENTER;
                }
                else
                {
                    // TODO: This switch only works for tutorial deck
                    switch (cardEffect.subType)
                    {
                        case SubType.NONE:
                            break;
                        case SubType.DRAW_CARD:
                        case SubType.DISCARD_CARD:
                        case SubType.INCREASE_MAX_MANA:
                        case SubType.FREE_MANA:
                        case SubType.GUARD:
                            cardEffect.applyTime = ApplyTime.ENTER;
                            break;
                        case SubType.LIFELINK:
                        case SubType.REBOUND:
                        case SubType.TRAMPLE:
                        case SubType.COMPARTMENTALIZE:
                        case SubType.SPONGE:
                            cardEffect.applyTime = ApplyTime.COMBAT;
                            break;
                        default:
                            cardEffect.applyTime = (ApplyTime)EditorGUILayout.EnumPopup("Apply time", cardEffect.applyTime);
                            break;
                    }
                }
            }
        }

        #region Card Options

        private void OpenCardOptions(CardsData card)
        {
            Event evt = Event.current;
            Vector2 mousePos = evt.mousePosition;
            EditorUtility.DisplayPopupMenu(new Rect(mousePos.x, mousePos.y, 0, 0), "Assets/Card Game/Card Options/", null);
            currentCard = card;
        }

        [MenuItem("Assets/Card Game/Card Options/Create Effect/Defensive effect")]
        public static void CreateDefensiveEffect()
        {
            currentCard.effects.Add(new CardEffect() { type = EffectType.DEFENSIVE });
        }

        [MenuItem("Assets/Card Game/Card Options/Create Effect/Offensive effect")]
        public static void CreateOffensiveEffect()
        {
            currentCard.effects.Add(new CardEffect() { type = EffectType.OFFENSIVE });
        }

        [MenuItem("Assets/Card Game/Card Options/Create Effect/Boost effect")]
        public static void CreateBoostEffect()
        {
            currentCard.effects.Add(new CardEffect() { type = EffectType.BOOST });
        }

        [MenuItem("Assets/Card Game/Card Options/Create Effect/Tactical effect")]
        public static void CreateTacticalEffect()
        {
            currentCard.effects.Add(new CardEffect() { type = EffectType.TACTICAL });
        }

        [MenuItem("Assets/Card Game/Card Options/Create Effect/Alternate Win Condition")]
        public static void CreateAlternateWinCondition()
        {
            currentCard.effects.Add(new CardEffect() { type = EffectType.ALTERNATE_WIN_CONDITION });
        }

        [MenuItem("Assets/Card Game/Card Options/Remove last effect")]
        public static void RemoveLastEffect()
        {
            if (currentCard.effects.Count > 0) currentCard.effects.RemoveAt(currentCard.effects.Count - 1);
        }

        [MenuItem("Assets/Card Game/Card Options/Copy card")]
        public static void CopyCard()
        {
            CleanClipBoard();
            CopyCard(currentCard);
        }

        [MenuItem("Assets/Card Game/Card Options/Duplicate card")]
        public static void DuplicateCard()
        {
            int i = cards.IndexOf(currentCard);

            cards.Insert(i, new CardsData(currentCard));

            showContent.Insert(i, true);
        }

        [MenuItem("Assets/Card Game/Card Options/Remove card")]
        public static void RemoveCard()
        {
            showContent.RemoveAt(cards.IndexOf(currentCard));
            cards.Remove(currentCard);
        }

        #endregion

        #region Deck Options

        private void OpenDeckOptions()
        {
            Event evt = Event.current;
            Vector2 mousePos = evt.mousePosition;
            EditorUtility.DisplayPopupMenu(new Rect(mousePos.x, mousePos.y, 0, 0), "Assets/Card Game/Deck Options/", null);
        }

        [MenuItem("Assets/Card Game/Deck Options/Add card")]
        public static void AddCard()
        {
            cards.Add(new CardsData());
            showContent.Add(true);
        }

        [MenuItem("Assets/Card Game/Deck Options/Paste card(s)")]
        public static void PasteCards()
        {
            string content = GUIUtility.systemCopyBuffer;

            while (content.Contains(cardSeparator))
            {
                string data = GetNextData(ref content, cardSeparator);
                PasteCard(data);
            }
        }

        [MenuItem("Assets/Card Game/Deck Options/Expand all")]
        public static void ExpandAll()
        {
            for (int i = 0; i < cards.Count; i++)
            {
                showContent[i] = true;
            }
        }

        [MenuItem("Assets/Card Game/Deck Options/Collapse all")]
        public static void CollapseAll()
        {
            for (int i = 0; i < cards.Count; i++)
            {
                showContent[i] = false;
            }
        }

        [MenuItem("Assets/Card Game/Deck Options/Copy all cards")]
        public static void CopyAllCards()
        {
            CleanClipBoard();
            foreach (CardsData card in cards)
            {
                CopyCard(card);
            }
        }

        [MenuItem("Assets/Card Game/Deck Options/Remove all cards")]
        public static void RemoveAllCards()
        {
            cards.Clear();
            showContent.Clear();
        }

        [MenuItem("Assets/Card Game/Deck Options/Save deck")]
        public static void SaveScriptableObject()
        {
            EditorUtility.SetDirty(cardsDataContainer);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        #endregion

        #region Utils
        private static void CleanClipBoard()
        {
            GUIUtility.systemCopyBuffer = "";
        }

        private static void CopyCard(CardsData card)
        {
            GUIUtility.systemCopyBuffer += card.name
                + attributeSeparator + card.cost
                + attributeSeparator + card.type
                + attributeSeparator + card.strength
                + attributeSeparator + card.defense
                + cardSeparator;
        }

        private static void PasteCard(string data)
        {
            CardsData card = new CardsData();

            card.name = GetNextData(ref data, attributeSeparator);
            card.cost = int.Parse(GetNextData(ref data, attributeSeparator));
            card.type = GetCardType(GetNextData(ref data, attributeSeparator));
            card.strength = int.Parse(GetNextData(ref data, attributeSeparator));
            card.defense = int.Parse(GetNextData(ref data, attributeSeparator));

            cards.Add(card);
            showContent.Add(true);
        }

        private static string GetNextData(ref string data, string separator)
        {
            if (data.Contains(separator))
            {
                int index = data.IndexOf(separator);

                string s = data.Substring(0, index);
                data = data.Substring(index + 1, data.Length - index - 1);
                return s;
            }
            else
            {
                return data;
            }
        }

        private static CardType GetCardType(string type)
        {
            switch (type)
            {
                case "ARGUMENT": return CardType.ARGUMENT;
                case "ACTION": return CardType.ACTION;
                case "FIELD": return CardType.FIELD;
            }
            return CardType.ARGUMENT;
        }

        #endregion
    }

}