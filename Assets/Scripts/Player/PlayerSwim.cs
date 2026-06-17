using UnityEngine;

public class PlayerSwim : MonoBehaviour
{
    private PlayerController playerController;

    [Header("Nado")]
    public float gravitySwim;
    public float speedSwim;
    public float salidaDelAguaSalto;
    [HideInInspector] public bool rangeSwim; 
    private bool isSwim;

    public bool IsSwim => isSwim; // Getter


    private void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    public void OnUpdate()
    {
        DetectSwim();
        MoveSwim();
    }

    void DetectSwim()
    {
        if (rangeSwim && !isSwim)
        {
            StartSwim();
        }
    }

    void StartSwim() // Lógica de movimiento vectorial
    {
        isSwim = true;
        playerController.rb.linearVelocity = Vector2.zero; //Al entrar al agua se resetean las velocidades
        
    }

    void MoveSwim(float cooldown = 0f) // Salida del modo escaleras
    {
        if (isSwim)
        {
            playerController.rb.gravityScale = gravitySwim;
        }
    }

    public void ExitSwim() {
        isSwim = false;
        playerController.rb.gravityScale = playerController.normalGravity;
        playerController.rb.linearVelocity = new Vector2(playerController.rb.linearVelocity.x, salidaDelAguaSalto);
    }

}
