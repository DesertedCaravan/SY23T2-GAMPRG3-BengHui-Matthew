using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Movement")]
    public bool _movementState;
    public bool MovementState => _movementState;

    public float _moveSpeed = 5.0f; // speed rate
    public float _zRotation = 0f; // current rotation of player

    public Transform movePoint; // position of player
    public Transform playerDirection; // rotation of player

    public LayerMask movementBlocker;

    [Header("Player Menu")]
    public bool _isSpeaking;
    public bool IsSpeaking => _isSpeaking;

    // Convert to Singleton
    public static PlayerController instance = null; // public static means that it can be accessed

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        movePoint.parent = null; // To ensure that the Move Point is a separate Game Object. This allows the player to move to the Move Point without shifting it along (due to being a child of the player).
        
        _movementState = true; // Allow player to recieve movement inputs

        _isSpeaking = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (_movementState)
        {
            // Player Rotation
            if (Input.GetAxisRaw("Horizontal") == -1f)
            {
                playerDirection.transform.eulerAngles = new Vector3(0, 0, 270f);
            }
            else if (Input.GetAxisRaw("Horizontal") == 1f)
            {
                playerDirection.transform.eulerAngles = new Vector3(0, 0, 90f);
            }
            else if (Input.GetAxisRaw("Vertical") == -1f)
            {
                playerDirection.transform.eulerAngles = new Vector3(0, 0, 0f);
            }
            else if (Input.GetAxisRaw("Vertical") == 1f)
            {
                playerDirection.transform.eulerAngles = new Vector3(0, 0, 180f);
            }

            // Player Movement
            transform.position = Vector3.MoveTowards(transform.position, movePoint.position, _moveSpeed * Time.deltaTime); // move Player position to Move Point position at a rate consistent to all machines (ie. Time.deltaTime)

            if (Vector3.Distance(transform.position, movePoint.position) <= 0.01f) // Only accept inputs if the player is not moving / not far from the Move Point (the difference in position gives leeway to the player's movement inputs)
            {
                if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f) // Use Mathf.Abs to check if the player is pressing all the way to the left or right
                {
                    if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f), 0.2f, movementBlocker)) // Checks if there is NO collider of a specific layer falls within a circular area
                    {
                        movePoint.position += new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f);

                        if (Input.GetAxisRaw("Horizontal") == -1f)
                        {
                            playerDirection.transform.eulerAngles = new Vector3(0, 0, 270f);
                        }
                        else if (Input.GetAxisRaw("Horizontal") == 1f)
                        {
                            playerDirection.transform.eulerAngles = new Vector3(0, 0, 90f);
                        }
                    }
                }
                else if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f) // Use Mathf.Abs to check if the player is pressing all the way to the down or up
                {
                    if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f), 0.2f, movementBlocker)) // Checks if there is NO collider of a specific layer falls within a circular area
                    {
                        movePoint.position += new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f);

                        if (Input.GetAxisRaw("Vertical") == -1f)
                        {
                            playerDirection.transform.eulerAngles = new Vector3(0, 0, 0f);
                        }
                        else if (Input.GetAxisRaw("Vertical") == 1f)
                        {
                            playerDirection.transform.eulerAngles = new Vector3(0, 0, 180f);
                        }
                    }
                }
            }
        }
    }

    public void StartMovement()
    {
        _movementState = true;
        _isSpeaking = false;
    }

    public void StopMovement()
    {
        _movementState = false;
        _isSpeaking = true;
    }

    public bool PlayerMovementCheck()
    {
        if (Vector3.Distance(transform.position, movePoint.position) <= 0.01f)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void TogglePlayerMovement(bool menuState)
    {
        _movementState = menuState;
    }
}

/*
Rotation Method A:

Quaternion targetHor = Quaternion.Euler(0, 0, 90f);
playerDirection.rotation = Quaternion.Slerp(playerDirection.rotation, targetHor, _rotateSpeed * Time.deltaTime);

Rotation Method B (Continuous):

playerDirection.Rotate(0, 0, 270f, Space.Self);

Rotation Method C:

Quaternion rotationHor = Quaternion.LookRotation(movePoint.position, Vector3.forward);
playerDirection.rotation = rotationHor;
*/
