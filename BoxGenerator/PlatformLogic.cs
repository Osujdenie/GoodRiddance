using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformLogic : MonoBehaviour
{
    [SerializeField] private float despawnTimer;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite SpriteUp;
    [SerializeField] private Sprite SpriteDown;

    private SpriteRenderer childSprite;

    private void Awake()
    {
        childSprite = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
    }


    void OnEnable()
    {
        PlatformSpriteSelector();
        StartCoroutine(ReturnPlatform(despawnTimer));
    }

    private IEnumerator ReturnPlatform(float cd)
    {
        yield return new WaitForSeconds(cd);
        PlatformPool.Instance.ReturnToPull(this);
    }

    // выбор спрайта с ускорением/замедлением
    private void PlatformSpriteSelector()
    {
        int rIndex = Random.Range(0, 100);

        if (rIndex < 85)
        {
            childSprite.sprite = defaultSprite;
        }
        else if(rIndex < 95)
        {
            childSprite.sprite = SpriteUp;
        }
        else
        {
            childSprite.sprite = SpriteDown;
        }
    }
}
