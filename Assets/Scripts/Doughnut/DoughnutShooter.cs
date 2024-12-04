using System;
using System.Collections;
using UnityEngine;

public class DoughnutShooter : MonoBehaviour
{
    public GameObject Item
    {
        get => item;
        set => item = value;
    }
    
    [SerializeField]
    private GameObject item;

    [SerializeField]
    private Vector2 itemPosition;

    [SerializeField] 
    private float resistanceStrength = 0.7f;

    [SerializeField] 
    private float forceMultiplier = 5;

    private Vector2 mouseDownPosition;

    private RandomDoughnutQueue queue => GameplayManager.Current.RandomDoughnutQueue;

    private void Start()
    {
        StartCoroutine(addDoughnut());
    }

    void Update()
    {
        if (Item == null)
            return;
        
        if (Input.GetMouseButtonDown(0))
        {
            mouseDownPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            Vector2 currentMousePosition = Input.mousePosition;

            var change = Camera.main!.ScreenToWorldPoint(currentMousePosition) - Camera.main!.ScreenToWorldPoint(mouseDownPosition);

            change *= change.magnitude <= 0 ? 0 : MathF.Pow(change.magnitude, resistanceStrength) / change.magnitude;

            if (change.magnitude == 0)
                return;

            item.transform.position = (Vector3)itemPosition + change;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            var rigidbody = item.GetComponent<Rigidbody2D>();
            rigidbody.AddForce(-((Vector2)item.transform.position - itemPosition) * forceMultiplier, ForceMode2D.Impulse);
            rigidbody.gravityScale = -0.5f;
            
            this.StartDelayedCoroutine(addDoughnut(), 0.6f);
        }
    }

    private IEnumerator addDoughnut()
    {
        var doughnut = queue.Get();
        
        item = doughnut.gameObject;
        item.transform.position = itemPosition;
        
        doughnut.transform.localScale = Vector3.zero;
        doughnut.ScaleTo(Vector3.one, 0.7f, Easing.OutElastic);

        yield return null;
    }
}