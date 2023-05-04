using System;
using System.Security.Cryptography.X509Certificates;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float gravity;
    public Vector2 velocity;

    [SerializeField] private float maxAcceleration = 10;
    [SerializeField] private float acceleration = 10;
    public float jumpVelocity = 20;
    [SerializeField] private float maxVelocity = 100;

    [SerializeField] public float distance  = 0;

    [SerializeField] private float groundHeight = 10;
    public float maxHoldJumpTime = 0.4f;
    [SerializeField] private float holdJumpTimer = 0.0f;
    [SerializeField] private float maxMaxHoldJumpTime = 0.4f;
    [SerializeField] private float jumpGroundThreshold = 1;

    public bool isGrounded = false;
    public bool isHoldingJump = false;


    private void Update() {

        Vector2 pos = transform.position;
        float groundDistance= Mathf.Abs(pos.y - groundHeight);
        if(isGrounded || groundHeight <= jumpGroundThreshold) 
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                isGrounded = false;
                velocity.y = jumpVelocity;
                isHoldingJump = true;
                 holdJumpTimer = 0;
            }
        }
        if(Input.GetKeyUp(KeyCode.Space))
        {
            isHoldingJump =false;
        }
    }

    private void FixedUpdate() {
        Vector2 pos = transform.position;

        if(!isGrounded)
        {
            if(isHoldingJump)
            {
                holdJumpTimer += Time.fixedDeltaTime;
                if(holdJumpTimer >= maxHoldJumpTime)
                {
                    isHoldingJump = false;
                }
            }

            pos.y += velocity.y * Time.fixedDeltaTime;
            if(!isHoldingJump)
            {
                 velocity.y += gravity * Time.fixedDeltaTime;
            }

            Vector2 rayOrigin = new Vector2(pos.x + 1f, pos.y);
            Vector2 rayDirection = Vector2.up;
            float rayDistance = velocity.y * Time.fixedDeltaTime;
            RaycastHit2D hit2D = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance);
            if(hit2D.collider != null)
            {
                Ground ground = hit2D.collider.GetComponent<Ground>();
                if(ground != null)
                {
                    groundHeight = ground.groundHeight;
                    pos.y = groundHeight;
                    velocity.y = 0;
                    isGrounded = true;
                }
            }
            Debug.DrawRay(rayOrigin, rayDirection * rayDistance,Color.red);
        }
        
        distance += velocity.x * Time.fixedDeltaTime;

        if(isGrounded)
        {
            float velocityRatio = velocity.x / maxVelocity;
            acceleration = maxAcceleration *  (1 - velocityRatio);
            maxHoldJumpTime = maxMaxHoldJumpTime * velocityRatio;

            velocity.x += acceleration + Time.fixedDeltaTime;
            if(velocity.x >= maxVelocity)
             {
                velocity.x = maxVelocity;
            }

            Vector2 rayOrigin = new Vector2(pos.x - 1f, pos.y);
            Vector2 rayDirection = Vector2.up;
            float rayDistance = velocity.y * Time.fixedDeltaTime;
            RaycastHit2D hit2D = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance);
            if(hit2D.collider == null)
            {
                isGrounded= false;
            }
            Debug.DrawRay(rayOrigin, rayDirection * rayDistance, Color.yellow);

        }

        Vector2 obstOrigin = new Vector2(pos.x, pos.y);
        RaycastHit2D obstHitX = Physics2D.Raycast(obstOrigin, Vector2.right,  velocity.x * Time.fixedDeltaTime);
        if(obstHitX.collider != null)
        {
            Obstacle obstacle = obstHitX.collider.GetComponent<Obstacle>();
            if(obstacle != null)
            {
                hitObstacle(obstacle);
            }
        }

        RaycastHit2D obstHitY = Physics2D.Raycast(obstOrigin, Vector2.up,  velocity.y * Time.fixedDeltaTime);
        if(obstHitY.collider != null)
        {
            Obstacle obstacle = obstHitY.collider.GetComponent<Obstacle>();
            if(obstacle != null)
            {
                hitObstacle(obstacle);
            }
        }
        transform.position = pos;
    }

    private void hitObstacle(Obstacle obstacle)
    {
        Destroy(obstacle.gameObject);

        velocity.x *= 0.7f;
    }
}

