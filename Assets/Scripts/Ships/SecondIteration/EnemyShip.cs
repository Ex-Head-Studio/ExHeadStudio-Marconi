using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
public class EnemyShip : AShip
{
    Move initialMove;
    int idMove = 0;
    
    //Quando viene chiamata la fine del turno, Execute Instructions fa fare l'azione migliore alla nave, a meno che non sia stata bloccata
    //dal giocatore, in quel caso non fa nulla
    public override void ExecuteMove()
    {
        if (!canMove && !canAttack)
        {
            SwitchOffMovesVisualization(shipMoves);

            shipMoves.Clear();
            return;
        }

        switch (initialMove.GetMessageType())
        {
            case MessageType.attack:
                StartCoroutine(Attack());
                break;
            case MessageType.movement:

                StartCoroutine(MoveShip());
                
                break;
        }
        moveDone = true;

        GridManager.Instance.GetTileAtPosition(initialMove.GetTargetPos()).SetTileNotInteractable(faction);
    }
    IEnumerator Attack()
    {

        //shipAnimator.Play("Attack");
        //yield return new WaitForSeconds(shipAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
        yield return new WaitForSeconds(0);
        shipSO.attackEvent?.Invoke(new ShipAttackStruct(initialMove.GetTargetPos(), shipSO.attackPower));
    }
    
    IEnumerator MoveShip()
    {
        // Impostiamo la destinazione del movimento
        Vector3 targetPosition = GridManager.Instance.GetTileAtPosition(initialMove.GetTargetPos()).transform.position;
        GridManager.Instance.GetTileAtPosition(initialMove.GetTargetPos()).SetType(TileType.Enemy, faction);
        GridManager.Instance.GetTileAtPosition(position).SetTypeEmpty();
        
        // Calcoliamo la direzione verso cui la nave deve guardare
        Vector3 direction = targetPosition - transform.position;
        
        // Cerchiamo l'oggetto ship e il suo figlio da ruotare
        Transform shipObject = null;
        Transform shipChild = null;
        
        // Cerchiamo prima l'oggetto ship tra i figli diretti
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name.ToLower().Contains("ship"))
            {
                shipObject = transform.GetChild(i);
                // Se ship ha almeno un figlio, prendiamo il primo
                if (shipObject.childCount > 0)
                {
                    shipChild = shipObject.GetChild(0);
                }
                break;
            }
        }
        
        // Se non abbiamo trovato l'oggetto ship nei figli diretti, lo cerchiamo ricorsivamente
        if (shipObject == null)
        {
            // Funzione ricorsiva per cercare un oggetto con "ship" nel nome
            Transform FindShipRecursive(Transform parent)
            {
                for (int i = 0; i < parent.childCount; i++)
                {
                    Transform child = parent.GetChild(i);
                    if (child.name.ToLower().Contains("ship"))
                    {
                        return child;
                    }
                    
                    Transform result = FindShipRecursive(child);
                    if (result != null)
                    {
                        return result;
                    }
                }
                return null;
            }
            
            shipObject = FindShipRecursive(transform);
            if (shipObject != null && shipObject.childCount > 0)
            {
                shipChild = shipObject.GetChild(0);
            }
        }
        
        // Se ancora non abbiamo trovato nulla, usiamo l'animator come riferimento
        if (shipObject == null && shipAnimator != null)
        {
            shipObject = shipAnimator.transform;
            
            // Cerca il padre che contiene "ship" nel nome
            Transform current = shipAnimator.transform;
            while (current != null && !current.name.ToLower().Contains("ship"))
            {
                current = current.parent;
            }
            
            if (current != null)
            {
                shipObject = current;
                // Trova il primo figlio se esiste
                if (shipObject.childCount > 0)
                {
                    shipChild = shipObject.GetChild(0);
                }
            }
            else if (shipAnimator.transform.childCount > 0)
            {
                // Se non troviamo un parent con "ship" nel nome, prendiamo il primo figlio dell'animator
                shipChild = shipAnimator.transform.GetChild(0);
            }
        }
        
        Debug.Log("ShipObject: " + (shipObject != null ? shipObject.name : "null") + 
                ", ShipChild: " + (shipChild != null ? shipChild.name : "null") +
                ", Animator: " + (shipAnimator != null ? shipAnimator.name : "null"));
        
        // Oggetto da ruotare (il figlio dell'oggetto ship se esiste, altrimenti l'oggetto ship stesso)
        Transform objectToRotate = shipChild != null ? shipChild : shipObject;
        
        // Ruotiamo l'oggetto trovato, se presente
        if (direction != Vector3.zero && objectToRotate != null)
        {
            // Calcola la rotazione guardando verso la direzione target
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            
            // Log della direzione per debug
            Debug.Log($"Direzione movimento: {direction}, angolo Y originale: {targetRotation.eulerAngles.y}");
            
            // Testa se la direzione è invertita
            bool isInverted = false;
            
            // Controlla se il modello è invertito nella gerarchia
            Transform modelTransform = objectToRotate;
            while (modelTransform != null)
            {
                // Se troviamo una rotazione di circa 180° sull'asse X o Z, consideriamo il modello invertito
                if (Mathf.Approximately(Mathf.Abs(modelTransform.localRotation.eulerAngles.x), 180f) || 
                    Mathf.Approximately(Mathf.Abs(modelTransform.localRotation.eulerAngles.z), 180f))
                {
                    isInverted = !isInverted; // Toggle l'inversione
                }
                modelTransform = modelTransform.parent;
            }
            
            // Se necessario, inverti l'angolo Y (giro di 180°)
            float yAngle = targetRotation.eulerAngles.y;
            if (isInverted)
            {
                yAngle = (yAngle + 180f) % 360f;
                Debug.Log($"Modello invertito: rotazione Y corretta a {yAngle}");
            }
            
            // Gestione speciale in base alla direzione di movimento
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
            {
                // Movimento principalmente orizzontale (lungo X)
                if (direction.x > 0)
                {
                    // Movimento verso destra
                    targetRotation = Quaternion.Euler(90f, 0f, 180f);
                }
                else
                {
                    // Movimento verso sinistra
                    targetRotation = Quaternion.Euler(90f, 0f, 0f);
                }
            }
            else
            {
                // Movimento principalmente verticale (lungo Z)
                if (direction.z > 0)
                {
                    // Movimento verso l'alto
                    targetRotation = Quaternion.Euler(90f, 0f, -90f);
                }
                else
                {
                    // Movimento verso il basso
                    targetRotation = Quaternion.Euler(90f, 0f, 90f);
                }
            }
            
            Debug.Log($"Rotazione finale applicata: {targetRotation.eulerAngles}");
            
            // Cerca l'oggetto Ship per applicare la rotazione direttamente ad esso
            Transform shipTransform = null;
            
            // Cerca prima nei figli diretti
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).name.ToLower().Contains("ship"))
                {
                    shipTransform = transform.GetChild(i);
                    break;
                }
            }
            
            // Se non l'abbiamo trovato nei figli diretti, controlliamo se shipObject è valido
            if (shipTransform == null && shipObject != null)
            {
                shipTransform = shipObject;
            }
            
            // Salva la rotazione iniziale dell'oggetto Ship
            Quaternion startRotation = shipTransform != null ? shipTransform.rotation : objectToRotate.rotation;
            
            // Avviamo l'animazione di Startup
            if (shipAnimator != null)
            {
                shipAnimator.SetBool("Move", true);
                shipAnimator.Play("Startup");
                
                // Ottieni la durata dell'animazione
                float animDuration = 0;
                if (shipAnimator.GetCurrentAnimatorClipInfo(0).Length > 0)
                {
                    // Calcolo della durata effettiva considerando la velocità
                    float clipLength = shipAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
                    float animSpeed = shipAnimator.GetCurrentAnimatorStateInfo(0).speed;
                    animDuration = clipLength / animSpeed;
                    animDuration = Mathf.Min(animDuration, 0.5f);
                }
                else
                {
                    animDuration = 0.5f;
                }
                
                // Variabili per la rotazione
                float startTime = Time.time;
                float elapsedTime = 0f;
                
                // Ruotiamo gradualmente l'oggetto Ship durante l'animazione di Startup
                while (elapsedTime < animDuration)
                {
                    float t = elapsedTime / animDuration;
                    
                    // Applica la rotazione graduale all'oggetto Ship
                    if (shipTransform != null)
                    {
                        shipTransform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
                    }
                    else
                    {
                        // Fallback all'oggetto objectToRotate se Ship non è stato trovato
                        objectToRotate.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
                    }
                    
                    elapsedTime = Time.time - startTime;
                    yield return null;
                }
                
                // Assicuriamoci che la rotazione finale sia esatta
                if (shipTransform != null)
                {
                    shipTransform.rotation = targetRotation;
                }
                else
                {
                    objectToRotate.rotation = targetRotation;
                }
                
                // Aspettiamo che l'animatore passi effettivamente allo stato "Move"
                while (!shipAnimator.GetCurrentAnimatorStateInfo(0).IsName("Move"))
                {
                    yield return null;
                }
                
                // Minima attesa per assicurarci che l'animazione di Move sia iniziata
                while (shipAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.05f)
                {
                    yield return null;
                }
            }
            else
            {
                // Se non c'è animator, ruotiamo gradualmente in un tempo predefinito
                float rotationDuration = 0.5f;
                float elapsedTime = 0f;
                float startTime = Time.time;
                
                while (elapsedTime < rotationDuration)
                {
                    float t = elapsedTime / rotationDuration;
                    objectToRotate.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
                    elapsedTime = Time.time - startTime;
                    yield return null;
                }
                
                objectToRotate.rotation = targetRotation;
                yield return new WaitForSeconds(0.2f);
            }
            
            // Iniziamo il movimento immediatamente dopo che la rotazione è completa
            Debug.Log("Starting movement immediately after rotation");
            startMovement = true;
        }
        else
        {
            // Se non c'è rotazione da fare, inizia subito il movimento
            startMovement = true;
        }

        // Threshold per quando considerare il movimento "quasi completato"
        float completionThreshold = 0.1f;

        // Attendiamo che il movimento sia quasi completato
        while (Vector3.Distance(transform.position, targetPosition) > completionThreshold)
        {
            yield return null;
        }
        
        // Quando siamo abbastanza vicini, fermiamo l'animazione di movimento
        if (shipAnimator != null)
        {
            shipAnimator.SetBool("Move", false);
        }
        
        // Completiamo il movimento con uno scatto finale
        startMovement = false;
        
        // Posizionamento finale immediato
        transform.position = targetPosition;
        
        // Aggiorniamo la posizione della nave sulla griglia
        GridManager.Instance.MoveShip(this.position, initialMove.GetTargetPos(), faction);
        this.position = initialMove.GetTargetPos();
    }

    void Update()
    {
        if (startMovement)
        {
            // Destinazione del movimento
            Vector3 targetPosition = GridManager.Instance.GetTileAtPosition(initialMove.GetTargetPos()).transform.position;
            
            // Aumentiamo considerevolmente la velocità base
            float baseSpeed = 1.0f; // Velocità base aumentata (era basata su timeToMove)
            
            // Calcolo della distanza per determinare una velocità proporzionale
            float distance = Vector3.Distance(transform.position, targetPosition);
            
            // Impostiamo una velocità minima più alta per evitare rallentamenti
            float speed = Mathf.Max(baseSpeed, distance * 2.0f);
            
            // Movimento a velocità aumentata
            transform.position = Vector3.MoveTowards(
                transform.position, 
                targetPosition, 
                speed * Time.deltaTime
            );
        }
    }
    public override void LookForMoves()
    {
        LookForMovement();
        LookForAttacks();
        ExecuteMove();
    }
    public override bool LookForMovement()
    {
        moveDone = false;
        canMove = false;
        shipMoves.Clear();
        List<Move> possibleMoves = new List<Move>();
        //Crea una lista di possibili mosse e aggiungi tutte le mosse in tutte le posizioni che rimangono all'interno della mappa, 
        // in verticale e orizzontale

        //ricerca verso sx
        for (int x = position.x; x >= position.x - movementRange; x--)
        {
            if (x >= 0 && x < GridManager.Instance._width && x != position.x)
            {
                Vector2Int pos = new Vector2Int(x, position.y);

                if (GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Obstacle ||
                    GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Ally)
                {
                    break;
                }

                float value = manager.InfluenceMap.CalculateMoveValue(pos.x, pos.y);
                if (GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Empty)
                {
                    possibleMoves.Add(new Move(idMove++, shipName, pos, MessageType.movement, value));
                }
            }
        }

        //ricerca verso dx
        for (int x = position.x; x <= position.x + movementRange; x++)
        {
            if (x >= 0 && x < GridManager.Instance._width && x != position.x)
            {
                Vector2Int pos = new Vector2Int(x, position.y);

                if (GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Obstacle ||
                    GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Ally)
                {
                    break;
                }

                float value = manager.InfluenceMap.CalculateMoveValue(pos.x, pos.y);
                if (GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Empty)
                {
                    possibleMoves.Add(new Move(idMove++, shipName, pos, MessageType.movement, value));
                }
            }
        }

        //ricerca verso basso
        for (int y = position.y; y >= position.y - movementRange; y--)
        {
            if (y >= 0 && y < GridManager.Instance._height && y != position.y)
            {
                Vector2Int pos = new Vector2Int(position.x, y);

                if (GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Obstacle ||
                    GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Ally)
                {
                    break;
                }

                float value = manager.InfluenceMap.CalculateMoveValue(pos.x, pos.y);
                if (GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Empty)
                {
                    possibleMoves.Add(new Move(idMove++, shipName, pos, MessageType.movement, value));
                }
            }
        }

        //ricerca verso alto
        for (int y = position.y; y <= position.y + movementRange; y++)
        {
            if (y >= 0 && y < GridManager.Instance._height && y != position.y)
            {
                Vector2Int pos = new Vector2Int(position.x, y);

                if (GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Obstacle ||
                    GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Ally)
                {
                    break;
                }

                float value = manager.InfluenceMap.CalculateMoveValue(pos.x, pos.y);
                if (GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Empty)
                {
                    possibleMoves.Add(new Move(idMove++, shipName, pos, MessageType.movement, value));
                }
            }
        }


        //Fatto ciò, elimina le mosse che portano a posizioni già occupate.
        possibleMoves = possibleMoves.Where(x => GridManager.Instance.GetTileAtPosition(x.GetTargetPos()).GetType() == TileType.Empty).ToList();
        //Ordina le mosse per valore decrescente, in modo da avere prima le mosse più vantaggiose.
        possibleMoves = possibleMoves.OrderByDescending(x => x.value).ToList();
        if (possibleMoves.Count > 0)
        {
            shipMoves = possibleMoves;
            canMove = true;
        }

        //Chiamo la funzione per il display delle mosse nemiche

        //SwitchOnMovesVisualization(shipMoves);
        return canMove;

    }

    public override bool LookForAttacks()
    {
        canAttack = false;
        foreach (Move move in shipMoves)
        {
            //Controlla se le mosse di movimento calcolate in LookForMovement portano a posizioni di attacco, se si, aggiungi il valore della mossa
            //più distanza c'è col nemico, più il valore è alto

            //ricerca verso sx
            for (int x = move.GetTargetPos().x; x >= move.GetTargetPos().x - attackRange; x--)
            {
                if (x >= 0 && x < GridManager.Instance._width && x != move.GetTargetPos().x)
                {

                    //Controlla se la tile è occupata da un nemico, se si, aggiungi il valore della mossa
                    if (GridManager.Instance.GetTileAtPosition(new Vector2Int(x, move.GetTargetPos().y))._type == TileType.Ally)
                    {
                        Vector2Int pos = new Vector2Int(x, move.GetTargetPos().y);

                        if (GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Obstacle)
                        {
                            Debug.Log("Break");
                            break;
                        }

                        if (Vector2Int.Distance(move.GetTargetPos(), pos) == attackRange)
                        {
                            move.value += 2;
                        }
                        else if (Vector2Int.Distance(move.GetTargetPos(), pos) < attackRange && Vector2Int.Distance(move.GetTargetPos(), pos) > 0)
                        {
                            move.value += 1;
                        }
                    }
                }
            }

            //ricerca verso dx
            for (int x = move.GetTargetPos().x; x <= move.GetTargetPos().x + attackRange; x++)
            {
                if (x >= 0 && x < GridManager.Instance._width && x != move.GetTargetPos().x)
                {

                    //Controlla se la tile è occupata da un nemico, se si, aggiungi il valore della mossa
                    if (GridManager.Instance.GetTileAtPosition(new Vector2Int(x, move.GetTargetPos().y)).GetType() == TileType.Ally)
                    {
                        Vector2Int pos = new Vector2Int(x, move.GetTargetPos().y);

                        if (GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Obstacle)
                        {
                            Debug.Log("Break");
                            break;
                        }

                        if (Vector2Int.Distance(move.GetTargetPos(), pos) == attackRange)
                        {
                            move.value += 2;
                        }
                        else if (Vector2Int.Distance(move.GetTargetPos(), pos) < attackRange && Vector2Int.Distance(move.GetTargetPos(), pos) > 0)
                        {
                            move.value += 1;
                        }
                    }
                }
            }

            //ricerca verso basso
            for (int y = move.GetTargetPos().y; y >= move.GetTargetPos().y - attackRange; y--)
            {
                if (y >= 0 && y < GridManager.Instance._height && y != move.GetTargetPos().y)
                {

                    //Controlla se la tile è occupata da un nemico, se si, aggiungi il valore della mossa
                    if (GridManager.Instance.GetTileAtPosition(new Vector2Int(move.GetTargetPos().x, y)).GetType() == TileType.Ally)
                    {
                        Vector2Int pos = new Vector2Int(move.GetTargetPos().x, y);


                        if (Vector2Int.Distance(move.GetTargetPos(), pos) == attackRange)
                        {
                            move.value += 2;
                        }
                        else if (Vector2Int.Distance(move.GetTargetPos(), pos) < attackRange && Vector2Int.Distance(move.GetTargetPos(), pos) > 0)
                        {
                            move.value += 1;
                        }
                    }
                }
            }

            //ricerca verso alto
            for (int y = move.GetTargetPos().y; y <= move.GetTargetPos().y - attackRange; y++)
            {
                if (y >= 0 && y < GridManager.Instance._height && y != move.GetTargetPos().y)
                {

                    //Controlla se la tile è occupata da un nemico, se si, aggiungi il valore della mossa
                    if (GridManager.Instance.GetTileAtPosition(new Vector2Int(move.GetTargetPos().x, y)).GetType() == TileType.Ally)
                    {
                        Vector2Int pos = new Vector2Int(move.GetTargetPos().x, y);

                        if (Vector2Int.Distance(move.GetTargetPos(), pos) == attackRange)
                        {
                            move.value += 2;
                        }
                        else if (Vector2Int.Distance(move.GetTargetPos(), pos) < attackRange && Vector2Int.Distance(move.GetTargetPos(), pos) > 0)
                        {
                            move.value += 1;
                        }
                    }
                }
            }
        }

        //Aggiungi le mosse di attacco dalla posizioni attuale della nave, non quelle future, aggiungi il valore della mossa
        // più distanza c'è col nemico, più il valore è alto, valori così alti servono perché se la nave può già attaccare,
        // non ha senso muoverla, quindi il valore della mossa di attacco deve essere più alto di quello di movimento


        //Cerca a sx
        for (int x = position.x - 1; x >= position.x - attackRange && x>=0 && x < GridManager.Instance._width; x--)
        {
            if (GridManager.Instance.GetTileAtPosition(new Vector2Int(x, position.y)).GetType() == TileType.Ally)
                {

                    Vector2Int pos = new Vector2Int(x, position.y);
                    if(GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Obstacle)
                    {
                    Move move = new Move(idMove++, shipName, pos, MessageType.attack, 0);
                    shipMoves.Add(move);
                    canAttack = true;
                    break;
                    }
                    Move newMove = new Move(idMove++, shipName, pos, MessageType.attack, 0);
                    if (Vector2Int.Distance(position, pos) == shipSO.attackRange)
                    {

                        newMove.value += 6;


                    }
                    else if (Vector2Int.Distance(position, pos) < shipSO.attackRange && Vector2Int.Distance(position, pos) > 0)
                    {
                        newMove.value += 5;
                    }
                    shipMoves.Add(newMove);
                }
        }
        //Cerca a dx
        for (int x = position.x + 1; x <= position.x + attackRange && x >= 0 && x < GridManager.Instance._width; x++)
        {
            if (GridManager.Instance.GetTileAtPosition(new Vector2Int(x, position.y)).GetType() == TileType.Ally)
            {
                Vector2Int pos = new Vector2Int(x, position.y);
                if(GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Obstacle)
                    {
                    Move move = new Move(idMove++, shipName, pos, MessageType.attack, 0);
                    shipMoves.Add(move);
                    canAttack = true;
                    break;
                    }
                Move newMove = new Move(idMove++, shipName, pos, MessageType.attack, 0);
                if (Vector2Int.Distance(position, pos) == shipSO.attackRange)
                {

                    newMove.value += 6;


                }
                else if (Vector2Int.Distance(position, pos) < shipSO.attackRange && Vector2Int.Distance(position, pos) > 0)
                {
                    newMove.value += 5;
                }
                shipMoves.Add(newMove);
            }

        }
        for (int y = position.y - 1; y >= position.y - attackRange && y >= 0 && y < GridManager.Instance._height; y--)
        {
            if (GridManager.Instance.GetTileAtPosition(new Vector2Int(position.x, y))._type == TileType.Ally)
                {
                    Vector2Int pos = new Vector2Int(position.x, y);
                    if(GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Obstacle)
                    {
                    Move move = new Move(idMove++, shipName, pos, MessageType.attack, 0);
                    shipMoves.Add(move);
                    canAttack = true;
                    break;
                    }
                    Move newMove = new Move(idMove++, shipName, pos, MessageType.attack, 0);
                    if (Vector2Int.Distance(position, pos) == attackRange)
                    {

                        newMove.value += 6;
                    }
                    else if (Vector2Int.Distance(position, pos) < attackRange && Vector2Int.Distance(position, pos) > 0)
                    {

                        newMove.value += 5;
                    }
                    shipMoves.Add(newMove);
                }
        }
        for (int y = position.y + 1 ; y <= position.y + attackRange && y >= 0 && y < GridManager.Instance._height; y++)
        {
            
                //Controlla se la tile è occupata da un nemico, se si, aggiungi il valore della mossa
                if (GridManager.Instance.GetTileAtPosition(new Vector2Int(position.x, y)).GetType() == TileType.Ally)
                {
                    Vector2Int pos = new Vector2Int(position.x, y);
                    if(GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Obstacle)
                    {
                    Move move = new Move(idMove++, shipName, pos, MessageType.attack, 0);
                    shipMoves.Add(move);
                    canAttack = true;
                    break;
                    }
                    Move newMove = new Move(idMove++, shipName, pos, MessageType.attack, 0);
                    if (Vector2Int.Distance(position, pos) == shipSO.attackRange)
                    {

                        newMove.value += 6;
                    }
                    else if (Vector2Int.Distance(position, pos) < shipSO.attackRange && Vector2Int.Distance(position, pos) > 0)
                    {

                        newMove.value += 5;
                    }
                    shipMoves.Add(newMove);
                }
            
        }



        
        //Ordina le mosse per valore decrescente, in modo da avere prima le mosse più vantaggiose.
        shipMoves = shipMoves.OrderByDescending(x => x.value).ToList();
        
        if (shipMoves.Count > 0 && shipMoves.Where(x => x.GetMessageType() == MessageType.attack).ToList().Count > 0)
        {
            canAttack = true;
            //Chiamo la funzione per il display delle mosse nemiche

        }

        if (canAttack || canMove)
        {
            initialMove = shipMoves[0];
        }
        Debug.Log(shipMoves.Count);
        
        //GridManager.Instance.GetTileAtPosition(initialMove.GetTargetPos()).SetTileInteractable(faction, initialMove);
        //Debug.Log("Ship: " + shipName + " performs: " + initialMove.GetMessageType() + " on: " + initialMove.GetTargetPos());
        return canAttack;
    }


    //Metodo da invocare nel caso si volesse bloccare l'azione di una nave nemica

    public void DisableShip()
    {
        canAttack = false;
        canMove = false;
    }

    //Funzioni da utilizzare per la risoluzione degli effetti delle carte
    public void NegateAction()
    {
        canAttack = false;
        canMove = false;
        //invoco ora la funzione per non attendere la fine del turno

    }

    private void SwitchOffMovesVisualization(List<Move> shipMoves)
    {
        foreach (Move move in shipMoves)
        {
            GridManager.Instance.GetTileAtPosition(move.GetTargetPos()).SetTileNotInteractable(faction);
        }
    }
    private void SwitchOnMovesVisualization(List<Move> shipMoves)
    {
        foreach (Move move in shipMoves)
        {
            GridManager.Instance.GetTileAtPosition(move.GetTargetPos()).SetTileInteractable(faction);
        }
    }

    public override bool LookForAttacks(List<AShip> nearbyShips)
    {
        throw new System.NotImplementedException();
    }

    public override void SendMessage(Move move)
    {
        throw new System.NotImplementedException();
    }
}
