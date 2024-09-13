using UnityEngine;
using UnityEngine.UI;

public class EmptyTile : MonoBehaviour
{
    public Sprite[] sprites;

    private void Start()
    {
        Transform road = transform.GetChild(1);
        Transform hint = transform.GetChild(3);
        Image childImage = road.GetComponent<Image>();
        
        if (childImage.sprite == null)
        {
            int randomIndex = Random.Range(0, sprites.Length);
            childImage.sprite = sprites[randomIndex];
            hint.gameObject.SetActive(false);
        }
    }
}