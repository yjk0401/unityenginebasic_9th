using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    public Vector2 top => (Vector2)transform.position +
                           _bound.offset +
                           Vector2.up * _bound.size.y / 2.0f;

    public Vector2 bottom => (Vector2)transform.position +
                              _bound.offset +
                              Vector2.down * _bound.size.y / 2.0f;

    public Vector2 upStartPos => bottom +
                                 Vector2.down * _upStartOfsetY;

    public Vector2 upEndPos => top +
                               Vector2.down * _upEndOfsetY;

    public Vector2 downStartPos => top + 
                                   Vector2.down * _downStartOfsetY;     
    
    public Vector2 downEndPos => bottom +
                                 Vector2.down * _downEndOfsetY;

    [SerializeField] private float _upStartOfsetY = 0.03f;
    [SerializeField] private float _upEndOfsetY = 0.03f;
    [SerializeField] private float _downStartOfsetY = 0.05f;
    [SerializeField] private float _downEndOfsetY = 0.05f;
    private BoxCollider2D _bound;

    private void Awake()
    {
        _bound = GetComponent<BoxCollider2D>();   
    }

    private void OnDrawGizmos()
    {
        if (_bound == null)
            _bound = GetComponent<BoxCollider2D>();

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(Vector3.left  * 0.1f + (Vector3)upStartPos,
                        Vector3.right * 0.1f + (Vector3)upStartPos);
        Gizmos.DrawLine(Vector3.left  * 0.1f + (Vector3)upEndPos,
                        Vector3.right * 0.1f + (Vector3)upEndPos);

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(Vector3.left  * 0.1f + (Vector3)downStartPos,
                        Vector3.right * 0.1f + (Vector3)downStartPos);
        Gizmos.DrawLine(Vector3.left  * 0.1f + (Vector3)downEndPos,
                        Vector3.right * 0.1f + (Vector3)downEndPos);
    }
}
