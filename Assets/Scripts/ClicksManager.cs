using UnityEngine;
using UnityEngine.EventSystems;

public class ClicksManager : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    private GameObject _crystalPrefab;

    [SerializeField]
    private Transform _crystalsParent;

    public void OnPointerDown(PointerEventData eventData)
    {
        GameObject Crystal = Instantiate(_crystalPrefab, eventData.pointerPressRaycast.worldPosition, _crystalPrefab.transform.rotation, _crystalsParent);
        //Crystal.GetComponent<Crystals>().OnInitializePotentialDrag(eventData);
    }
}
