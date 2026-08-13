using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance { get; private set; }
    public ISelectable CurrentSelected { get; private set; }
    public BaseUnit SelectedAlly { get; private set; }
    public BaseUnit InspectedEnemy { get; private set; }

    public event Action<BaseUnit> OnAllySelected;
    public event Action<BaseUnit> OnEnemyInspected;
    public event Action<ISelectable> OnInteractableSelected;
    public event Action OnSelectionCleared;

    [SerializeField] private LayerMask selectableLayer;

    //PLAYING CARDS

    // Estado de carta pendiente
    private CardData pendingCard;
    public bool IsTargetingCard => pendingCard != null;
    public event Action<CardData> OnCardTargetingStarted;
    public event Action<CardData> OnCardTargetingEnded;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public void SelectEntity(ISelectable target)
    {
        if (target == null)
        {
            DeselectAll();
            return;
        }

        // 1. Notificamos al objeto actual que se desselecciona y al nuevo que se selecciona
        CurrentSelected?.OnDeselect();
        CurrentSelected = target;
        CurrentSelected.OnSelect();

        // 2. Comprobamos qué tipo de entidad es a través de sus componentes
        if (target.gameObject.TryGetComponent<BaseUnit>(out var unit))
        {
            switch (unit.Faction)
            {
                case Faction.Player:
                    SelectedAlly = unit;
                    InspectedEnemy = null;
                    OnAllySelected?.Invoke(SelectedAlly);
                    break;

                case Faction.Enemy:
                    InspectedEnemy = unit;
                    OnEnemyInspected?.Invoke(InspectedEnemy);
                    break;
            }
        }
        else
        {
            // Si es un objeto del mapa (cofre, palanca, barril)
            SelectedAlly = null;
            InspectedEnemy = null;
            OnInteractableSelected?.Invoke(target);
        }
    }

    private void Update()
    {
        //Evitar que haga nada al hacer click sobre la UI (sobre todo para que no de error al elegir las cartas)
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            SelectTarget();
        }
        // Cancelar apuntado con clic derecho
        if (Input.GetKeyDown(KeyCode.Mouse1) && IsTargetingCard)
        {
            CancelCardTargeting();
        }
    }

    public void SelectTarget()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, selectableLayer))
        {
            GameObject hitObject = hit.collider.gameObject;

            // MODO A: Estamos eligiendo objetivo para la carta
            if (IsTargetingCard)
            {
                ExecuteCardOnTarget(hitObject);
                return;
            }

            // MODO B: Selección normal de unidades / objetos
            if (hitObject.TryGetComponent<ISelectable>(out var unit))
            {
                SelectEntity(unit);
                return;
            }
        }

        if (IsTargetingCard) CancelCardTargeting();
        else SelectEntity(null);
    }

    private void ExecuteCardOnTarget(GameObject target)
    {
        //comprobar si el objetivo es valido
        if (pendingCard.IsValidTarget(SelectedAlly, target) == false)
        {
            Debug.Log("Invalid target");
            return;
        }

        // Creamos el contexto enviando Caster y Target juntos
        EffectContext context = new EffectContext
        {
            Caster = SelectedAlly.gameObject,
            Target = target
        };

        pendingCard.PlayCard(context);
        pendingCard = null; // Limpiamos el estado tras ejecutar
        DeselectAll();
    }

    public void DeselectAll()
    {
        Debug.Log("Deselecting everything");
        CurrentSelected?.OnDeselect();
        CurrentSelected = null;
        SelectedAlly = null;
        InspectedEnemy = null;
        OnSelectionCleared?.Invoke();
        OnCardTargetingEnded?.Invoke(pendingCard);
        pendingCard = null;
    }

    //CARD TARGETING

    // AVISO DESDE LA CARTA: La UI llama a esto al pulsar una carta
    public void StartCardTargeting(CardData card)
    {
        if (SelectedAlly == null)
        {
            Debug.Log("Unable to play card");
            return; // No se puede jugar sin aliada seleccionada
        }

        pendingCard = card;
        OnCardTargetingStarted?.Invoke(card);
    }

    public void CancelCardTargeting()
    {
        OnCardTargetingEnded?.Invoke(pendingCard);
        pendingCard = null;
    }
}