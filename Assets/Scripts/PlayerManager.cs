using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    Camera playerCamera, menuCamera;
    [SerializeField]
    GameObject menu, spawners;

    [SerializeField]
    GameObject _baseModel;

    [SerializeField]
    GameObject _deathModel;
    private Vector3 _deathPosition;

    AudioSource playerHurt;
    AudioSource winSound;

    private int health;
    private readonly int MAX_HEALTH = 1;

    // delay between hits
    private bool invincible;

    // Dies once
    public bool dead;

    // Wins once
    public bool win;

    // Initial character position
    Vector3 initialPos;

    void Start()
    {
        //Comment these two lines to start with the playerCamera
        playerCamera.gameObject.SetActive(false);
        menuCamera.gameObject.SetActive(true);

        health = MAX_HEALTH;

        AudioSource[] sfx = GetComponents<AudioSource>();
        playerHurt = sfx[0];
        winSound = sfx[1];
        initialPos = this.transform.position;

        win = false;
    }


    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Ladder"))
        {
            if (!win)
            {
                win = true;
                winSound.Play();
                StartCoroutine(GetComponentInChildren<ThirdPersonOrbitCam>().GameOver("YOU WIN!"));
            }
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag.Equals("Large Enemy") || col.gameObject.tag.Equals("Flocker"))
        {
            if (!invincible)
            {
                invincible = true;

                playerHurt.Play();

                // Slight knockback
                Vector3 oppositeDir = (this.transform.position - col.transform.position).normalized;
                // Remove y-component to prevent flight
                Vector3 forceDir = new Vector3(oppositeDir.x, 0, oppositeDir.z);
                // Add knockback force
                GetComponent<Rigidbody>().AddForce(forceDir * 1000000, ForceMode.Force);

                // If not last hit -- play hit sound and blink
                if (--health > 0)
                {
                    StartCoroutine(TakeHit());
                }
            }
        }
    }

    void Update()
    {
        if (playerCamera.gameObject.activeSelf)
        {
            gameObject.GetComponent<ShootBehaviour>().enabled = true;
            spawners.SetActive(true);
        }

        // Play death sound if f
        if (health < 1)
        {
            if (!dead)
            {
                dead = true;
                _baseModel.SetActive(false);
                _deathModel.SetActive(true);
                _deathModel.GetComponent<Animator>().Play("Dying", -1, 0f);
                _deathPosition = transform.position;
            }
            transform.position = _deathPosition; // Make player not get pushed around
        }
        else
        {
            _baseModel.SetActive(true);
            _deathModel.SetActive(false);
        }
    }

    // Player takes a hit -- blink and become temporarily invincible
    IEnumerator TakeHit()
    {
        SkinnedMeshRenderer renderer = GetComponentInChildren<SkinnedMeshRenderer>();

        yield return new WaitForEndOfFrame();

        // Blink twice
        for (int i = 0; i < 2; i++)
        {
            yield return new WaitForSeconds(0.5f);
            renderer.enabled = false;
            yield return new WaitForSeconds(0.5f);
            renderer.enabled = true;
        }

        invincible = false;
    }

    // Reset player position
    public void ResetPlayer()
    {
        health = MAX_HEALTH;
        this.transform.position = initialPos;
        dead = false;
        invincible = false;
    }

    // Health accessor
    public int Health { get; set; }

}
