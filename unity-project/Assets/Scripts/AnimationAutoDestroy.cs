using UnityEngine;

/// <summary>
/// After an optional delay, destroy game object.
/// Used to automatically destroy objects after animation.
/// </summary>
public class SummonedTree : MonoBehaviour
{
	public float Delay = 0f;

	void Start ()
    {
		Destroy (gameObject, GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length + Delay); 
	}
}
