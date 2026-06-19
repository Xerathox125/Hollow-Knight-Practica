using UnityEngine;

public abstract class StatesAnimsAbstract // Clase base para estados de animacion (Patrón State)
{
    private string name; // Nombre del parámetro en el Animator
    private int animationId; // ID del estado (int) para la transición

    public void ActiveAnimation(string nameAnim, int animId, ref Animator playerAnimator) // Activa la animación en el componente
    {
        name = nameAnim; // Asigna nombre del parámetro (ej. "State")
        animationId = animId; // Asigna ID del estado numérico
        playerAnimator.SetInteger(name, animationId); // Cambia el valor en el Animator para disparar la transición
    }
}