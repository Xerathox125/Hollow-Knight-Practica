using UnityEngine;

public class IdleCrouchPlayerStateAnim : StatesAnimsAbstract
{  
    public IdleCrouchPlayerStateAnim(Animator animPlayer)
    {
        ActiveAnimation("stateAnim", 5, ref animPlayer);
    }
}