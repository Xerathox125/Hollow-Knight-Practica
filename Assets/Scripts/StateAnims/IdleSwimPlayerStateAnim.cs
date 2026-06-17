using UnityEngine;

public class IdleSwimPlayerStateAnim : StatesAnimsAbstract
{
    public IdleSwimPlayerStateAnim(Animator animPlayer)
    {
        ActiveAnimation("stateAnim", 10, ref animPlayer);
    }
}