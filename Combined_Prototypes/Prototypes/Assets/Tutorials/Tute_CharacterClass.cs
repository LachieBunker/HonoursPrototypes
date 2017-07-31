using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tute_CharacterClass : MonoBehaviour {

    public int playerNum;
    public PlayerRoles pRole;
    public KeyCode leftKey;
    public KeyCode rightKey;
    public KeyCode upKey;
    public KeyCode downKey;
    public KeyCode interactKey;
    public float moveSpeed;
    public float rotSpeed;
    public float interactionRange;
    public Transform camPos;
    private Animator animator;
    public Tute_GameManager gManager;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetRole(PlayerRoles _role)
    {
        pRole = _role;
    }

    public void SetOwner(int _playerNum)
    {
        playerNum = _playerNum;
    }

    public virtual void Interact(GameObject interactionObject, string objectTag)
    {

    }

    public void Respawn()
    {
        gManager.RespawnPlayer(gameObject);
    }
}
