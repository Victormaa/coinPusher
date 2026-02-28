using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class OnTopToggle : MonoBehaviour
{
    private Toggle toggle;
    private Transparent2 transparentScript;

    void Awake()
    {
        transparentScript = FindObjectOfType<Transparent2>();
    }
    // Start is called before the first frame update
    void Start()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(transparentScript.SetOnTop);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
