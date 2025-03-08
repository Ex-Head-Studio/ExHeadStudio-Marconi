using UnityEngine;

public class Ship : MonoBehaviour
{
    enum ShipState{
        Waiting,
        Moving,
        Shooting
    }
    string name;
    Vector2 position;
    Vector2 nextPos;
    ShipState currentState;

    public Ship(string name){
        this.name=name;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetupMessage(){

    }
    void SendMessage(){
        //TODO: La nave manda un messaggio al giocatore per dirgli cosa intende fare
    }

    void ReceiveInstructions(){
        //TODO: La nava manda il messaggio e aspetta istruzioni su cosa fare
    }

    void ExecuteInstructions(){
        
    }

    void SetNextPosition(Vector2 newPos){
        nextPos=newPos;
    }
}
