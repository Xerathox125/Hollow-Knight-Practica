using UnityEngine;
using UnityEngine.Rendering;

public class PlayerWallJump : MonoBehaviour
{
    PlayerController playerController;

    [Header("Wall Jump")]
    //Variables para crear el raycast
    public float rayCheckDistance; // Distancia del raycast para detectar paredes
    public LayerMask wallLayer; // Identifica que el layer será considerado una pared

    [Header("Bloqueo de Input")]
    public float wallJumpDuration = 0.2f; // El tiempo que el jugador no puede controlar el input
    private float wallJumpTimer;
    public bool isWallJumpActive { get; private set; } // Flag para saber si estamos en el "aire" del salto

    //Variables slide wall e impulso en muro
    public float slideWallSpeed; // Gravedad cuando estás en la pared
    public float wallJumpForceX; // Impulso de la fuerza en X cuando saltamos desde un muro
    public float wallJumpForceY; // Impulso de la fuerza en Y cuando saltamos desde un muro

    //Variables cooldown para saltar entre paredes
    public float wallJumpCoolDown; // Tiempo durante el cual no puede volver a sostenerse del muro después de hacer un salto
    private float coolDownWallTimer = 0; // Contador para disminuir el cooldown
    private bool canAttach = true; // Cuando esté en true significa que terminó el cooldown

    //Variables para saltar correctamente de pared en otra, congelando inputs por milésimas de segundo
    public float unStickCooldown; // Tiempo espera para volver a sostenerse en un muro luego de dejarnos caer cuando estábamos sostenidos
    public float oppositeInputTime; // Tiempo que el jugador sostiene el input contrario para soltarse de un muro
    private float oppositeInputCounter = 0; // Contador del oppositeInputTime

    private bool isWall; //Si estamos sostenidos en un muro
    private bool isWallJump; //Si estamos saltando de un muro

    public bool IsWall => isWall;
    public bool IsWallJump => isWallJump;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    public bool CanMoveOpposite()
    {
        return oppositeInputCounter >= oppositeInputTime;
    }

    public void OnUpdate()
    {

        if (isWallJumpActive)
        {
            wallJumpTimer -= Time.deltaTime;
            if (wallJumpTimer <= 0)
            {
                isWallJumpActive = false; // Recuperamos el control
            }
        }


        //Verificar que el player no está nadando
        if (playerController.swim != null && playerController.swim.IsSwim)
        {
            isWall = false;
            isWallJump = false;
            oppositeInputCounter = 0;
            return;
        }

        DetectWallJump();
        UpdateTimers();
        UpdateWallJump();
    }

    public void UpdateTimers()
    {
        if (coolDownWallTimer > 0)
        {
            coolDownWallTimer -= Time.fixedDeltaTime;
            if (coolDownWallTimer <= 0f)
            {
                coolDownWallTimer = 0f;
                isWallJump = false;
                canAttach = true;
            }
        }
    }

    public void DetectWallJump() // Método para saber si estamos detectando una pared
    {
        if (!canAttach) //Si no podemos attacharnos a la pared, entonces no detectamos la pared
        {
            isWall = false;
            return;
        }

        if (playerController.jump.IsGrounded && playerController.jump != null) // Si estmamos en el suelo, no detectamos la pared
        {
            isWall = false;
            return;
        }

        //Creacion del raycast que sale frente al jugador para detectar muros
        Vector2 direction;
        direction = playerController.movement.IsFacingRight ? Vector2.right : Vector2.left; //Ver hacia donde apunta el jugador cuando detecta un raycast
        /*
        if (playerController.movement.IsFacingRight)

            direction = Vector2.right;

        else

            direction = Vector2.left;
        */

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, rayCheckDistance, wallLayer);

        //Verificamos si estamos en pared

        isWall = (hit.collider != null) ? true : false;
        /*
        if (hit.collider != null)
        {
            isWall = true;
        }
        else
        {
            isWall = false;
        }
        */
    }

    public void UpdateWallJump()
    {
        if (isWall && !playerController.jump.IsGrounded) //Estamos en el muro y no estamos en el suelo
        {
            //-------------------------------------------------
            //Verificar input contrario y dar margen de tiempo
            //-------------------------------------------------

            //Vector en donde se almacena el input del jugador que se recibe desde PlayerController
            Vector2 input = playerController.controles.Player.Move.ReadValue<Vector2>();

            //Verifica si estamos presionando una tecla de dirección contraria a la que estamos sostenidas del muro
            if ((playerController.movement.IsFacingRight && input.x < -0.2f) && (!playerController.movement.IsFacingRight && input.x > 0.2f))
            {
                oppositeInputCounter += Time.fixedDeltaTime;

                //Detecta si se cumplió el tiempo máximo de mantener la tecla opuesta en un muro para dejar caer al jugador
                if (oppositeInputCounter >= oppositeInputTime)
                {
                    isWall = false;
                    canAttach = false;
                    oppositeInputCounter = 0;
                    coolDownWallTimer = unStickCooldown;

                    return;
                }
            }
            else //Si el jugador se arrpiente y oprimer el input hacia el mismo muro, el counter se regresa a 0 para que se siga manteniendo pegado al muro
            {
                oppositeInputCounter = 0;
            }

            //-----------------------------------------------------------------------
            //Aplicamos gravedad para que el player se deslice hacia abajo de a pocos
            //-----------------------------------------------------------------------
            if (!isWallJump && playerController.rb.linearVelocity.y < slideWallSpeed)
                playerController.rb.linearVelocity = new Vector2(playerController.rb.linearVelocity.x, slideWallSpeed);

            //--------------------------------------------------------------
            //Llamamos a wallJump en caso de que el jugador oprima saltar
            //--------------------------------------------------------------
            if (playerController.controles.Player.Jump.triggered)
            {
                WallJump();
                oppositeInputCounter = 0;
            }
        }
        else
            oppositeInputCounter = 0;
    }

    public void WallJump()
    {
        isWallJumpActive = true; // Iniciamos el estado de bloqueo
        wallJumpTimer = wallJumpDuration;

        isWallJump = true;
        coolDownWallTimer = wallJumpCoolDown;
        canAttach = true;

        //Aplicar fuerzas o velocidades en X y Y para el impulso del jugador cuando salta desde el muro
        Vector2 dirX;
        dirX = playerController.movement.IsFacingRight ? Vector2.left : Vector2.right;

        playerController.rb.linearVelocity = new Vector2(dirX.x * wallJumpForceX, wallJumpForceY);

        //Llamar a SetFacing desde PlayerMovement
        playerController.movement.SetFacing(dirX.x > 0f);
        isWall = false;
        oppositeInputCounter = 0;
    }
}