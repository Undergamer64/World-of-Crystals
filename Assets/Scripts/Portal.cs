using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField]
    private LayerMask _layerMask;

    private List<GameObject> _currentObjectsInPortal = new List<GameObject>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (1 << collision.gameObject.layer != _layerMask)
        {
            return;
        }

        if (_currentObjectsInPortal.Count <= 0)
        {
            GameManager.Instance.EndLevel();
        }
        _currentObjectsInPortal.Add(collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (1 << collision.gameObject.layer != _layerMask)
        {
            return;
        }

        _currentObjectsInPortal.Remove(collision.gameObject);
        if (_currentObjectsInPortal.Count == 0)
        {
            GameManager.Instance.CancelEndLevel();
        }
    }

}
