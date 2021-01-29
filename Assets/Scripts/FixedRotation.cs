using UnityEngine;

public class FixedRotation : MonoBehaviour {

    private Quaternion lockedRotation;

    private void Awake() {
        lockedRotation = transform.rotation;
    }

    // Update is called once per frame
    private void LateUpdate() {
        transform.rotation = lockedRotation;
    }
}
