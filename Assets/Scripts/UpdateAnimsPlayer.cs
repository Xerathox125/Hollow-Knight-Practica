using UnityEngine;

public class UpdateAnimsPlayer : MonoBehaviour
{
    private AnimationManager animationManager;
    private PlayerController playerController;

    // Se añaden SwimIdle y SwimMove para proteger el estado de nado
    private enum AnimState { None, Idle, Run, JumpStart, JumpEnd, CrouchIdle, CrouchRun, Dash, StairsIdle, StairsMove, SwimIdle, SwimMove }
    private AnimState currentAnim = AnimState.None;

    private void Awake()
    {
        animationManager = new AnimationManager();
        playerController = GetComponent<PlayerController>();
    }

    // Función auxiliar para centralizar la validación y evitar el frame-spam
    private void TrySetAnim(AnimState newState, StatesAnimsAbstract animClass)
    {
        if (currentAnim != newState)
        {
            animationManager.SetState(animClass);
            currentAnim = newState;
        }
    }

    public void UpdateAnimation()
    {
        Vector2 move = playerController.controles.Player.Move.ReadValue<Vector2>();

        // 1. Dash
        if (playerController.dash != null && playerController.dash.isDash)
        {
            TrySetAnim(AnimState.Dash, new DashPlayerStateAnim(playerController.animPlayer));
            return;
        }

        // 2. Nado (Ahora protegido con el enum)
        if (playerController.swim.IsSwim)
        {
            if (Mathf.Abs(move.x) > 0.1f || Mathf.Abs(move.y) > 0.1f)
                TrySetAnim(AnimState.SwimMove, new MoveSwimPlayerStateAnim(playerController.animPlayer));
            else
                TrySetAnim(AnimState.SwimIdle, new IdleSwimPlayerStateAnim(playerController.animPlayer));
            return;
        }

        // 3. Escaleras
        if (playerController.stairs.IsStairs)
        {
            if (Mathf.Abs(move.x) > 0.1f || Mathf.Abs(move.y) > 0.1f)
                TrySetAnim(AnimState.StairsMove, new MoveStairsPlayerStateAnim(playerController.animPlayer));
            else
                TrySetAnim(AnimState.StairsIdle, new IdleStairsPlayerStateAnim(playerController.animPlayer));
            return;
        }

        // 4. Salto
        if (!playerController.jump.IsGrounded)
        {
            float velY = playerController.rb.linearVelocity.y;
            if (velY > 0.1f)
                TrySetAnim(AnimState.JumpStart, new JumpStartPlayerStateAnim(playerController.animPlayer));
            else if (velY < -0.1f)
                TrySetAnim(AnimState.JumpEnd, new JumpEndPlayerStateAnim(playerController.animPlayer));
            return;
        }

        // 5. Agachado
        if (Mathf.RoundToInt(move.y) == -1 || !playerController.crouch.canStandUp)
        {
            if (playerController.movement.IsMoving)
                TrySetAnim(AnimState.CrouchRun, new RunCrouchPlayerStateAnim(playerController.animPlayer));
            else
                TrySetAnim(AnimState.CrouchIdle, new IdleCrouchPlayerStateAnim(playerController.animPlayer));
            return;
        }

        // 6. Movimiento Terrestre Normal
        if (playerController.movement.IsMoving)
            TrySetAnim(AnimState.Run, new RunPlayerStateAnim(playerController.animPlayer));
        else
            TrySetAnim(AnimState.Idle, new IdlePlayerStateAnim(playerController.animPlayer));
    }
}