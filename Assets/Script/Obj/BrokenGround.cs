using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenGround : MonoBehaviour
{
    private Explodable _explodable;

    void Start()
    {
        _explodable = GetComponent<Explodable>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (GameManager.instance.playerScript.GetLastMoveDirection() == Vector2.left || GameManager.instance.playerScript.GetLastMoveDirection() == Vector2.right)
            {
                if (transform.position.y <= collision.transform.position.y + .5f && transform.position.y >= collision.transform.position.y - .5f)
                {
                    _explodable.explode();
                    ExplosionForce ef = GameObject.FindObjectOfType<ExplosionForce>();
                    ef.doExplosion(transform.position);
                }
            }
            else
            {
                if (transform.position.x <= collision.transform.position.x + .5f && transform.position.x >= collision.transform.position.x - .5f)
                {
                    _explodable.explode();
                    ExplosionForce ef = GameObject.FindObjectOfType<ExplosionForce>();
                    ef.doExplosion(transform.position);
                }
            }

        }
    }
}
