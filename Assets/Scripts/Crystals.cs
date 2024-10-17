using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Crystals : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField]
    private GameObject _crystalPrefab;

    [SerializeField]
    private GameObject _linkPrefab;

    [SerializeField] 
    private bool _movable = true;

    [SerializeField]
    private LayerMask _layerMask;

    [SerializeField]
    private Sprite _energyBallSprite;

    [SerializeField]
    private Sprite _crystalSprite;

    private List<Collider2D> _closeCrystals = new();
    private List<Collider2D> _closeRedCrystals = new();
    public List<LinkScript> ConnectedCrystalsLinks = new();
    public List<SpringJoint2D> ConnectedCrystals = new();

    private Rigidbody2D _crystalRigidbody;
    
    [SerializeField]
    private float _redCrystalDetectionRange = 3f;

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
        if (!_movable)
        {
            return;
        }
        _crystalRigidbody.simulated = false;

        //Delete links if theres any
        DeleteAllLinks();

        //Checks for links and show them
        CheckLinks();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_movable)
        {
            return;
        }
        if (eventData.pointerCurrentRaycast.worldPosition != Vector3.zero)
        {
            gameObject.transform.position = eventData.pointerCurrentRaycast.worldPosition;
        }
        _crystalRigidbody.velocity = Vector2.zero;

        CheckLinks();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_crystalRigidbody == null || !_movable)
        {
            return;
        }

        CheckLinks();

        _crystalRigidbody.simulated = true;
        _crystalRigidbody.velocity = Vector2.zero;

        if (!CanConnect())
        {
            _closeCrystals.Clear();
            return;
        }
        foreach (LinkScript link in ConnectedCrystalsLinks)
        {
            SpringJoint2D joint = link.Joint;

            joint.enabled = true;
            link.GetComponent<LineRenderer>().startColor = Color.white;
            link.GetComponent<LineRenderer>().endColor = Color.white;
            joint.autoConfigureDistance = false;
            joint.distance = Mathf.Clamp(joint.distance, _minDistance, _detectionRange);
            joint.enableCollision = true;

            link.SecondCrystal().GetComponent<Crystals>().ConnectedCrystals.Add(joint);
            
        }

        _closeCrystals.Clear();
        transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        GetComponent<BoxCollider2D>().size = new Vector2(0.35f, 1);
        gameObject.layer = 10;
        GetComponent<SpriteRenderer>().sprite = _crystalSprite;
    }

    public bool CanConnect()
    {
        if (_closeRedCrystals.Count >= 1)
        {
            return false;
        }

        int connectionNumber = 0;

        foreach (Collider2D crystalCollider in _closeCrystals)
        {
            GameObject otherCrystal = crystalCollider.gameObject;
            if (otherCrystal != null && 1 << otherCrystal.layer == _layerMask)
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

    public void ShowPreview()
    {
        foreach (Collider2D crystalCollider in _closeCrystals)
        {
            GameObject otherCrystal = crystalCollider.gameObject;
            if (otherCrystal == null || 1 << otherCrystal.layer != _layerMask)
            {
                continue;
            }

            LinkScript link = Instantiate(_linkPrefab, transform).GetComponent<LinkScript>();
            SpringJoint2D joint = gameObject.AddComponent<SpringJoint2D>();
            link.InitiatlizeLink(gameObject, otherCrystal, joint);
            joint.enabled = false;
            joint.connectedBody = otherCrystal.GetComponent<Rigidbody2D>();
            link.GetComponent<LineRenderer>().startColor = new Color(1, 1, 1, 0.25f);
            link.GetComponent<LineRenderer>().endColor = new Color(1, 1, 1, 0.25f);
            joint.frequency = _frequencity;
            joint.dampingRatio = _dampingRatio;

            ConnectedCrystalsLinks.Add(link);
        }
    }

    public void CheckLinks()
    {
        _closeCrystals.Clear();

        _closeRedCrystals = Physics2D.OverlapCircleAll(gameObject.transform.position, _redCrystalDetectionRange, 1 << 12).ToList();

        _closeCrystals = Physics2D.OverlapCircleAll(gameObject.transform.position, _detectionRange, _layerMask).ToList();


        DeleteAllLinks();

        if (CanConnect())
        {
            ShowPreview();
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
        gameObject.layer = 0;
        GetComponent<SpriteRenderer>().sprite = _energyBallSprite;
        transform.localScale = Vector3.one;
        GetComponent<BoxCollider2D>().size = new Vector2(1, 1);

        ConnectedCrystalsLinks.Clear();
        foreach (LinkScript link in GetComponentsInChildren<LinkScript>())
        {
            Destroy(link.gameObject);
        }
        foreach (SpringJoint2D joint in GetComponents<SpringJoint2D>().ToList())
        {
            if (joint.connectedBody == null)
            {
                Destroy(joint);
                continue;
            }
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
