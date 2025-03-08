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
    private List<Ship>allies;
    private List<Ship>enemies;

    private Ship allyAttacker;
    private Ship enemyAttacker;

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

    void ChooseShips(){
        for(int i=0; i<shipsToSelect; i++){
            
            int index=Random.Range(0,allies.Count-1);
            
        }
        
    }

    void CallAttack(){
        allyAttacker.SetupMessage();
        enemyAttacker.SetupMessage();
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    void InstantiateInMap(Ship ship){

    }
    void OrderMovement(){

    }
}
