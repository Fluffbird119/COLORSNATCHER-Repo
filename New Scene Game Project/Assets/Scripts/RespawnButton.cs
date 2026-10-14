using UnityEngine;

public class RespawnButton : MonoBehaviour
{
    //public PlayerHealth healthScript;
    public void OnButtonClicked()
    {
        gameObject.SetActive(false);
        PlayerHealth.Instance.ResetPlayerStuff();
        PlayerHealth.Instance.ResetWaveStuff();
    }
}
