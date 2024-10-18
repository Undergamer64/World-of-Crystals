using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField]
    private LayerMask _layerMask;

    private List<Collider2D> _currentObjectsInPortal = new List<Collider2D>();
    private BoxCollider2D _portalRange;

    private void Start()
    {
        _portalRange = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (1 << collision.gameObject.layer != _layerMask)
        {
            return;
        }
        GameManager.Instance.EndLevel();
        _currentObjectsInPortal.Add(collision);
    }

    private void Update()
    {
        _currentObjectsInPortal = Physics2D.OverlapBoxAll(transform.position, _portalRange.size*transform.parent.transform.localScale, 0, _layerMask).ToList();
        if (_currentObjectsInPortal.Count <= 0)
        {
            GameManager.Instance.CancelEndLevel();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (1 << collision.gameObject.layer != _layerMask)
        {
            return;
        }

        _currentObjectsInPortal.Remove(collision);
        if (_currentObjectsInPortal.Count == 0)
        {
            GameManager.Instance.CancelEndLevel();
        }
    }

}
