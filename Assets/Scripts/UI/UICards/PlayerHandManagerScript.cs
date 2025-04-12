using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Splines;

public class PlayerHandManagerScript : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private int maxCardsInHand = 5;
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private Transform spawnPoint;

    private List<GameObject> cardsInHand = new List<GameObject>();


    //Iscrizione agli eventi dichiarati nel deck manager
    private void OnEnable()
    {
        DeckDraw.cardDrawed += DrawCard;
    }

    private void OnDisable()
    {
        
    }

    [ContextMenu("Draw Card")]
    private void DrawCard(DeckType deckType, bool hasDrawingCost)
    {
        if (cardsInHand.Count >= maxCardsInHand) return;
        GameObject newCard = Instantiate(cardPrefab, spawnPoint.position, spawnPoint.rotation);
        cardsInHand.Add(newCard);
        UpdateCardPosition();
    }

    private void UpdateCardPosition()
    {
        if(cardsInHand.Count == 0) return;
        float cardSpacing = 1f/maxCardsInHand;
        float firstCardPosition = 0.5f - (cardsInHand.Count-1) * cardSpacing / 2f;
        Spline spline = splineContainer.Spline;
        for (int i = 0; i < cardsInHand.Count; i++)
        {
            float t = firstCardPosition + i * cardSpacing;
            Vector3 splinePosition = splineContainer.EvaluatePosition(t);
            Vector3 forward = splineContainer.EvaluateTangent(t);
            Vector3 up = splineContainer.EvaluateUpVector(t);
            Quaternion rotation = Quaternion.LookRotation(up, Vector3.Cross(up, forward).normalized);

            cardsInHand[i].transform.DOMove(splinePosition, 0.25f);
            cardsInHand[i].transform.DOLocalRotateQuaternion(rotation, 0.25f);
        }
    }
}
