using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowScript : MonoBehaviour
{    
    void LateUpdate()
    {
        transform.localScale = new Vector3(
            Mathf.Abs(transform.localScale.x),  // Всегда положительный X
            Mathf.Abs(transform.localScale.y),  // Всегда положительный Y
            transform.localScale.z);
            
    }
}
