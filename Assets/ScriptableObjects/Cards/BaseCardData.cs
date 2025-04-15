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

    [SerializeField] public Image cardImage;
    [SerializeField] public string cardName;
    [SerializeField] public string cardDescription;
    [SerializeField] public int cardCost;

    [SerializeField] public int lastingTurns = 1;

    [SerializeField] public GameObject cardPrefab; 

    [Header("Card Effects")]
    [SerializeField] public List<AbstractEffectSO> cardEffects = new List<AbstractEffectSO>();

}
