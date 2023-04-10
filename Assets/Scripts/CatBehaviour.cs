using System.Collections.Generic;
using UnityEngine;

public class CatBehaviour : MonoBehaviour
{
    private readonly int _initCat = 3;

    public List<Cat> Cats { get; } = new List<Cat>();

    [SerializeField]
    private Cat catPrefab;

    private void Start()
    {
        Spawn(GameController.Instance.RoomXLength, GameController.Instance.RoomYLength);
    }

    private void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
    }

    private void Spawn(float rx, float ry)
    {
        for (int i = 0; i < _initCat; i++)
        {
            var x = UnityEngine.Random.Range(-rx, rx);
            var y = UnityEngine.Random.Range(-ry, 0);

            var path = $"Sprites/cats/cat_{UnityEngine.Random.Range(1, 10).ToString().PadLeft(2, '0')}";
            var avatar = Resources.Load<Sprite>(path);

            if (!avatar)
            {
                Debug.Log("Cat avatar not found: " + path);
                return;
            }

            var cat = Instantiate(catPrefab, new Vector3(x, y, 0), Quaternion.identity);

            var renderer = cat.GetComponent<SpriteRenderer>();
            renderer.sprite = avatar;
            renderer.sortingOrder = 2;
            cat.transform.localScale = new Vector2(0.5f, 0.5f);

            Cats.Add(cat);

            Debug.Log("Spawn cat at:  x " + catPrefab.transform.position.x + ", y " + catPrefab.transform.position.y);
        }
    }
}