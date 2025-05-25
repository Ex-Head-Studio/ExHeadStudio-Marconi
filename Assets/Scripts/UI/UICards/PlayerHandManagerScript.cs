using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;


[RequireComponent(typeof(PlanningPhaseEndListener))]
[RequireComponent(typeof(ActionPhaseEndEvent))]
[RequireComponent(typeof(EndedTurnEventListener))]
public class PlayerHandManagerScript : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private Transform spawnPoint;

    public int maxCardsInHand;

    private List<GameObject> cardsInHand = new List<GameObject>();


    //Iscrizione agli eventi
    private void OnEnable()
    {
        DeckManager.cardDrawed += DrawCard;
        AbstractCard.abstractCardUsed += OnCardUsed;
        UICardDragNDropHandler.cardUsedEvent += OnCardUsed;
        UICardDragNDropHandler.droppableCardSelectedEvent += OnCardSelected;
        UICard.cardSelectedEvent += OnCardSelected;


    }

    private void OnDisable()
    {
        DeckManager.cardDrawed -= DrawCard;
        AbstractCard.abstractCardUsed -= OnCardUsed;
        UICardDragNDropHandler.cardUsedEvent -= OnCardUsed;
        UICardDragNDropHandler.droppableCardSelectedEvent -= OnCardSelected;
        UICard.cardSelectedEvent -= OnCardSelected;
    }

    [ContextMenu("Draw Card")]
    private void DrawCard(BaseCardData cardData)
    {
        if (cardsInHand.Count >= maxCardsInHand) return;
        GameObject newCard = Instantiate(cardPrefab, spawnPoint.position, spawnPoint.rotation);
        PlayCartDraw();
        //voglio settare questo oggetto come parent
        //newCard.transform.SetParent(gameObject.transform, true);
        SetUpCard(cardData, newCard);

        cardsInHand.Add(newCard);
        UpdateCardPosition();
    }

    private void SetUpCard(BaseCardData cardData, GameObject newCard)
    {
        //aggiungo al prefab lo script della carta corrsipondente, per ora solo command. Si può fare uno switch sul tipo
        newCard.AddComponent<CommandCard>();
        AbstractCard cardScript = newCard.GetComponent<AbstractCard>(); 
        cardScript.SetCardData(cardData);
        newCard.GetComponent<UICard>().SetupUICard(cardScript);
    }

    //funzione di callback per la carta usata, distruzione della carta e aggiornamento della posizione

    private void OnCardUsed(GameObject cardUsed)
    {
        Destroy(cardUsed);
        cardsInHand.Remove(cardUsed);
        UpdateCardPosition();
    }

    private void OnCardUsed(AbstractCard cardUsed)
    {
        Destroy(cardUsed.gameObject);
        cardsInHand.Remove(cardUsed.gameObject);
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
            //Quaternion rotation = Quaternion.LookRotation(up, Vector3.Cross(up, forward).normalized);

            cardsInHand[i].transform.DOMove(splinePosition, 0.25f);
            cardsInHand[i].transform.DOLocalRotateQuaternion(spawnPoint.rotation, 0f);
        }
    }


    /// <summary>
    /// Methods which destroys player hand at the end of a turn
    /// </summary> 
    public void DestroyPlayerHand()
    {
        StartCoroutine(WaitBeforeDestroy(0.5f, cardsInHand.Count - 1, 0));
    }

    //funzione ricorsiva per distruggere le carte
    private IEnumerator WaitBeforeDestroy(float waitTime,int iterationIndex, int cardIndex)
    {

        if(iterationIndex < 0)
        {
            yield break;
        }
            yield return WaitBeforeDestroy(waitTime, iterationIndex - 1, cardIndex + 1);
            yield return new WaitForSeconds(waitTime);
            Destroy(cardsInHand.ElementAt(cardIndex));
            cardsInHand.RemoveAt(cardIndex);
            UpdateCardPosition();
    }


    /// <summary>
    /// Function which controls the selection of the cards in hand. Hand manager respond to
    /// card event and check wich cards to disable.
    /// </summary>
    private void OnCardSelected(AbstractCard card)
    {
        foreach(GameObject cardInHand in cardsInHand)
        {
            if(cardInHand.GetComponent<UICard>().IsCardSelected() && cardInHand != card.gameObject)
            {
                cardInHand.GetComponent<UICard>().DeselectCard(cardInHand.GetComponent<AbstractCard>());
            }
        }
    }

    // <summary>
    // Play the card draw sound
    // </summary>
    private FMOD.Studio.EventInstance cartDraw;

    public void PlayCartDraw()
    {
        cartDraw = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/Cards/CardDraw");
        cartDraw.start();
        cartDraw.release();
    }
    
    public int GetHandCardsCount()
    {
        return cardsInHand.Count;
    }

}
