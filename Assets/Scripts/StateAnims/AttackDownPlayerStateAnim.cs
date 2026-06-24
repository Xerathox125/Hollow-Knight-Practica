using UnityEngine;

public class AttackDownPlayerStateAnim : StatesAnimsAbstract
{
    public AttackDownPlayerStateAnim(Animator animPlayer)
    {
        ActiveAnimation("stateAnim", 15, ref animPlayer);
    }
}
