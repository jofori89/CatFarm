using UnityEngine;

public class BowlBehaviour : MonoBehaviour
{
    [SerializeField]
    private GameObject _pfBowlFull;

    [SerializeField]
    private GameObject _pfBowlEmpty;

    private readonly int _capacity = 10;

    public float LevelPercentage => _level / _capacity * 100;

    [SerializeField]
    private float _level;

    public float Level => _level;

    public bool IsEmpty => _level == 0;

    public bool IsFull => _level == _capacity;

    // Update is called once per frame
    private void Update()
    {
        //if (IsEmpty && gameObject.CompareTag("bowl_full"))
        //{
        //    Debug.Log("Empty:  " + _level);
        //    Destroy(gameObject);

        //    Instantiate(_pfBowlEmpty, transform.position, transform.rotation);
        //}
    }

    public Object SpawnBowl(float x, float y)
    {
        Debug.Log("Spawn at  x:" + x + " y:" + y);
        return Instantiate(_pfBowlEmpty, new Vector3(x, y, 0), Quaternion.identity);
    }

    private void OnMouseDown()
    {
        Refill();
    }

    /// <summary>
    /// Refill
    /// </summary>
    private void Refill()
    {
        if (IsFull)
        {
            return;
        }

        Destroy(gameObject);

        _level = _capacity;
        Instantiate(_pfBowlFull, transform.position, transform.rotation);

        Debug.Log("Full:  " + _level);
    }

    // Eat in
    public void DecreaseFood()
    {
        if (_level <= 0)
        {
            return;
        }

        _level--;
    }
}