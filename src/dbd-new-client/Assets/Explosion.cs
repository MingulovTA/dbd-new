using System.Collections.Generic;
using System.Runtime.CompilerServices;
using App.Player;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float explosionForce = 10f;
    [SerializeField] private SphereCollider _sphereCollider;

    [SerializeField] private List<PlayerMovement> _players = new List<PlayerMovement>();

    private void OnTriggerExit(Collider other)
    {
        PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
        if (playerMovement!=null&&_players.Contains(playerMovement))
            _players.Remove(playerMovement);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
        if (playerMovement!=null&&!_players.Contains(playerMovement))
            _players.Add(playerMovement);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Explode();
        }
    }

    private void Explode()
    {
        foreach (var playerMovement in _players)
            PushPlayer(playerMovement);
    }

    private void PushPlayer(PlayerMovement pm)
    {
        Vector3 explosionDirection = pm.transform.position - transform.position;
        float dist = Vector3.Distance(pm.transform.position, transform.position);

        float power = 1-(dist / _sphereCollider.radius);
        pm.AddForce(explosionDirection.normalized * explosionForce);
    }
}
