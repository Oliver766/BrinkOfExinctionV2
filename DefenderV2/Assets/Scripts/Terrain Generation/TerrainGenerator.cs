using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public Transform tile;

    Mesh mesh;
    public MeshCollider meshCollider;

    public LayerMask mask;

    Vector3[] vertices;
    Color[] colors;
    int[] triangles;

    private int mapSize = 200;

    private TerrainSettings terrainSettings;

    //Spawners, created by Aidan
    private EnemySpawner es;
    private HumanSpawner hs;

    private float minGradient;
    private float maxGradient;

    private void Awake()
    {
        mesh = new Mesh();
        mesh.name = "Generated Terrain";
        tile.GetComponent<MeshFilter>().mesh = mesh;
    }

    public void GenerateTerrain(TerrainSettings terrainSettings)
    {
        this.terrainSettings = terrainSettings;

        es = EnemySpawner.Instance;
        hs = HumanSpawner.Instance;

        // Generate vertices with randomised height values
        CreateShape(); 
        // Create mesh from vertice data
        UpdateMesh();
        // Place scenery on the terrain

        if (!DebugHelper.debugHelper.disableScenery && terrainSettings.scenery.Length > 0)
        {
            StartCoroutine(CreateScenery());
        }
        else
        {
            if (Director.Instance != null)
            {
                //Set difficulty
                Director.Instance.StartLevel();
                es.enemyCount = Director.Instance.enemiesToSpawn;
                es.capturerCount = Director.Instance.capsToSpawn;
                hs.humanCount = Director.Instance.humansToSpawn;
            }

            TileTerrain();
        }
    }

    /// Generate vertices from randomised height values (with some variables)
    /// </summary>
    void CreateShape()
    {
        // Create a grid of vertices the size of the map
        vertices = new Vector3[(mapSize + 1) * (mapSize + 1)];

        // Loop through the vertices and set the position of each
        for (int i = 0, z = 0; z <= mapSize; z++)
        {
            for (int x = 0; x <= mapSize; x++)
            {
                // Calculate the distance from the edges of the map (for creating an island ensuring the edges of the map fall below sea level)
                float xDistance = x / (float)mapSize * 2 - 1;
                float zDistance = z / (float)mapSize * 2 - 1;

                float value = Mathf.Max(Mathf.Abs(xDistance), Mathf.Abs(zDistance));

                float edgeMofifier = Mathf.Pow(value, 3) / (Mathf.Pow(value, 3) + Mathf.Pow(2.2f - 2.2f * value, 3));


                // Generate the overall island shapes from perlin noise
                float islands = Mathf.PerlinNoise(x * terrainSettings.islandSize, z * terrainSettings.islandSize) * terrainSettings.islandHeight;

                // Add nother level of perlin noise for the finer details, and incorporate this with the larger island shapes, centre falloff and map height
                float y = ((Mathf.PerlinNoise(x * 0.2f, z * 0.2f) * terrainSettings.roughness) + islands + terrainSettings.heightModifier) - (edgeMofifier * terrainSettings.edgeFalloff);

                // Clamp the height, if required (Creates flat mountains)
                y = Mathf.Clamp(y, terrainSettings.minHeight, terrainSettings.maxHeight);


                if (y > maxGradient)
                {
                    maxGradient = y;
                }

                if (y < minGradient)
                {
                    minGradient = y;
                }

                // Apply the created x, y and z values to the vertice in the array
                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }

        // Create an new array of triangles to match the map size (times by 6 for the number of vertice we need to create each square)
        triangles = new int[mapSize * mapSize * 6];

        int vert = 0;
        int tris = 0;

        // Loop through each row on the grid
        for (int z = 0; z < mapSize; z++)
        {
            // Loop through each column of the row
            for (int x = 0; x < mapSize; x++)
            {
                // Set the triangles in a clockwise order (ensures the normals face the correct way when we create our mesh)
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + mapSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + mapSize + 1;
                triangles[tris + 5] = vert + mapSize + 2;

                // Move on to the next vertice in the row
                vert++;
                // Increment by six to reach the next square in the grid
                tris += 6;
            }
            // Increment the vertice count at the end of every row so we don't join the end of a row with the start of the next one
            vert++;
        }

        colors = new Color[vertices.Length];

        for (int i = 0, z = 0; z <= mapSize; z++)
        {
            for (int x = 0; x <= mapSize; x++)
            {
                float height = Mathf.InverseLerp(minGradient, maxGradient, vertices[i].y);
                colors[i] = terrainSettings.gradient.Evaluate(height);
                i++;
            }
        }
    }


    /// <summary>
    /// Generate mesh from the given generated vertice
    /// </summary>
    void UpdateMesh()
    {
        // Clear the mesh of any data
        mesh.Clear();

        // Assign mesh vertices, triangles and UVs
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        //mesh.uv = uvs;
        mesh.colors = colors;

        // Calculate normals to fix lighting
        mesh.RecalculateNormals();

        // Calculate bounds to generate collision data
        mesh.RecalculateBounds();
        meshCollider = tile.GetComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
    }


    /// <summary>
    /// Simple instantiate to duplicate nine different tiles around the origin
    /// </summary>
    void TileTerrain()
    {
        //Spawn enemies & humans
        es.SpawnEnemies();
        hs.SpawnHumans();

        Instantiate(tile, new Vector3(200, 0, 0), transform.rotation, transform);
        Instantiate(tile, new Vector3(-200, 0, 0), transform.rotation, transform);
        Instantiate(tile, new Vector3(0, 0, 200), transform.rotation, transform);
        Instantiate(tile, new Vector3(0, 0, -200), transform.rotation, transform);
        Instantiate(tile, new Vector3(-200, 0, -200), transform.rotation, transform);
        Instantiate(tile, new Vector3(200, 0, -200), transform.rotation, transform);
        Instantiate(tile, new Vector3(200, 0, 200), transform.rotation, transform);
        Instantiate(tile, new Vector3(-200, 0, 200), transform.rotation, transform);

        //Create persistence.
        es.SetPath();
        es.SetTarget();
    }

    private Quaternion terrainNormal;
    private IEnumerator CreateScenery()
    {
        if (Director.Instance != null)
        {
            //Set difficulty
            Director.Instance.StartLevel();
            es.enemyCount = Director.Instance.enemiesToSpawn;
            es.capturerCount = Director.Instance.capsToSpawn;
            hs.humanCount = Director.Instance.humansToSpawn;
        }

        foreach (Scenery sceneryItem in terrainSettings.scenery)
        {
            List <Vector2> previousPlacements = new List<Vector2>();
            int placementAttempts = 0;

            GameObject parent = new GameObject(sceneryItem.name);

            parent.transform.parent = tile.Find("Scenery");

            for (int i = 0; i < sceneryItem.amount; i++)
            {
                if(i % 100 == 0)
                {
                    yield return new WaitForSeconds(0.01f);
                }

                Vector2 position = new Vector2(Random.Range(5f, 195f), Random.Range(5f, 195f));

                float yPosition = -1;

                // Check that the position chosen is not too close to any other placements, based on the scenery's density value
                if (!previousPlacements.Exists(previousPositon => Vector2.Distance(previousPositon, position) < sceneryItem.density))
                {
                    // Ensure the item can be placed at the chosen position given the scenery constraits (max height, placement on land or water)
                    yPosition = CheckSceneryPlacement(position, sceneryItem.minHeight, sceneryItem.maxHeight);
                }
                else
                {
                    // If the position is too close to a previous placement, increment the placementAttempts counter
                    placementAttempts++;
                }

                // Ensure the y position returned is valid (CheckSceneryPlacement will return -1 if item cannot be placed in the position given)
                if (yPosition >= 0)
                {
                    Quaternion randomRotation;

                    if (sceneryItem.alignToSurface)
                    {
                        randomRotation = terrainNormal;
                        randomRotation *= Quaternion.Euler(0, 0, Random.Range(0, 360));
                    }
                    else
                    {
                        randomRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                    }

                    // Instantiate the scenery item at the chosen location
                    Instantiate(sceneryItem.models[Random.Range(0, sceneryItem.models.Length)], new Vector3(position.x, yPosition, position.y), randomRotation, parent.transform);


                    // Add the item's position to the list of previous positions
                    previousPlacements.Add(position);

                    // Reset the placement attempts counter
                    placementAttempts = 0;
                }
                else
                {
                    // If placement fails, try again
                    i--;
                }

                //
                if(placementAttempts > 50)
                {
                    Debug.LogWarning("Density or amount too high, failed to place scenery item: " + sceneryItem.name);
                    i++;
                }
            }
        }

        // This needs to be called here if scenery is generated, otherwise the tiles will be duplicated before all the scenery has been placed and scenery will be missing from the duplicated tiles
        TileTerrain();
    }


    /// <summary>
    /// Ensure the scenery meets the placement criteria, such as the height
    /// </summary>
    /// <param name="position"> The position to attempt placement</param>
    /// <param name="minHeight">The minimum height the scenery should be placed at</param>
    /// <param name="maxHeight">The maximum height the scenery should be placed at</param>
    /// <returns></returns>
    private float CheckSceneryPlacement(Vector2 position, float minHeight, float maxHeight)
    {
        if (Physics.Raycast(new Vector3(position.x, 30, position.y), Vector3.down, out RaycastHit hit, 100f, mask))
        {
            if (hit.point.y < minHeight || hit.point.y > maxHeight)
            {
                return -1;
            }
            else
            {
                terrainNormal = Quaternion.LookRotation(hit.normal);
                return hit.point.y;
            }

        }

        return -1;
    }
}
