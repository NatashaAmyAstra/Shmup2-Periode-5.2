using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;

public class BackgroundParalax : MonoBehaviour
{
    [Header("Paralax")]
    [SerializeField] private float _paralaxSpeed;
    [SerializeField] private GameObject[] _backgroundTiles;

    [Header("Looping")]
    [SerializeField] private float _tileSpawnDistance;
    [SerializeField] private float _maxXOffset;
    private List<GameObject> _tilePool = new List<GameObject>();
    private List<GameObject> _activeTiles = new List<GameObject>();

    private float _worldScreenHeight;

    private void Awake() {
        _worldScreenHeight = Camera.main.ViewportToWorldPoint(Vector3.up).y;

        InstantiateInitialTiles();
        PlaceInitialTiles();
    }

    private void InstantiateInitialTiles() {
        foreach(GameObject tile in _backgroundTiles)
        {
            GameObject instance = Instantiate(tile, transform);
            instance.SetActive(false);
            _tilePool.Add(instance);
        }
    }

    private void PlaceInitialTiles() {
        Vector3 currentSpawnPos = Vector3.up * -_worldScreenHeight;
        while(currentSpawnPos.y < _worldScreenHeight + _tileSpawnDistance)
        {
            PlaceTile(currentSpawnPos);
            currentSpawnPos.y += _tileSpawnDistance;
        }
    }

    private void Update() {
        MoveTiles();
    }

    private void MoveTiles() {
        // reverse for loop so removed items don't interupt the flow
        for(int i = _activeTiles.Count - 1; i >= 0; i--)
        {
            GameObject tile = _activeTiles[i];
            tile.transform.position += Vector3.down * _paralaxSpeed * Time.deltaTime;
            if(tile.transform.position.y < -_worldScreenHeight - _tileSpawnDistance / 2)
            {
                ReturnToPool(tile);
                PlaceTile(_activeTiles[_activeTiles.Count - 1].transform.position + Vector3.up * _tileSpawnDistance);
            }
        }
    }

    private void PlaceTile(Vector3 position) {
        position.x = Random.Range(-_maxXOffset, _maxXOffset);
        GameObject tile = _tilePool[Random.Range(0, _tilePool.Count)];
        _tilePool.Remove(tile);
        _activeTiles.Add(tile);
        tile.transform.position = position;
        tile.SetActive(true);
    }

    private void ReturnToPool(GameObject tile) {
        tile.SetActive(false);
        _activeTiles.Remove(tile);
        _tilePool.Add(tile);
    }
}
