using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 8f; // Mobilde biraz daha hýzlý hissettirmeli
    public MobileJoystick joystick; // Inspector'dan atayacaðýz

    void Update()
    {
        // Klavye GÝTTÝ -> Joystick GELDÝ
        float x = joystick.InputDirection.x;
        float z = joystick.InputDirection.y;

        Vector3 moveDir = new Vector3(x, 0, z);

        // Hareket varsa
        if (moveDir.magnitude > 0.1f) // Küçük titremeleri önlemek için 0.1 eþik deðeri
        {
            // Hareketi uygula
            transform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.World);

            // Dönüþü yumuþat (LookRotation)
            Quaternion toRotation = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 720 * Time.deltaTime);
        }
    }
}