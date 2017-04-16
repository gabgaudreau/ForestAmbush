using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
    [SerializeField]
    GameObject _enemy; //Might be type Enemy.

    private int _enemiesToSpawn;
    private bool _spawning;
    GameObject _player;
    private float _spawnRadius;

    void Start() {
        _player = GameObject.FindGameObjectWithTag("Player");
        if (_player == null)
            Debug.Log("Cant find player");
        if (_enemy.tag == "Large Enemy"  || _enemy.tag == "Flock Mind")
            _enemiesToSpawn = 1;
        else
            _enemiesToSpawn = 5;
        _spawnRadius = 60.0f; //TBD 60.0f seems reasonable.
    }

    void Update() {
        if (!_spawning && Vector3.Distance(transform.position, _player.transform.position) < _spawnRadius) { //Will only start spawning once the player has entered spawnRadius.
            StartCoroutine(SpawnEnemies());
        }
    }

    IEnumerator SpawnEnemies() {
        _spawning = true;
        for (int i = 0; i < _enemiesToSpawn; i++) {
            Instantiate(_enemy, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(5.0f); //Delay between each spawn, TBD.
        }
    }
}
