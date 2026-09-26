using UnityEngine;

public class RespawnButton : MonoBehaviour
{
    public PlayerHealth healthScript;
    public void OnButtonClicked()
    {
        healthScript.ResetPlayerStuff();

        healthScript.ResetWaveStuff();
    }
}
