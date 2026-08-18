using UnityEngine;

public class CardTesting : MonoBehaviour
{
    public CardData cardToTest;
    private TMPro.TMP_Text cardNameTxt;

    private void Start()
    {
        if(transform.GetChild(0).TryGetComponent(out cardNameTxt))
        {
            cardNameTxt.text = cardToTest.CardName;
        }
        if(TryGetComponent<UnityEngine.UI.Button>(out var btn))
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(PlaytestCard);
        }
    }

    public void PlaytestCard()
    {
        SelectionManager.Instance.StartCardTargeting(cardToTest);
    }
}
