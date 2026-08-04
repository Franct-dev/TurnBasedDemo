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

}
