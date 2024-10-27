using System.Collections;
using UnityEngine;

public class OneWayPlatformHandler : MonoBehaviour
{
    private GameObject currentOneWayPlatform;
    private bool detectOneWayPlatform = true;

    [SerializeField] private CapsuleCollider2D playerCollider;

    /// <summary>
    /// moe skill nay k biet dat ten la gi ._.
    /// </summary>
    public void OnDropMyself()
    {
        if (currentOneWayPlatform != null && detectOneWayPlatform)
        {
            StartCoroutine(DisableCollision());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(ConstValue.Tags.ONE_WAY_PLATFORM))
        {
            currentOneWayPlatform = collision.gameObject;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(ConstValue.Tags.ONE_WAY_PLATFORM))
        {
            currentOneWayPlatform = null;
        }
    }

    private IEnumerator DisableCollision()
    {
        detectOneWayPlatform = false;
        BoxCollider2D platformCollider = currentOneWayPlatform.GetComponent<BoxCollider2D>();

        Physics2D.IgnoreCollision(playerCollider, platformCollider);
        yield return new WaitForSeconds(0.25f);
        Physics2D.IgnoreCollision(playerCollider, platformCollider, false);
        detectOneWayPlatform = true;
    }
}
