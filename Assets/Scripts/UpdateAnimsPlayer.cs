using UnityEngine;

public class UpdateAnimsPlayer : MonoBehaviour
{
    private AnimationManager animationManager;
    private PlayerController playerController;

    private enum AnimState { None, Idle, Run, JumpStart, JumpEnd, CrouchIdle, CrouchRun, Dash, StairsIdle, StairsMove }
    private AnimState currentAnim = AnimState.None;

    private void Awake()
    {
        animationManager = new AnimationManager();
        playerController = GetComponent<PlayerController>();
    }

    public void UpdateAnimation()
    {
        // 1. Si está haciendo Dash, congelamos cualquier otra animación
        if (playerController.dash != null && playerController.dash.isDash)
        {
            if (currentAnim != AnimState.Dash)
            {
                animationManager.SetState(new DashPlayerStateAnim(playerController.animPlayer));
                currentAnim = AnimState.Dash;
            }
            return; // Salimos inmediatamente para no evaluar el resto del código
        }

        Vector2 move = playerController.controles.Player.Move.ReadValue<Vector2>();

        // NUEVO BLOQUE: 2. Animaciones de Escaleras
        if (playerController.stairs.IsStairs)
        {
            // Verificamos si el jugador está presionando los controles para moverse
            if (Mathf.Abs(move.x) > 0.1f || Mathf.Abs(move.y) > 0.1f)
            {
                if (currentAnim != AnimState.StairsMove)
                {
                    // Asegúrate de tener esta clase creada
                    animationManager.SetState(new MoveStairsPlayerStateAnim(playerController.animPlayer));
                    currentAnim = AnimState.StairsMove;
                }
            }
            else // Si no se mueve, está quieto en la reja
            {
                if (currentAnim != AnimState.StairsIdle)
                {
                    // Asegúrate de tener esta clase creada
                    animationManager.SetState(new IdleStairsPlayerStateAnim(playerController.animPlayer));
                    currentAnim = AnimState.StairsIdle;
                }
            }
            return; // ¡Crucial! Salimos aquí para que no reproduzca animaciones de salto o correr
        }


        // 2. Actualizar animaciones de salto
        if (!playerController.jump.IsGrounded)
        {
            if (playerController.rb.linearVelocity.y > 0.1f)
            {
                if (currentAnim != AnimState.JumpStart)
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

        // 3. Actualizar animaciones de crouch
        if (Mathf.RoundToInt(move.y) == -1 || !playerController.crouch.canStandUp)
        {
            if (playerController.movement.IsMoving)
            {
                if (currentAnim != AnimState.CrouchRun)
                {
                    animationManager.SetState(new RunCrouchPlayerStateAnim(playerController.animPlayer));
                    currentAnim = AnimState.CrouchRun;
                }
            }
            else
            {
                if (currentAnim != AnimState.CrouchIdle)
                {
                    animationManager.SetState(new IdleCrouchPlayerStateAnim(playerController.animPlayer));
                    currentAnim = AnimState.CrouchIdle;
                }
            }
            return;
        }

        // 4. Actualizar animaciones de movimiento            
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