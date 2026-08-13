using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitDisplayUI : MonoBehaviour
{
    [SerializeField] private TMP_Text unitNameTXT;
    [SerializeField] private TMP_Text unitHealthTXT;
    [SerializeField] private GameObject isTargetingWithCardDisplay; //Objecto que se activa cuando esta en modo seleccionar objetivo

    private void Start()
    {
        SelectionManager.Instance.OnAllySelected += DisplayAllyInfo;
        SelectionManager.Instance.OnEnemyInspected += DisplayEnemyInfo;
        SelectionManager.Instance.OnCardTargetingStarted += (c) => isTargetingWithCardDisplay.SetActive(true);
        SelectionManager.Instance.OnCardTargetingEnded += (c) => isTargetingWithCardDisplay.SetActive(false);

        isTargetingWithCardDisplay.SetActive(false);
    }

    void DisplayGeneralInfo(BaseUnit unit)
    {
        unitNameTXT.text = unit.name;
    }

    void DisplayAllyInfo(BaseUnit ally)
    {
        DisplayGeneralInfo(ally);
        unitNameTXT.color = Color.yellow;
    }

    void DisplayEnemyInfo(BaseUnit enemy)
    {
        DisplayGeneralInfo(enemy);
        unitNameTXT.color = Color.red;
    }
}
