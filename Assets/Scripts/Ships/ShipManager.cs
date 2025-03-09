using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random=UnityEngine.Random;
public class ShipManager : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] float attackRange;
    [SerializeField] int movementRange;
    [SerializeField] int shipsToSelect;
    public List<String> shipNames=new List<String>();
    private List<Ship>enemies;

    private List<Ship>allies;
    private List<Ship> allyAttackers;
    private List<Ship> enemyAttackers;

    List<Ship> ships;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake(){
        
        ships = new List<Ship>();
        shipNames.OrderBy(x => Random.value);
        foreach (string shipName in shipNames){
            ships.Add(new Ship(shipName));
        }
    }
    void Start()
    {
        int listIndex = Random.Range(0, shipNames.Count-1);
        for(int i=0; i<shipNames.Count;i++){
            if(listIndex==shipNames.Count){
                listIndex=0;
            }
            if(i<shipNames.Count/2){
                allies.Add(ships[listIndex]);
            }
            else{
                enemies.Add(ships[listIndex]);
            }
            InstantiateInMap(ships[listIndex]);
            listIndex++;
        }
    }

    public void ChooseShips(){
        //Seleziona le navi che possono attaccare e decidi tra loro chi attaccherà
        List<Ship> selectedAllies = allies.Where(x => x.LookForObjectives(enemies)==true).ToList();
        List<Ship> selectedEnemies = enemies.Where(x => x.LookForObjectives(allies)==true).ToList();
        foreach(Ship ship in ships){
            ship.LookForMovement(ships);
        }
        if(selectedAllies.Count>0){
            selectedAllies.OrderBy(x => Random.value);
            for(int i=0; i<Random.Range(1, shipsToSelect);i++){
                allyAttackers.Add(selectedAllies[i]);
            }
        }
        else{
            allies.OrderBy(x => Random.value);
            for(int i=0; i<Random.Range(1, shipsToSelect);i++){
                allies[i].SetState(Ship.ShipState.Moving);
            }
        }

        
        if(selectedEnemies.Count>0){
            selectedEnemies.OrderBy(x => Random.value);
            for(int i=0; i<Random.Range(1, shipsToSelect);i++){
                enemyAttackers.Add(selectedEnemies[i]);
            }
        }
        else{
            allies.OrderBy(x => Random.value);
            for(int i=0; i<Random.Range(1, shipsToSelect);i++){
                allies[i].SetState(Ship.ShipState.Moving);
            }
        }
        
    }

    void CallAttack(){
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    void InstantiateInMap(Ship ship){
        //TODO: istanzia la nave nella mappa, di Beto
    }
    void OrderMovement(){

    }
}
