using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CatManager : MonoBehaviourSingleton<CatManager>
{
    private int _initCat;

    public int TotalCat { get; private set; }

    [field: SerializeField]
    public List<Cat> Cats { get; } = new List<Cat>();

    [SerializeField]
    private Cat _catPrefab;

    private void Awake()
    {
        _initCat = 2;
    }

    public void StartNew()
    {
        Cats.Clear();
        TotalCat = 0;
        SpawnAsync(_initCat).Wait();
    }

    public Task<bool> SpawnAsync(int? number = 1, float? x = null, float? y = null)
    {
        var rx = GameController.Instance.RoomXLength;
        var ry = GameController.Instance.RoomYLength;

        for (int i = 0; i < number; i++)
        {
            x ??= UnityEngine.Random.Range(-rx, rx);
            y ??= UnityEngine.Random.Range(-ry, i);

            var path = $"Sprites/cats/cat_{UnityEngine.Random.Range(1, 10).ToString().PadLeft(2, '0')}";
            var avatar = Resources.Load<Sprite>(path);

            if (!avatar)
            {
                Debug.Log("Cat avatar not found: " + path);
                return Task.FromResult(false);
            }

            var cat = Instantiate(_catPrefab, new Vector3(x.Value, y.Value, 0), Quaternion.identity);

            var renderer = cat.GetComponent<SpriteRenderer>();
            renderer.sprite = avatar;
            renderer.sortingOrder = 2;
            cat.transform.localScale = new Vector3(0.5f, 0.5f);
            cat.OnDie += () =>
            {
                TotalCat--;
                GameController.Instance.ChangeScore(ScoreChangeType.CatDecrease);
            };

            Cats.Add(cat);

            Debug.Log("Spawn cat at:  x " + _catPrefab.transform.position.x + ", y " + _catPrefab.transform.position.y);

            GameController.Instance.ChangeScore(ScoreChangeType.CatIncrease);
        }

        TotalCat += number.GetValueOrDefault(0);

        // Increase bowl

        if (TotalCat % 5 == 0 && BowlManager.Instance.Bowls.Count * 5 <= TotalCat)
        {
            BowlManager.Instance.AddMoreBowl();
        }

        return Task.FromResult(true);
    }
}