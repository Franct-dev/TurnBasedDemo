using System.Collections.Generic;
using UnityEngine;

public class HandUIManager : MonoBehaviour
{
    [SerializeField] private CardUI cardPrefab;
    [SerializeField] private Transform handLayoutContainer; // Gameobject con HorizontalLayoutGroup
    [SerializeField] private int maxHandCapacity = 10; // Capacidad máxima del pool

    private List<CardUI> cardPool = new List<CardUI>();

    private void Awake()
    {
        InitializePool();
    }

    private void Start()
    {
        SelectionManager.Instance.OnAllySelected += UpdateHandUI;
        SelectionManager.Instance.OnSelectionCleared += ClearHandUI;
    }

    private void OnDisable()
    {
        if (SelectionManager.Instance != null)
        {
            SelectionManager.Instance.OnAllySelected -= UpdateHandUI;
            SelectionManager.Instance.OnSelectionCleared -= ClearHandUI;
        }
    }

    private void InitializePool()
    {
        // Instanciamos el número máximo de cartas que el jugador podría llegar a tener a la vez
        for (int i = 0; i < maxHandCapacity; i++)
        {
            CardUI cardInstance = Instantiate(cardPrefab, handLayoutContainer);
            cardInstance.Unbind(); // Nace desactivada
            cardPool.Add(cardInstance);
        }
    }

    private void UpdateHandUI(BaseUnit ally)
    {
        ClearHandUI();

        if (ally.TryGetComponent<UnitCardController>(out var cardController))
        {
            List<CardData> hand = cardController.Hand;

            // Recorremos las cartas de la unidad y activamos las del pool necesarias
            for (int i = 0; i < hand.Count; i++)
            {
                if (i < cardPool.Count)
                {
                    cardPool[i].Bind(hand[i]);
                }
            }
        }
    }

    private void ClearHandUI()
    {
        // En lugar de destruir, simplemente ocultamos todas las cartas
        foreach (CardUI card in cardPool)
        {
            card.Unbind();
        }
    }
}