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

                    GUI.enabled = false;

                    EditorGUILayout.IntField("Effects:", card.effects.Count);

                    GUI.enabled = true;

                    //card.effects.Clear();

                    for (int j = 0; j < card.effects.Count; j++)
                    {
                        CardEffect cardEffect = card.effects[j];

                        GUI.enabled = false;

                        EditorGUILayout.TextField("Type:", cardEffect.type.ToString());

                        GUI.enabled = true;

                        switch (cardEffect.type)
                        {
                            case CardEffect.Type.NONE:
                                break;
                            case CardEffect.Type.DEFENSIVE:

                                CardEffect.Defensive defensive = (CardEffect.Defensive)Enum.Parse(typeof(CardEffect.Defensive), cardEffect.subType.ToString());
                                cardEffect.SetSubTypeFromChild((CardEffect.Defensive)EditorGUILayout.EnumPopup("SubType", defensive));

                                switch (cardEffect.subType)
                                {
                                    case CardEffect.SubType.NONE:
                                        break;
                                    case CardEffect.SubType.RESTORE_LIFE:
                                    case CardEffect.SubType.INCREASE_MAX_MANA:
                                        cardEffect.intParameter1 = EditorGUILayout.IntField("Parameter1: ", cardEffect.intParameter1);
                                        break;
                                }

                                break;
                            case CardEffect.Type.OFFENSIVE:

                                CardEffect.Offensive offensive = (CardEffect.Offensive)Enum.Parse(typeof(CardEffect.Offensive), cardEffect.subType.ToString());
                                cardEffect.SetSubTypeFromChild((CardEffect.Offensive)EditorGUILayout.EnumPopup("SubType", offensive));

                                switch (cardEffect.subType)
                                {
                                    case CardEffect.SubType.NONE:
                                        break;
                                    case CardEffect.SubType.DESTROY_CARD:
                                        cardEffect.boolParameter = EditorGUILayout.Toggle("Parameter3: ", cardEffect.boolParameter);
                                        break;
                                    case CardEffect.SubType.DEAL_DAMAGE:
                                        cardEffect.intParameter1 = EditorGUILayout.IntField("Parameter1: ", cardEffect.intParameter1);
                                        cardEffect.boolParameter = EditorGUILayout.Toggle("Parameter3: ", cardEffect.boolParameter);
                                        break;
                                    case CardEffect.SubType.DECREASE_MANA:
                                        cardEffect.intParameter1 = EditorGUILayout.IntField("Parameter1: ", cardEffect.intParameter1);
                                        break;
                                }

                                break;
                            case CardEffect.Type.BOOST:

                                CardEffect.Boost boost = (CardEffect.Boost)Enum.Parse(typeof(CardEffect.Boost), cardEffect.subType.ToString());
                                cardEffect.SetSubTypeFromChild((CardEffect.Boost)EditorGUILayout.EnumPopup("SubType", boost));

                                switch (cardEffect.subType)
                                {
                                    case CardEffect.SubType.NONE:
                                    case CardEffect.SubType.LIFELINK:
                                    case CardEffect.SubType.REBOUND:
                                    case CardEffect.SubType.TRAMPLE:
                                    case CardEffect.SubType.GUARD:
                                        break;
                                    case CardEffect.SubType.STAT_BOOST:
                                        cardEffect.intParameter1 = EditorGUILayout.IntField("Parameter1: ", cardEffect.intParameter1);
                                        cardEffect.intParameter2 = EditorGUILayout.IntField("Parameter2: ", cardEffect.intParameter2);
                                        cardEffect.boolParameter = EditorGUILayout.Toggle("Parameter3: ", cardEffect.boolParameter);
                                        break;
                                    case CardEffect.SubType.ADD_EFFECT:
                                        // Add effectParameter
                                        break;
                                }

                                break;
                            case CardEffect.Type.TACTICAL:

                                CardEffect.Tactical tactical = (CardEffect.Tactical)Enum.Parse(typeof(CardEffect.Tactical), cardEffect.subType.ToString());
                                cardEffect.SetSubTypeFromChild((CardEffect.Tactical)EditorGUILayout.EnumPopup("SubType", tactical));

                                switch (cardEffect.subType)
                                {
                                    case CardEffect.SubType.NONE:
                                    case CardEffect.SubType.SWAP_POSITION:
                                    case CardEffect.SubType.SWAP_CONTENDER:
                                    case CardEffect.SubType.RETURN_CARD:
                                        cardEffect.boolParameter = true;
                                        break;
                                    case CardEffect.SubType.CREATE_CARD:
                                        // Add gameobjectParameter
                                        break;
                                    case CardEffect.SubType.DRAW_CARD:
                                        cardEffect.intParameter1 = EditorGUILayout.IntField("Parameter1: ", cardEffect.intParameter1);
                                        break;
                                    case CardEffect.SubType.DISCARD_CARD:
                                        cardEffect.intParameter1 = EditorGUILayout.IntField("Parameter1: ", cardEffect.intParameter1);
                                        break;
                                }

                                break;
                        }

                        if (card.type != CardType.ACTION) cardEffect.applyTime = (CardEffect.ApplyTime)EditorGUILayout.EnumPopup("Apply time", cardEffect.applyTime);
                        else cardEffect.applyTime = CardEffect.ApplyTime.ENTER;
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
            currentCard.effects.Add(new CardEffect() { type = CardEffect.Type.DEFENSIVE });
        }

        [MenuItem("Assets/Card Game/Card Options/Create Effect/Offensive effect")]
        public static void CreateOffensiveEffect()
        {
            currentCard.effects.Add(new CardEffect() { type = CardEffect.Type.OFFENSIVE });
        }

        [MenuItem("Assets/Card Game/Card Options/Create Effect/Boost effect")]
        public static void CreateBoostEffect()
        {
            currentCard.effects.Add(new CardEffect() { type = CardEffect.Type.BOOST });
        }

        [MenuItem("Assets/Card Game/Card Options/Create Effect/Tactical effect")]
        public static void CreateTacticalEffect()
        {
            currentCard.effects.Add(new CardEffect() { type = CardEffect.Type.TACTICAL });
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
            cards.Insert(i, currentCard);
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