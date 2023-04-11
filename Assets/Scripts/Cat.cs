using System;
using System.Collections;
using System.Linq;
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

    private Coroutine cidHungry;

    private Coroutine cidPlayAround;

    private Rigidbody2D _rb;

    private Vector3? _targetPoint;

    public delegate void OnDieAction();
    public event OnDieAction OnDie;

    private void Start()
    {
        //_rb = GetComponent<Rigidbody2D>();

        cidHungry = StartCoroutine(DoCountDownHungry());  //fires coroutine

        cidPlayAround = StartCoroutine(RandomMovingAround());
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

        if(otherCat.IsDoingSomething || !otherCat.CanReproduce)
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

        CatBehaviour.Instance.Spawn(1, transform.position.x, transform.position.y);

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
    private IEnumerator RandomMovingAround()
    {
        while (true)
        {
            if (_isMoving || IsDoingSomething)
            {
                yield return new WaitForSeconds(1.0f);
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

            yield return new WaitForSeconds(1.0f);
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
            FullLevel =  _maxFullLevel;
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

    private IEnumerator DoCountDownHungry()
    {
        while (FullLevel >= 0)
        {
            yield return new WaitForSeconds(1);
            FullLevel--;

            if (IsHungry && FullLevel > 0 && _action != CatAction.Eating)
            {
                GotoEat();
            }

            // timer is finished...
            if (FullLevel == 0)
            {
                OnDie?.Invoke();
                Destroy(gameObject, 1);
                StopCoroutine(cidHungry);

                if (cidPlayAround != null)
                {
                    StopCoroutine(cidPlayAround);
                }
            }
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