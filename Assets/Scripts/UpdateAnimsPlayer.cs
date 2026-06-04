using UnityEngine;

public class UpdateAnimsPlayer : MonoBehaviour
{
    private AnimationManager animationManager;
    private PlayerController playerController;
    private PlayerMovement playerMovement;

    private void Awake()
    {
        animationManager = new AnimationManager();
        playerController = GetComponent<PlayerController>();
        playerMovement = GetComponent<PlayerMovement>();

    }
    public void UpdateAnimation()
    {
        if (playerMovement.IsMoving) //Si el jugador se está moviendo
            animationManager.SetState(new RunPlayerStateAnim(playerController.animPlayer));
        else //Si el jugador no se está moviendo
            animationManager.SetState(new IdlePlayerStateAnim(playerController.animPlayer));       

    }
}
