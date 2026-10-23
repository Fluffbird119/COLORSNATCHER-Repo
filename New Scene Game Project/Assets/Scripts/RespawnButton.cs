using UnityEngine;

public class RespawnButton : MonoBehaviour
{
    public void OnButtonClicked()
    {
        PlayerHealth.Instance.ResetPlayerStuff();
        PlayerHealth.Instance.ResetWaveStuff();
    }
}
