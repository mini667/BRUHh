using UnityEngine;
using UnityEngine.UI;

public class FuelUI : MonoBehaviour
{
    public PlayerJetpack2D player;
    public Image fuelBar;

    void Update()
    {
        if(player != null && fuelBar != null)
            fuelBar.fillAmount = player.fuel / player.maxFuel;
    }
}
