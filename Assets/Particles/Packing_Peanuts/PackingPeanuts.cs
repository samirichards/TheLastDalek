using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackingPeanuts : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DeleteThis(8f);
    }

    IEnumerator DeleteThis(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
