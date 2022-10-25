using System.Linq;
using UnityEngine;

public class MapRendererGrid : MonoBehaviour {

    [Header("Config")]
    [SerializeField] private UrlProvider provider;
    [SerializeField] private string providerToken;
    [SerializeField] private Sprite defaultSprite;

    [Header("Size")]
    [SerializeField] private uint amountX = 3;
    [SerializeField] private uint amountY = 3;
    [SerializeField] private Vector2 fragmentSize = new(256, 256);

    [Header("Position")]
    [SerializeField] private int zoom = 10;
    [SerializeField] private double latitude = 39.299236d;
    [SerializeField] private double longitude = -76.609383d;

    // fields
    private UrlFetcher fetcher;
    private MapRendererFragment[,] renderers;

    private void Start() {
        ReloadUrlFetcher();
        UpdateChildren();
    }

    public void SetPosition(Vector2 position) {
        latitude  = position.x;
        longitude = position.y;
    }

    public void ReloadUrlFetcher() {
        fetcher = provider switch {
            UrlProvider.GoogleApi => new GoogleMapUrlFetcher(providerToken, (uint) fragmentSize.x, (uint) fragmentSize.y),
            UrlProvider.OpenStreetTiles => new OpenStreetMapURL(),
            UrlProvider.GeoApiFy => new GeoApiFyUrlFetcher(providerToken),
            _ => throw new System.Exception("Unknown provider : " + provider + ".")
        };
    }

    public void UpdateChildren() {
        // Remove immediate all children
        var tempList = transform.Cast<Transform>().ToList();
        foreach(var child in tempList) {
            if(child.gameObject.GetComponent<MapRendererFragment>() != null)
                DestroyImmediate(child.gameObject);
        }
        float dx = fragmentSize.x / 100f;
        float dy = fragmentSize.y / 100f;
        // Add new children
        renderers = new MapRendererFragment[amountX, amountY];
        for(int i = 0; i < amountX; i++) {
            for(int j = 0; j < amountY; j++) {
                GameObject go = new("map_" + i + "_" + j);
                var mapPart = go.GetOrAddComponent<MapRendererFragment>();
                mapPart.Init(fragmentSize.x, fragmentSize.y);
                go.transform.SetParent(transform);
                go.transform.localPosition = new(i * dx, j * dy, 0);
                go.GetOrAddComponent<SpriteRenderer>().sprite = defaultSprite;
                renderers[i, j] = mapPart;
            }
        }
    }

    public void UpdateMap() {
        Vector2Int center = MapUtils.GetTile(latitude, longitude, zoom);
        for(int i = 0; i < amountX; i++) {
            for(int j = 0; j < amountY; j++) {
                renderers[i, j].UpdateMap(fetcher, center.x + i - 1, center.y - j + 1, zoom);
            }
        }
    }
}