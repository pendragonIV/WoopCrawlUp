using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Head : MonoBehaviour
{
    public event Action OnHeadCollision, OnReachDestination, OnHeadStay;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            if (GameManager.instance.playerScript.GetLastMoveDirection() == Vector2.left || GameManager.instance.playerScript.GetLastMoveDirection() == Vector2.right)
            {
                if (transform.position.y <= collision.transform.position.y + .5f && transform.position.y >= collision.transform.position.y - .5f)
                {
                    OnHeadCollision?.Invoke();
                }
            }
            else
            {
                if (transform.position.x <= collision.transform.position.x + .5f && transform.position.x >= collision.transform.position.x - .5f)
                {
                    OnHeadCollision?.Invoke();
                }
            }
        }
        else if (collision.gameObject.CompareTag("Tail"))   
        {
            if (GameManager.instance.playerScript.GetLastMoveDirection() == Vector2.left || GameManager.instance.playerScript.GetLastMoveDirection() == Vector2.right)
            {
                if (collision.gameObject.GetComponent<Tail>().GetTailType() == TailType.Vertical)
                {
                    OnHeadCollision?.Invoke();
                }
            }
            else
            {
                if (collision.gameObject.GetComponent<Tail>().GetTailType() == TailType.Horizontal)
                {
                    OnHeadCollision?.Invoke();
                }
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            if (GameManager.instance.playerScript.GetLastMoveDirection() == Vector2.left || GameManager.instance.playerScript.GetLastMoveDirection() == Vector2.right)
            {
                if (transform.position.y <= collision.transform.position.y + .5f && transform.position.y >= collision.transform.position.y - .5f)
                {
                    OnHeadStay?.Invoke();
                }
            }
            else
            {
                if (transform.position.x <= collision.transform.position.x + .5f && transform.position.x >= collision.transform.position.x - .5f)
                {
                    OnHeadStay?.Invoke();
                }
            }
        }
        else
        {
            if (collision.gameObject.CompareTag("Tail"))
            {
                if (GameManager.instance.playerScript.GetLastMoveDirection() == Vector2.left || GameManager.instance.playerScript.GetLastMoveDirection() == Vector2.right)
                {
                    if (collision.gameObject.GetComponent<Tail>().GetTailType() == TailType.Vertical)
                    {
                        OnHeadStay?.Invoke();
                    }
                }
                else
                {
                    if (collision.gameObject.GetComponent<Tail>().GetTailType() == TailType.Horizontal)
                    {
                        OnHeadStay?.Invoke();
                    }
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Destination"))
        {
            OnReachDestination?.Invoke();

        }
    }

}
