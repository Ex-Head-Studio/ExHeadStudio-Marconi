using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.Cinemachine;
using System.Collections;


public class AllyShip : AShip
{
    
    int moveId = 0;
    Vector3 targetPosition;
    private CardAllyShip cardAllyScript;
    
    private void OnEnable()
    {
        Tile.tileSelected += ReceiveTile;
    }

    private void OnDisable()
    {
        Tile.tileSelected -= ReceiveTile;
    }

    //Le funzioni che seguono servono per dare al giocatore la possibilità di scegliere solo azioni consentite
    //e non tutte le azioni possibili, come nel caso delle navi nemiche
    [ContextMenu("LookForMovement")]
    public override bool LookForMovement()
    {
        //quando la invoco, pulisco la lista delle azioni possibili e la riempio con le nuove
        shipMoves.Clear();

        canMove = false;

        //ricerca verso sx
        for (int x = position.x; x >= position.x - movementRange; x--)
        {
            if (x >= 0 && x < GridManager.Instance._width && x != position.x)
            {
                Vector2Int pos = new Vector2Int(x, position.y);

                if (GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Obstacle ||
                    GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Enemy)
                {
                    break;
                }

                Move move = new Move(moveId++, shipName, pos, MessageType.movement, 0);
                if (move != null && GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Empty)
                    shipMoves.Add(move);
            }
        }

        //ricerca verso dx
        for (int x = position.x; x <= position.x + movementRange; x++)
        {
            if (x >= 0 && x < GridManager.Instance._width && x != position.x)
            {
                Vector2Int pos = new Vector2Int(x, position.y);

                if (GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Obstacle ||
                    GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Enemy)
                {
                    break;
                }

                Move move = new Move(moveId++, shipName, pos, MessageType.movement, 0);
                if (move != null && GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Empty)
                    shipMoves.Add(move);
            }
        }

        //ricerca verso basso
        for (int y = position.y; y >= position.y - movementRange; y--)
        {
            if (y >= 0 && y < GridManager.Instance._height && y != position.y)
            {
                Vector2Int pos = new Vector2Int(position.x, y);

                if (GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Obstacle ||
                    GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Enemy)
                {
                    break;
                }

                Move move = new Move(moveId++, shipName, pos, MessageType.movement, 0);
                if (move != null && GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Empty)
                    shipMoves.Add(move);
            }
        }

        //ricerca verso alto
        for (int y = position.y; y <= position.y + movementRange; y++)
        {
            if (y >= 0 && y < GridManager.Instance._height && y != position.y)
            {
                Vector2Int pos = new Vector2Int(position.x, y);

                if (GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Obstacle ||
                    GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Enemy)
                {
                    break;
                }

                Move move = new Move(moveId++, shipName, pos, MessageType.movement, 0);
                if (move != null && GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Empty)
                    shipMoves.Add(move);
            }
        }

        //Se le carte permettono giusto di muoversi e poi il player decide dove, questa lista gli farà vedere solo dove potrà spostarsi.
        shipMoves = shipMoves.Where(x => GridManager.Instance.GetTileAtPosition(x.GetTargetPos())._type == TileType.Empty).ToList();
        if (shipMoves.Count > 0)
        {
            foreach (Move move in shipMoves)
            {
                GridManager.Instance.GetTileAtPosition(move.GetTargetPos()).SetTileInteractable(faction, move);
            }
            canMove = true;
        }
        else
        {
            ByPassEffect();
        }
        return canMove;
    }
    public override bool LookForAttacks()
    {

        //quando la invoco, pulisco la lista delle azioni possibili e la riempio con le nuove
        shipMoves.Clear();

        canAttack = false;

        //ricerca verso sx
        for (int x = position.x; x >= position.x - attackRange; x--)
        {
            if (x >= 0 && x < GridManager.Instance._width && x != position.x)
            {
                Vector2Int pos = new Vector2Int(x, position.y);

                if (GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Obstacle)
                {
                    Move move = new Move(moveId++, shipName, pos, MessageType.attack, 0);
                    shipMoves.Add(move);
                    canAttack = true;
                    break;
                }

                if (GridManager.Instance.GetTileAtPosition(pos)._type == TileType.Enemy
                     && GridManager.Instance.GetTileAtPosition(pos).GetShip() != null)
                {
                    Move move = new Move(moveId++, shipName, pos, MessageType.attack, 0);
                    shipMoves.Add(move);
                    canAttack = true;
                }
            }
        }

        //ricerca verso dx
        for (int x = position.x; x <= position.x + attackRange; x++)
        {
            if (x >= 0 && x < GridManager.Instance._width && x != position.x)
            {
                Vector2Int pos = new Vector2Int(x, position.y);

                if (GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Obstacle)
                {
                    Move move = new Move(moveId++, shipName, pos, MessageType.attack, 0);
                    shipMoves.Add(move);
                    canAttack = true;
                    break;
                }

                if (GridManager.Instance.GetTileAtPosition(pos)._type == TileType.Enemy
                     && GridManager.Instance.GetTileAtPosition(pos).GetShip() != null)
                {
                    Move move = new Move(moveId++, shipName, pos, MessageType.attack, 0);
                    shipMoves.Add(move);
                    canAttack = true;
                }
            }
        }

        //ricerca verso basso
        for (int y = position.y; y >= position.y - attackRange; y--)
        {
            if (y >= 0 && y < GridManager.Instance._height && y != position.y)
            {
                Vector2Int pos = new Vector2Int(position.x, y);

                if (GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Obstacle)
                {
                    Move move = new Move(moveId++, shipName, pos, MessageType.attack, 0);
                    shipMoves.Add(move);
                    canAttack = true;
                    break;
                }

                if (GridManager.Instance.GetTileAtPosition(pos)._type == TileType.Enemy
                     && GridManager.Instance.GetTileAtPosition(pos).GetShip() != null)
                {
                    Move move = new Move(moveId++, shipName, pos, MessageType.attack, 0);
                    shipMoves.Add(move);
                    canAttack = true;
                }
            }
        }

        //ricerca verso alto
        for (int y = position.y; y <= position.y + attackRange; y++)
        {
            if (y >= 0 && y < GridManager.Instance._height && y != position.y)
            {
                Vector2Int pos = new Vector2Int(position.x, y);

                if (GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Obstacle)
                {
                    Move move = new Move(moveId++, shipName, pos, MessageType.attack, 0);
                    shipMoves.Add(move);
                    canAttack = true;
                    break;
                }

                if (GridManager.Instance.GetTileAtPosition(pos)._type == TileType.Enemy
                     && GridManager.Instance.GetTileAtPosition(pos).GetShip() != null)
                {
                    Move move = new Move(moveId++, shipName, pos, MessageType.attack, 0);
                    shipMoves.Add(move);
                    canAttack = true;
                }
            }
        }


        //Se il giocatore ha la libertà di scegliere in quale posizione attaccare, allora questo metodo
        //gli farà vedere solo le posizioni in cui può attaccare.
        shipMoves = shipMoves.Where(x => (GridManager.Instance.GetTileAtPosition(x.GetTargetPos())._type == TileType.Enemy ||
                                    GridManager.Instance.GetTileAtPosition(x.GetTargetPos())._type == TileType.Obstacle)).ToList();
        if (shipMoves.Count > 0)
        {
            foreach (Move move in shipMoves)
            {
                GridManager.Instance.GetTileAtPosition(move.GetTargetPos()).SetTileInteractable(faction, move);
            }
            canAttack = true;
        }
        else
        {
            ByPassEffect();
        }
        return canAttack;

    }

    public override bool LookForAttacks(List<AShip> nearbyShips)
    {
        throw new System.NotImplementedException();
    }
    public override void SendMessage(Move move)
    {
        throw new System.NotImplementedException();
    }

    public void PerformAttack(Vector2 targetPos)
    {
        shipSO.attackEvent.Invoke(new ShipAttackStruct(targetPos, shipSO.attackPower));
    }
    void Update()
    {
        if (startMovement)
        {
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
        throw new System.NotImplementedException();
    }
    public void ReceiveTile(Tile tile)
    {

        if (gameObject.TryGetComponent<CardAllyShip>(out cardAllyScript))
        {
            if (!cardAllyScript.IsSelected()) return;
        }
        
        if (effectSO is AttackInLineEffect)
        {
            if(GridManager.Instance.GetPositionFromTile(tile).x == position.x)
            {
                AttackInLine(true);
            }
            else
            {
                AttackInLine(false);
            }
        }
        else if (tile._type == TileType.Empty)
        {
            Debug.Log("Movement in tile: " + tile.name);
            targetPosition = tile.transform.position;

            StartCoroutine(MoveShip(tile));

        }
        else if (tile._type == TileType.Enemy || tile._type == TileType.Obstacle)
        {
            Debug.Log("Attack in tile: " + tile.name);
            StartCoroutine(Attack(tile));
        }

        //disattivo le tile interagibili
        foreach (Move move in shipMoves)
        {
            GridManager.Instance.GetTileAtPosition(move.GetTargetPos()).SetTileNotInteractable(faction);
        }

        //comunico che l'evento è terminato
        Debug.Log("Effect ended: " + effectSO.name);
        effectSO.EndEffect(0);
    }
    IEnumerator Attack(Tile tile)
    {
        //shipAnimator.Play("Attack");
        //yield return new WaitForSeconds(shipAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
        yield return new WaitForSeconds(0);
        PerformAttack(GridManager.Instance.GetPositionFromTile(tile));
    }

    IEnumerator MoveShip(Tile tile)
    {
        // Impostiamo la destinazione del movimento
        targetPosition = tile.transform.position;
        
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
        GridManager.Instance.MoveShip(this.position, GridManager.Instance.GetPositionFromTile(tile), this.faction);
        this.position = Vector2Int.RoundToInt(GridManager.Instance.GetPositionFromTile(tile));
    }

    private void ByPassEffect()
    {
        transform.DOShakeRotation(0.5f, 10, 10, 90).OnComplete(() =>
        {
            transform.DOKill(true);
        });

        effectSO.EndEffect(0);
    }



    #region Effetti richiamabili dalle carte


    public void AddHealth(int healthToAdd)
    {
        health += healthToAdd;
        displayHealthScript.AddHealth(healthToAdd);
    }

    /// <remark><summary>
    /// The function deals 1 damage to all entities in a square (1 tile) area around the ship.
    /// </summary></remark>
    public void SquareDamage()
    {
        CicloX();
        CicloY();
        CicloDiagonaliQuadrato();
    }

    private void CicloDiagonaliQuadrato()
    {

        //diagonale
        for (int x = (int)position.x - 1, y = (int)position.y - 1;
            x < (int)position.x + 1 && y < (int)position.y + 1;
            x++, y++)
        {
            if (x != (int)position.x && y != (int)position.y)
            {
                shipSO.attackEvent.Invoke(new ShipAttackStruct(new Vector2(x, y), 1));
                InstantiateEffect(new Vector2(x, y));
            }
        }

        //antidiagonale
        for (int x = (int)position.x - 1, y = (int)position.y + 1;
            x < (int)position.x + 1 && y > (int)position.y - 1;
            x++, y--)
        {
            if (x != (int)position.x && y != (int)position.y)
            {
                shipSO.attackEvent.Invoke(new ShipAttackStruct(new Vector2(x, y), 1));
                InstantiateEffect(new Vector2(x, y));
            }
        }
    }

    private void CicloX()
    {
        for (int x = (int)position.x - 1; x < (int)position.x + 1; x++)
        {
            if (x != (int)position.x)
            {
                shipSO.attackEvent.Invoke(new ShipAttackStruct(new Vector2(x, position.y), 1));
                InstantiateEffect(new Vector2(x, (int)position.y));
            }
        }
    }

    private void CicloY()
    {
        for (int y = (int)position.y - 1; y < (int)position.y + 1; y++)
        {
            if (y != (int)position.y)
            {
                shipSO.attackEvent.Invoke(new ShipAttackStruct(new Vector2(position.x, y), 1));
                InstantiateEffect(new Vector2((int)position.x, y));
            }
        }
    }

    private void InstantiateEffect(Vector2 position)
    {
        //Istanziare qui l'effetto visivo dell'attacco
    }


    public bool LookForAttacksWithoutObstacles()
    {

        //quando la invoco, pulisco la lista delle azioni possibili e la riempio con le nuove
        shipMoves.Clear();

        canAttack = false;

        //ricerca verso sx
        for (int x = position.x; x >= position.x - attackRange; x--)
        {
            if (x >= 0 && x < GridManager.Instance._width && x != position.x)
            {
                Vector2Int pos = new Vector2Int(x, position.y);

                if ((GridManager.Instance.GetTileAtPosition(pos)._type == TileType.Enemy ||
                    GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Obstacle)
                     && GridManager.Instance.GetTileAtPosition(pos).GetShip() != null)
                {
                    Move move = new Move(moveId++, shipName, pos, MessageType.attack, 0);
                    shipMoves.Add(move);
                    canAttack = true;
                }
            }
        }

        //ricerca verso dx
        for (int x = position.x; x <= position.x + attackRange; x++)
        {
            if (x >= 0 && x < GridManager.Instance._width && x != position.x)
            {
                Vector2Int pos = new Vector2Int(x, position.y);

                if (GridManager.Instance.GetTileAtPosition(pos)._type == TileType.Enemy
                    || GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Obstacle
                     && GridManager.Instance.GetTileAtPosition(pos).GetShip() != null)
                {
                    Move move = new Move(moveId++, shipName, pos, MessageType.attack, 0);
                    shipMoves.Add(move);
                    canAttack = true;
                }
            }
        }

        //ricerca verso basso
        for (int y = position.y; y >= position.y - attackRange; y--)
        {
            if (y >= 0 && y < GridManager.Instance._height && y != position.y)
            {
                Vector2Int pos = new Vector2Int(position.x, y);

                if ((GridManager.Instance.GetTileAtPosition(pos)._type == TileType.Enemy
                    || GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Obstacle)
                     && GridManager.Instance.GetTileAtPosition(pos).GetShip() != null)
                {
                    Move move = new Move(moveId++, shipName, pos, MessageType.attack, 0);
                    shipMoves.Add(move);
                    canAttack = true;
                }
            }
        }

        //ricerca verso alto
        for (int y = position.y; y <= position.y + attackRange; y++)
        {
            if (y >= 0 && y < GridManager.Instance._height && y != position.y)
            {
                Vector2Int pos = new Vector2Int(position.x, y);

                if ((GridManager.Instance.GetTileAtPosition(pos)._type == TileType.Enemy
                    || GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Obstacle)
                     && GridManager.Instance.GetTileAtPosition(pos).GetShip() != null)
                {
                    Move move = new Move(moveId++, shipName, pos, MessageType.attack, 0);
                    shipMoves.Add(move);
                    canAttack = true;
                }
            }
        }


        //Se il giocatore ha la libertà di scegliere in quale posizione attaccare, allora questo metodo
        //gli farà vedere solo le posizioni in cui può attaccare.
        shipMoves = shipMoves.Where(x => GridManager.Instance.GetTileAtPosition(x.GetTargetPos())._type == TileType.Enemy ||
                                    GridManager.Instance.GetTileAtPosition(x.GetTargetPos())._type == TileType.Obstacle).ToList();
        if (shipMoves.Count > 0)
        {
            foreach (Move move in shipMoves)
            {
                GridManager.Instance.GetTileAtPosition(move.GetTargetPos()).SetTileInteractable(faction, move);
            }
            canAttack = true;
        }
        else
        {
            ByPassEffect();
        }
        return canAttack;
    }



    public bool LookForAttackInLine()
    {
        //quando la invoco, pulisco la lista delle azioni possibili e la riempio con le nuove
        shipMoves.Clear();

        int gridHeight = GridManager.Instance._height;
        int gridWidth = GridManager.Instance._width;

        for (int x = 0; x < gridWidth; x++)
        {
            //tengo y fissa
            if (x != position.x)
            {
                Vector2Int pos = new Vector2Int(x, position.y);
                Move move = new Move(moveId++, shipName, pos, MessageType.attack, 0);
                shipMoves.Add(move);
                canAttack = true;
            }
        }

        for (int y = 0; y < gridHeight; y++)
        {
            //tengo x fissa
            if (y != position.y)
            {
                Vector2Int pos = new Vector2Int(position.x, y);
                Move move = new Move(moveId++, shipName, pos, MessageType.attack, 0);
                shipMoves.Add(move);
                canAttack = true;
            }
        }    

        shipMoves = shipMoves.Where(x => GridManager.Instance.GetTileAtPosition(x.GetTargetPos())).ToList();
        if (shipMoves.Count > 0)
        {
            foreach (Move move in shipMoves)
            {
                GridManager.Instance.GetTileAtPosition(move.GetTargetPos()).SetTileInteractable(faction, move);
            }
            canAttack = true;
        }
        else
        {
            ByPassEffect();
        }
        return canAttack;
    }

    private void AttackInLine(bool isRow)
    {
        if (isRow)
        {
            for (int x = 0; x < GridManager.Instance._width; x++)
            {
                if (x != position.x)
                {
                    shipSO.attackEvent.Invoke(new ShipAttackStruct(new Vector2(x, position.y), 1));
                    InstantiateEffect(new Vector2(x, position.y));
                }
            }
        }
        else
        {
            for (int y = 0; y < GridManager.Instance._height; y++)
            {
                if (y != position.y)
                {
                    shipSO.attackEvent.Invoke(new ShipAttackStruct(new Vector2(position.x, y), 1));
                    InstantiateEffect(new Vector2(position.x, y));
                }
            }

        }
    }


    #endregion
}
