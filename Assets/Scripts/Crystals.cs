using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class Crystals : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField]
    private GameObject _crystalPrefab;

    [SerializeField]
    private GameObject _linkPrefab;

    [SerializeField]
    private LayerMask layerMask;

    private List<Collider2D> _closeCrystals = new();
    public List<LinkScript> ConnectedCrystalsLinks = new();
    public List<SpringJoint2D> ConnectedCrystals = new();

    private Rigidbody2D _crystalRigidbody;


    [SerializeField]
    private float _detectionRange = 3f;

    [SerializeField]
    private float _frequencity = 5f;

    [SerializeField]
    private float _minDistance = 1f;

    [SerializeField]
    private float _dampingRatio = 10f;

    private void Awake()
    {
        _crystalRigidbody = gameObject.GetComponent<Rigidbody2D>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _crystalRigidbody.simulated = false;

        //Delete links if theres any
        DeleteAllLinks();

        //Checks for links and show them
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

        if (!CanConnect())
        {
            _closeCrystals.Clear();
            return;
        }
        foreach (Collider2D crystalCollider in _closeCrystals)
        {
            GameObject otherCrystal = crystalCollider.gameObject;
            if (otherCrystal == null || 1 << otherCrystal.layer != layerMask)
            {
                continue;
            }

            LinkScript link = Instantiate(_linkPrefab, transform).GetComponent<LinkScript>();
            SpringJoint2D joint = gameObject.AddComponent<SpringJoint2D>();
            link.InitiatlizeLink(gameObject, otherCrystal, joint);

            ConnectedCrystalsLinks.Add(link);

            joint.connectedBody = otherCrystal.GetComponent<Rigidbody2D>();
            joint.autoConfigureDistance = false;
            joint.distance = Mathf.Clamp(joint.distance, _minDistance, _detectionRange);
            joint.frequency = _frequencity;
            joint.dampingRatio = _dampingRatio;
            joint.enableCollision = true;

            otherCrystal.GetComponent<Crystals>().ConnectedCrystals.Add(joint);
            
        }

        _closeCrystals.Clear();
    }

    public bool CanConnect()
    {
        int connectionNumber = 0;

        foreach (Collider2D crystalCollider in _closeCrystals)
        {
            GameObject otherCrystal = crystalCollider.gameObject;
            if (otherCrystal != null && 1 << otherCrystal.layer == layerMask)
            {
                connectionNumber ++;
                if (connectionNumber >= 2)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void CheckLinks()
    {
        _closeCrystals = Physics2D.OverlapCircleAll(gameObject.transform.position, _detectionRange, layerMask).ToList();

        if (CanConnect())
        {
            //ShowPreview();
        }
    }
    
    //Checks if theres is at least 2 links
    public bool IsConnected(int destroyedComponentsNumber = 0)
    {
        int links = ConnectedCrystals.Count + GetComponents<SpringJoint2D>().Length;
        
        return links - destroyedComponentsNumber > 1;
    }

    public void DeleteAllLinks()
    {
        List<SpringJoint2D> springJoint2Ds = GetComponents<SpringJoint2D>().ToList();

        ConnectedCrystalsLinks.Clear();
        foreach (LinkScript link in GetComponentsInChildren<LinkScript>())
        {
            Destroy(link.gameObject);
        }
        foreach (SpringJoint2D joint in springJoint2Ds)
        {
            Crystals connectedCrystalScript = joint.connectedBody.GetComponent<Crystals>();
            connectedCrystalScript.ConnectedCrystals.Remove(joint);
            Destroy(joint);
            if (!connectedCrystalScript.IsConnected()) connectedCrystalScript.DeleteAllLinks();
        }

        for (int i = 0; i < ConnectedCrystals.Count;)
        {
            SpringJoint2D joint = ConnectedCrystals[i];
            Crystals connectedCrystalScript = joint.GetComponent<Crystals>();
            ConnectedCrystals.Remove(joint);
            foreach (LinkScript link in connectedCrystalScript.gameObject.GetComponentsInChildren<LinkScript>())
            {
                if (link.Joint == joint)
                {
                    connectedCrystalScript.ConnectedCrystalsLinks.Remove(link);
                    Destroy(link.gameObject);
                    break;
                }
            }
            Destroy(joint);
            if (!connectedCrystalScript.IsConnected(1)) connectedCrystalScript.DeleteAllLinks();
        }
    }
}
