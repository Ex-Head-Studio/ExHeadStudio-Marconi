using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Animations;
using Unity.Mathematics;

[CreateAssetMenu(fileName = "ShipSO", menuName = "Scriptable Objects/ShipSO")]
public class ShipSO : ScriptableObject
{
    [SerializeField] public string className;
    [SerializeField] public int movementRange;
    [SerializeField] public int attackRange;
    [SerializeField] public int attackPower;
    [SerializeField] public int health;
    public string[] statNames = { "Movement Range", "Attack Range", "Attack Power", "Health" };
    // per ogni variabile aggiunta allo scriptable object, aggiungere un nome alla lista statNames

    [Header("Events")]
    [SerializeField] public MessageSentEvent messageSentEvent;
    [SerializeField] public OnShipDestroyedEvent shipDestroyedEvent;
    [SerializeField] public OnShipAttackEvent attackEvent;

    /*[Header("Animator")]
    [SerializeField] public RuntimeAnimatorController shipAnimatorController;*/

    [Header("Effects")]
    [SerializeField] public ParticleSystem attackReceivedParticle;
    [SerializeField] public ParticleSystem changeClassParticle = null;
    [SerializeField] public ParticleSystem attackParticle = null;
    [SerializeField] public ParticleSystem dodgeEffect = null;

    public Dictionary<string, int> statsDictionary = new Dictionary<string, int>();
    public float shipInfluence;

    [SerializeField] public GameObject shipModelPrefab;
    [SerializeField] public GameObject shipClassModel;
    [SerializeField] public GameObject shipModelMesh;

    [Header("Ship Class Image")]
    [SerializeField] public Sprite shipClassImage = null;

    [Header("UI Visualization")]
    [Tooltip("Offset di posizione per il modello wireframe nell'UI")]
    public Vector3 uiRepositionOffset = new Vector3(0, -50, 0);
    [Tooltip("Fattore di scala per il modello wireframe nell'UI")]
    public Vector3 uiScaleFactor = new Vector3(40, 40, 40);


    private void OnEnable()
    {
        statsDictionary["Movement Range"] = movementRange;
        statsDictionary["Attack Range"] = attackRange;
        statsDictionary["Attack Power"] = attackPower;
        statsDictionary["Health"] = health;
    }
    private void OnDisable()
    {
        statsDictionary.Clear();
    }
}
