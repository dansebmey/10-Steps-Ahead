using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

public class MechaTotem : MovableObject, IPlayerCommandListener
{
    private const int ACTION_IDLE = 0;
    private const int ACTION_ATTACK = 1;
    private const int ACTION_SPLIT_ATTACK = 2;
    private const int ACTION_DELAYED_ATTACK = 3;
    
    public LinkedList<EnemyAction> queuedActions;
    private LinkedList<TotemLayer> _totemLayers;

    public int amtOfVisibleActions = 10;

    [Header("Prefabs")]
    [SerializeField] private List<EnemyAction> actionPrefabs;
    
    [Header("Delayed projectiles")]
    [SerializeField] public DelayedProjectile delayedProjPrefab;
    private ConcurrentQueue<DelayedProjectile> _delayedProjectilesInGame;
    
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
        actionPrefabs[3].action = DelayedAttack;
        
        QueueNewAction(ACTION_IDLE);
        QueueNewAction(ACTION_IDLE);
        QueueNewAction(ACTION_IDLE);
        QueueNewAction(ACTION_ATTACK);
        QueueNewAction(ACTION_IDLE);
        QueueNewAction(ACTION_IDLE);
        QueueNewAction(ACTION_IDLE);
        QueueNewAction(ACTION_ATTACK);
        QueueNewAction(ACTION_IDLE);
        QueueNewAction(ACTION_DELAYED_ATTACK);
        for (var i = 0; i < amtOfVisibleActions - 10; i++)
        {
            QueueNewAction(ACTION_IDLE);
        }
        
        _delayedProjectilesInGame = new ConcurrentQueue<DelayedProjectile>();
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

    private void Idle()
    {
        Gm.SwitchState(typeof(WaitingForPlayerAction));
    }

    private void BasicAttack()
    {
        Gm.AudioManager.Play("BasicAttack", 0.05f);
        Gm.ApplyDamage(1);
    }

    private void SplitAttack()
    {
        Gm.AudioManager.Play("SplitAttack", 0.05f);
        Gm.ApplyDamage(1, Gm.CurrentPosIndex-1);
        Gm.ApplyDamage(1, Gm.CurrentPosIndex+1);
    }

    private void DelayedAttack()
    {
        DelayedProjectile proj = Instantiate(delayedProjPrefab, transform);
        proj.CurrentPosIndex = Gm.CurrentPosIndex;
        _delayedProjectilesInGame.Enqueue(proj);

        Gm.AudioManager.Play("DelayedAttackFired", 0.05f);
        Gm.SwitchState(typeof(WaitingForPlayerAction));
        // TODO: State switching should be done somewhere more central
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
        List<EnemyAction> actions = DetermineAvailableActions();
        actions.Sort(new ActionSorter());
        
        int totalWeight = CalculateTotalWeight(actions);
        int cumulativeWeight = 0;
        int newActionIndex = -1;
        
        int rn = UnityEngine.Random.Range(0, totalWeight);
        foreach (EnemyAction action in actions)
        {
            cumulativeWeight += action.chance;
            if (rn <= cumulativeWeight)
            {
                // Debug.Log("Action ["+action.name+"] was chosen because rn was ["+rn+"/"+totalWeight+"] and cumulativeWeight was ["+cumulativeWeight+"]");
                newActionIndex = actionPrefabs.IndexOf(action);
                break;
            }
        }

        QueueNewAction(newActionIndex);
    }

    private int CalculateTotalWeight(List<EnemyAction> actions)
    {
        int total = 0;
        foreach (EnemyAction action in actions)
        {
            total += action.chance;
        }

        return total;
    }

    private List<EnemyAction> DetermineAvailableActions()
    {
        List<EnemyAction> result = new List<EnemyAction>();
        foreach (EnemyAction action in actionPrefabs)
        {
            if (Gm.IsScoreHigherThan(action.scoreReq))
            {
                result.Add(action);
            }
        }

        return result;
    }

    public void OnPlayerCommandPerformed()
    {
        foreach (DelayedProjectile proj in _delayedProjectilesInGame)
        {
            DelayedProjectile thisProj = proj;
            if (!thisProj.MoveForward())
            {
                _delayedProjectilesInGame.TryDequeue(out thisProj);
            }
        }
    }
}

class ActionSorter : IComparer<EnemyAction> 
{
    public int Compare(EnemyAction a, EnemyAction b)
    {
        if (a.chance < b.chance)
        {
            return -1;
        }
        return b.chance > a.chance ? 1 : 0;
    }
} 