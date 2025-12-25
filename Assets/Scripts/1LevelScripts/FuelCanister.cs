using UnityEngine;

public class FuelCanister2D : MonoBehaviour
{
    public PlayerJetpack2D player; 
    public float fuelAmount = 30f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject == player.gameObject)
        {
            player.AddFuel(fuelAmount);
            Destroy(gameObject);
        }
    }
}
