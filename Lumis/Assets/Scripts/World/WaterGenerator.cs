using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class WaterGenerator : MonoBehaviour
{
    [Header("Water tilemap")]
    public Tilemap waterMarkerTilemap;

    [Header("Water material")]
    public Material waterMaterial;

    [Header("Heightmap padding")]
    public int heightmapPadding = 2;

    [Header("Distance field ")]
    public float maxDepthDistance = 10f;

    private Texture2D heightmapTexture;
    private bool[,] waterMask;
    private int mapWidth, mapHeight;

    void Start()
    {
        GenerateHeightmap();
        GenerateMesh();

        if (waterMarkerTilemap != null)
        {
            var renderer = waterMarkerTilemap.GetComponent<TilemapRenderer>();
            if (renderer != null) renderer.enabled = false;
        }
    }

    void GenerateHeightmap()
    {
        if (waterMarkerTilemap == null) return;

        BoundsInt bounds = waterMarkerTilemap.cellBounds;
        mapWidth = bounds.size.x + heightmapPadding * 2;
        mapHeight = bounds.size.y + heightmapPadding * 2;

        waterMask = new bool[mapWidth, mapHeight];
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                Vector3Int cellPos = new Vector3Int(
                    bounds.x + x - heightmapPadding,
                    bounds.y + y - heightmapPadding,
                    0);
                waterMask[x, y] = waterMarkerTilemap.GetTile(cellPos) != null;
            }
        }

        float[,] distanceField = ComputeDistanceField(waterMask, mapWidth, mapHeight);

        heightmapTexture = new Texture2D(mapWidth, mapHeight, TextureFormat.R8, false);
        heightmapTexture.filterMode = FilterMode.Bilinear;
        heightmapTexture.wrapMode = TextureWrapMode.Clamp;

        Color[] pixels = new Color[mapWidth * mapHeight];
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                float value = distanceField[x, y];
                pixels[y * mapWidth + x] = new Color(value, value, value, 1f);
            }
        }

        heightmapTexture.SetPixels(pixels);
        heightmapTexture.Apply();

        if (waterMaterial != null)
            waterMaterial.SetTexture("_HeightMap", heightmapTexture);
    }

    float[,] ComputeDistanceField(bool[,] mask, int width, int height)
    {
        float[,] distances = new float[width, height];
        const float INF = 9999f;

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                distances[x, y] = mask[x, y] ? INF : 0f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (!mask[x, y]) continue;

                float minDist = INF;

                int searchRadius = Mathf.CeilToInt(maxDepthDistance) + 1;
                for (int dx = -searchRadius; dx <= searchRadius; dx++)
                {
                    for (int dy = -searchRadius; dy <= searchRadius; dy++)
                    {
                        int nx = x + dx;
                        int ny = y + dy;

                        if (nx < 0 || nx >= width || ny < 0 || ny >= height)
                        {
                            float distToEdge = Mathf.Sqrt(dx * dx + dy * dy);
                            if (distToEdge < minDist) minDist = distToEdge;
                            continue;
                        }

                        if (!mask[nx, ny])
                        {
                            float dist = Mathf.Sqrt(dx * dx + dy * dy);
                            if (dist < minDist) minDist = dist;
                        }
                    }
                }

                distances[x, y] = minDist;
            }
        }

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                distances[x, y] = Mathf.Clamp01(distances[x, y] / maxDepthDistance);

        return distances;
    }

    void GenerateMesh()
    {
        if (waterMarkerTilemap == null) return;

        BoundsInt bounds = waterMarkerTilemap.cellBounds;
        Vector3 worldMin = waterMarkerTilemap.CellToWorld(new Vector3Int(bounds.xMin, bounds.yMin, 0));
        Vector3 worldMax = waterMarkerTilemap.CellToWorld(new Vector3Int(bounds.xMax, bounds.yMax, 0));

        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(worldMin.x, worldMin.y, -0.1f),
            new Vector3(worldMax.x, worldMin.y, -0.1f),
            new Vector3(worldMin.x, worldMax.y, -0.1f),
            new Vector3(worldMax.x, worldMax.y, -0.1f)
        };

        Vector2[] uvs = new Vector2[]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };

        int[] triangles = new int[] { 0, 2, 1, 2, 3, 1 };

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().material = waterMaterial;
    }
}