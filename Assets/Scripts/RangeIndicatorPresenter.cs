using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RangeIndicatorPresenter : MonoBehaviour
{
    [SerializeField] private DecalProjector globalRangeDecal;
    [SerializeField] private float height = 2f; //altura respecto a la posicion Y de la unidad

    private void Start()
    {
        //RANGO DE MOVIMIENTO
        SelectionManager.Instance.OnUnitMovementStarted += ShowMovementRange;
        SelectionManager.Instance.OnUnitMovementEnded += (u)=> HideRange();
        
        //RANGO DE CARTAS
        SelectionManager.Instance.OnCardTargetingStarted += ShowCardRange;
        SelectionManager.Instance.OnCardTargetingEnded += (cd)=> HideRange();
        HideRange();
    }

    void ShowMovementRange(BaseUnit unit)
    {
        ShowRangeForUnit(unit, unit.Data.BaseMovementRange);
    }

    void ShowCardRange(CardData card)
    {
        if (card.hasInfiniteRange) return;

        ShowRangeForUnit(SelectionManager.Instance.SelectedAlly, card.castRange);
    }

    public void ShowRangeForUnit(BaseUnit unit, float rangeInMeters)
    {
        if (unit == null || globalRangeDecal == null) return;

        // 1. Mover el Decal a la posición de la unidad seleccionada
        globalRangeDecal.transform.position = unit.transform.position + Vector3.up * (height * 0.5f);

        // 2. Opcional: Hacer que el Decal sea hijo de la unidad si quieres que se mueva con ella
        //globalRangeDecal.transform.SetParent(unit.transform);

        // 3. Ajustar el tamaño (Diámetro = Radio * 2)
        float diameter = rangeInMeters * 2f;
        globalRangeDecal.size = new Vector3(diameter, diameter, 10); //sumar un poco al projection range del decal para que llegue desde la nueva altura

        // 4. Activar
        globalRangeDecal.enabled = true;
    }

    public void HideRange()
    {
        if (globalRangeDecal == null) return;

        globalRangeDecal.enabled = false;
/*        globalRangeDecal.transform.SetParent(null);*/ // Desdesparentar
    }
}
