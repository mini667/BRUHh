using UnityEngine;

[System.Serializable]
public class ParallaxLayer
{
    public Transform layer; // фон
    [Range(0f, 1f)]
    public float speed;     // сила параллакса
}

public class ParallaxManager : MonoBehaviour
{
    public Transform cameraTransform;
    public ParallaxLayer[] layers;

    private Vector3 lastCameraPos;

    void Start()
    {
        lastCameraPos = cameraTransform.position;
    }

    void LateUpdate()
    {
        float deltaX = cameraTransform.position.x - lastCameraPos.x;

        foreach (ParallaxLayer layer in layers)
        {
            layer.layer.position += new Vector3(deltaX * layer.speed, 0f, 0f);
        }

        lastCameraPos = cameraTransform.position;
    }
}
