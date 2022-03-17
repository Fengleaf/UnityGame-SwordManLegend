using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Monster[] monsters;
    public Vector2[] positios;
    public float spawnFrequency;
    public int maxEnemyNumber;
    [Tooltip("Allows to spawn when total monster number is less or equal spawnCondition.")]
    public int spawnCondition;
    public List<int> spawnRatio;

    private int nowNumber;

    // Start is called before the first frame update
    void Start()
    {
        nowNumber = 0;
        if (spawnRatio.Count < monsters.Length)
        {
            int total = 0;
            for (int i = 0; i < spawnRatio.Count; i++)
                total += spawnRatio[i];
            if (total < maxEnemyNumber)
            {
                int d = maxEnemyNumber - total;
                int md = monsters.Length - spawnRatio.Count;
                for (int i = 0; i < md; i++) 
                    spawnRatio.Add(Mathf.CeilToInt((float)d / md));
            }
        }
        StartCoroutine("Spawn");
    }

    IEnumerator Spawn()
    {
        while (true)
        {
            if (nowNumber <= spawnCondition)
            {
                yield return new WaitForSeconds(1 / spawnFrequency);
                if (nowNumber < maxEnemyNumber)
                {
                    int canSpawnNumber = maxEnemyNumber - nowNumber;
                    List<Vector2> ps = new List<Vector2>(positios);

                    for (int i = 0; i < spawnRatio.Count; i++)
                    {
                        for (int j = 0; j < spawnRatio[i] * canSpawnNumber / maxEnemyNumber; j++)
                        {
                            if (ps.Count == 0)
                                break;
                            int index = Random.Range(0, ps.Count);
                            Instantiate(monsters[i], ps[index], Quaternion.identity, transform);
                            nowNumber++;
                            ps.RemoveAt(index);
                        }
                    }
                    yield return new WaitForSeconds(1 / spawnFrequency);
                }
            }
            yield return null;
        }
    }

    public void DeleteEnemy()
    {
        nowNumber--;
    }
}
