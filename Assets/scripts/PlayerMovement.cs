using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Rigidbody'yi otomatik bul
    }

    void Update() // Fizik i�lemleri i�in FixedUpdate daha iyidir ama basitlik i�in Update kals�n
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 moveDir = new Vector3(x, 0, z);

        if (moveDir.magnitude > 0.1f)
        {
            // 1. Y�n� ayarla (D�n��)
            Quaternion toRotation = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 720 * Time.deltaTime);

            // 2. H�z� ayarla (Hareket)
            // Mevcut Y h�z�n� (yer�ekimi d�����n�) koruyarak sadece X ve Z'de hareket veriyoruz.
            Vector3 newVelocity = moveDir * moveSpeed;
            newVelocity.y = rb.linearVelocity.y; // Yer�ekimini bozma!

            rb.linearVelocity = newVelocity;
        }
        else
        {
            // Tu�a basm�yorsak kaymay� �nlemek i�in X ve Z h�z�n� s�f�rla (Y kals�n)
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }
}