using UnityEngine;

public class AttackSidePlayerStateAnim : StatesAnimsAbstract
{
    public AttackSidePlayerStateAnim(Animator animPlayer)
    {
        ActiveAnimation("stateAnim", 13, ref animPlayer);
    }
}
