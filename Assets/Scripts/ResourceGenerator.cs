using UnityEngine;

public class ResourceGenerator : MonoBehaviour
{
    [SerializeField] private Resource _prefab;
    [SerializeField] private int _minAmountInPile = 2;
    [SerializeField] private int _maxAmountInPile = 8;
    [SerializeField] private int _pileSpawnRadius = 10;
    [SerializeField] private int _generatedPiles = 10;
    [SerializeField] private Vector2 _generationAreaSize = new Vector2(200f, 200f);

    private void Awake()
    {
        Generate();
    }

    private void Generate()
    {
        Resource spawnedObject;
        Vector3 spawnSpot;
        Vector2 spreadArea;
        int amountInPile;
        float xPos, zPos;

        for (int i = 0; i < _generatedPiles; i++)
        {
            amountInPile = Random.Range(_minAmountInPile, _maxAmountInPile + 1);
            xPos = Random.Range(transform.position.x - _generationAreaSize.x, transform.position.x + _generationAreaSize.x);
            zPos = Random.Range(transform.position.z - _generationAreaSize.y, transform.position.z + _generationAreaSize.y);

            for (int j = 0; j < amountInPile; j++)
            {
                spreadArea = Random.insideUnitCircle * _pileSpawnRadius;
                spawnSpot = new Vector3(xPos + spreadArea.x, transform.position.y, zPos + spreadArea.y);
                spawnedObject = Instantiate(_prefab);
                spawnedObject.transform.position = spawnSpot;
                spawnedObject.transform.SetParent(transform);
            }
        }
    }
}
