using Mirror;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : NetworkBehaviour {

    private static int NUMBER_OF_PLAYERS = 0;

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
        NUMBER_OF_PLAYERS++;
    }

    private void OnNameChanged(string _Old, string _New) {
        playerNameText.text = playerName;
    }

    private void OnColorChanged(Color _Old, Color _New) {
        playerNameText.color = _New;
        playerMaterialClone = new Material(GetComponent<Renderer>().material);
        playerMaterialClone.color = _New;
        GetComponent<Renderer>().material = playerMaterialClone;
    }

    public override void OnStartLocalPlayer() {
        // Set Position and Parent Camera

        string name = "Player" + Random.Range(100, 999);
        Color color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        CmdSetupPlayer(name, color);
    }

    [Command]
    public void CmdSetupPlayer(string _name, Color _col) {
        // player info sent to server, then server updates sync vars which handles it on all clients
        playerName = _name;
        sr.sprite = spriteList[NUMBER_OF_PLAYERS - 1];
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
        NUMBER_OF_PLAYERS--;
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
