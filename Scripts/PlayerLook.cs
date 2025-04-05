using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public Transform flashlightPivot; // The object that rotates
    public SpriteRenderer playerSpriteRenderer;
    public Sprite upSprite, downSprite, leftSprite, rightSprite;

    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePos - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 🔁 This rotates the pivot — now the flashlight will orbit
        flashlightPivot.rotation = Quaternion.Euler(0, 0, angle);

        // Cardinal direction snap
        float angle360 = (angle + 360) % 360;

        if (angle360 >= 45 && angle360 < 135)
        {
            playerSpriteRenderer.sprite = upSprite;
            GetComponent<SpriteRenderer>().color = Color.white; // test color
        }
        else if (angle360 >= 135 && angle360 < 225)
        {
            playerSpriteRenderer.sprite = leftSprite;
            GetComponent<SpriteRenderer>().color = Color.green; // test color
        }
        else if (angle360 >= 225 && angle360 < 315)
        {
            playerSpriteRenderer.sprite = downSprite;
            GetComponent<SpriteRenderer>().color = Color.blue; // test color
        }
        else
        {
            playerSpriteRenderer.sprite = rightSprite;
            GetComponent<SpriteRenderer>().color = Color.red; // test color
        }
    }
}
