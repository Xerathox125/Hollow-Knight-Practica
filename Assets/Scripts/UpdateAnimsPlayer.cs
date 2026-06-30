using UnityEngine;

public class UpdateAnimsPlayer : MonoBehaviour
{
    private AnimationManager animationManager;
    private PlayerController playerController;

    private enum AnimState { None, Idle, Run, JumpStart, JumpEnd, CrouchIdle, CrouchRun, Dash, StairsIdle, StairsMove, SwimIdle, SwimMove, WallIdle, AttackUp, AttackDown, AttackSide }
    private AnimState currentAnim = AnimState.None;

    private void Awake()
    {
        animationManager = new AnimationManager();
        playerController = GetComponent<PlayerController>();
    }

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

        // Ataque — TrySetAnim para no reiniciar la animación cada frame
        if (playerController.attacks.IsAttack)
        {
            if (playerController.attacks.AttackDirection == Vector2.up)
                TrySetAnim(AnimState.AttackUp, new AttackUpPlayerStateAnim(playerController.animPlayer));
            else if (playerController.attacks.AttackDirection == Vector2.down)
                TrySetAnim(AnimState.AttackDown, new AttackDownPlayerStateAnim(playerController.animPlayer));
            else
                TrySetAnim(AnimState.AttackSide, new AttackSidePlayerStateAnim(playerController.animPlayer));
            return;
        }

        // Dash
        if (playerController.dash != null && playerController.dash.isDash)
        {
            TrySetAnim(AnimState.Dash, new DashPlayerStateAnim(playerController.animPlayer));
            return;
        }

        // Nado
        if (playerController.swim.IsSwim)
        {
            if (Mathf.Abs(move.x) > 0.1f || Mathf.Abs(move.y) > 0.1f)
                TrySetAnim(AnimState.SwimMove, new MoveSwimPlayerStateAnim(playerController.animPlayer));
            else
                TrySetAnim(AnimState.SwimIdle, new IdleSwimPlayerStateAnim(playerController.animPlayer));
            return;
        }

        // Escaleras
        if (playerController.stairs.IsStairs)
        {
            if (Mathf.Abs(move.x) > 0.1f || Mathf.Abs(move.y) > 0.1f)
                TrySetAnim(AnimState.StairsMove, new MoveStairsPlayerStateAnim(playerController.animPlayer));
            else
                TrySetAnim(AnimState.StairsIdle, new IdleStairsPlayerStateAnim(playerController.animPlayer));
            return;
        }

        // WallJump
        if (playerController.wallJump.IsWall && !playerController.jump.IsGrounded)
        {
            TrySetAnim(AnimState.WallIdle, new IdleHandWallPlayerStateAnim(playerController.animPlayer));
            return;
        }

        // Salto
        if (!playerController.jump.IsGrounded)
        {
            float velY = playerController.rb.linearVelocity.y;
            if (velY > 0.1f)
                TrySetAnim(AnimState.JumpStart, new JumpStartPlayerStateAnim(playerController.animPlayer));
            else if (velY < -0.1f)
                TrySetAnim(AnimState.JumpEnd, new JumpEndPlayerStateAnim(playerController.animPlayer));
            return;
        }

        // Agachado
        if (Mathf.RoundToInt(move.y) == -1 || !playerController.crouch.canStandUp)
        {
            if (playerController.movement.IsMoving)
                TrySetAnim(AnimState.CrouchRun, new RunCrouchPlayerStateAnim(playerController.animPlayer));
            else
                TrySetAnim(AnimState.CrouchIdle, new IdleCrouchPlayerStateAnim(playerController.animPlayer));
            return;
        }

        // Movimiento Terrestre Normal
        if (playerController.movement.IsMoving)
            TrySetAnim(AnimState.Run, new RunPlayerStateAnim(playerController.animPlayer));
        else
            TrySetAnim(AnimState.Idle, new IdlePlayerStateAnim(playerController.animPlayer));
    }
}