using UnityEngine;

public class LinkScript : MonoBehaviour
{
    [SerializeField]
    private LineRenderer _lineRenderer;

    public SpringJoint2D Joint;

    private GameObject _firstCrystal;
    private GameObject _secondCrystal;

    public GameObject FirstCrystal() { return _firstCrystal; }
    public GameObject SecondCrystal() { return _secondCrystal; }

    private float _detectionRange;

    [SerializeField]
    private float _minDistance = 1f;

    public void InitiatlizeLink(GameObject firstCrystal, GameObject secondCrystal, SpringJoint2D joint)
    {
        _firstCrystal = firstCrystal;
        _secondCrystal = secondCrystal;
        Joint = joint;

        _lineRenderer.SetPosition(0, _firstCrystal.transform.position);
        _lineRenderer.SetPosition(1, _secondCrystal.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        _lineRenderer.SetPosition(0, _firstCrystal.transform.position);
        _lineRenderer.SetPosition(1, _secondCrystal.transform.position);
    }
}
