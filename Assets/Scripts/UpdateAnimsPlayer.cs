using UnityEngine;

public class UpdateAnimsPlayer : MonoBehaviour
{
    private AnimationManager animationManager;
    private PlayerController playerController;

    // OPTIMIZACIÓN: Enum para rastrear el estado actual y no repetir instancias
    private enum AnimState { None, Idle, Run, JumpStart, JumpEnd, CrouchIdle, CrouchRun }
    private AnimState currentAnim = AnimState.None;

    private void Awake()
    {
        animationManager = new AnimationManager();
        playerController = GetComponent<PlayerController>();
    }

    public void UpdateAnimation()
    {
        Vector2 move = playerController.controles.Player.Move.ReadValue<Vector2>();

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


        //Actualizar animaciones de crouch
        if (Mathf.RoundToInt(move.y) == -1 || !playerController.crouch.canStandUp)
        {
            if (playerController.movement.IsMoving) // Si con la tecla presionada hacia abajo nos movemos
            {

                if (currentAnim != AnimState.CrouchRun)
                { 
                    animationManager.SetState(new RunCrouchPlayerStateAnim(playerController.animPlayer));
                    currentAnim = AnimState.CrouchRun;
                }
            }
            else  // Si con la tecla presionada hacia abajo no nos movemos
            {
                if (currentAnim != AnimState.CrouchIdle)
                {
                        animationManager.SetState(new IdleCrouchPlayerStateAnim(playerController.animPlayer));
                        currentAnim = AnimState.CrouchIdle;
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