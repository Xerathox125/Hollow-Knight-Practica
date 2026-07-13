using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransition : MonoBehaviour
{
    [Header("Configuración de Transición")]
    [SerializeField] private string nextSceneName = "Level2"; // Nombre de la siguiente escena
    [SerializeField] private string playerTag = "Player";     // Tag del jugador
    [SerializeField] private bool requireSpecificObject = false; // Si true, busca un objeto específico
    [SerializeField] private string requiredObjectName = "mine"; // Nombre del objeto requerido (ej: "mine")

    [Header("Referencias")]
    [SerializeField] private ScenesManager scenesManager; // Referencia al SceneManager

    [Header("Configuración de Trigger")]
    [SerializeField] private bool useTrigger = true; // Usar OnTriggerEnter2D (true) o OnCollisionEnter2D (false)

    private void Awake()
    {
        // Buscar SceneManager automáticamente si no está asignado
        if (scenesManager == null)
        {
            scenesManager = FindFirstObjectByType<ScenesManager>();
        }

        // Configurar el collider como trigger si es necesario
        Collider2D col = GetComponent<Collider2D>();
        if (col != null && useTrigger)
        {
            col.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!useTrigger) return;
        CheckTransition(other.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (useTrigger) return;
        CheckTransition(collision.gameObject);
    }

    private void CheckTransition(GameObject other)
    {
        bool canTransition = false;

        if (requireSpecificObject)
        {
            // Verificar si es el objeto específico (ej: "mine")
            if (other.name == requiredObjectName || other.CompareTag(requiredObjectName))
            {
                canTransition = true;
            }
        }
        else
        {
            // Verificar por tag del jugador
            if (other.CompareTag(playerTag))
            {
                canTransition = true;
            }
        }

        if (canTransition)
        {
            LoadNextScene();
        }
    }

    private void LoadNextScene()
    {
        if (scenesManager != null)
        {
            // Usar el SceneManager personalizado
            scenesManager.LoadLevel2();
        }
        else
        {
            // Fallback: cargar directamente por nombre
            SceneManager.LoadScene(nextSceneName);
        }
    }

    private void OnDrawGizmos()
    {
        // Visualizar el área del trigger en el editor
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
            if (col is BoxCollider2D box)
            {
                Gizmos.DrawCube(transform.position + (Vector3)box.offset, box.size);
            }
            else if (col is CircleCollider2D circle)
            {
                Gizmos.DrawWireSphere(transform.position + (Vector3)circle.offset, circle.radius);
            }
        }
    }
}