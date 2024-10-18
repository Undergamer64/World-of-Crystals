using UnityEngine;
using UnityEngine.EventSystems;

public class ClicksManager : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    private bool _enabled = true;

    [SerializeField]
    private GameObject _crystalPrefab;

    [SerializeField]
    private Transform _crystalsParent;

    private void Awake()
    {
        this.enabled = _enabled;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        GameObject Crystal = Instantiate(_crystalPrefab, eventData.pointerPressRaycast.worldPosition, _crystalPrefab.transform.rotation, _crystalsParent);
        //Crystal.GetComponent<Crystals>().OnInitializePotentialDrag(eventData);
    }
}
