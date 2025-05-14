using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;   
using System;
using Unity.VisualScripting;
using DG.Tweening;
public class UICardDragNDropHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
        public static event Action<AbstractCard> cardDroppedEvent;
        public static event Action<GameObject> cardUsedEvent;

        public static event Action<AbstractCard> droppableCardSelectedEvent;
        public static event Action<AbstractCard> droppableCardDeselectedEvent;
        private Vector3 startCardDragPosition;
        private Vector3 mousePos;

        [SerializeField] private float minCardOffesetFromCamera = 10f;
        private AbstractCard cardScript;
        private EnergySystem energySystem;
        private Collider cardCollider;

        private GridManager gridManager;


        private void Start()
        {
            cardScript = GetComponent<AbstractCard>();
            cardCollider = GetComponent<Collider>();
            //molto importante, non modificare, evita che le navi debbano avere un rigidbody
            cardCollider.providesContacts = true;

            gridManager = FindFirstObjectByType<GridManager>();

        }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(energySystem != null && energySystem.currentEnergy <cardScript.GetCardCost())
        {
            transform.DOShakePosition(0.5f, 0.1f, 10, 90, false, true);
            //cambiare il colore per un attimo
            return;
        }
        if(cardScript.isWolrdInteractive())
        {
            startCardDragPosition = transform.position;
            transform.position = GetPointerPositionInWorldSpace();
            droppableCardSelectedEvent?.Invoke(cardScript);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(cardScript.isWolrdInteractive())
        {
            int i = 0;

            cardCollider.enabled = false;
            Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, transform.localScale * 2, Quaternion.identity);
            while (i < hitColliders.Length)
            {
                if (hitColliders[i] != null && hitColliders[i].TryGetComponent<ICardDropArea>(out ICardDropArea dropArea))
                {
                    dropArea.CardDrop(cardScript);
                    cardDroppedEvent?.Invoke(cardScript);

                    //impedisco che la carta venga usata più volte se entra in più aree
                    break;
                }
                else
                {
                    transform.position = startCardDragPosition;
                }
                i++;
            }

            cardCollider.enabled = true;

            transform.position = startCardDragPosition;

            droppableCardDeselectedEvent?.Invoke(cardScript);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = GetPointerPositionInWorldSpace();
    }

    private Vector3 GetPointerPositionInWorldSpace()
    {
        //bisogna tenere a mente le dimensioni della finestra. Gli assi dello schermo hanno origine in basso a sx

        if(Input.mousePosition.y >= Screen.height/4)
        {
            
            mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, minCardOffesetFromCamera);
        }
        else
        {
            mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, minCardOffesetFromCamera);
        }
      
      Vector3 objPos = Camera.main.ScreenToWorldPoint(mousePos);
      return objPos;
    }



    public void SetEnergySystem(EnergySystem energySystem)
    {
        this.energySystem = energySystem;
    }


}
