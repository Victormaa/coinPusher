using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.UIElements;

public class WalletController : MonoBehaviour
{
    public static WalletController Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    public int points = 0; 
    public int tokens = 0;

    //when points、tokens change，update debug manager
}
