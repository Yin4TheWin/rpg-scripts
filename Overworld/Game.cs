using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(transform.gameObject);
        Storage.LoadGame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
