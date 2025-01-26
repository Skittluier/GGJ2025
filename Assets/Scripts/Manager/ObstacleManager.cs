using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    public ObstacleSpawner[] objectSpawner
        ;
    [ContextMenu("Spawning Objects")]
    public void SpawnObjects()
    {
        foreach (ObstacleSpawner item in objectSpawner)
        {
            if(item.type == ObstacleType.Dirt)
            {
                GameObject prefab = GameObject.Instantiate(item.prefab);
                prefab.transform.position = item.spot.transform.position;
            }
        }
    }

}
[System.Serializable]
public struct ObstacleSpawner
{
    public ObstacleType type;
    public GameObject prefab;
    public Transform spot;

}
public enum ObstacleType
{
    Dirt = 0,
    RotatingTubes = 1
}