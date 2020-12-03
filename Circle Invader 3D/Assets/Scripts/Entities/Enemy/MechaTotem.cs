using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechaTotem : MovableObject
{
    private const int ACTION_IDLE = 0;
    private const int ACTION_ATTACK = 1;
    private const int ACTION_SPLIT_ATTACK = 2;
    
    public LinkedList<EnemyAction> queuedActions;
    private LinkedList<TotemLayer> _totemLayers;

    public int amtOfVisibleActions = 10;

    [SerializeField] private List<EnemyAction> actionPrefabs;

    // Start is called before the first frame update
    protected override void Start()
    {
        Gm.enemy = this;
        
        queuedActions = new LinkedList<EnemyAction>();
        _totemLayers = new LinkedList<TotemLayer>();

        actionPrefabs[0].action = Idle;
        actionPrefabs[1].action = BasicAttack;
        
        QueueNewAction(ACTION_IDLE);
        QueueNewAction(ACTION_IDLE);
        QueueNewAction(ACTION_IDLE);
        QueueNewAction(ACTION_ATTACK);
        QueueNewAction(ACTION_IDLE);
        QueueNewAction(ACTION_IDLE);
        QueueNewAction(ACTION_IDLE);
        QueueNewAction(ACTION_ATTACK);
        QueueNewAction(ACTION_IDLE);
        QueueNewAction(ACTION_ATTACK);
        for (var i = 0; i < amtOfVisibleActions - 10; i++)
        {
            QueueNewAction(ACTION_IDLE);
        }
    }

    private void QueueNewAction(int actionIndex)
    {
        EnemyAction action = actionPrefabs[actionIndex];
        
        TotemLayer layer = Instantiate(
            action.layerPrefab,
            new Vector3(0, 1.025f * queuedActions.Count, 0),
            transform.rotation);
        layer.transform.parent = transform;
        
        _totemLayers.AddLast(layer);
        queuedActions.AddLast(action);
    }

    void Idle()
    {
        Gm.SwitchState(typeof(WaitingForPlayerAction));
    }

    void BasicAttack()
    {
        Gm.ApplyDamage(1);
    }

    public void InvokeNextAction()
    {
        queuedActions.First.Value.action.Invoke();
        
        Destroy(_totemLayers.First.Value.gameObject);
        _totemLayers.RemoveFirst();

        queuedActions.RemoveFirst();
        QueueRandomAction();
        
        MoveTotemLayersDown();
    }

    private void MoveTotemLayersDown()
    {
        List<TotemLayer> layers = new List<TotemLayer>(_totemLayers);
        for (int i = 0; i < layers.Count; i++)
        { 
            layers[i].targetPos = new Vector3(0, 1.025f * i, 0);
        }
    }

    private void QueueRandomAction()
    {
        
        int newActionIndex = ACTION_IDLE;

        int rn = UnityEngine.Random.Range(0, 30);
        if (rn > 20)
        {
            newActionIndex = ACTION_ATTACK;
        }

        QueueNewAction(newActionIndex);
    }
}
