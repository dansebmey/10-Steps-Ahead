using UnityEngine;

[CreateAssetMenu(menuName = "Hammer")]
public class Hammer : Powerup
{
    public int range;
    public int healValue;

    protected override void Awake()
    {
        Gm = FindObjectOfType<GameManager>();
        Debug.Log(Gm.name);
    }
    
    public override void OnConsume()
    {
        FindObjectOfType<GameManager>().BarrierManager.RepairBarriers(range, healValue);
    }
}