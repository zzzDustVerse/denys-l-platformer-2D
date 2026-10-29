using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MonoBehave : MonoBehaviour
{
    int xyz = 1;
    // Start is called before the first frame update
    void Start()
    {
        List<int> dane = new List<int> { 1, 3, 3, 2, 2, 2, 1, 3, 2, 1 };
        List<int> jedynki = new List<int>();
        List<int> dwojki = new List<int>();
        List<int> trojki = new List<int>();

        foreach (int i in dane)
        {

            if (i == 1)
            {
                jedynki.Add(i);
            }
            else if (i == 2)
            {
                dwojki.Add(i);
            }
            else if (i == 3)
            {
                trojki.Add(i);
            }
        }
        dane.Clear();
        Debug.Log("Dane: " + dane.Count);
        
        Debug.Log("Jedynki: " + jedynki.Count);
        Debug.Log("Dwojki: " + dwojki.Count);
        Debug.Log("Trojki: " + trojki.Count);
        
    }
}

