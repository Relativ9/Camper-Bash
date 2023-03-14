using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    //TODO - Add Player Alive checker - freeze all movements + freeze TimeScale. if player is dead . Eg isPlayerAlive then allow for movement and all that put everything in update of playermovement

    [Header("Manually assigned variable")]

    //Assigned in start
    [SerializeField] public PlayerMovement playerMove;
    [SerializeField] public BreathingCheck holdBreath;

    [Header("Must remain publicly accessible")]
    public bool isAlive;
    public float currentHealth;

    [Header("Editable in inspector")]
    public float MaxPlayerHealth = 50f;

    private float currentHealthPercent;
    private float fallDamageVal;
    private bool hasTakenFallDamage;
    private bool takeDamageOnLanding;
    private bool isBurning;
    private int currentLevel;

    [Header("Visible for debugging")]
    public float airTimeOnLanding;

    private void Start()
    {
        currentHealth = MaxPlayerHealth;
        playerMove = FindObjectOfType<PlayerMovement>();
        isAlive = true;
    }

    private void Update()
    {
        if (currentHealth <= 0) //Kills the player once health is below 0
        {
            Debug.Log("player is dead");
            isAlive = false;
            currentHealth = 0;
            StartCoroutine("RestartLevel");
        }
    }


    private void FixedUpdate() 
    {
        FallDamage();
        NoBreathDamage();
        currentHealthPercent = (currentHealth * 100) / MaxPlayerHealth;

        if (currentHealth > MaxPlayerHealth)
        {
            currentHealth = MaxPlayerHealth;
        }
        if (takeDamageOnLanding) //applies fall damage
        {
            currentHealth = currentHealth - fallDamageVal;
            takeDamageOnLanding = false;
            Debug.Log("Take Damage");
        }
    }

    public void PlayerDamage(float hit) //function is used by the bullet prefabs to apply damage to the player
    {
        if (currentHealth > 0)
        {
            currentHealth -= hit;
        }
    }

    public void IncreaseHealth(int number) //function is used by health pickup prefabs to heal player to max health.
    {
        currentHealth += number;

        if (currentHealth > MaxPlayerHealth)
        {
            currentHealth = MaxPlayerHealth;
        }
    }

    public void FallDamage() //sets different fall damage values depending on velocity of player when ground was hit (how high the fall was)
    {
        if (playerMove.currentVel.y <= -25f)
        {
            fallDamageVal = 10f;
        }

        if (playerMove.currentVel.y <= -30f)
        {
            fallDamageVal = 15f;
        }

        if (playerMove.currentVel.y <= -40f)
        {
            fallDamageVal = 20f;
        }
    }

    public void NoBreathDamage() //damage tick for underwater damage, only applies once stamina is out
    {
        if (playerMove.currentStaminaValue < 1 && !holdBreath.canBreathe || isBurning)
        {
            StartCoroutine("DamageTick");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        if (playerMove.currentVel.y <= -20 && !hasTakenFallDamage)
        {
            Debug.Log("Is Colliding!");
            StartCoroutine("DamageTakenImmunity");
            airTimeOnLanding = playerMove.airTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Fire")
        {
            isBurning = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Fire")
        {
            isBurning = false;
        }
    }

    IEnumerator DamageTakenImmunity()
    {
        hasTakenFallDamage = true;
        takeDamageOnLanding = true;
        yield return new WaitForSeconds(2f);
        fallDamageVal = 0f;
        hasTakenFallDamage = false;
        takeDamageOnLanding = false;
    }

    IEnumerator RestartLevel()
    {
        yield return new WaitForSeconds(3f);
        currentLevel = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentLevel);
    }

    IEnumerator DamageTick()
    {
        if (!isBurning)
        {
            yield return new WaitForSeconds(1f);
            currentHealth -= Time.deltaTime;
        }

        if (!holdBreath.canBreathe && isBurning)
        {
            currentHealth -= Time.deltaTime * 2;
        }

        if (isBurning)
        {
            currentHealth -= Time.deltaTime * 1.4f;
        }
    }
}
