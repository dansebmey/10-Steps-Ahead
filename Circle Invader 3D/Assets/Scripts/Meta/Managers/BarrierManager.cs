using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class BarrierManager : MonoBehaviour, IResetOnGameStart
{
    [Range(2,32)] public int amountOfBarriers;
    [HideInInspector] public float barrierDistanceFromCenter;
    
    [Header("Barrier variables")]
    [SerializeField] private Barrier barrierPrefab;
    [Range(1,10)][SerializeField] public int initBarrierHealth = 3;
    [Range(1,10)][SerializeField] public int maxBarrierHealth = 4;
    [SerializeField] public Color[] healthColours;
    [Range(10, 100)] public int collapsedTurnCount = 30;
    
    [SerializeField] private int initiallyDestroyedBarriers = 0;

    public Barrier[] Barriers { get; private set; }
    public int InitBarrierHealth => amountOfBarriers * initBarrierHealth;

    public int CurrentBarrierHealth
    {
        get
        {
            int total = 0;
            foreach (Barrier bar in Barriers)
            {
                total += bar.Health;
            }

            return total;
        } 
    }

    private GameManager _gm;

    private void Awake()
    {
        _gm = GetComponentInParent<GameManager>();
    }

    protected void Start()
    {
        InitialiseBarriers();
    }

    public void OnNewGameStart()
    {
        foreach (Barrier bar in Barriers)
        {
            bar.IsCollapsed = false;
            bar.Health = initBarrierHealth;
        }
    }

    private void InitialiseBarriers()
    {
        Barriers = new Barrier[amountOfBarriers];

        barrierDistanceFromCenter = 2 + 0.0625f * amountOfBarriers;
        for (int i = 0; i < amountOfBarriers; i++)
        {
            Vector3 pos = new Vector3(
                barrierDistanceFromCenter * Mathf.Cos((Mathf.PI * 2 / amountOfBarriers) * i),
                0, 
                barrierDistanceFromCenter * Mathf.Sin((Mathf.PI * 2 / amountOfBarriers) * i));
            Quaternion rot = Quaternion.LookRotation(transform.position - pos);

            var bar = Instantiate(barrierPrefab, pos, rot);
            bar.CurrentPosIndex = i;
            bar.name = "Barrier (index " + i + ")";
            bar.transform.parent = transform;
            Barriers[i] = bar;
        }

        if (initiallyDestroyedBarriers > 0)
        {
            // CollapseBarriers(initiallyDestroyedBarriers);
        }
    }

    private void CollapseBarriers(int amount)
    {
        List<int> rns = new List<int>();
        
        for (int i = 0; i < amount; i++)
        {
            int rn;
            rns.Add(rn = GenerateRandomUniqueInt(rns));
            Barriers[rn].Health -= initBarrierHealth;
            Debug.Log(Barriers[rn].name + " collapsed!");
        }
    }

    private int GenerateRandomUniqueInt(List<int> rns)
    {
        int rn = Random.Range(0, amountOfBarriers-1);
        return rns.Contains(rn) ? GenerateRandomUniqueInt(rns) : rn;
    }

    public void DamageBarrier(int damageDealt, int posIndex)
    {
        foreach (Barrier bar in Barriers)
        {
            if (bar.CurrentPosIndex % amountOfBarriers == posIndex)
            {
                bar.TakeDamage(damageDealt);
            }
        }
    }

    public bool IsBarrierCollapsed(int posIndex)
    {
        foreach (Barrier bar in Barriers)
        {
            if (bar.CurrentPosIndex % amountOfBarriers == posIndex)
            {
                return bar.IsCollapsed;
            }
        }
        
        Debug.LogError("No barrier was found with posIndex ["+posIndex+"]");
        return false;
    }

    public void RepairBarriers(int range, int healValue)
    {
        for (int i = _gm.CurrentPosIndex - range; i <= _gm.CurrentPosIndex + range; i++)
        {
            Barrier bar = Barriers[(Barriers.Length+i) % Barriers.Length];
            if (!bar.IsCollapsed)
            {
                bar.RestoreHealth(healValue);
            }
        }
    }

    public void RepairAllBarriers(int healValue)
    {
        StartCoroutine(_RepairAllBarriers(healValue));
    }

    private IEnumerator _RepairAllBarriers(int healValue)
    {
        int range = 0;
        while (true)
        {
            _gm.AudioManager.PlayPitched("HammerFix", 1 + range * 0.1f, 0.05f);
            Barrier left = Barriers[(amountOfBarriers + _gm.CurrentPosIndex-range) % amountOfBarriers];
            Barrier right = Barriers[(amountOfBarriers + _gm.CurrentPosIndex + range) % amountOfBarriers];

            left.IsCollapsed = false;
            left.RestoreHealth(healValue);
            if (left != right)
            {
                right.IsCollapsed = false;
                right.RestoreHealth(healValue);
            }
            else if (range > 0)
            {
                break;
            }

            range++;
            yield return new WaitForSeconds(0.075f);
        }
    }

    public int DetermineRemainingBarrierHealth()
    {
        return Barriers.Sum(bar => bar.Health);
    }
    
    #region OnGameLoad

    public void OnGameLoad(GameData gameData)
    {
        BarrierManagerData bmData = gameData.bmData;
        foreach (BarrierManagerData.BarrierData barrierData in bmData.barriers)
        {
            Barrier bar = Barriers[barrierData.posIndex];
            bar.Health = barrierData.health;
            
            bar.IsCollapsed = barrierData.isCollapsed;
            if (bar.IsCollapsed)
            {
                bar.transform.position = new Vector3(bar.transform.position.x, -0.8f, bar.transform.position.z);
            }
        }
    }
    
    #endregion
}
