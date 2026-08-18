using System.Collections.Generic;
using UnityEngine;

public class UnitCardController : MonoBehaviour
{
    [SerializeField] private UnitData unitData;

    // Listas dinámicas en tiempo de ejecución (Read-Only para el exterior)
    public List<CardData> DrawPile { get; private set; } = new List<CardData>();
    public List<CardData> Hand { get; private set; } = new List<CardData>();
    public List<CardData> DiscardPile { get; private set; } = new List<CardData>();

    private void Awake()
    {
        InitializeDeck();
    }

    private void InitializeDeck()
    {
        if (unitData == null || unitData.startingDeck == null) return;

        DrawPile.Clear();

        // Convertimos las entradas de cantidad a instancias individuales en el mazo
        foreach (CardEntry entry in unitData.startingDeck)
        {
            if (entry.card == null) continue;

            for (int i = 0; i < entry.amount; i++)
            {
                DrawPile.Add(entry.card);
            }
        }

        ShuffleDrawPile();

        //robar la mano inicial
        DrawCards(4);

    }

    public void DrawCards(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if (DrawPile.Count == 0)
            {
                // Si no quedan cartas para robar, reciclamos el mazo de descartes
                RefillDrawPileFromDiscard();
            }

            if (DrawPile.Count > 0)
            {
                CardData drawnCard = DrawPile[0];
                DrawPile.RemoveAt(0);
                Hand.Add(drawnCard);
            }
        }
    }

    public void DiscardCard(CardData card)
    {
        if (Hand.Contains(card))
        {
            Hand.Remove(card);
            DiscardPile.Add(card);
        }
    }

    private void RefillDrawPileFromDiscard()
    {
        DrawPile.AddRange(DiscardPile);
        DiscardPile.Clear();
        ShuffleDrawPile();
    }

    private void ShuffleDrawPile()
    {
        // Lógica sencilla para barajar la lista
        for (int i = 0; i < DrawPile.Count; i++)
        {
            CardData temp = DrawPile[i];
            int randomIndex = Random.Range(i, DrawPile.Count);
            DrawPile[i] = DrawPile[randomIndex];
            DrawPile[randomIndex] = temp;
        }
    }
}