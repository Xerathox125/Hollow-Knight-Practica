using UnityEngine;

public class UpdateAnimsPlayer : MonoBehaviour
{
    private AnimationManager animationManager; // Gestor encargado de cambiar el estado de la animación
    private PlayerController playerController; // Referencia al controlador principal del jugador

    private enum AnimState { None, Idle, Run, JumpStart, JumpEnd, CrouchIdle, CrouchRun, Dash, StairsIdle, StairsMove } // Estados posibles de la animación
    private AnimState currentAnim = AnimState.None; // Variable para trackear el estado actual y evitar reinicios innecesarios

    private void Awake() // Se ejecuta al inicializar el componente
    {
        animationManager = new AnimationManager(); // Instancia el gestor de animaciones
        playerController = GetComponent<PlayerController>(); // Obtiene la referencia al PlayerController
    }

    public void UpdateAnimation() // Función principal para actualizar el estado del animador
    {
        // 1. Dash
        if (playerController.dash != null && playerController.dash.isDash) // Verifica si el componente dash existe y está activo
        {
            if (currentAnim != AnimState.Dash) // Comprueba si no estamos ya en estado Dash
            {
                animationManager.SetState(new DashPlayerStateAnim(playerController.animPlayer)); // Asigna el estado de Dash
                currentAnim = AnimState.Dash; // Actualiza el estado actual
            }
            return; // Interrumpe la ejecución para priorizar el Dash
        }

        Vector2 move = playerController.controles.Player.Move.ReadValue<Vector2>(); // Lee el input de movimiento actual

        // 2. Escaleras
        if (playerController.stairs.IsStairs) // Verifica si el jugador está interactuando con escaleras
        {
            if (Mathf.Abs(move.x) > 0.1f || Mathf.Abs(move.y) > 0.1f) // Comprueba si el jugador se está moviendo en la escalera
            {
                if (currentAnim != AnimState.StairsMove) // Si no está ya moviéndose en escaleras
                {
                    animationManager.SetState(new MoveStairsPlayerStateAnim(playerController.animPlayer)); // Asigna estado de movimiento en escaleras
                    currentAnim = AnimState.StairsMove; // Actualiza el estado actual
                }
            }
            else if (currentAnim != AnimState.StairsIdle) // Si está en escaleras pero quieto
            {
                animationManager.SetState(new IdleStairsPlayerStateAnim(playerController.animPlayer)); // Asigna estado de reposo en escaleras
                currentAnim = AnimState.StairsIdle; // Actualiza el estado actual
            }
            return; // Interrumpe la ejecución
        }

        // 3. Salto
        if (!playerController.jump.IsGrounded) // Verifica si el jugador está en el aire
        {
            float velY = playerController.rb.linearVelocity.y; // Obtiene la velocidad vertical actual
            if (velY > 0.1f) // Si sube (velocidad positiva)
            {
                if (currentAnim != AnimState.JumpStart) // Si no está en estado de inicio de salto
                {
                    animationManager.SetState(new JumpStartPlayerStateAnim(playerController.animPlayer)); // Asigna inicio de salto
                    currentAnim = AnimState.JumpStart; // Actualiza el estado actual
                }
            }
            else if (velY < -0.1f) // Si baja (velocidad negativa)
            {
                if (currentAnim != AnimState.JumpEnd) // Si no está en estado de fin de salto
                {
                    animationManager.SetState(new JumpEndPlayerStateAnim(playerController.animPlayer)); // Asigna fin de salto
                    currentAnim = AnimState.JumpEnd; // Actualiza el estado actual
                }
            }
            return; // Interrumpe la ejecución
        }

        // 4. Agachado
        if (Mathf.RoundToInt(move.y) == -1 || !playerController.crouch.canStandUp) // Detecta si el jugador intenta agacharse o está bloqueado
        {
            if (playerController.movement.IsMoving) // Verifica si el jugador se desplaza mientras está agachado
            {
                if (currentAnim != AnimState.CrouchRun) // Si no está ya en estado de movimiento agachado
                {
                    animationManager.SetState(new RunCrouchPlayerStateAnim(playerController.animPlayer)); // Asigna estado de correr agachado
                    currentAnim = AnimState.CrouchRun; // Actualiza el estado actual
                }
            }
            else if (currentAnim != AnimState.CrouchIdle) // Si está agachado pero quieto
            {
                animationManager.SetState(new IdleCrouchPlayerStateAnim(playerController.animPlayer)); // Asigna estado de reposo agachado
                currentAnim = AnimState.CrouchIdle; // Actualiza el estado actual
            }
            return; // Interrumpe la ejecución
        }

        // 5. Movimiento Terrestre
        if (playerController.movement.IsMoving) // Verifica si el jugador se mueve en el suelo
        {
            if (currentAnim != AnimState.Run) // Si no está ya corriendo
            {
                animationManager.SetState(new RunPlayerStateAnim(playerController.animPlayer)); // Asigna estado de correr
                currentAnim = AnimState.Run; // Actualiza el estado actual
            }
        }
        else if (currentAnim != AnimState.Idle) // Si el jugador está quieto en el suelo
        {
            animationManager.SetState(new IdlePlayerStateAnim(playerController.animPlayer)); // Asigna estado de reposo
            currentAnim = AnimState.Idle; // Actualiza el estado actual
        }
    }
}