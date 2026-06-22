using UnityEngine;

public class IdleHandWallPlayerStateAnim : StatesAnimsAbstract
{
    public IdleHandWallPlayerStateAnim(Animator animPlayer)
    {
        ActiveAnimation("stateAnim", 12, ref animPlayer);
    }
}