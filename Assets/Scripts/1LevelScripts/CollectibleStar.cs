using UnityEngine;

public class CollectibleStar : MonoBehaviour
{
    [Header("Player Reference")]
    public GameObject player;

    [Header("UI Star")]
    public StarUI starUI;

    private bool collected = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected) return;

        if (other.gameObject == player)
        {
            collected = true;

            StarManager.Instance.CollectStar(); // добавляем к счётчику уровня

            if (starUI != null)
                starUI.CollectStar();

            Destroy(gameObject);
        }
    }
}
