using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer head;
    [SerializeField] private SpriteRenderer legs;
    [SerializeField] private SpriteRenderer tentecle;
    [SerializeField] private SpriteRenderer eye;

    public static PlayerManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// When passed false this method hides the player behind the background then stops them from being able to move and use abilites
    /// When passed true it does all that stuff in reverse
    /// </summary>
    public void SetPlayerActive(bool state)
    {
        // cuz when SetPlayerActive is true the player should not be stunend and vice versa
        PlayerMovement.Instance.SetIsStunned(!state);
        if (state)
        {
            head.sortingLayerName = "Player";
            legs.sortingLayerName = "Player";
            tentecle.sortingLayerName = "Player";
            eye.sortingLayerName = "Player";
        }
        else
        {
            head.sortingLayerName = "Default";
            legs.sortingLayerName = "Default";
            tentecle.sortingLayerName = "Default";
            eye.sortingLayerName = "Default";
        }
    }

    public void SetTentacleActive(bool state)
    {
        if (state)
        {
            
        }
        else
        {
            
        }
    }
}
