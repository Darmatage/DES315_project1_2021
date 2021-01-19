﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingEnemy_AS : MonoBehaviour
{
    public float speed = 1f; // speed of enemy
    private Transform target; // the player target
    public int damage = 1; // how much damage it deals to player
    public int EnemyLives = 2;

    public GameObject explosionObj;

    private Renderer rend;
    private SpriteRenderer spriteRenderer;
    private GameHandler gameHandlerObj;
    private Animator anim;
    public Grid grid;

    public static float strobeDelay = .15f;
    float strobeDelayTimer = strobeDelay;
    public float explodeRange = 2.0f;
    bool toggle = false;
    float detonateTimer = 2f; // in seconds
    bool bExplode = false;
    private bool attackPlayer = false;
    public int damageAmount = 10;
    AStarPather pather;
    CircleCollider2D circleCollider;

    // Start is called before the first frame update
    void Start()
    {
        anim = gameObject.GetComponentInChildren<Animator>();
        rend = GetComponentInChildren<Renderer>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        }
        GameObject gameHandlerLocation = GameObject.FindWithTag("GameHandler");
        if (gameHandlerLocation != null)
        {
            gameHandlerObj = gameHandlerLocation.GetComponent<GameHandler>();
        }

        circleCollider = GetComponent<CircleCollider2D>();

        circleCollider.enabled = false;


        pather = new AStarPather();
        grid = FindObjectOfType<Grid>();
        pather.setGrid(grid);
        pather.setObject(explosionObj);
        pather.init(grid);



        //if(path.Count > 0)
        //{
        //    foreach (Vector3 pos in path)
        //    {
        //        Debug.Log("Position: " + pos.ToString());
        //    }
        //}

        //List<Vector3> path = pather.computePath(transform.position, target.position);

        //if (path != null)
        //{
        //    foreach (Vector3 pos in path)
        //    {
        //        Debug.Log("Position: " + pos.ToString());
        //    }
        //}
        //else
        //{
        //    Debug.Log("PATH IS NULL");
        //}


    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //pather.DrawDebug();

        //Debug.DrawLine(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 10000.0f, 0.0f), Color.red, 0f,false);
        if (target != null)
        {
            // if the player is within range, then blow up
            if(Vector2.Distance(target.position, transform.position) <= explodeRange)
            {
                bExplode = true;
            }
            else if(Vector2.Distance(target.position, transform.position) > explodeRange && !bExplode)
            {
                attackPlayer = true;
            }

            if(bExplode)
            {
                attackPlayer = false;

               // Debug.Log("Explode");

                if (detonateTimer >= 0)
                {

                    Strobe();
                    detonateTimer -= Time.deltaTime;
                }
                else
                {
                    circleCollider.enabled = true;

                    StartCoroutine(Explode());

                     // if the player is in range when the enemy explodes, they take damage
                    if(Vector2.Distance(target.position, transform.position) <= explodeRange)
                        gameHandlerObj.TakeDamage(damageAmount);

                    detonateTimer = 3f;
                }
            }

            if (attackPlayer == true)
            {
                //transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

                //pather.computePath(this.transform.position, target.position);

                List<Vector3> path = pather.computePath(transform.position, target.position);

                if (path != null)
                {
                    //for (int i = 0; i < path.Count - 1; ++i)
                    //{
                    //    transform.position = Vector2.MoveTowards(transform.position, path[i], speed * Time.deltaTime);

                    //    //GameObject.Instantiate(explosionObj, path[i], Quaternion.identity);
                    //}

                    transform.position = Vector2.MoveTowards(transform.position, path[0], speed * Time.deltaTime);

                }
            }
            else if (attackPlayer == false)
            {
                transform.position = Vector2.MoveTowards(transform.position, target.position, speed * 0.0f * Time.deltaTime);
            }
        }
    }

    private void Strobe()
    {
        if (strobeDelayTimer <= 0f)
        {
            strobeDelayTimer = strobeDelay;

            toggle = !toggle;

            if (toggle)
                spriteRenderer.enabled = true;
            else
                spriteRenderer.enabled = false;
        }
        else
            strobeDelayTimer -= Time.deltaTime;
    }

    IEnumerator Explode()
    {
        spriteRenderer.color = new Color(2.0f, 1.0f, 0.0f, 0.5f); // changes color of enemy to yellow
       GameObject test =  Instantiate(explosionObj.gameObject, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(.5f); // waits so that the color can actually change before it is destroyed
        
        
        
        Destroy(gameObject);

        
        bExplode = false;

       // yield return new WaitForSeconds(1f);

       Destroy(test);
    }
}
