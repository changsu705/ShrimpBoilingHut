	using UnityEngine;
using DG.Tweening;

public class ObjectBounce : MonoBehaviour
{
    [Header("[튕김 설정]")]
    [SerializeField] private float bounceHeight = 0.4f; 
    [SerializeField] private float duration = 0.25f;    

    private bool isBounced = false; 

   
    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (isBounced || !collision.gameObject.CompareTag("Ground")) return;

        isBounced = true; 
        
       
      

        Sequence bounceSequence = DOTween.Sequence();

       
        bounceSequence.Append(transform.DOBlendableMoveBy(new Vector3(0, bounceHeight, 0), duration).SetEase(Ease.OutQuad));
        bounceSequence.Append(transform.DOBlendableMoveBy(new Vector3(0, -bounceHeight, 0), duration).SetEase(Ease.InQuad));

     
        transform.DOPunchScale(new Vector3(0.15f, -0.15f, 0f), duration * 2, 5, 0.5f);
    }

  
    private void OnEnable()
    {
        isBounced = false;
    }
}