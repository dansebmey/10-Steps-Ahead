using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : MovableObject, IPlayerCommandListener, IResetOnGameStart
{
    private const int ACTION_IDLE = 0;
    private const int ACTION_ATTACK = 1;
    private const int ACTION_SPLIT_ATTACK = 2;
    private const int ACTION_DELAYED_ATTACK = 3;
    
    public LinkedList<EnemyAction> queuedActions;
    private LinkedList<EnemyLayer> _totemLayers;

    [Header("Prefabs")]
    [SerializeField] private List<EnemyAction> actionPrefabs;
    [SerializeField] public Missile delayedProjPrefab;
    [SerializeField] public FieldItem minePrefab;
    public ConcurrentQueue<Missile> MissilesInField { get; private set; }
    
    public float layerHeight = 0.5f;

    protected override void Awake()
    {
        base.Awake();
        
        Gm.enemy = this;
        
        actionPrefabs[0].action = Idle;
        actionPrefabs[1].action = BasicAttack;
        actionPrefabs[2].action = SplitAttack;
        actionPrefabs[3].action = DelayedAttack;
        actionPrefabs[4].action = TwinAttack;
        actionPrefabs[5].action = MineAttack;
        
        InitActionQueue();
        
        MissilesInField = new ConcurrentQueue<Missile>();
    }

    private void InitActionQueue()
    {
        queuedActions = new LinkedList<EnemyAction>();
        _totemLayers = new LinkedList<EnemyLayer>();
        
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
        
        MoveTotemLayersDown();
    }

    private void QueueNewAction(int actionIndex, bool addFirst = false)
    {
        QueueNewAction(actionPrefabs[actionIndex]);
    }

    private void QueueNewAction(EnemyAction enemyAction, bool addFirst = false)
    {
        EnemyLayer layer = Instantiate(
            enemyAction.layerPrefab,
            new Vector3(0, (layerHeight + 0.025f) * queuedActions.Count, 0),
            transform.rotation);
        layer.transform.parent = transform;
        
        enemyAction.remainingCooldown = enemyAction.cooldownInTurns;
        
        if (!addFirst)
        {
            _totemLayers.AddLast(layer);
            queuedActions.AddLast(enemyAction);
        }
        else
        {
            _totemLayers.AddFirst(layer);
            queuedActions.AddFirst(enemyAction);
        }

        MoveTotemLayersDown();
    }

    private void Idle()
    {
        Gm.SwitchState(typeof(WaitingForPlayerActionState));
    }

    private void BasicAttack()
    {
        Gm.AudioManager.Play("BasicAttack");
        Gm.ApplyDamage(1);
    }
    
    private void TwinAttack()
    {
        Gm.AudioManager.Play("BasicAttack");
        Gm.ApplyDamage(2);
    }
    
    private void SplitAttack()
    {
        Gm.AudioManager.Play("SplitAttack");
        Gm.ApplyDamage(1, Gm.CurrentPosIndex-1);
        Gm.ApplyDamage(1, Gm.CurrentPosIndex+1);
    }

    private void DelayedAttack()
    {
        Missile proj = Instantiate(delayedProjPrefab, transform);
        proj.CurrentPosIndex = Gm.CurrentPosIndex;
        MissilesInField.Enqueue(proj);

        Gm.AudioManager.Play("DelayedAttackFired");
        Gm.SwitchState(typeof(WaitingForPlayerActionState));
        // TODO: State switching should be done somewhere more central
    }

    private void MineAttack()
    {
        FieldItemManager fim = Gm.FieldItemManager;
        FieldItem mine;
        
        // Spawn mine on the left side
        int targetPosIndex = Gm.WrapPosIndex(Gm.CurrentPosIndex - (Gm.BarrierManager.amountOfBarriers / 4));
        if (!fim.IsBarrierOccupied(targetPosIndex))
        {
            mine = Instantiate(minePrefab, Vector3.zero, Quaternion.identity);
            mine.distanceFromCenter = Gm.BarrierManager.barrierDistanceFromCenter + 0.75f;
            mine.CurrentPosIndex = targetPosIndex;
            mine.transform.position = mine.targetPos;
        }
        
        // Spawn mine on the right side
        targetPosIndex = Gm.WrapPosIndex(Gm.CurrentPosIndex + (Gm.BarrierManager.amountOfBarriers / 4));
        if (!fim.IsBarrierOccupied(targetPosIndex))
        {
            mine = Instantiate(minePrefab, Vector3.zero, Quaternion.identity);
            mine.distanceFromCenter = Gm.BarrierManager.barrierDistanceFromCenter + 0.75f;
            mine.CurrentPosIndex = targetPosIndex;
            mine.transform.position = mine.targetPos;
        }
        
        Gm.AudioManager.Play("MinePlaced");
        Gm.SwitchState(typeof(WaitingForPlayerActionState));
    }

    public void InvokeNextAction()
    {
        queuedActions.First.Value.action.Invoke();

        _totemLayers.First.Value.animator.Play("layer-despawn");
        // Destroy(_totemLayers.First.Value.gameObject);
        _totemLayers.RemoveFirst();

        queuedActions.RemoveFirst();
        QueueRandomAction();
    }

    private void MoveTotemLayersDown()
    {
        List<EnemyLayer> layers = new List<EnemyLayer>(_totemLayers);
        for (int i = 0; i < layers.Count; i++)
        { 
            layers[i].targetPos = new Vector3(0, (layerHeight + 0.025f) * i, 0);
            
            Quaternion targetRot = Quaternion.LookRotation(Gm.player.targetPos - transform.position);
            targetRot.x = 0;
            targetRot.z = 0;
            layers[i].targetRot = targetRot;
        }
    }

    private void QueueRandomAction()
    {
        List<EnemyAction> actions = DetermineAvailableActions();
        actions.Sort(new ActionSorter());
        
        int totalWeight = CalculateTotalWeight(actions);
        int cumulativeWeight = 0;
        int newActionIndex = -1;
        
        int rn = Random.Range(0, totalWeight);
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
            if (Gm.IsScoreHigherThan(action.scoreReq) && action.remainingCooldown <= 0)
            {
                result.Add(action);
            }
        }

        return result;
    }

    public void OnPlayerCommandPerformed(KeyCode keyCode)
    {
        foreach (EnemyAction action in actionPrefabs)
        {
            action.remainingCooldown--;
        }
        
        if (MissilesInField != null)
            // dirty fix for bug where totem doesn't respond to player action (in build)
        {
            foreach (Missile proj in MissilesInField)
            {
                Missile thisProj = proj;
                if (!thisProj.MoveForward())
                {
                    MissilesInField.TryDequeue(out thisProj);
                }
            }
        }
    }
    public void SilenceLayers(int amount)
    {
        ClearMissilesInField();
        
        for (int i = 0; i < amount; i++)
        {
            // TODO: Replace all layers with idle layers
            
            Destroy(_totemLayers.First.Value.gameObject);
            _totemLayers.RemoveFirst();

            queuedActions.RemoveFirst();
            QueueNewAction(ACTION_IDLE, true);
        }
        MoveTotemLayersDown();
    }
    
    public void OnNewGameStart()
    {
        ClearMissilesInField();
        
        foreach (EnemyLayer layer in _totemLayers)
        {
            Destroy(layer.gameObject);
        }
        InitActionQueue();
    }

    private void ClearMissilesInField()
    {
        foreach (Missile missile in MissilesInField)
        {
            Destroy(missile.gameObject);
        }
        MissilesInField = new ConcurrentQueue<Missile>();
    }

    #region OnGameLoad
    
    public void OnGameLoad(GameData gameData)
    {
        EnemyData enemyData = gameData.enemyData;

        foreach (EnemyLayer layer in _totemLayers)
        {
            Destroy(layer.gameObject);
        }
        _totemLayers = new LinkedList<EnemyLayer>();
        
        queuedActions = new LinkedList<EnemyAction>();
        foreach (string enemyActionName in enemyData.actionQueue)
        {
            QueueNewAction(FindEnemyActionByName(enemyActionName));
        }

        foreach (EnemyData.MissileData missileData in enemyData.missiles)
        {
            Missile missile = Instantiate(delayedProjPrefab, transform);
            missile.CurrentPosIndex = missileData.posIndex;
            missile.SetStepsTaken(missileData.stepsTaken);
            MissilesInField.Enqueue(missile);
        }
    }

    private EnemyAction FindEnemyActionByName(string enemyActionName)
    {
        foreach (EnemyAction actionPrefab in actionPrefabs)
        {
            if (actionPrefab.actionName == enemyActionName)
            {
                return actionPrefab;
            }
        }

        return null;
    }
    
    #endregion
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