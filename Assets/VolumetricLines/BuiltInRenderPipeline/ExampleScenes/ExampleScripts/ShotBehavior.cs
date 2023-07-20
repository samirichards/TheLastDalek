using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]

public class ShotBehavior : MonoBehaviour {
    private AudioSource audioSource;
	[SerializeField] AudioClip RaygunLandHitSound;

	// Use this for initialization
	void Start () {
		audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		transform.position += transform.forward * Time.deltaTime * 1000f;
	}

    private void OnTriggerEnter(Collider other)
    {
		if (!other.GetComponent<CannonBehavior>())
		{
            AudioSource.PlayClipAtPoint(RaygunLandHitSound, other.transform.position);
            Debug.Log("Object hit");
            Destroy(gameObject);
        }
    }
}
