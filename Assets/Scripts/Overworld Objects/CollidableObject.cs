using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollidableObject : MonoBehaviour
{
    // [SerializeField] is used to expose a private variable to the inspector

    protected Collider2D thisCollider;
    [SerializeField] protected ContactFilter2D filter;
    protected List<Collider2D> collidedObjects = new List<Collider2D>(1); // Ensures the List only stores one object.

    // Start is called before the first frame update
    protected virtual void Start() // protected allows this to be visible to classes that inherit it (ie. child classes) // virtual (only for protected and virtual) means that it can be overridden child classes
    {
        thisCollider = GetComponent<Collider2D>(); // Use to fetch the Collider2D from this Game Object.
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        thisCollider.OverlapCollider(filter, collidedObjects);

        foreach (var o in collidedObjects) // Checks how many other Game Objects are currently colliding with this Game Object.
        {
            OnCollided(o.gameObject);
        }
    }

    protected virtual void OnCollided(GameObject collidedObject) // Separating the functionalities allows the collision function to be reused
    {
        Debug.Log("Collide with " + collidedObject.name); // Debug Test
    }
}
