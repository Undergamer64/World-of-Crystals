using UnityEngine;

public class DeathZone : MonoBehaviour
{
    [SerializeField]
    private bool _isKillZone = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null && collision.TryGetComponent<Crystals>(out Crystals crystal))
        {
            crystal.DeleteAllLinks();
            if (_isKillZone) Destroy(collision.gameObject);
        }
    }
}
