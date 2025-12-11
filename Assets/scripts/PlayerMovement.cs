using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    void Update()
    {
        // WASD veya Ok Tuþlarý ile hareket
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 moveDir = new Vector3(x, 0, z);

        if (moveDir.magnitude > 0)
        {
            // Hareketi uygula
            transform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.World);

            // Karakteri gittiði yöne döndür
            Quaternion toRotation = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 720 * Time.deltaTime);
        }
    }
}