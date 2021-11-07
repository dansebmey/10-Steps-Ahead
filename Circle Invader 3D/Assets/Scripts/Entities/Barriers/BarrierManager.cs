using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BarrierManager : GmAwareObject, IResetOnGameStart
{
    [Range(2,32)] public int amountOfBarriers;
    [HideInInspector] public float barrierDistanceFromCenter;
    
    [Header("Barrier variables")]
    [SerializeField] private Barrier barrierPrefab;
    [Range(1,10)][SerializeField] public int initBarrierHealth = 3;
    [Range(1,10)][SerializeField] public int maxBarrierHealth = 4;
    [SerializeField] public Color[] healthColours;
    
    [SerializeField] private int initiallyDestroyedBarriers = 0;

    [Header("Barrier health indicators")]
    private int _initTotalBarrierHealth;
    [SerializeField] private Image barrierHealthIcon;
    private Animator barrierHealthAnimator;
    [HideInInspector] public int remainingBarrierHealth;
    [SerializeField] private Text remainingBarrierHealthText;
    [SerializeField] public DamageIndicator damageIndicator;

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

    protected override void Awake()
    {
        base.Awake();
        barrierHealthAnimator = barrierHealthIcon.GetComponent<Animator>();
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
        
        UpdateRemainingBarrierHealth(true);
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
            CollapseBarriers(initiallyDestroyedBarriers);
        }

        UpdateRemainingBarrierHealth(true);
        _initTotalBarrierHealth = remainingBarrierHealth;
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

    public int DamageBarrier(int damageDealt, int posIndex)
    {
        foreach (Barrier bar in Barriers)
        {
            if (bar.CurrentPosIndex % amountOfBarriers == posIndex)
            {
                return bar.TakeDamage(damageDealt);
            }
        }

        return 0;
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

    public int RepairBarriers(int range, int healValue)
    {
        int totalHPRestored = 0;
        for (int i = Gm.CurrentPosIndex - range; i <= Gm.CurrentPosIndex + range; i++)
        {
            Barrier bar = Barriers[(Barriers.Length+i) % Barriers.Length];
            if (!bar.IsCollapsed)
            {
                totalHPRestored += bar.RestoreHealth(healValue);
            }
        }

        return totalHPRestored;
    }

    public void RepairAllBarriers(int healValue)
    {
        StartCoroutine(_RepairAllBarriers(healValue));
    }

    private IEnumerator _RepairAllBarriers(int healValue)
    {
        int range = 0;
        int totalHPRestored = 0;
        
        while (true)
        {
            Gm.AudioManager.PlayPitched("HammerFix", 1 + range * 0.1f, 0.05f);
            Barrier left = Barriers[(amountOfBarriers + Gm.CurrentPosIndex-range) % amountOfBarriers];
            Barrier right = Barriers[(amountOfBarriers + Gm.CurrentPosIndex + range) % amountOfBarriers];

            left.IsCollapsed = false;
            totalHPRestored += left.RestoreHealth(healValue);
            if (left != right)
            {
                right.IsCollapsed = false;
                totalHPRestored += right.RestoreHealth(healValue);
            }
            else if (range > 0)
            {
                break;
            }

            range++;
            yield return new WaitForSeconds(0.075f);
        }
        
        if (totalHPRestored == 20)
        {
            Gm.AudioManager.Play("PerfectPerf");
            EventManager<AchievementManager.AchievementType, int>
                .Invoke(EventType.IncrementAchievementProgress, AchievementManager.AchievementType.OptimalMultiHammerUse, 1);
        }
        else
        {
            EventManager<AchievementManager.AchievementType, int>
                .Invoke(EventType.ResetAchievementProgress, AchievementManager.AchievementType.OptimalMultiHammerUse, 0);
        }
    }

    public void UpdateRemainingBarrierHealth(bool healthIncreased)
    {
        remainingBarrierHealth = Barriers.Sum(bar => bar.Health);
        remainingBarrierHealthText.text = remainingBarrierHealth.ToString();

        if (remainingBarrierHealth < 0.333f * _initTotalBarrierHealth)
        {
            barrierHealthIcon.color = healthColours[0];
        }
        else if (remainingBarrierHealth < 0.667f * _initTotalBarrierHealth)
        {
            barrierHealthIcon.color = healthColours[1];
        }
        else if (remainingBarrierHealth < 1 * _initTotalBarrierHealth)
        {
            barrierHealthIcon.color = healthColours[2];
        }
        else
        {
            barrierHealthIcon.color = healthColours[3];
        }

        Color c = barrierHealthIcon.color;
        barrierHealthIcon.color = new Color(c.r, c.g, c.b, 0.5f);

        if (healthIncreased)
        {
            barrierHealthAnimator.Play("barrier-health-icon-plus");
        }
        else
        {
            barrierHealthAnimator.Play("barrier-health-icon-minus");
        }

        EventManager<AchievementManager.AchievementType, int>.Invoke(EventType.SetAchievementProgress,
            AchievementManager.AchievementType.BarrierHPAbove, remainingBarrierHealth - 60);
        
        EventManager<AchievementManager.AchievementType, int>.Invoke(EventType.SetAchievementProgress,
            AchievementManager.AchievementType.BarrierHPLost, (maxBarrierHealth * amountOfBarriers) - remainingBarrierHealth - 20);
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

    public void HandleEqualBarrierHPCheck()
    {
        foreach (Barrier b in Barriers)
        {
            if (b.Health != 1)
            {
                return;
            }
        }
        
        EventManager<AchievementManager.AchievementType, int>.Invoke(EventType.SetAchievementProgress,
            AchievementManager.AchievementType.AllBarriersSameHP, 1);
    }
}
