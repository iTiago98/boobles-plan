using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DataModel
{
    [CustomEditor(typeof(CardsDataContainer))]
    public class CardsDataEditor : Editor
    {
        SerializedProperty cardsData;
        CardsDataContainer cardsDataContainer;
        List<CardsData> cards;
        List<bool> showContent;

        private void OnEnable()
        {
            cardsData = serializedObject.FindProperty("cards");
            cardsDataContainer = (CardsDataContainer)cardsData.serializedObject.targetObject;
            cards = cardsDataContainer.cards;
            showContent = new List<bool>();
            for (int i = 0; i < cards.Count; i++)
            {
                showContent.Add(true);
                //cards[i].sprite = GetSpriteFromName(cardsDataContainer.resourcesPath, cards[i].name);
            }
        }

        private Sprite GetSpriteFromName(string rootPath, string name)
        {
            return Resources.Load<Sprite>(rootPath + "/" + name);
        }

        public override void OnInspectorGUI()
        {
            int cardToRemove = -1;

            cardsDataContainer.resourcesPath = EditorGUILayout.TextField("Sprites path: ", cardsDataContainer.resourcesPath);

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

                    GUI.enabled = false;

                    EditorGUILayout.IntField("Effects:", card.effects.Count);

                    GUI.enabled = true;

                    for (int j = 0; j < card.effects.Count; j++)
                    {
                        CardEffect cardEffect = card.effects[j];

                        cardEffect.effect = (CardEffect.Effect)EditorGUILayout.EnumPopup(" ", cardEffect.effect);

                        switch (cardEffect.effect)
                        {
                            case CardEffect.Effect.STAT_BOOST:
                                cardEffect.strengthBoost = EditorGUILayout.IntField("Strength boost: ", cardEffect.strengthBoost);
                                cardEffect.defenseBoost = EditorGUILayout.IntField("Defense boost: ", cardEffect.defenseBoost);
                                break;
                            case CardEffect.Effect.DRAW_CARDS:
                            case CardEffect.Effect.DISCARD_CARDS:
                                cardEffect.numberCards = EditorGUILayout.IntField("Number of cards: ", cardEffect.numberCards);
                                break;
                            case CardEffect.Effect.vinculo_vital:
                                break;
                        }
                    }

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(160f);
                    if (GUILayout.Button("Add effect"))
                    {
                        card.effects.Add(new CardEffect { effect = CardEffect.Effect.NONE });
                    }
                    if (GUILayout.Button("Remove last"))
                    {
                        card.effects.RemoveAt(card.effects.Count - 1);
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(50f);
                    if (GUILayout.Button("Duplicate card"))
                    {
                        cards.Insert(i, card);
                        showContent.Insert(i, true);
                    }
                    if (GUILayout.Button("Remove card"))
                    {
                        cardToRemove = cards.IndexOf(card);
                    }
                    GUILayout.Space(50f);
                    GUILayout.EndHorizontal();
                }

                EditorGUILayout.EndFoldoutHeaderGroup();
                EditorGUILayout.Space(10);
            }

            if (cardToRemove != -1)
            {
                cards.RemoveAt(cardToRemove);
                showContent.RemoveAt(cardToRemove);
            }

            if (GUILayout.Button("Add card"))
            {
                cards.Add(new CardsData());
                showContent.Add(true);
            }

            if (GUILayout.Button("Expand all"))
            {
                for (int i = 0; i < cards.Count; i++)
                {
                    showContent[i] = true;
                }
            }

            if (GUILayout.Button("Collapse all"))
            {
                for (int i = 0; i < cards.Count; i++)
                {
                    showContent[i] = false;
                }
            }
        }
    }

}