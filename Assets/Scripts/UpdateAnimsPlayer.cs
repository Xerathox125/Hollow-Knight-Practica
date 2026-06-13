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
        // 1. Dash
        if (playerController.dash != null && playerController.dash.isDash)
        {
            if (currentAnim != AnimState.Dash)
            {
                animationManager.SetState(new DashPlayerStateAnim(playerController.animPlayer));
                currentAnim = AnimState.Dash;
            }
            return;
        }

        Vector2 move = playerController.controles.Player.Move.ReadValue<Vector2>();

        // 2. Escaleras
        if (playerController.stairs.IsStairs)
        {
            if (Mathf.Abs(move.x) > 0.1f || Mathf.Abs(move.y) > 0.1f)
            {
                if (currentAnim != AnimState.StairsMove)
                {
                    animationManager.SetState(new MoveStairsPlayerStateAnim(playerController.animPlayer));
                    currentAnim = AnimState.StairsMove;
                }
            }
            else if (currentAnim != AnimState.StairsIdle)
            {
                animationManager.SetState(new IdleStairsPlayerStateAnim(playerController.animPlayer));
                currentAnim = AnimState.StairsIdle;
            }
            return;
        }

        // 3. Salto
        if (!playerController.jump.IsGrounded)
        {
            float velY = playerController.rb.linearVelocity.y;
            if (velY > 0.1f)
            {
                if (currentAnim != AnimState.JumpStart)
                {
                    animationManager.SetState(new JumpStartPlayerStateAnim(playerController.animPlayer));
                    currentAnim = AnimState.JumpStart;
                }
            }
            else if (velY < -0.1f)
            {
                if (currentAnim != AnimState.JumpEnd)
                {
                    animationManager.SetState(new JumpEndPlayerStateAnim(playerController.animPlayer));
                    currentAnim = AnimState.JumpEnd;
                }
            }
            return;
        }

        // 4. Agachado
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
            else if (currentAnim != AnimState.CrouchIdle)
            {
                animationManager.SetState(new IdleCrouchPlayerStateAnim(playerController.animPlayer));
                currentAnim = AnimState.CrouchIdle;
            }
            return;
        }

        // 5. Movimiento Terrestre
        if (playerController.movement.IsMoving)
        {
            if (currentAnim != AnimState.Run)
            {
                animationManager.SetState(new RunPlayerStateAnim(playerController.animPlayer));
                currentAnim = AnimState.Run;
            }
        }
        else if (currentAnim != AnimState.Idle)
        {
            animationManager.SetState(new IdlePlayerStateAnim(playerController.animPlayer));
            currentAnim = AnimState.Idle;
        }
    }
}