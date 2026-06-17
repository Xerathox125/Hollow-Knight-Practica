using UnityEngine;

public abstract class StatesAnimsAbstract // Clase base para estados de animacion
{
    private string name; // Nombre del parámetro en el Animator
    private int animationId; // ID del estado (int)

    public void ActiveAnimation(string nameAnim, int animId, ref Animator playerAnimator) // Activa la animación en el componente
    {
        name = nameAnim; // Asigna nombre
        animationId = animId; // Asigna ID
        playerAnimator.SetInteger(name, animationId); // Cambia el valor en el Animator
    }
}