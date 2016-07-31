using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PlayerControls : MonoBehaviour
{

    public string playerNumber;
    public float speed = 7f;
    public float dashSpeed = 7f;
    public float dashTime = .5f;
    public float dashTimeCurrent = .5f;
    public float dashCooldown = 2f;
    public float dashCooldownCurrent = 0f;

    public AudioClip dashSound;
    public AudioClip attackSound;

    private TrailRenderer Trail;
    private Timers Dash = new Timers(0, 0, 0, 0);
    private Vector2 speedVector;
    private Vector2 movement;
    private Vector2 prevMovement;
    private Rigidbody2D rb2d;
    private bool isDashing = false;

    private bool releasedAttack = true;
    private bool releasedDash = true;

    void Start()
    {
        speedVector = new Vector2(speed, speed);
        rb2d = GetComponent<Rigidbody2D>();
        Trail = GetComponentInChildren<TrailRenderer>();
        Dash.updateValues(dashTime, dashTimeCurrent, dashCooldown, dashCooldownCurrent);

        prevMovement = Vector2.zero;
    }

    void Update()
    {
        float inputX = Input.GetAxis("Horizontal" + playerNumber);
        float inputY = Input.GetAxis("Vertical" + playerNumber);
        float inputDash = Input.GetAxisRaw("Dash" + playerNumber);
        float inputAttack = Input.GetAxisRaw("Attack" + playerNumber);

        movement = new Vector2(
            inputX * speedVector.x,
            inputY * speedVector.y
        );

        handleDashing(inputDash);
        handleAttack(inputAttack);

        if (Input.GetButtonDown("Submit" + playerNumber))
        {
            GameObject.Find("_Manager").GetComponent<GameMenu>().createGameMenu();
        }
    }

    void FixedUpdate()
    {
        rb2d.velocity = movement;
    }

    private void handleDashing(float inputDash)
    {
        Dash.updateCooldown(Dash.CurrentCooldownTime - Time.deltaTime);

        bool canDash = (Dash.CurrentCooldownTime < 0);

        // If you can't dash, you DONE son!
        if (!canDash) return;

        if (inputDash == 0) releasedDash = true;
        if (inputDash == 1 && releasedDash)
        {
            isDashing = true;
            releasedDash = false;
        }

        if (isDashing)
        {
            // Start decrementing dash time
            Dash.CurrentTime -= Time.deltaTime;

            // Turn on both continuous collision mode and the dash trail
            rb2d.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            if (Trail != null) Trail.gameObject.SetActive(true);

            // Finished dashing
            if (Dash.CurrentTime < 0)
            {
                rb2d.collisionDetectionMode = CollisionDetectionMode2D.Discrete;

                isDashing = false;
                Trail.probeAnchor = rb2d.transform;
                prevMovement = Vector2.zero;

                Dash.CurrentTime = Dash.DefaultTime;
                Dash.CurrentCooldownTime = Dash.DefaultCooldownTime;
            }
            // Still dashing
            else
            {
                if (prevMovement == Vector2.zero)
                {
                    SoundManager.instance.RandomizeSfx(dashSound);
                    prevMovement = movement;
                }
                movement = prevMovement * dashSpeed;
                isDashing = true;
            }
        }
        else
        {
            // Remove the DASH object so that it is not visible
            if (Trail != null) Trail.gameObject.SetActive(false);
        }
    }

    private void handleFacing(float inputX, float inputY)
    {
        string facing = "down";

        // HORIZONTAL MOVEMENT
        if (Mathf.Abs(inputX) > Mathf.Abs(inputY))
        {
            if (inputX > 0) facing = "right";
            else facing = "left";
        }

        // VERTICAL MOVEMENT
        else if (Mathf.Abs(inputY) > 0)
        {
            if (inputY > 0) facing = "down";
            else facing = "up";
        }
    }

    private void handleAttack(float inputAttack)
    {
        if (inputAttack == 1 && releasedAttack)
        {
            //SoundManager.instance.RandomizeSfx(attackSound);
            GameObject.Find("bottle").GetComponent<AudioSource>().Play();
            releasedAttack = false;

            foreach(var enemy in nearEnemy) {
                Debug.Log("BOOM! Hit " + enemy);
            }
        }
        if (inputAttack == 0) releasedAttack = true;
    }

    List<GameObject> nearEnemy = new List<GameObject>();
    void OnTriggerEnter2D(Collider2D col)
    {
/*
        Debug.Log("Collider: " + col.gameObject.name);
        Debug.Log("This object: " + this.gameObject.name);
        Debug.Log(string.Compare(col.gameObject.name, this.gameObject.name));
*/

        if (col.gameObject.tag == "Player" && string.Compare(col.gameObject.name, this.gameObject.name) > -1)
        {
            nearEnemy.Add(col.gameObject);
        }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player") nearEnemy.Remove(col.gameObject);
    }
}

