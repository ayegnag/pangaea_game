using UnityEngine;
using System.Collections.Generic;

public class HealthHearts : MonoBehaviour {

    public int maxHearts;
    private int health;

    // [Range(0.1f, 3.0f)] 
    // public float Gap = 1f;

    // private GameObject fullHeartSprite;

    // private GameObject emptyHeartSprite;
    // private float spriteX = 11.62f;

    private List<Transform> HeartArray = new List<Transform>();

    private void Awake() {
		// fullHeartSprite = Resources.Load<GameObject>("Prefabs/Heart_Full");
		// emptyHeartSprite = Resources.Load<GameObject>("Prefabs/Heart_Empty");        
        Debug.Log(transform);
        Debug.Log(transform.GetChild(0));
        Debug.Log(transform.GetChild(0).gameObject.GetComponent<Transform>());
        InitializeHearts();
    }

    private void Start() {
    }

    private void InitializeHearts() {
        // Create list of hearts to be processed
        HeartArray.Add(transform.GetChild(0));
        HeartArray.Add(transform.GetChild(1));
        HeartArray.Add(transform.GetChild(2));
        HeartArray.Add(transform.GetChild(3));
        HeartArray.Add(transform.GetChild(4));
        Debug.Log(HeartArray[2]);
    }

    public void ShowHearts(int maxHearts, int currentHealth) {
        health = currentHealth;
        for (int i = 0; i < 5; i ++)
        {
            if(i < health) {
                HeartArray[i].gameObject.SetActive(true);
            } else {
                HeartArray[i].gameObject.SetActive(false);
            }
        }
    }

    // public void GenerateHearts() {
    //     // generate filled hearts to the count of current health.
    //     GameObject heartSprite;
    //     for (int i = 0; i < maxHearts; i ++)
    //     {
    //         heartSprite = i < health ? fullHeartSprite : emptyHeartSprite;
    //         Vector3 place;
    //         if (i==0) {
    //             place = new Vector3(
    //                 gameObject.transform.position.x,
    //                 gameObject.transform.position.y,
    //                 0
    //             );
    //         } else {
    //             place = new Vector3(
    //                 gameObject.transform.position.x + Gap * i,
    //                 gameObject.transform.position.y,
    //                 0
    //             );
    //         }
    //         Instantiate (
    //             heartSprite,
    //             place,
    //             Quaternion.identity,
    //             gameObject.transform
    //         );
    //         // Make ToolTip Rect container's width a function of the number of hearts.
    //     }
    //     // generate empty hearts to the remaining points from Max Health.
    // }

    // public void OnDisable() {
    //     foreach (Transform child in transform)
    //     {
    //         Destroy(child.gameObject);
    //     }
    // }
}