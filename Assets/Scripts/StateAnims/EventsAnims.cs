using UnityEngine;

public class EventsAnims : MonoBehaviour
{
    private PlayerAttacks playerAttacks;

    private void Awake()
    {
        playerAttacks = GetComponentInParent<PlayerAttacks>();
    }

    public void OnAttackHit()
    {
        playerAttacks.ActiveHitbox();
    }

    public void OnAttackEnd()
    {
        playerAttacks.EndAttack();
    }
}
