using UnityEngine;

namespace StealthGame
{
    public class Observer : MonoBehaviour
    {
        [Header("Referencias")]
        [Tooltip("Arrastra aquí el objeto Player desde la jerarquía")]
        public GameObject playerObject;

        [Header("Configuración")]
        public float detectionRadius = 2f;

        [Header("Debug")]
        public bool showDebugLogs = true;

        private Transform playerTransform;
        private bool playerDetected = false;

        void Start()
        {
            // Configurar collider
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.isTrigger = true;
            }

            // Configurar Rigidbody
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
            }
            rb.isKinematic = true;
            rb.useGravity = false;

            // Buscar Player si no está asignado
            if (playerObject == null)
            {
                playerObject = GameObject.FindGameObjectWithTag("Player");
                if (playerObject == null)
                {
                    Debug.LogError("[Observer] ¡No se encontró el Player! Asígnalo en el Inspector");
                }
                else
                {
                    Debug.Log($"[Observer] Player encontrado automáticamente: {playerObject.name}");
                }
            }

            if (playerObject != null)
            {
                playerTransform = playerObject.transform;
            }

            Debug.Log($"[Observer] Inicializado en {gameObject.name}");
        }

        void OnTriggerEnter(Collider other)
        {
            // Ignorar PointOfView
            if (other.gameObject.name == "PointOfView" || 
                other.gameObject.GetComponent<Camera>() != null)
            {
                if (showDebugLogs)
                    Debug.Log("[Observer] Ignorando PointOfView/Camera");
                return;
            }

            if (showDebugLogs)
            {
                Debug.Log($"<color=yellow>[Observer] Objeto detectado: {other.gameObject.name}</color>");
                Debug.Log($"  → Tag: '{other.tag}'");
                Debug.Log($"  → Tiene PlayerMovement: {(other.GetComponent<PlayerMovement>() != null ? "SÍ" : "NO")}");
            }

            // Verificar si es el Player (múltiples métodos)
            bool isPlayer = false;

            // Método 1: Por Tag
            if (other.CompareTag("Player"))
            {
                isPlayer = true;
                if (showDebugLogs) Debug.Log("  ✓ Detectado por TAG");
            }

            // Método 2: Por componente PlayerMovement
            if (other.GetComponent<PlayerMovement>() != null)
            {
                isPlayer = true;
                if (showDebugLogs) Debug.Log("  ✓ Detectado por componente PlayerMovement");
            }

            // Método 3: Por referencia directa
            if (playerObject != null && (other.gameObject == playerObject || other.transform.IsChildOf(playerObject.transform)))
            {
                isPlayer = true;
                if (showDebugLogs) Debug.Log("  ✓ Detectado por referencia directa");
            }

            if (isPlayer)
            {
                playerDetected = true;
                Debug.Log($"<color=green>🎯 ¡PLAYER ATRAPADA!</color> Detectada por {gameObject.name}");
            }
        }

        void OnTriggerStay(Collider other)
        {
            if (other.gameObject.name == "PointOfView" || other.GetComponent<Camera>() != null)
                return;

            bool isPlayer = other.CompareTag("Player") || 
                          other.GetComponent<PlayerMovement>() != null ||
                          (playerObject != null && (other.gameObject == playerObject || other.transform.IsChildOf(playerObject.transform)));

            if (isPlayer && showDebugLogs)
            {
                // Solo mostrar cada 2 segundos para no spam
                if (Time.frameCount % 120 == 0)
                {
                    Debug.Log($"<color=cyan>👁️ Player dentro del área de {gameObject.name}</color>");
                }
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.gameObject.name == "PointOfView" || other.GetComponent<Camera>() != null)
                return;

            bool isPlayer = other.CompareTag("Player") || 
                          other.GetComponent<PlayerMovement>() != null ||
                          (playerObject != null && (other.gameObject == playerObject || other.transform.IsChildOf(playerObject.transform)));

            if (isPlayer)
            {
                playerDetected = false;
                Debug.Log($"<color=orange>👋 Player salió del área de {gameObject.name}</color>");
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = playerDetected ? Color.green : new Color(1f, 0f, 0f, 0.3f);
            
            Collider col = GetComponent<Collider>();
            if (col is CapsuleCollider capsule)
            {
                Gizmos.DrawWireSphere(transform.position + Vector3.up * 0.9f, capsule.radius);
            }
            else
            {
                Gizmos.DrawWireSphere(transform.position, detectionRadius);
            }
        }
    }
}
