using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechaTotem : MonoBehaviour
{
    public LinkedList<EnemyAction> queuedActions;
    private LinkedList<Transform> _totemLayers;
    
    public int amtOfVisibleActions = 10;
    private GameManager _gameManager;

    [SerializeField] private List<EnemyAction> actionPrefabs;


    private void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        queuedActions = new LinkedList<EnemyAction>();
        _totemLayers = new LinkedList<Transform>();
        
        QueueNewAction(actionPrefabs[0]);
        QueueNewAction(actionPrefabs[0]);
        QueueNewAction(actionPrefabs[0]);
        QueueNewAction(actionPrefabs[1]);
        QueueNewAction(actionPrefabs[0]);
        QueueNewAction(actionPrefabs[0]);
        QueueNewAction(actionPrefabs[0]);
        QueueNewAction(actionPrefabs[1]);
        QueueNewAction(actionPrefabs[0]);
        QueueNewAction(actionPrefabs[1]);
    }

    private void QueueNewAction(EnemyAction action)
    {
        Transform obj = Instantiate(
            action.layerPrefab,
            new Vector3(0, 1.025f * queuedActions.Count, 0),
            transform.rotation);
        obj.parent = transform;
        
        queuedActions.AddLast(action);
    }

    void Idle()
    {
        // nothing happens
    }

    void BasicAttack()
    {
        _gameManager.BarrierManager.DamageBarrier(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
