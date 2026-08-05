using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "ScriptableObjects/CardData")]
public class CardData : ScriptableObject
{
    public string cardName;
    public Sprite artwork;
    public string cardType;
    [TextArea]
    public string description;

    [SerializeReference, SubclassSelector]
    private List<CardEffect> effects = new List<CardEffect>();

    public void PlayCard(EffectContext context)
    {
        Debug.Log($"Played card {cardName}");
        foreach (var effect in effects)
        {
            effect.Execute(context);
        }
    }
}
