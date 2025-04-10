using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "BaseCardData", menuName = "Scriptable Objects/BaseCardData")]
public class BaseCardData : ScriptableObject
{
    [SerializeField] public Image cardImage;
    [SerializeField] public string cardName;
    [SerializeField] public string cardDescription;
    [SerializeField] public int cardCost;

    [SerializeField] public GameObject cardPrefab; 

}
