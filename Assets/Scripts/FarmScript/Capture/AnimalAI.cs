using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class AnimalAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AnimalType animalType;
    [SerializeField] private GameObject area;
    [SerializeField] private Item favoriteFruit;
    [SerializeField] private GameObject animalCanvas;
    [SerializeField] private Image animalFruitImage;
    [SerializeField] private TMP_Text animalNameText;
    [SerializeField] private Slider lifeSlider;
    [SerializeField] private Color lifeColor;
    [SerializeField] private Color lifePauseColor;
    [SerializeField] private string animalName;
    [SerializeField] private bool isAttracted = false;
    [SerializeField] private bool canBeAttracted = false;

    [Header("Movement")]
    [SerializeField] private float speed = 3.5f;
    [SerializeField] private float runAwaySpeedModifier = 1.5f;
    [SerializeField] private float stoppingDistance = 0f;
    [SerializeField] private float refreshRate = 0.1f;
    [SerializeField] private float distanceMinToChange = 2f;
    [SerializeField] private Vector3 destination;
    [SerializeField] private bool isMoving = false;
    [SerializeField] private bool nearFruit = false;
    private bool handleDestination = false;
    
    [Header("Timers")]
    [SerializeField] private float timeBeforeMoving = 3f;
    [SerializeField] private float timeBeforeEatingFruit = 5f;
    [SerializeField] private float timeRunAwaySpeed = 2f;
    [SerializeField] private Vector2 timeAnimalLife = new Vector2(15, 30);
    [SerializeField] private bool pauseLifeTimer = false;
    [SerializeField] private bool timerLife = false;
    
    [Header("Player Detection")]
    [SerializeField] private bool playerIsNear;
    [SerializeField] private bool playerIsBehind;

    private GameObject currentFruitPlaced;
    private float distance;
    private float timeRemainingLife = 0f;
    private float timeRemainingFruit = 0f;
    private bool timerStartedLife = false;
    private bool timerStartedFruit = false;
    private bool runAway = false;

    private NavMeshAgent agent;
    private Animator animator;

    #region Getters / Setters

    public AnimalType AnimalType
    {
        get { return animalType; }
        set { animalType = value; }
    }

    public Item FavoriteFruit
    {
        get { return favoriteFruit; }
        set { favoriteFruit = value; }
    }

    public GameObject Area
    {
        get { return area; }
        set { area = value; }
    }

    public GameObject CurrentFruitPlaced
    {
        get { return currentFruitPlaced; }
        set { currentFruitPlaced = value; }
    }

    public bool PlayerIsNear
    {
        get { return playerIsNear; }
        set { playerIsNear = value; }
    }

    public bool PlayerIsBehind
    {
        get { return playerIsBehind; }
        set { playerIsBehind = value; }
    }

    public bool PauseLifeTimer
    {
        get { return pauseLifeTimer; }
        set { pauseLifeTimer = value; }
    }

    public bool IsAttracted
    {
        get { return isAttracted; }
        set { isAttracted = value; }
    }

    public bool CanBeAttracted
    {
        get { return canBeAttracted; }
        set { canBeAttracted = value; }
    }

    public bool RunAway
    {
        get { return runAway; }
        set { runAway = value; }
    }

    #endregion

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        agent.speed = speed;
        agent.stoppingDistance = stoppingDistance;

        animalNameText.text = animalName;
        animalFruitImage.sprite = favoriteFruit.itemSprite;
    }

    private void Update()
    {
        HandleLife();
        
        animalCanvas.transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);

        HandleMovement();
    }

    #region Movement

    private void HandleMovement()
    {
        animator.SetBool("Walking", isMoving);

        if (CaptureManager.instance.FruitPlaced != null)
            currentFruitPlaced = CaptureManager.instance.FruitPlaced;

        ActualizeDirection();

        HandleFruit();

        if (runAway)
            isMoving = false;

        if (agent == null || isMoving) return;
        
        if (!handleDestination)
            HandleDestination();

        StartCoroutine(GoToDestination());
    }

    public void ForceRunAway(bool eatFruit = true)
    {
        runAway = true;
        
        if (eatFruit) EatFruit();

        StartCoroutine(RunAwaySpeed());
    }

    private IEnumerator RunAwaySpeed()
    {
        speed *= runAwaySpeedModifier;
        
        yield return new WaitForSeconds(timeRunAwaySpeed);

        speed /= runAwaySpeedModifier;
    }

    #region Direction & Distance

    private void HandleDestination()
    {
        handleDestination = true;
        
        if (currentFruitPlaced == null || runAway)
        {
            destination = SearchDestination();
            pauseLifeTimer = false;
            isAttracted = false;
        }
        else
        {
            Item itemFruitPlaced = currentFruitPlaced.GetComponent<KeepItem>().Item;

            if (itemFruitPlaced == favoriteFruit && canBeAttracted)
            {
                destination = currentFruitPlaced.transform.position;
                pauseLifeTimer = true;
                isAttracted = true;
            }
            else
            {
                destination = SearchDestination();
                pauseLifeTimer = false;
                isAttracted = false;
            }
        }
    }

    private Vector3 SearchDestination()
    {
        if (area == null) return Vector3.zero;

        float xLimit = area.GetComponent<Renderer>().bounds.size.x;
        float zLimit = area.GetComponent<Renderer>().bounds.size.z;

        float randomX = Random.Range(-xLimit / 2, xLimit / 2);
        float randomZ = Random.Range(-zLimit / 2, zLimit / 2);

        Vector3 randomPosition = area.transform.position + new Vector3(randomX, 0f, randomZ);

        return randomPosition;
    }

    private void ActualizeDirection()
    {
        Vector3 direction = transform.position - destination;
        distance = direction.magnitude;
    }

    #endregion

    private IEnumerator GoToDestination()
    {
        isMoving = true;

        Vector3 currentFruitPosition = Vector3.zero;

        if (currentFruitPlaced != null)
            currentFruitPosition = currentFruitPlaced.transform.position;

        bool canContinue = true;

        while (distance > distanceMinToChange && canContinue && timerLife)
        {
            if (!canContinue) yield break;

            Item itemPlaced = null;
            if (currentFruitPlaced != null) itemPlaced = currentFruitPlaced.GetComponent<KeepItem>().Item;

            if ((destination != currentFruitPosition && itemPlaced == favoriteFruit)
                || (destination == currentFruitPosition && (itemPlaced == null || itemPlaced != favoriteFruit)))
                canContinue = false;

            if (!timerLife) yield break;
            
            agent.SetDestination(destination);

            yield return new WaitForSeconds(refreshRate);
        }

        yield return new WaitForSeconds(TimeToWaitAtEnd());

        isMoving = false;

        if (runAway) runAway = false;

        handleDestination = false;
    }

    private float TimeToWaitAtEnd()
    {
        float timeToWait = timeBeforeMoving;

        int random = Random.Range(0, 3);

        if (random == 2) timeToWait = 0f;

        return timeToWait;
    }

    #endregion

    private void HandleFruit()
    {
        if (distance > distanceMinToChange && nearFruit)
        {
            nearFruit = false;
            timerStartedFruit = false;
        }
        
        if (currentFruitPlaced == null) return;

        if (destination == currentFruitPlaced.transform.position && distance <= distanceMinToChange && !timerStartedFruit)
        {
            nearFruit = true;
            timeRemainingFruit = timeBeforeEatingFruit;
        }

        if (nearFruit)
        {
            if (timeRemainingFruit > 0)
            {
                timerStartedFruit = true;

                timeRemainingFruit -= Time.deltaTime;
            }
            else
            {
                if (currentFruitPlaced != null) EatFruit();

                nearFruit = false;

                timerStartedFruit = false;
            }
        }
    }

    private void EatFruit()
    {
        CaptureManager.instance.RemoveItem();
        currentFruitPlaced = null;
        canBeAttracted = false;

        if (pauseLifeTimer) pauseLifeTimer = false;
    }

    private void HandleLife()
    {
        if (!timerLife && !timerStartedLife)
        {
            timerLife = true;
            timeRemainingLife = Random.Range(timeAnimalLife.x, timeAnimalLife.y);
            
            lifeSlider.maxValue = timeRemainingLife;
            lifeSlider.value = timeRemainingLife;
        }

        if (timerLife)
        {
            if (pauseLifeTimer)
            {
                lifeSlider.fillRect.GetComponent<Image>().color = lifePauseColor;
                return;
            }
            
            if (timeRemainingLife > 0)
            {
                timerStartedLife = true;

                timeRemainingLife -= Time.deltaTime;

                lifeSlider.value = timeRemainingLife;
                lifeSlider.fillRect.GetComponent<Image>().color = lifeColor;
            }
            else
            {
                EndLife();
            }
        }
    }

    public void EndLife()
    {
        StopAllCoroutines();
                
        timerLife = false;

        CaptureManager.instance.WildsAnimals.Remove(gameObject);

        Destroy(gameObject);
    }
}