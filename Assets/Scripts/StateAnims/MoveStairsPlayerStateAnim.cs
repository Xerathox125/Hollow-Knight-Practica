using UnityEngine;

public class MoveStairsPlayerStateAnim : StatesAnimsAbstract
{
    public MoveStairsPlayerStateAnim(Animator animPlayer)
    {
        ActiveAnimation("stateAnim", 9, ref animPlayer);
    }
}