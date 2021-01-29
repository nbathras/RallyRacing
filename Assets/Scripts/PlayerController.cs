using Mirror;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : NetworkBehaviour {

    [SerializeField]
    private List<Sprite> spriteList;

    [SerializeField]
    private float acceleration;
    [SerializeField]
    private float steering;

    [SerializeField] 
    private TextMesh playerNameText;
    [SerializeField]
    private GameObject floatingInfo;

    [SyncVar(hook = nameof(OnNameChanged))]
    public string playerName;

    private Material playerMaterialClone;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        playerNameText.gameObject.GetComponent<MeshRenderer>().sortingLayerName = "ForegroundObjects";
    }

    private void OnNameChanged(string _Old, string _New) {
        playerNameText.text = playerName;
    }

    public override void OnStartLocalPlayer() {
        // Set Position and Parent Camera
        Camera.main.transform.SetParent(transform);
        Camera.main.transform.localPosition = new Vector3(0, 0, -10);

        string name = "Player" + Random.Range(100, 999);
        CmdSetupPlayer(name);
    }

    [Command]
    public void CmdSetupPlayer(string _name) {
        // player info sent to server, then server updates sync vars which handles it on all clients
        playerName = _name;
        sr.sprite = spriteList[0];
    }

    private void FixedUpdate() {
        if (isLocalPlayer) {
            HandleMovement();
        }
    }

    private void HandleMovement() {
        float h = -Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector2 speed = transform.up * (v * acceleration);
        rb.AddForce(speed);

        float direction = Vector2.Dot(rb.velocity, rb.GetRelativeVector(Vector2.up));
        if (direction >= 0.0f) {
            rb.rotation += h * steering * (rb.velocity.magnitude / 5.0f);
        } else {
            rb.rotation -= h * steering * (rb.velocity.magnitude / 5.0f);
        }

        Vector2 forward = new Vector2(0.0f, 0.5f);
        float steeringRightAngle;
        if (rb.angularVelocity > 0) {
            steeringRightAngle = -90;
        } else {
            steeringRightAngle = 90;
        }

        Vector2 rightAngleFromForward = Quaternion.AngleAxis(steeringRightAngle, Vector3.forward) * forward;

        float driftForce = Vector2.Dot(rb.velocity, rb.GetRelativeVector(rightAngleFromForward.normalized));

        Vector2 relativeForce = (rightAngleFromForward.normalized * -1.0f) * (driftForce * 10.0f);

        rb.AddForce(rb.GetRelativeVector(relativeForce));
    }

    /*
    [SerializeField]
    private List<Sprite> spriteList;
    private static int numberOfPlayers = 0;

    [SerializeField]
    private float acceleration;
    [SerializeField]
    private float steering;

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        sr.enabled = false;
        numberOfPlayers++;
    }

    private void Start() {
        CmdSetPlayerSprite(numberOfPlayers - 1);
        sr.enabled = true;
    }

    [Command]
    public void CmdSetPlayerSprite(int i) {
        RpcSetPlayerSprite(i);
    }

    [ClientRpc]
    public void RpcSetPlayerSprite(int i) {
        sr.sprite = spriteList[i];
    }

    private void FixedUpdate() {
        if (isLocalPlayer) {
            HandleMovement();
        }
    }

    private void HandleMovement() {
        float h = -Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector2 speed = transform.up * (v * acceleration);
        rb.AddForce(speed);

        float direction = Vector2.Dot(rb.velocity, rb.GetRelativeVector(Vector2.up));
        if (direction >= 0.0f) {
            rb.rotation += h * steering * (rb.velocity.magnitude / 5.0f);
        } else {
            rb.rotation -= h * steering * (rb.velocity.magnitude / 5.0f);
        }

        Vector2 forward = new Vector2(0.0f, 0.5f);
        float steeringRightAngle;
        if (rb.angularVelocity > 0) {
            steeringRightAngle = -90;
        } else {
            steeringRightAngle = 90;
        }

        Vector2 rightAngleFromForward = Quaternion.AngleAxis(steeringRightAngle, Vector3.forward) * forward;

        float driftForce = Vector2.Dot(rb.velocity, rb.GetRelativeVector(rightAngleFromForward.normalized));

        Vector2 relativeForce = (rightAngleFromForward.normalized * -1.0f) * (driftForce * 10.0f);

        rb.AddForce(rb.GetRelativeVector(relativeForce));
    }

    private void OnDestroy() {
        numberOfPlayers--;
    }
    */
}
