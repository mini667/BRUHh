using UnityEngine;

public class StarManager : MonoBehaviour
{
    public static StarManager Instance;
    private int collectedStars = 0;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void CollectStar()
    {
        collectedStars++;
    }

    public int GetStars()
    {
        return collectedStars;
    }

    public void ResetStars()
    {
        collectedStars = 0;
    }
}
