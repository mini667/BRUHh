using UnityEngine;

public class MoveBetweenPoints : MonoBehaviour
{
    [Header("Points")]
    public Transform pointA;
    public Transform pointB;

    [Header("Movement")]
    public float speed = 2f;          // скорость движения
    public float waitTime = 0f;       // пауза на точках
    private bool movingToB = true;    // направление движения
    private float waitCounter = 0f;

    void Update()
    {
        if(pointA == null || pointB == null) return;

        Transform targetPoint = movingToB ? pointB : pointA;

        // Двигаемся к точке
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);

        // Проверяем, достигли ли точки
        if(Vector2.Distance(transform.position, targetPoint.position) < 0.01f)
        {
            if(waitTime > 0)
            {
                waitCounter += Time.deltaTime;
                if(waitCounter >= waitTime)
                {
                    movingToB = !movingToB;
                    waitCounter = 0f;
                }
            }
            else
            {
                movingToB = !movingToB;
            }
        }
    }
}
