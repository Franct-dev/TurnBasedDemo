using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitDisplayUI : MonoBehaviour
{
    [SerializeField] private TMP_Text unitNameTXT;
    [SerializeField] private TMP_Text unitHealthTXT;


    private void Start()
    {
        SelectionManager.Instance.OnAllySelected += DisplayAllyInfo;
        SelectionManager.Instance.OnEnemyInspected += DisplayEnemyInfo;

        SelectionManager.Instance.OnSelectionCleared += HideInfo;
    }

    void DisplayGeneralInfo(BaseUnit unit)
    {
        if (unit.Data == null) return;
        unitNameTXT.text = unit.Data.UnitName;
        unitHealthTXT.text = unit.Data.BaseHealth.ToString();
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

    void HideInfo()
    {
        unitNameTXT.text = string.Empty;
        unitHealthTXT.text = string.Empty;
    }
}
