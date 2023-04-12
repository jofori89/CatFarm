using UnityEngine;

public class Bowl : MonoBehaviour
{
    private readonly uint _capacity = 10;

    public float LevelPercentage => Level / _capacity * 100;

    [SerializeField]
    public uint Level { get; private set; }

    public bool IsEmpty => Level == 0;

    public bool IsFull => Level == _capacity;

    public delegate void OnRefilledAction();
    public event OnRefilledAction OnRefilled;

    public void Init()
    {
        Level = 0;

        var path = $"Sprites/bowls/cat_bowl_{(IsEmpty ? "empty" : "full")}";
        var avatar = Resources.Load<Sprite>(path);

        if (!avatar)
        {
            Debug.Log("Bowl avatar not found: " + path);
            return;
        }

        var renderer = gameObject.GetComponent<SpriteRenderer>();
        renderer.sprite = avatar;
        renderer.sortingOrder = 1;
    }

    /// <summary>
    /// Refill
    /// </summary>
    public void Refill()
    {
        if (IsFull)
        {
            return;
        }

        Level = _capacity;        
        gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>($"Sprites/bowls/cat_bowl_full");

        OnRefilled?.Invoke();
    }

    // Eat in
    public uint DecreaseFood(uint eatAmount)
    {
        if (IsEmpty)
        {
            return 0;
        }

        uint availableAmount = Level > eatAmount ? eatAmount : Level;

        Level -= availableAmount;

        if (IsEmpty)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>($"Sprites/bowls/cat_bowl_empty");
        }

        return availableAmount;
    }

    private void OnMouseDown()
    {
        Refill();
    }
}