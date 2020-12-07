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

    public float layerHeight = 0.5f;

    // Start is called before the first frame update
    protected override void Start()
    {
        Gm.enemy = this;
        
        queuedActions = new LinkedList<EnemyAction>();
        _totemLayers = new LinkedList<TotemLayer>();

        actionPrefabs[0].action = Idle;
        actionPrefabs[1].action = BasicAttack;
        actionPrefabs[2].action = SplitAttack;
        
        QueueNewAction(ACTION_IDLE);
        QueueNewAction(ACTION_IDLE);
        QueueNewAction(ACTION_IDLE);
        QueueNewAction(ACTION_ATTACK);
        QueueNewAction(ACTION_IDLE);
        QueueNewAction(ACTION_IDLE);
        QueueNewAction(ACTION_IDLE);
        QueueNewAction(ACTION_ATTACK);
        QueueNewAction(ACTION_IDLE);
        QueueNewAction(ACTION_SPLIT_ATTACK);
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
            new Vector3(0, (layerHeight + 0.025f) * (queuedActions.Count + 2), 0),
            transform.rotation);
        layer.transform.parent = transform;
        
        _totemLayers.AddLast(layer);
        queuedActions.AddLast(action);
        MoveTotemLayersDown();
    }

    void Idle()
    {
        Gm.SwitchState(typeof(WaitingForPlayerAction));
    }

    void BasicAttack()
    {
        Gm.ApplyDamage(1);
    }

    void SplitAttack()
    {
        Gm.ApplyDamage(1, Gm.CurrentPosIndex-1);
        Gm.ApplyDamage(1, Gm.CurrentPosIndex+1);
    }

    public void InvokeNextAction()
    {
        queuedActions.First.Value.action.Invoke();
        
        Destroy(_totemLayers.First.Value.gameObject);
        _totemLayers.RemoveFirst();

        queuedActions.RemoveFirst();
        QueueRandomAction();
    }

    private void MoveTotemLayersDown()
    {
        List<TotemLayer> layers = new List<TotemLayer>(_totemLayers);
        for (int i = 0; i < layers.Count; i++)
        { 
            layers[i].targetPos = new Vector3(0, (layerHeight + 0.025f) * i, 0);
            if (i >= 0)
            {
                Quaternion targetRot = Quaternion.LookRotation(Gm.player.targetPos - transform.position);
                targetRot.x = 0;
                targetRot.z = 0;
                layers[i].targetRot = targetRot;
            }
        }
    }

    private void QueueRandomAction()
    {
        int newActionIndex = ACTION_IDLE;

        int rn = UnityEngine.Random.Range(0, 30);
        if (rn > 27)
        {
            newActionIndex = ACTION_SPLIT_ATTACK;
        } 
        else if (rn > 20)
        {
            newActionIndex = ACTION_ATTACK;
        }

        QueueNewAction(newActionIndex);
    }
}
