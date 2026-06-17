using UnityEngine;

public class MoveSwimPlayerStateAnim : StatesAnimsAbstract
{
    public MoveSwimPlayerStateAnim(Animator animPlayer)
    {
        ActiveAnimation("stateAnim", 11, ref animPlayer);
    }
}