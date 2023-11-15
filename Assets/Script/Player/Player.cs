using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Playables;

enum PlayerState
{
    Idle,
    Moving
}

public class Player : MonoBehaviour
{
    [SerializeField]
    LineRenderer lineRenderer;
    [SerializeField]
    private GameObject linePrefab;
    [SerializeField]
    private Rigidbody2D rb;
    [SerializeField]
    private PlayerState playerState;
    [SerializeField]
    private Transform head;
    [SerializeField]
    private GameObject headGO;
    [SerializeField]
    private GameObject tailGO;
    [SerializeField]
    private GameObject spawnHole;

    [SerializeField]
    private Transform lineContainer;
    [SerializeField]
    private List<GameObject> lineList;
    [SerializeField]
    private GameObject currentLine;

    private LineRenderer currentTail = null;
    private List<Vector2> linePoints;

    #region Movement variables

    [SerializeField]
    private Vector2 currentVelocity;
    [SerializeField]
    private Vector2 workspaceVelocity;

    [SerializeField]
    private Vector2 mouseDownPosition;
    [SerializeField]
    private Vector2 mouseUpPosition;
    [SerializeField]
    private Vector2 moveDirection;

    [SerializeField]
    private float maxSpeed;

    #endregion

    #region Game status

    [SerializeField]
    private bool isMoving = false;
    [SerializeField]
    private bool isCanControl = true;

    #endregion

    private void Awake()
    {
        maxSpeed = 20f;

        head.GetComponent<Head>().OnHeadCollision += OnHeadCollision;
        head.GetComponent<Head>().OnReachDestination += OnReachDestination;
        head.GetComponent<Head>().OnHeadStay += OnHeadStay;
    }

    private void Start()
    {
        linePoints = new List<Vector2>();
        tailGO.transform.position = head.position;
        spawnHole.transform.position = head.position;
        InitNewLine();
    }

    private void Update()
    {
        CheckLose();

        if (isCanControl)
        {
            InputHandler();
        }
        currentVelocity = rb.velocity;

        if (isMoving)
        {
            playerState = PlayerState.Moving;
        }
    }

    private void FixedUpdate()
    {
        ChangePlayerBehavior();
        if(lineRenderer != null)
        {
            lineRenderer.SetPosition(1, head.position);
        }

        if (currentTail != null)
        {
            currentTail.SetPosition(0, tailGO.transform.position);
        }
    }

    private void CheckLose()
    {
        if (!GameManager.instance.IsGameLose())
        {
            if (head.position.x < -10 || head.position.x > 10 || head.position.y < -10 || head.position.y > 10)
            {
                GameManager.instance.Lose();
            }
        }
    }

    private void InitNewLine()
    {
        currentLine = Instantiate(linePrefab, lineContainer);
        currentLine.name = "Line " + lineList.Count;
        lineRenderer = currentLine.GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, head.position);
        lineRenderer.SetPosition(1, head.position);

        if (lineList.Count >= 2)
        {
            linePoints.Clear();
            linePoints.Add(currentLine.transform.InverseTransformPoint(lineList[lineList.Count - 2].GetComponent<LineRenderer>().GetPosition(0)));
            linePoints.Add(currentLine.transform.InverseTransformPoint(lineList[lineList.Count - 2].GetComponent<LineRenderer>().GetPosition(1)));
            lineList[lineList.Count - 2].GetComponent<EdgeCollider2D>().points = linePoints.ToArray();
            lineList[lineList.Count - 2].GetComponent<EdgeCollider2D>().enabled = true;
        }

        lineList.Add(currentLine);
    }


    private IEnumerator MoveTail()
    {
        int child = lineContainer.childCount;
        for (int i = 0; i < child; i++)
        {
            currentTail = lineContainer.GetChild(0).GetComponent<LineRenderer>();

            Vector3 movePos = currentTail.GetPosition(1);
            yield return tailGO.transform.DOMove(movePos, 0.3f).WaitForCompletion();

            Destroy(currentTail.gameObject);
            yield return null;
        }

        GameManager.instance.Win();
    }


    private void ChangePlayerBehavior()
    {
        switch (playerState)
        {
            case PlayerState.Idle:
                {
                    ChangeVelocityX(0);
                    ChangeVelocityY(0);
                    break;
                }
            case PlayerState.Moving:
                {
                    isCanControl = false;
                    Moving(moveDirection);
                    if(moveDirection == Vector2.left || moveDirection == Vector2.right)
                    {
                        currentLine.GetComponent<Tail>().SetTailType(TailType.Horizontal);
                    }
                    else
                    {
                        currentLine.GetComponent<Tail>().SetTailType(TailType.Vertical);
                    }
                    break;
                }
            default:
                {
                    break;
                }
        }
    }

    #region Events
    private void OnReachDestination()
    {
        isMoving = false;
        playerState = PlayerState.Idle;
        headGO.GetComponent<SpriteRenderer>().enabled = false;
        StartCoroutine(MoveTail());
    }
    private void OnHeadCollision()
    {
        isMoving = false;
        isCanControl = true;
        playerState = PlayerState.Idle;
        currentLine.GetComponent<LineRenderer>().SetPosition(1, head.position);
        InitNewLine();
    }

    private void OnHeadStay()
    {
        isCanControl = true;
    }
    #endregion

    public Vector2 GetLastMoveDirection()
    {
        return moveDirection;
    }

    private void Moving(Vector2 direction)
    {
        if (direction.x != 0)
        {
            ChangeVelocityX(direction.x * maxSpeed);
        }
        else if (direction.y != 0)
        {
            ChangeVelocityY(direction.y * maxSpeed);
        }   
    }

    private void InputHandler()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                mouseDownPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            if (Input.GetMouseButtonUp(0))
            {
                mouseUpPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 lastMoveDirection = moveDirection;
                CalculateDirection(mouseDownPosition, mouseUpPosition);
                if (lastMoveDirection == Vector2.left || lastMoveDirection == Vector2.right)
                {
                    if (moveDirection == Vector2.down || moveDirection == Vector2.up)
                    {
                        isMoving = true;
                    }
                }
                else if (lastMoveDirection == Vector2.zero)
                {
                    isMoving = true;
                }
                else
                {
                    if (moveDirection == Vector2.left || moveDirection == Vector2.right)
                    {
                        isMoving = true;
                    }
                }
            }
        }
    }

    private void CalculateDirection(Vector2 startPos, Vector2 endPos)
    {
        if (Mathf.Abs(startPos.x - endPos.x) > Mathf.Abs(startPos.y - endPos.y))
        {
            if (startPos.x > endPos.x)
            {
                moveDirection = Vector2.left;
            }
            else
            {
                moveDirection = Vector2.right;
            }
        }
        else
        {
            if (startPos.y > endPos.y)
            {
                moveDirection = Vector2.down;
            }
            else
            {
                moveDirection = Vector2.up;
            }
        }
    }

    private void ChangeVelocityX(float X)
    {
        workspaceVelocity.Set(X, 0);
        currentVelocity = workspaceVelocity;
        rb.velocity = currentVelocity;
    }

    private void ChangeVelocityY(float Y)
    {
        workspaceVelocity.Set(0, Y);
        currentVelocity = workspaceVelocity;
        rb.velocity = currentVelocity;
    }
}
