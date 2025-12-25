using UnityEngine;
using UnityEngine.UI;

public class StarUI : MonoBehaviour
{
    public Image starImage;
    public Color collectedColor = Color.yellow;
    public Color uncollectedColor = Color.gray;

    void Start()
    {
        if (starImage != null)
            starImage.color = uncollectedColor;
    }

    public void CollectStar()
    {
        if (starImage != null)
            starImage.color = collectedColor;
    }
}
