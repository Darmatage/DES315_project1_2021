﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraFollowRS : MonoBehaviour
{
    private Transform player;
    private List<Transform> enemies;
    List<Transform> notInList;
    
    [Range(0.1f, 10.0f)] public float TrackingSpeed = 1.0f;
    [Range(1.0f, 50.0f)] public float TargetingRadius = 10.0f;

    [Range(0.0f, 1.0f)] public float PlayerWeight = 1.0f;
    [Range(0.0f, 1.0f)] public float EnemyBehindWeight = 0.4f;
    [Range(0.0f, 1.0f)] public float EnemyAheadWeight = 0.6f;

    [Range(0.0f, 5.0f)] public float FadeInTime; 
    [Range(0.0f, 5.0f)] public float GoalShowcaseTime; 
    [Range(0.0f, 10.0f)] public float LevelPanTime;
    [Range(0.0f, 5.0f)] public float StartShowcaseTime;
    private float fadeInTimer;
    private float goalTimer;
    private float panTimer;
    private float startShowcaseTimer;
    private Vector3 originalPosition;

    public Tilemap Walls;
    public Vector3Int[] TileWalls;
    private bool deletedWalls = false;

    public SpriteRenderer Fade;

    private Camera camera;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        enemies = new List<Transform>();
        notInList = new List<Transform>();
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Enemy"))
            enemies.Add(obj.transform);

        fadeInTimer = FadeInTime;
        goalTimer = GoalShowcaseTime;
        panTimer = LevelPanTime;
        startShowcaseTimer = StartShowcaseTime;
        
        originalPosition = transform.position;
        Fade.color = Color.black;

        camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeInTimer > 0.0f)
        {
            fadeInTimer -= Time.deltaTime;
            
            if (fadeInTimer < FadeInTime / 2.0f)
                Fade.color = Color.Lerp(Color.black, Color.clear, 1.0f - fadeInTimer / (FadeInTime / 2.0f));
        }
        else if (goalTimer > 0.0f)
            goalTimer -= Time.deltaTime;
        else if (panTimer > 0.0f)
        {
            panTimer -= Time.deltaTime;
            
            Vector3 newPos = Vector2.Lerp(originalPosition, player.position, Mathf.Pow(1.0f - panTimer / LevelPanTime, 0.75f));
            newPos.z = transform.position.z;
            transform.position = newPos;

            camera.orthographicSize = Mathf.Lerp(6.0f, 8.0f, 1.0f - panTimer / LevelPanTime);
        }
        else if (startShowcaseTimer > 0.0f)
        {
            startShowcaseTimer -= Time.deltaTime;
            camera.orthographicSize = Mathf.Lerp(8.0f, 6.0f, Mathf.Pow(1.0f - startShowcaseTimer / StartShowcaseTime, 2.0f));
        }
        else
        {
            if (!deletedWalls)
            {
                deletedWalls = true;
                foreach (Vector3Int pos in TileWalls)
                {
                    Walls.SetTile(pos, null);
                }
            }
            
            CameraFollow();
        }
    }

    // Follow the player and also track nearby enemies
    private void CameraFollow()
    {
        Vector3 target = player.position * PlayerWeight;
        
        int numNearbyEnemiesBehind = 0;
        int numNearbyEnemiesAhead = 0;
        
        foreach (Transform enemy in enemies)
        {
            if (enemy == null)
            {
                notInList.Add(enemy);
                continue;
            }

            if (Vector2.Distance(player.position, enemy.position) < TargetingRadius)
            {
                if (enemy.position.x > player.position.x)
                {
                    target += enemy.position * EnemyAheadWeight;
                    numNearbyEnemiesAhead++;
                }
                else
                {
                    target += enemy.position * EnemyBehindWeight;
                    numNearbyEnemiesBehind++;
                }
            }
        }

        target /= PlayerWeight + EnemyBehindWeight * numNearbyEnemiesBehind + EnemyAheadWeight * numNearbyEnemiesAhead;
        target.z = transform.position.z;

        transform.position = Vector3.Lerp(transform.position, target, TrackingSpeed * Time.deltaTime);
        
        if (notInList.Any())
        {
            enemies = enemies.Except(notInList).ToList();
            notInList.Clear();
        }
    }
}