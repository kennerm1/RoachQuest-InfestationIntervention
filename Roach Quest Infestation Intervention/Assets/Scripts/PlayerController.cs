using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPun
{
    public Button jarButton;
    public Button toDormButton;

    [HideInInspector]
    public int id;

    [Header("Info")]
    public float moveSpeed;
    public int roach;
    public int curHp;
    public int maxHp;
    public bool dead;

    [Header("Attack")]
    public int damage;
    public float attackRange;
    public float attackRate;
    private float lastAttackTime;

    [Header("Components")]
    public Rigidbody2D rig;
    public Player photonPlayer;
    public SpriteRenderer sr;
    public Animator weaponAnim;

    public static PlayerController me;
    public HeaderInfo headerInfo;

    void Update()
    {
        if (!photonView.IsMine)
            return;
        Move();
        if (Input.GetMouseButtonDown(0) && Time.time - lastAttackTime > attackRate)
            Attack();

        float mouseX = (Screen.width / 2) - Input.mousePosition.x;
        if (mouseX < 0)
            weaponAnim.transform.parent.localScale = new Vector3(1, 1, 1);
        else
            weaponAnim.transform.parent.localScale = new Vector3(-1, 1, 1);
    }

    void Move()
    {
        // get the horizontal and vertical inputs
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        // apply that to our velocity
        rig.velocity = new Vector2(x, y) * moveSpeed;
    }

    void Attack()
    {
        lastAttackTime = Time.time;
        // calculate the direction
        Vector3 dir = (Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position)).normalized;
        // shoot a raycast in the direction
        RaycastHit2D hit = Physics2D.Raycast(transform.position + dir, dir, attackRange);
        // did we hit an enemy?
        if (hit.collider != null && hit.collider.gameObject.CompareTag("Enemy"))
        {
            // get the enemy and damage them
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            enemy.photonView.RPC("TakeDamage", RpcTarget.MasterClient, damage);
        }
        // play attack animation
        weaponAnim.SetTrigger("Attack");
    }

    [PunRPC]
    public void TakeDamage(int damage)
    {
        curHp -= damage;
        // update the health bar
        if (curHp <= 0)
            Die();
        else
        {
            StartCoroutine(DamageFlash());
            IEnumerator DamageFlash()
            {
                sr.color = Color.red;
                yield return new WaitForSeconds(0.05f);
                sr.color = Color.white;
            }
        }
        // update the health bar
        headerInfo.photonView.RPC("UpdateHealthBar", RpcTarget.All, curHp);
    }

    void Die()
    {
        dead = true;
        rig.isKinematic = true;
        transform.position = new Vector3(0, 99, 0);
        Vector3 spawnPos = GameManager.instance.spawnPoints[Random.Range(0, GameManager.instance.spawnPoints.Length)].position;
        StartCoroutine(Spawn(spawnPos, GameManager.instance.respawnTime));
    }

    IEnumerator Spawn(Vector3 spawnPos, float timeToSpawn)
    {
        yield return new WaitForSeconds(timeToSpawn);
        dead = false;
        transform.position = spawnPos;
        curHp = maxHp;
        rig.isKinematic = false;
        // update the health bar
    }

    [PunRPC]
    public void Initialize(Player player)
    {
        id = player.ActorNumber;
        photonPlayer = player;
        // initialize the health bar
        if (player.IsLocal)
            me = this;
        else
            rig.isKinematic = true;
        GameManager.instance.players[id-1] = this;

        // initialize the health bar
        headerInfo.Initialize(player.NickName, maxHp);
    }

    [PunRPC]
    void Heal(int amountToHeal)
    {
        curHp = Mathf.Clamp(curHp + amountToHeal, 0, maxHp);
        // update the health bar
        headerInfo.photonView.RPC("UpdateHealthBar", RpcTarget.All, curHp);
    }

    [PunRPC]
    void GiveRoach(int roachToGive)
    {
        roach += roachToGive;

        // update the ui
        GameUI.instance.UpdateRoachText(roach);
    }

    [PunRPC]
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Door"))
        {
            if (gameObject.GetComponent<PlayerController>().roach >= 25)
            {
                TeleportHousing();
            }
            else
            {
                Debug.Log("Not enough.");
            }
        }
    }

    public void TeleportHousing()
    {
        transform.position = new Vector3(62, -18, 0);
    }

    public void TeleportDorm()
    {
        transform.position = new Vector3(0, 0, 0);
    }

    public void OnJarButton()
    {
        TakeRoach();
        toDormButton.interactable = true;
    }

    public void TakeRoach()
    {
        roach -= 25;

        // update the ui
        GameUI.instance.UpdateRoachText(roach);
    }

    public void OnToDormButton()
    {
        TeleportDorm();
    }

}
