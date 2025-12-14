using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Resource : MonoBehaviour, IPickable
{
    private Rigidbody Rigidbody;
    private Collider Collider;

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        Collider = GetComponent<Collider>();
    }

    private void TogglePhysics()
    {
        Rigidbody.isKinematic = !Rigidbody.isKinematic;
        Collider.isTrigger = Collider.isTrigger;
    }

    private void DisableCollider()
    {
        Collider.enabled = false;
    }

    public void PickUp()
    {
        TogglePhysics();
        DisableCollider();
    }
}
