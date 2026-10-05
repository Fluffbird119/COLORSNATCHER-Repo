using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    public static BackgroundManager Instance { get; private set; }

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite redBackgrond;
    [SerializeField] private Sprite yellowBackground;
    [SerializeField] private Sprite greenBackground;
    [SerializeField] private Sprite blueBackground;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material blueBackgroundShader;

    private void Awake()
    {
        // Check if an instance already exists
        if (Instance != null && Instance != this)
        {
            // If a duplicate exists, destroy this GameObject to enforce the single instance
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void ChangeBackground(float backgroundNum)
    {
        spriteRenderer.material = defaultMaterial;
        if (backgroundNum == 0)
        {
            spriteRenderer.sprite = redBackgrond;
        }
        else if (backgroundNum == 1)
        {
            spriteRenderer.sprite = yellowBackground;
        }
        else if (backgroundNum == 2)
        {
            spriteRenderer.sprite = greenBackground;
        }
        else if (backgroundNum == 3)
        {
            spriteRenderer.sprite = blueBackground;
            spriteRenderer.material = blueBackgroundShader;
        }
    }
}
