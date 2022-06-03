using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    public GameObject[] flashHolders;
    public Sprite[] flashSprites;
    public SpriteRenderer[] spriteRenderers;
    public float flashTime;

    private void Start()
    {
        Deactivate();
    }

    public void Activate()
    {
        for (int i = 0; i < flashHolders.Length; i++)
        {
            flashHolders[i].SetActive(true);

            int flashSpriteIndex = Random.Range(0, flashSprites.Length);

            for (int j = 0; j < spriteRenderers.Length; j++)
            {
                spriteRenderers[j].sprite = flashSprites[flashSpriteIndex];
            }

            Invoke("Deactivate", flashTime);
        }
        
    }

    private void Deactivate()
    {

        for (int i = 0; i < flashHolders.Length; i++)
        {
            flashHolders[i].SetActive(false);
        }
    }
}
