using App.Player;
using UnityEngine;

public class Mine : MonoBehaviour
{
    [SerializeField] private float explosionForce = 10f;
    [SerializeField] private float explosionRadius = 5f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Подбрасываем игрока при взрыве
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                Vector3 explosionDirection = other.transform.position - transform.position;
                explosionDirection.y = 0;  // Игрок подбрасывается по вертикали, не отклоняясь по горизонтали
                playerMovement.AddForce(explosionDirection.normalized * explosionForce + Vector3.up * explosionForce);
            }

            // Взрыв — уничтожаем мину
            Destroy(gameObject);
        }
    }
}
