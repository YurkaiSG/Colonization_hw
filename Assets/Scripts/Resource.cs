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

    public void PickUp(Transform parent)
    {
        TogglePhysics();
        transform.SetPositionAndRotation(parent.position, parent.rotation);
        transform.SetParent(parent);
    }

    private void TogglePhysics()
    {
        Rigidbody.isKinematic = !Rigidbody.isKinematic;
        Collider.enabled = !Collider.enabled;
    }
}
