using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameTxt;
    [SerializeField] private TextMeshProUGUI descriptionTxt;
    [SerializeField] private TextMeshProUGUI targetTypeTxt;
    [SerializeField] private Image cardArtwork;

    public CardData CurrentCardData { get; private set; }

    // Rellena la carta con la información del ScriptableObject
    public void Bind(CardData card)
    {
        CurrentCardData = card;

        nameTxt.text = card.CardName;
        descriptionTxt.text = card.Description;
        cardArtwork.sprite = card.Artwork;
        targetTypeTxt.text = nameof(card.validTargets);

        //acceder al componente boton para borrar todos sus eventos y añadir el de esta propia carta
        if(TryGetComponent(out Button btn))
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(OnCardClicked);
        }

        gameObject.SetActive(true);
    }

    public void Unbind()
    {
        CurrentCardData = null;
        gameObject.SetActive(false);
    }

    // Método que se llama desde el evento OnClick del Button de la carta
    public void OnCardClicked()
    {
        if (CurrentCardData != null)
        {
            SelectionManager.Instance.StartCardTargeting(CurrentCardData);
        }
    }
}