using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null && collision.TryGetComponent<Crystals>(out Crystals crystal))
        {
            crystal.DeleteAllLinks();
            Destroy(collision.gameObject);
        }
    }
}
