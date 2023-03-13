using System;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    //TODO - Add Player Alive checker - freeze all movements + freeze TimeScale. if player is dead . Eg isPlayerAlive then allow for movement and all that put everything in update of playermovement

    [SerializeField] public float MaxPlayerHealth = 50f;
    [SerializeField] public float currentHealth;
    [SerializeField] public float currentHealthPercent;
    [SerializeField] public float fallDamageVal;

    [SerializeField] public bool hasTakenFallDamage;
    [SerializeField] public bool takeDamageOnLanding;
    [SerializeField] public bool isBurning;
    [SerializeField] public bool isAlive;

    [SerializeField] public PlayerMovement playerMovement;
    [SerializeField] public int currentLevel;

    [SerializeField] public BreathingCheck holdBreath;

    public float airTimeOnLanding;

    private void Start()
    {
        currentHealth = MaxPlayerHealth;
        playerMovement = FindObjectOfType<PlayerMovement>();
        isAlive = true;
    }

    private void Update()
    {
        if (currentHealth <= 0)
        {
            Debug.Log("player is dead");
            isAlive = false;
            currentHealth = 0;
            StartCoroutine(restartLevel());
        }
    }


    private void FixedUpdate()
    {
        fallDamage();
        noBreathDamage();
        currentHealthPercent = (currentHealth * 100) / MaxPlayerHealth;

        if (currentHealth > MaxPlayerHealth)
        {
            currentHealth = MaxPlayerHealth;
        }

        if (takeDamageOnLanding)
        {
            currentHealth = currentHealth - fallDamageVal;
            takeDamageOnLanding = false;
            Debug.Log("Take Damage");
        }
    }

    public void playerDamage(float hit)
    {
        if (currentHealth > 0)
        {
            currentHealth -= hit;
        }
    }

    public void IncreaseHealth(int number)
    {
        currentHealth += number;

        if (currentHealth > MaxPlayerHealth)
        {
            currentHealth = MaxPlayerHealth;
        }
    }

    public void fallDamage()
    {
        if (playerMovement.currentVel.y <= -25f)
        {
            fallDamageVal = 10f;
        }

        if (playerMovement.currentVel.y <= -30f)
        {
            fallDamageVal = 15f;
        }

        if (playerMovement.currentVel.y <= -40f)
        {
            fallDamageVal = 20f;
        }
    }

    public void noBreathDamage()
    {
        if (playerMovement.currentStaminaValue < 1 && !holdBreath.canBreathe || isBurning)
        {
            StartCoroutine("damageTick");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        if (playerMovement.currentVel.y <= -20 && !hasTakenFallDamage)
        {
            Debug.Log("Is Colliding!");
            StartCoroutine("damageTakenImmunity");
            airTimeOnLanding = playerMovement.airTime;
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

    IEnumerator damageTakenImmunity()
    {
        hasTakenFallDamage = true;
        takeDamageOnLanding = true;
        yield return new WaitForSeconds(2f);
        fallDamageVal = 0f;
        hasTakenFallDamage = false;
        takeDamageOnLanding = false;
    }

    IEnumerator restartLevel()
    {
        yield return new WaitForSeconds(3f);
        currentLevel = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentLevel);
    }

    IEnumerator damageTick()
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
