using UnityEngine;

public class AnimationManager // Encargado de gestionar el estado actual
{
    private StatesAnimsAbstract actualState; // Referencia al estado que se está reproduciendo

    public void SetState(StatesAnimsAbstract newState) // Cambia el estado actual
    {
        actualState = newState; // Actualiza el objeto
    }
}