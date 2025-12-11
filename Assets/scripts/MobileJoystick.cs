using UnityEngine;
using UnityEngine.EventSystems;

public class MobileJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    private RectTransform background;
    private RectTransform handle;

    // Diðer scriptlerden eriþeceðimiz Yön Verisi
    public Vector2 InputDirection { set; get; }

    private void Start()
    {
        background = GetComponent<RectTransform>();
        handle = transform.GetChild(0).GetComponent<RectTransform>(); // Ýlk çocuk Handle olmalý
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position = Vector2.zero;

        // Dokunulan noktanýn Background içindeki yerini bul
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background,
            eventData.position,
            eventData.pressEventCamera,
            out position
        );

        // Pozisyonu normalize et (0 ile 1 arasýna sýkýþtýr)
        position.x = (position.x / background.sizeDelta.x);
        position.y = (position.y / background.sizeDelta.y);

        // Pivot ayarýna göre (merkez 0,0 olsun diye)
        // Eðer pivotun ortadaysa bu hesaplama gerekebilir, pivot sol alttaysa farklýdýr.
        // Genelde Image oluþturunca pivot 0.5, 0.5 gelir. O yüzden:
        // Bu hesabý basitleþtirelim:

        InputDirection = new Vector2(position.x * 2, position.y * 2); // -1 ile 1 arasýna çek

        // Halka dýþýna çýkmasýn (Clamp)
        InputDirection = (InputDirection.magnitude > 1) ? InputDirection.normalized : InputDirection;

        // Görseli (Handle) hareket ettir
        handle.anchoredPosition = new Vector2(
            InputDirection.x * (background.sizeDelta.x / 2),
            InputDirection.y * (background.sizeDelta.y / 2)
        );
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData); // Dokunur dokunmaz algýlasýn
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        InputDirection = Vector2.zero; // Býrakýnca sýfýrla
        handle.anchoredPosition = Vector2.zero; // Merkeze dön
    }
}