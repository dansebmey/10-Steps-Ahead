using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechaTotem : CIObject
{
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
        for (var i = 0; i < amtOfVisibleActions - 10; i++)
        {
            QueueNewAction(actionPrefabs[0]);
        }
    }

    private void QueueNewAction(EnemyAction action)
    {
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
        
        EnemyAction newAction = actionPrefabs[0];

        int rn = UnityEngine.Random.Range(0, 30);
        if (rn > 20)
        {
            newAction = actionPrefabs[1];
        }

        QueueNewAction(newAction);
    }
}
