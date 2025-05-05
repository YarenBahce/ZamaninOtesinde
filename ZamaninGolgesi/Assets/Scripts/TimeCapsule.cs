using UnityEngine;

public class TimeCapsule : MonoBehaviour
{
    public float rewindDuration = 1.5f; // Kaç saniye geri gidecek

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            EnemyPatrol enemy = FindObjectOfType<EnemyPatrol>();
            if (enemy != null)
            {
                enemy.TriggerRewind(rewindDuration);
            }
            GameManager.Instance.CatchCapsule();
            Destroy(gameObject);
        }
    }
}