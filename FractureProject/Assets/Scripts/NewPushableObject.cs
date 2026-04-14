using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class NewPushableObject : MonoBehaviour
{
    public float unitsPerPush = 1f;
    public float pushSpeed = 5f;
    public float pushDelay = 0.5f;
    
    public LayerMask obstacleLayer;

    private bool isMoving = false;
    private float pushTimer = 0f;
    
    private Rigidbody rb;
    private BoxCollider boxCol;

    public bool isPlayerNear, onX, onZ;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        boxCol = GetComponent<BoxCollider>();
        baseSprite = spriteRenderer.sprite;
        if (spriteWhenPush == null)
            spriteWhenPush = baseSprite;
        
        rb.isKinematic = true; 
    }

    private void Update()
    {
        if (isPlayerNear)
        {
            spriteRenderer.sprite = spriteWhenPush;
            if (isMoving) return;
            if (Input.GetKey(KeyCode.Q) || Input.GetButton("Fire1"))
            {
                Player.instance.locked = true;
                Vector3 dir = new Vector3(0f, 0f, 0f);
                if(onX)dir +=(new Vector3(Input.GetAxis("Horizontal"), 0f, 0f));
                if(onZ)dir +=(new Vector3( 0f, 0f,Input.GetAxis("Vertical")));
                pushTimer += Time.deltaTime;

                if (pushTimer >= pushDelay)
                {
                    TryPush(dir.normalized);
                }
            }
            else
            {
                Player.instance.locked = false;
            }
        }
        else
        {
            spriteRenderer.sprite = baseSprite;
        }
    }

    public SpriteRenderer spriteRenderer;
    public Sprite spriteWhenPush;
    private Sprite baseSprite;

    private void TryPush(Vector3 direction)
    {
        Vector3 testSize = boxCol.size * 0.45f; 
        Vector3 center = transform.TransformPoint(boxCol.center);

        bool isBlocked = Physics.BoxCast(center, testSize, direction, transform.rotation, unitsPerPush, obstacleLayer);

        if (!isBlocked)
        {
            StartCoroutine(Push(direction));
        }
    }

    private IEnumerator Push(Vector3 direction)
    {
        isMoving = true;

        Vector3 startPos = rb.position;
        Vector3 targetPos = startPos + (direction * unitsPerPush);
        Vector3 offset = startPos - Player.instance.transform.position;
        
        while (Vector3.Distance(rb.position, targetPos) > 0.01f)
        {
            Vector3 newPos = Vector3.MoveTowards(rb.position, targetPos, pushSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
            Player.instance.rb.MovePosition(newPos-offset);
            yield return new WaitForFixedUpdate();
        }

        rb.MovePosition(targetPos);
        pushTimer = 0f;
        isMoving = false;
    }

    private Vector3 GetSnappyDirection(Vector3 inputDir)
    {
        if (Mathf.Abs(inputDir.x) > Mathf.Abs(inputDir.z))
            return new Vector3(Mathf.Sign(inputDir.x), 0, 0);
        else
            return new Vector3(0, 0, Mathf.Sign(inputDir.z));
    }
}