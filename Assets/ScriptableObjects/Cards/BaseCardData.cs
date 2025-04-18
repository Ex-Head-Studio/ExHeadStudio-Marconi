using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public enum CardEntityType
{
    AllyShip,
    EnemyShip,
    AllyAndEnemyShip,
}

[CreateAssetMenu(fileName = "BaseCardData", menuName = "Scriptable Objects/BaseCardData")]
public class BaseCardData : ScriptableObject
{
    [Header("Card Data")]
    [SerializeField] public CardEntityType cardEntityType;
    [SerializeField] public DeckType cardType;
    [SerializeField] public Image cardImage;
    [SerializeField] public string cardName;
    [SerializeField] public string cardDescription;

    [Tooltip("The cost of this card")]
    [SerializeField] public int cardCost;

    [Tooltip("Whether the card can be dragged in the world or not")]
    [SerializeField] public bool isWorldInteractive = false;

    
    [SerializeField] public int lastingTurns = 1;

    //[SerializeField] public GameObject cardPrefab; 

    [Header("Card Effects")]
    [SerializeField] public List<AbstractEffectSO> cardEffects = new List<AbstractEffectSO>();

}
