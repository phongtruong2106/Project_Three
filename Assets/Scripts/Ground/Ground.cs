using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    private Player player;
    public float groundHeight;
    public float groundRight;
    public float screenRight;
    private new BoxCollider2D collider;

    private bool didGenerateGround = false;
    private Vector2 originalPosition;

    [SerializeField] Obstacle boxTemplate;

    private void Awake() {

        player = GameObject.Find("Player").GetComponent<Player>();
        collider = GetComponent<BoxCollider2D>();

        groundHeight = collider.bounds.max.y;
        screenRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;
        originalPosition = transform.position;
    }

    private void FixedUpdate() {

        Vector2 pos = transform.position;
        pos.x -= player.velocity.x * Time.fixedDeltaTime;
        groundRight = transform.position.x + (collider.size.x / 2);

        if(groundRight < -94.1f)
        {
            Destroy(gameObject);
            return;
        }

        if(!didGenerateGround)
        {
            if(groundRight < screenRight)
            {
                didGenerateGround = true;
                generateGround();
            }
        }

        transform.position = pos;
    }

    private void generateGround()
    {
        GameObject go = Instantiate(gameObject);
        BoxCollider2D goCollider = go.GetComponent<BoxCollider2D>();
        Vector2 pos;    

        float  h1 = player.jumpVelocity + player.maxHoldJumpTime;
        float t = player.jumpVelocity / -player.gravity;
        float h2 = player.jumpVelocity * t + (0.7f  * (player.gravity * (t * t)));
        float maxJumpHeight = h1 + h2;
        float maxY =  maxJumpHeight  * 0.25f;
        maxY += groundHeight;
        float minY = -67.9f;
        float actualY = Random.Range(minY, maxY) - goCollider.size.y / 2;

        pos.y = actualY;
        if(pos.y > -94.1f)
        {
            pos.y = -94.1f;
        }

        float t1 = t + player.maxHoldJumpTime;
        float t2 = Mathf.Sqrt((2f * (maxY - actualY)) / -player.gravity);
        float totalTime = t1 + t2;
        float maxX= totalTime * player.velocity.x;
        maxX *= 0.1f;
        maxX += groundRight;
        float minX = screenRight + 20;
        float actualX = Random.Range(minX, maxX);

        pos.x = actualX + goCollider.size.x / 2;
        
        go.transform.position = pos;

        // Update 'groundHeight' of 'go'
        Ground goGround = go.GetComponent<Ground>();
        goGround.groundHeight = go.transform.position.y + (goCollider.size.y / 2);
        goGround.screenRight = screenRight;
        Debug.Log("actualY: " + actualY);
        Debug.Log("actualX: " + actualX);


        // Update box random'
        int odstacleNum = Random.Range(0, 4);
        for(int i = 0; i <  odstacleNum ; i++)
        {
            GameObject box = Instantiate(boxTemplate.gameObject);
            
            float y = goGround.groundHeight;
            float halfWidth = goCollider.size.x /2 -1 ;
            float left  = go.transform.position.x - halfWidth;
            float right = go.transform.position.x + halfWidth;
            float x= Random.Range(left, right);


            Vector2 boxPos = new Vector2(x, y); 
            box.transform.position = boxPos;
        }
    }
}
