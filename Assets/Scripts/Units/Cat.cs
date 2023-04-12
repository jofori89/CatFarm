using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Cat : MonoBehaviour
{
    private readonly uint _maxFullLevel = 30;

    private readonly uint _maxMature = 3;

    public uint FullLevel { get; private set; } = 10;

    public bool IsHungry => FullLevel <= 15;

    public bool IsFull => FullLevel == _maxFullLevel;

    public uint MatureLevel { get; set; } = 1;

    public bool CanReproduce => MatureLevel == _maxMature;

    public bool IsDoingSomething => _action != null;

    private CatAction? _action;

    private bool _isMoving;

    private Rigidbody2D _rb;

    private Vector3? _targetPoint;

    public delegate void OnDieAction();

    public event OnDieAction OnDie;

    private void Start()
    {
        //_rb = GetComponent<Rigidbody2D>();

        DoCountDownHungryAsync();
        RandomMovingAroundAsync();        
    }

    private void FixedUpdate()
    {
        MoveFunction();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Eat(collision.gameObject);

        Mating(collision.gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Eat(collision.gameObject);
    }

    private void OnMouseDown()
    {
        TryRunAwayAsync(Input.mousePosition);
    }

    private Task<bool> TryRunAwayAsync(Vector3 position)
    {
        _action = null;
        _targetPoint += VectorFromAngle(Vector2.Angle(position, transform.position)) * 2;

        return Task.FromResult(true);
    }

    private Vector2 VectorFromAngle(float theta)
    {
        return new Vector2(Mathf.Cos(theta), Mathf.Sin(theta)); // Trig is fun
    }

    public void StartMating()
    {
        _action = CatAction.Mating;
    }

    public void StopMating()
    {
        _action = null;
    }

    private void Mating(GameObject obj)
    {
        if (!obj.CompareTag("cat"))
        {
            return;
        }

        var otherCat = obj.GetComponent<Cat>();

        if (otherCat.IsDoingSomething || !otherCat.CanReproduce)
        {
            return;
        }

        Reproduce(otherCat);
    }

    private void Reproduce(Cat otherCat)
    {
        Debug.Log("Reproduce");

        StartMating();
        otherCat.StartMating();

        CatManager.Instance.SpawnAsync(1, transform.position.x, transform.position.y);

        MatureLevel = 1;
        otherCat.MatureLevel = 1;
        StopMating();
        otherCat.StopMating();
    }

    private void Eat(GameObject obj)
    {
        if (!obj.CompareTag("bowl") || _action != CatAction.Eating)
        {
            return;
        }

        var bowl = obj.GetComponent<Bowl>();

        // try with other bowl
        if (bowl.IsEmpty)
        {
            GotoEat();
        }
        else
        {
            EatAndGrow(bowl);
        }
    }

    /// <summary>
    /// Wander around the room, change direction each 3 seconds
    /// </summary>
    /// <param name="roomX"></param>
    /// <param name="roomY"></param>
    /// <returns></returns>
    private async Task RandomMovingAroundAsync()
    {
        while (FullLevel > 0)
        {
            await Task.Delay(1000);
            await Task.Yield();

            if (_isMoving || IsDoingSomething)
            {                
                continue;
            }

            var targetX = (RandomDirection() * UnityEngine.Random.value) + transform.position.x;
            var targetY = (RandomDirection() * UnityEngine.Random.value) + transform.position.y;

            if (Math.Abs(targetX) >= GameController.Instance.RoomXLength)
            {
                targetX = transform.position.x;
            }

            if (Math.Abs(targetY) >= GameController.Instance.RoomYLength)
            {
                targetY = transform.position.y;
            }

            _targetPoint = new Vector3(targetX, targetY, 0);
        }
    }

    private int RandomDirection()
    {
        return UnityEngine.Random.value < 0.5f ? 1 : -1;
    }

    private void GotoEat()
    {
        _action = CatAction.Eating;

        var (nearestBowl, magnitude) = FindBowl();

        if (nearestBowl == null)
        {
            _action = null;
            Debug.Log("No bowl.");
            return;
        }

        _targetPoint = nearestBowl.transform.position;
    }

    /// <summary>
    /// Eat and grow, increase the size, mature level
    /// </summary>
    /// <param name="nearestBowl"></param>
    private void EatAndGrow(Bowl bowl)
    {
        new WaitForSeconds(0.5f);

        if (FullLevel < _maxFullLevel)
        {
            bowl.DecreaseFood(1);
            FullLevel = _maxFullLevel;
        }

        // grow
        if (MatureLevel < _maxFullLevel)
        {
            MatureLevel++;

            // increase the body size
            if (gameObject.transform.localScale.x < 1)
            {
                gameObject.transform.localScale += new Vector3(0.1f, 0.1f);
            }
        }

        _action = null;
    }

    /// <summary>
    /// Find the nearest bowl which not empty
    /// </summary>
    /// <returns></returns>
    private (Bowl bowl, float magnitude) FindBowl()
    {
        Bowl nearestBowl = null;
        float magnitude = 0;
        foreach (var bowl in BowlManager.Instance.Bowls.Where(o => !o.IsEmpty))
        {
            var offset = transform.position - bowl.transform.position;

            if (nearestBowl == null || offset.magnitude < magnitude)
            {
                nearestBowl = bowl;
                magnitude = offset.magnitude;
            }
        }

        return (nearestBowl, magnitude);
    }

    private void MoveFunction()
    {
        if (_targetPoint == null || _isMoving || transform.position == _targetPoint.GetValueOrDefault())
        {
            return;
        }

        _isMoving = true;
        transform.Translate((_targetPoint.GetValueOrDefault() - transform.position) * Time.deltaTime, Space.Self);
        _isMoving = false;
    }

    private async Task DoCountDownHungryAsync()
    {
        while (FullLevel > 0)
        {
            await Task.Delay(1000);
            await Task.Yield();

            FullLevel--;

            if (IsHungry && FullLevel > 0 && _action != CatAction.Eating)
            {
                GotoEat();
            }
        }

        // timer is finished...
        if (FullLevel == 0)
        {
            OnDie?.Invoke();
            Destroy(gameObject);
        }
    }
}

internal enum CatAction
{
    Eating,
    Playing,
    TakeShit,
    Mating
}