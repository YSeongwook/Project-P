using UnityEngine;

public class Net : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Fish"))
        {
            var fish =  collision.GetComponent<Fish>();
            if (fish == null) return;

            fish.SetClearAble(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Fish"))
        {
            var fish = collision.GetComponent<Fish>();
            if (fish == null) return;

            fish.SetClearAble(false);
        }
    }
}
