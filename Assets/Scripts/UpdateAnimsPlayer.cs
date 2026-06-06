using UnityEngine;

public class UpdateAnimsPlayer : MonoBehaviour
{
    private AnimationManager animationManager;
    private PlayerController playerController;

    // OPTIMIZACIÓN: Enum para rastrear el estado actual y no repetir instancias
    private enum AnimState { None, Idle, Run, JumpStart, JumpEnd }
    private AnimState currentAnim = AnimState.None;

    private void Awake()
    {
        animationManager = new AnimationManager();
        playerController = GetComponent<PlayerController>();
    }

    public void UpdateAnimation()
    {
        // 1. Actualizar animaciones de salto
        if (!playerController.jump.IsGrounded)
        {
            if (playerController.rb.linearVelocity.y > 0.1f)
            {
                if (currentAnim != AnimState.JumpStart) // Solo instanciamos si cambiamos de estado
                {
                    animationManager.SetState(new JumpStartPlayerStateAnim(playerController.animPlayer));
                    currentAnim = AnimState.JumpStart;
                }
            }
            else if (playerController.rb.linearVelocity.y < -0.1f)
            {
                if (currentAnim != AnimState.JumpEnd)
                {
                    animationManager.SetState(new JumpEndPlayerStateAnim(playerController.animPlayer));
                    currentAnim = AnimState.JumpEnd;
                }
            }
            return;
        }

        // 2. Actualizar animaciones de movimiento            
        if (playerController.movement.IsMoving)
        {
            if (currentAnim != AnimState.Run)
            {
                animationManager.SetState(new RunPlayerStateAnim(playerController.animPlayer));
                currentAnim = AnimState.Run;
            }
        }
        else
        {
            if (currentAnim != AnimState.Idle)
            {
                animationManager.SetState(new IdlePlayerStateAnim(playerController.animPlayer));
                currentAnim = AnimState.Idle;
            }
        }
    }
}