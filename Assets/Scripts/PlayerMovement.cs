using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerController playerController;
    private bool isFacingRight = true;
    private AnimationManager animationManager;


    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        animationManager = new AnimationManager();
    }

    public void Move()
    {
        Vector2 move = playerController.controles.Player.Move.ReadValue<Vector2>();
        playerController.rb.linearVelocity = new Vector2(move.x * playerController.speed, playerController.rb.linearVelocity.y);

        if (move.x == 0)
        {
            animationManager.SetState(new IdlePlayerStateAnim(playerController.animPlayer));
        }
        else
        {
            animationManager.SetState(new RunPlayerStateAnim(playerController.animPlayer));
        }

        if (move.x > 0 && !isFacingRight)        
            Flip();        
        else if (move.x < 0 && isFacingRight)
            Flip();

        
        
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;

        if (isFacingRight)
        {
            transform.localScale = new Vector3(1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1);
        }


    }

}
