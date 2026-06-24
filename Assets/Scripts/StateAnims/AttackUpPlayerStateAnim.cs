using UnityEngine;

public class AttackUpPlayerStateAnim : StatesAnimsAbstract
{
    public AttackUpPlayerStateAnim(Animator animPlayer)
    {
        ActiveAnimation("stateAnim", 14, ref animPlayer);
    }
}

