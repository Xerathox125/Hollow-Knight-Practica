using UnityEngine;

public class RunCrouchPlayerStateAnim : StatesAnimsAbstract
{
    public RunCrouchPlayerStateAnim(Animator animPlayer)
    {
        ActiveAnimation("stateAnim", 6, ref animPlayer);
    }
}


