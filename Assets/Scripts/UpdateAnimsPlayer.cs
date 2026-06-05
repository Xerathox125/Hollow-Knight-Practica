using UnityEngine;

public class UpdateAnimsPlayer : MonoBehaviour
{
    private AnimationManager animationManager;
    private PlayerController playerController;

    private void Awake()
    {
        animationManager = new AnimationManager();
        playerController = GetComponent<PlayerController>();
    }

    public void UpdateAnimation()
    {

        //Atualizar animaciones de salto
        if (!playerController.jump.IsGrounded) //Si el jugador no está tocando el suelo
        {
            //Estamos en el aire
            if(playerController.rb.linearVelocity.y > 0.1)
            {
                //subiendo
                animationManager.SetState(new JumpStartPlayerStateAnim(playerController.animPlayer));
            }
            else if(playerController.rb.linearVelocity.y < -0.1)
            {
                //bajando
                animationManager.SetState(new JumpEndPlayerStateAnim(playerController.animPlayer));
            }

            return; //Evitar otras animaciones
        }


        // Actualizar animaciones de movimiento            
        if (playerController.movement.IsMoving) //Si el jugador se está moviendo
            animationManager.SetState(new RunPlayerStateAnim(playerController.animPlayer));
        else //Si el jugador no se está moviendo
            animationManager.SetState(new IdlePlayerStateAnim(playerController.animPlayer));       

    }
}
