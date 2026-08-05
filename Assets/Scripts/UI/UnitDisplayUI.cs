using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitDisplayUI : MonoBehaviour
{
    private void Start()
    {
        SelectionManager.Instance.OnAllySelected += DisplayAllyInfo;
        SelectionManager.Instance.OnEnemyInspected += DisplayEnemyInfo;
    }

    void DisplayAllyInfo(BaseUnit ally)
    {

    }

    void DisplayEnemyInfo(BaseUnit enemy)
    {

    }
}
