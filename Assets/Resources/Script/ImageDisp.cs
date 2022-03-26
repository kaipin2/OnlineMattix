using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class ImageDisp : MonoBehaviour
{
    private static int ImageMax = 6;
    public Image image;
    public Sprite[] sprite = new Sprite[ImageMax]; //ƒCƒ[ƒW‰æ‘œ

    // Start is called before the first frame update
    void Start()
    {
        image = this.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChengeImage(int number)
    {
        if (number < ImageMax)
        {
            image.sprite = sprite[number];
        }
    }
    
}
