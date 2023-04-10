using UnityEngine;

public class Bowl : MonoBehaviour
{
    private readonly int _capacity = 10;

    public float LevelPercentage => Level / _capacity * 100;

    [SerializeField]
    public int Level { get; private set; }

    public bool IsEmpty => Level == 0;

    public bool IsFull => Level == _capacity;

    private BowlManager _manager;

    public void Init(BowlManager manager)
    {
        _manager = manager;
        Level = _capacity;

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

        if (IsFull)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>($"Sprites/bowls/cat_bowl_full");
        }
    }

    // Eat in
    public int DecreaseFood(int eatAmount)
    {
        if (IsEmpty)
        {
            return 0;
        }

        var availableAmount = Level > eatAmount ? eatAmount : Level;

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