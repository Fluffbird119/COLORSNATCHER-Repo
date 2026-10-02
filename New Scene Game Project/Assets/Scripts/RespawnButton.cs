using UnityEngine;

public class RespawnButton : MonoBehaviour
{
    //public PlayerHealth healthScript;
    public void OnButtonClicked()
    {
        PlayerHealth.Instance.ResetPlayerStuff();

        PlayerHealth.Instance.ResetWaveStuff();
    }
}
