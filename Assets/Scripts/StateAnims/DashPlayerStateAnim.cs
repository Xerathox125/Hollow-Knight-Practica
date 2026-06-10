using UnityEngine;

public class DashPlayerStateAnim : StatesAnimsAbstract
{
    public DashPlayerStateAnim(Animator animPlayer)
    {
        ActiveAnimation("stateAnim", 7, ref animPlayer);
    }
}