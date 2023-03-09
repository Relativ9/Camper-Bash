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

    [SerializeField] public PlayerMovement playerMovement;
    [SerializeField] public int currentLevel;

    [SerializeField] public BreathingCheck holdBreath;

    private void Start()
    {
        currentHealth = MaxPlayerHealth;
        playerMovement = FindObjectOfType<PlayerMovement>();
    }


    private void Update()
    {
        currentHealthPercent = (currentHealth * 100) / MaxPlayerHealth;

        if (currentHealth > MaxPlayerHealth)
        {
            currentHealth = MaxPlayerHealth;
        }

        fallDamage();
        noBreathDamage();
    }
    public void playerDamage(float hit)
    {
        if (currentHealth > 0)
        {
            currentHealth -= hit;
        }

        if (currentHealth <= 0)
        {
            Debug.Log("player is dead");
            currentHealth = 0;
            StartCoroutine(restartLevel());
            // Destroy(gameObject);
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
        if (playerMovement.currentVel.y <= -20)
        {
            fallDamageVal = 50f;
        }

        //if (playerMovement.currentVel.y <= -25)
        //{
        //    fallDamageVal = 15f;
        //}

        //if (playerMovement.currentVel.y <= -30)
        //{
        //    fallDamageVal = 50f;
        //}

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
        if (playerMovement.airTime >= 1.5f && playerMovement.currentVel.y <= -20 && !hasTakenFallDamage)
        {
            StartCoroutine("damageTakenImmunity");
        }

        if (takeDamageOnLanding)
        {
            currentHealth = currentHealth - fallDamageVal;
            takeDamageOnLanding = false;
            Debug.Log("Take Damage");
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
        hasTakenFallDamage = false;
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
