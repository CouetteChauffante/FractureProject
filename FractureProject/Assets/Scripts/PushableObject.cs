using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class PushableObject : MonoBehaviour
{
    public float unitsPerPush = 1f;
    public float pushSpeed = 5f;
    public float pushDelay = 0.5f;
    
    public LayerMask obstacleLayer;

    private bool isMoving = false;
    private float pushTimer = 0f;
    private Vector3 lastPushDirection = Vector3.zero;
    
    private Rigidbody rb;
    private BoxCollider boxCol;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        boxCol = GetComponent<BoxCollider>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        baseSprite = spriteRenderer.sprite;
        
        rb.isKinematic = true; 
    }

    private SpriteRenderer spriteRenderer;
    public Sprite spriteWhenPush;
    private Sprite baseSprite;

    private void OnCollisionStay(Collision collision)
    {
        if (isMoving) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            Vector3 playerDir = Player.instance.GetPushDirection();
            
            if (playerDir != Vector3.zero)
            {
                Vector3 dirToCrate = (transform.position - collision.transform.position).normalized;
                dirToCrate.y = 0;

                float pushAlignment = Vector3.Dot(playerDir, dirToCrate);

                if (pushAlignment > 0.7f) 
                {
                    Vector3 currentPushDir = GetSnappyDirection(playerDir);

                    if (currentPushDir != lastPushDirection)
                    {
                        pushTimer = 0f;
                        lastPushDirection = currentPushDir;
                    }

                    pushTimer += Time.deltaTime;

                    spriteRenderer.sprite = spriteWhenPush;

                    if (pushTimer >= pushDelay)
                    {
                        TryPush(currentPushDir);
                        ResetPushStatus(); 
                    }
                }
                else
                {
                    ResetPushStatus();
                }
            }
            else
            {
                ResetPushStatus();
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ResetPushStatus();
        }
    }

    private void ResetPushStatus()
    {
        pushTimer = 0f;
        lastPushDirection = Vector3.zero;
        spriteRenderer.sprite = baseSprite;
    }

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

        while (Vector3.Distance(rb.position, targetPos) > 0.01f)
        {
            Vector3 newPos = Vector3.MoveTowards(rb.position, targetPos, pushSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
            
            yield return new WaitForFixedUpdate();
        }

        rb.MovePosition(targetPos);
        
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