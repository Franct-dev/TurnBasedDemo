using UnityEngine;

public class SelectionUIFeedbackPresenter : MonoBehaviour
{
    [SerializeField] private GameObject cardTargetFdbck; //Objecto que se activa cuando esta en modo seleccionar objetivo
    [SerializeField] private GameObject moveUnitFdbck; //Objecto que se activa cuando esta en modo seleccionar objetivo

    void Start()
    {
        //Targeteo de carta
        SelectionManager.Instance.OnCardTargetingStarted += (c) => cardTargetFdbck.SetActive(true);
        SelectionManager.Instance.OnCardTargetingEnded += (c) => cardTargetFdbck.SetActive(false);
        cardTargetFdbck.SetActive(false);

        //Movimiento de unidades
        SelectionManager.Instance.OnUnitMovementStarted += (u) => moveUnitFdbck.SetActive(true);
        SelectionManager.Instance.OnUnitMovementEnded += (u) => moveUnitFdbck.SetActive(false);

        moveUnitFdbck.SetActive(false);
    }
}
