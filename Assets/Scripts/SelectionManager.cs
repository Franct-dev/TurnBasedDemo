using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public enum TargetingMode { Normal, Card, Movement }

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

    // ESTADO DE INTERACCIÓN
    public TargetingMode CurrentMode { get; private set; } = TargetingMode.Normal;

    [Header("Raycast Layers")]
    [SerializeField] private LayerMask selectableLayer;
    [SerializeField] private LayerMask groundLayer; // Capa para detectar clics en el mapa/suelo

    //PLAYING CARDS

    // Estado de carta pendiente
    private CardData pendingCard;
    public bool IsTargetingCard => CurrentMode == TargetingMode.Card;
    public bool IsMovingUnit => CurrentMode == TargetingMode.Movement;
    public event Action<CardData> OnCardTargetingStarted;
    public event Action<CardData> OnCardTargetingEnded;
    //callbacks para cuando se empieza y termina/cancela el mover una unidad
    public event Action<BaseUnit> OnUnitMovementStarted, OnUnitMovementEnded;

    private MovementPathService mps; //para poder mover a las unidades seleccionadas

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        //inicializar el servicio de movimiento
        mps = new MovementPathService();
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
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            CancelCurrentTargeting();
        }
    }

    public void SelectTarget()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Si estamos en modo movimiento raycasteamos contra el suelo, si no, contra entidades
        LayerMask activeMask = (CurrentMode == TargetingMode.Movement) ? groundLayer : selectableLayer;

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, activeMask))
        {
            GameObject hitObject = hit.collider.gameObject;

            // MODO CARTA
            if (CurrentMode == TargetingMode.Card)
            {
                ExecuteCardOnTarget(hitObject);
                return;
            }

            // MODO MOVIMIENTO
            if (CurrentMode == TargetingMode.Movement)
            {
                ExecuteMovement(hit.point);
                return;
            }

            // MODO NORMAL (Selección)
            if (hitObject.TryGetComponent<ISelectable>(out var unit))
            {
                SelectEntity(unit);
                return;
            }
        }

        if (CurrentMode != TargetingMode.Normal) CancelCurrentTargeting();
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

        // 4. AVISAMOS AL ALIADO: Descarta la carta jugada de su mano a su pila de descartes
        if (SelectedAlly.TryGetComponent<UnitCardController>(out var cardController))
        {
            cardController.DiscardCard(pendingCard);
        }

        pendingCard = null; // Limpiamos el estado tras ejecutar
        CurrentMode = TargetingMode.Normal;

        //DeselectAll();
        DeselectAfterPlayingCard();

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

    void DeselectAfterPlayingCard()
    {
        OnAllySelected?.Invoke(SelectedAlly);
        OnCardTargetingEnded?.Invoke(pendingCard);
        pendingCard = null;
    }

    private void ExecuteMovement(Vector3 destination)
    {
        if (SelectedAlly == null)
        {
            return;
        }

        if (mps.TryGetValidPath(SelectedAlly.transform.position, destination, SelectedAlly.MovementRange, out NavMeshPath path, out float totalDistance))
        {
            mps.MoveUnitAlongPath(SelectedAlly, path.corners, 6, () => Debug.Log("Unit movement ended"));
        }

        CurrentMode = TargetingMode.Normal;

        OnUnitMovementEnded?.Invoke(SelectedAlly);
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

        if(IsMovingUnit)
        {
            CancelMoveTargeting();
        }

        pendingCard = card;
        CurrentMode = TargetingMode.Card;
        OnCardTargetingStarted?.Invoke(card);
    }

    public void CancelCardTargeting()
    {
        OnCardTargetingEnded?.Invoke(pendingCard);
        pendingCard = null;
        CurrentMode = TargetingMode.Normal;
    }

    public void StartMoveTargeting()
    {
        if (SelectedAlly == null) return;

        if (IsTargetingCard) CancelCardTargeting();
        CurrentMode = TargetingMode.Movement;

        OnUnitMovementStarted?.Invoke(SelectedAlly);
    }

    void CancelMoveTargeting()
    {
        OnUnitMovementEnded?.Invoke(SelectedAlly);
    }

    public void CancelCurrentTargeting()
    {
        if (IsTargetingCard) CancelCardTargeting();
        if (IsMovingUnit) CancelMoveTargeting();
        CurrentMode = TargetingMode.Normal;
    }
}