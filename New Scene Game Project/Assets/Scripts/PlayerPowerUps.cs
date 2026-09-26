using UnityEngine;

public class PlayerPowerUps : MonoBehaviour
{
    public bool beatenRed = false; // bool for now cuz lazy can make it better latter
    [SerializeField] float rage = 20f; // kill 20 eneimes to get 20 rage and able to use powerup
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && beatenRed && rage == 20)
        {
            RagePowerUpEffect();
        }
    }

    public void IncreaseRage(float amount)
    {
        rage += amount;

        if (rage > 20)
        {
            rage = 20;
        }
    }

    void RagePowerUpEffect()
    {
        rage = 0;
        // turn head and eye and tentacle red
        // increase speed
    }
}
