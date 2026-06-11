using UnityEngine;

public class IdleStairsPlayerStateAnim : StatesAnimsAbstract
{
    public IdleStairsPlayerStateAnim(Animator animPlayer)
    {
        ActiveAnimation("stateAnim", 8, ref animPlayer);
    }
}