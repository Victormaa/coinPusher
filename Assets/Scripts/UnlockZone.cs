using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;

public class UnlockZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("333");
        if (other.CompareTag("Coin") || other.CompareTag("Special Coin"))
        {
            Debug.Log("222");
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Debug.Log("constraints unlocked for " + other.name);
                rb.constraints = RigidbodyConstraints.None; // 解锁旋转约束，让硬币可以正常掉落
            }
        }
    }

}
