using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class Crystals : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField]
    private GameObject _crystalPrefab;

    [SerializeField]
    private LayerMask layerMask;

    private List<Collider2D> _connectedCrystals = new();

    private Rigidbody2D _crystalRigidbody;


    [SerializeField]
    private float _detectionRange = 3f;

    [SerializeField]
    private float _frequencity = 5f;

    [SerializeField]
    private float _distance = 2f;

    [SerializeField]
    private float _dampingRatio = 10f;

    private void Awake()
    {
        _crystalRigidbody = gameObject.GetComponentInParent<Rigidbody2D>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("down");
        _crystalRigidbody.simulated = false;

        //Delete links if theres any then check for some and show them

        

        CheckLinks();
    }

    public void OnDrag(PointerEventData eventData)
    {
        gameObject.transform.position = eventData.pointerCurrentRaycast.worldPosition;
        _crystalRigidbody.velocity = Vector2.zero;

        CheckLinks();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_crystalRigidbody == null)
        {
            return;
        }

        _crystalRigidbody.simulated = true;
        _crystalRigidbody.velocity = Vector2.zero;

        foreach (Collider2D crystalCollider in _connectedCrystals)
        {
            GameObject otherCrystal = crystalCollider.gameObject;
            if (otherCrystal == null || 1 << otherCrystal.layer != layerMask)
            {
                Debug.Log("skipped");
                continue;
            }

            SpringJoint2D joint = gameObject.AddComponent<SpringJoint2D>();
            joint.connectedBody = otherCrystal.GetComponent<Rigidbody2D>();
            joint.autoConfigureDistance = false;
            joint.distance = _distance;
            joint.frequency = _frequencity;
            joint.dampingRatio = _dampingRatio;
            joint.enableCollision = true;
        }

        _crystalRigidbody = null;
        _connectedCrystals.Clear();

        Debug.Log("up");
    }

    public void CheckLinks()
    {
        _connectedCrystals = Physics2D.OverlapCircleAll(gameObject.transform.position, _detectionRange, layerMask).ToList();
        //Debug.Log(1 + LayerMask.NameToLayer("Crystals"));
        //Debug.Log(_connectedCrystals[0]);
        //Debug.Log(_connectedCrystals[1]);
    }
}
