using UnityEngine;

public class Collectable : MonoBehaviour
{
    // Bu deðiþken, ayný odunu iki kere toplamamýzý engeller
    public bool IsCollected { get; private set; } = false;

    public void Collect()
    {
        IsCollected = true;

        // Yerdeki fiziði kapat (Düþmesin veya çarpmasýn)
        GetComponent<Collider>().enabled = false;

        // Eðer Rigidbody varsa onu da sustur
        if (TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = true; // Fizikten etkilenmesin
            rb.useGravity = false; // Yerçekimi kapansýn
        }
    }
}