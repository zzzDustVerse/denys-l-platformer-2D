using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objects : MonoBehaviour
{
   public List<GameObject> obiekty = new List<GameObject>();
    
    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject obj in obiekty)
        {
            obj.GetComponent<SpriteRenderer>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
