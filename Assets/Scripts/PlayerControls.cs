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
    public float lightCooldown = .1f;
    public float damageCooldown = 1f;

    public AudioClip dashSound;
    public AudioClip attackSound;

    private Timers Dash = new Timers(0, 0, 0, 0);
    private Timers Light = new Timers(0, 0, 0, 0);
    private Timers Damage = new Timers(0, 0, 0, 0);
    private Vector2 speedVector;
    private Vector2 movement;
    private Vector2 prevMovement;
    private Rigidbody2D rb2d;
    private bool isDashing = false;
    private string facing = "down";

    private bool releasedAttack = true;
    private bool releasedDash = true;

    private bool beingDamaged = false;

    public GameObject TrailPrefab;
    private GameObject trail;

    void Start()
    {
        speedVector = new Vector2(speed, speed);
        rb2d = GetComponent<Rigidbody2D>();
        Dash.updateValues(dashTime, dashTimeCurrent, dashCooldown, dashCooldownCurrent);
        Light.updateValues(0, 0, lightCooldown, 0);
        Damage.updateValues(0, 0, damageCooldown, 0);

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

        handleCooldowns();

        if (!beingDamaged)
        {
            handleFacing(inputX, inputY);
            handleDashing(inputDash);
            handleAttack(inputAttack);
        }

        handleDamage("", false);

        if (Input.GetButtonDown("Submit" + playerNumber))
        {
            GameObject.Find("_Manager").GetComponent<GameMenu>().createGameMenu();
        }
    }

    void FixedUpdate()
    {
        rb2d.velocity = movement;
    }

    private void handleCooldowns()
    {
        Dash.updateCooldown(Dash.CurrentCooldownTime - Time.deltaTime);
        Light.updateCooldown(Light.CurrentCooldownTime - Time.deltaTime);
        Damage.updateCooldown(Damage.CurrentCooldownTime - Time.deltaTime);
    }

    private void handleDashing(float inputDash)
    {
        bool canDash = (Dash.CurrentCooldownTime < 0);

        // If you can't dash, you DONE son!
        if (!canDash) return;

        if (inputDash == 0) releasedDash = true;
        if (inputDash == 1 && releasedDash && movement.magnitude > 0)
        {
            isDashing = true;
            releasedDash = false;

            trail = (GameObject)Instantiate(TrailPrefab);
            trail.transform.parent = transform;
            trail.transform.position = this.gameObject.transform.position;
        }

        if (isDashing)
        {
            // Start decrementing dash time
            Dash.CurrentTime -= Time.deltaTime;

            // Turn on both continuous collision mode and the dash trail
            rb2d.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            // Finished dashing
            if (Dash.CurrentTime < 0)
            {
                rb2d.collisionDetectionMode = CollisionDetectionMode2D.Discrete;

                isDashing = false;
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
                    prevMovement.Normalize();
                }
                movement = prevMovement * speed * dashSpeed;
                isDashing = true;
            }
        }
        else
        {
            // Remove the DASH object so that it is not visible
            Destroy(trail);
        }
    }

    private void handleFacing(float inputX, float inputY)
    {
        // HORIZONTAL MOVEMENT
        if (Mathf.Abs(inputX) > Mathf.Abs(inputY))
        {
            if (inputX > 0) facing = "right";
            else facing = "left";
        }

        // VERTICAL MOVEMENT
        else if (Mathf.Abs(inputY) > 0)
        {
            if (inputY < 0) facing = "down";
            else facing = "up";
        }
    }

    private void handleAttack(float inputAttack)
    {
        if (Light.CurrentCooldownTime < 0) 
            this.gameObject.GetComponent<Light>().enabled = false;

        if (inputAttack == 1 && releasedAttack)
        {
            releasedAttack = false;
            Debug.Log(playerNumber + " is attacking");

            foreach(var enemy in nearEnemy) {
                Debug.Log(playerNumber + " is attacking " + enemy.name);
                this.gameObject.GetComponent<Light>().enabled = true;
                GameObject.Find("Blade").GetComponent<AudioSource>().Play();
                
                enemy.BroadcastMessage("damage", facing);
                Light.CurrentCooldownTime = Light.DefaultCooldownTime;
            }
        }
        if (inputAttack == 0) releasedAttack = true;
    }

    public void damage(string direction)
    {
        Damage.CurrentCooldownTime = Damage.DefaultCooldownTime;
        handleDamage(direction, true);
    }

    private void handleDamage(string opponentDirection, bool damaged)
    {
        if (Damage.CurrentCooldownTime < 0)
            beingDamaged = false;

        if (damaged || beingDamaged)
        {

            float knockbackX = 0f, knockbackY = 0f;
            switch (opponentDirection)
            {
                case ("right"): knockbackX = 20f; break;
                case ("left"): knockbackX = -20f; break;
                case ("down"): knockbackY = -20f; break;
                case ("up"): knockbackY = 20f; break;
            }

            movement = new Vector2(knockbackX, knockbackY);
            beingDamaged = true;
        }
    }

    List<GameObject> nearEnemy = new List<GameObject>();
    void OnTriggerEnter2D(Collider2D col)
    {
        var collidedObject = col.gameObject;

        Debug.Log("This: " + this.gameObject.name);
        Debug.Log("Collided Object is player: " + (collidedObject.tag == "Player"));

        // Make sure we're colliding with a player, and that it is a different player than the user
        if (collidedObject.tag == "Player" && !string.Equals(collidedObject.name, this.gameObject.name))
        {
            nearEnemy.Add(col.gameObject);
        }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player") nearEnemy.Remove(col.gameObject);
    }
}

