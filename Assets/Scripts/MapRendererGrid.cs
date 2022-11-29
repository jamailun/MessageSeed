using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class MapRendererGrid : MonoBehaviour {

    [Header("Config")]
    [SerializeField] private UrlProvider _provider;
    [SerializeField] private string _providerToken;
    [SerializeField] private Sprite _defaultSprite;

    [Header("Size")]
    [SerializeField] private uint _amountX = 3;
    [SerializeField] private uint _amountY = 3;
    [SerializeField] private Vector2 _fragmentSize = new(256, 256);

    [Header("Position")]
    [SerializeField] private int _zoom = 10;
    [SerializeField] private double _latitude = 39.299236d;
    [SerializeField] private double _longitude = -76.609383d;

    // fields
    private UrlFetcher _fetcher;
    //private MapRendererFragment[,] _renderers;
    private readonly List<MapRendererFragment> _renderers = new();
    private Vector3 _unityPosition;
    public Bounds CurrentBounds { get; private set; }

    private void Start() {
        ReloadUrlFetcher();
        UpdateChildren();
    }

    public void SetPosition(Vector2 position) {
        _latitude  = position.x;
        _longitude = position.y;
        _unityPosition = new((float) _latitude, (float) _longitude, transform.position.z);
    }

	private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_unityPosition - transform.position, 0.1f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.15f);
        Gizmos.DrawWireCube(CurrentBounds.center, CurrentBounds.size);
	}

	public void ReloadUrlFetcher() {
        _fetcher = _provider switch {
            UrlProvider.GoogleApi => new GoogleMapUrlFetcher(_providerToken, (uint) _fragmentSize.x, (uint) _fragmentSize.y),
            UrlProvider.OpenStreetTiles => new OpenStreetMapURL(),
            UrlProvider.GeoApiFy => new GeoApiFyUrlFetcher(_providerToken),
            _ => throw new System.Exception("Unknown provider : " + _provider + ".")
        };
    }

    public void UpdateChildren() {
        // Remove immediate all children
        var tempList = transform.Cast<Transform>().ToList();
        foreach(var child in tempList) {
            if(child.gameObject.GetComponent<MapRendererFragment>() != null)
                DestroyImmediate(child.gameObject);
        }
        float dx = _fragmentSize.x / 100f;
        float dy = _fragmentSize.y / 100f;

        CurrentBounds = new Bounds(
            // center
            transform.position,
            // size
            new Vector3()
        );

        // Add new children
        //_renderers = new MapRendererFragment[_amountX, _amountY];
        for(int i = 0; i < _amountX; i++) {
            for(int j = 0; j < _amountY; j++) {
                GameObject go = new("map_" + i + "_" + j);
                var mapPart = go.GetOrAddComponent<MapRendererFragment>();
                mapPart.Init(_fragmentSize.x, _fragmentSize.y, i, j);
                go.transform.SetParent(transform);
                go.transform.localPosition = new(i * dx, j * dy, 0);
                go.GetOrAddComponent<SpriteRenderer>().sprite = _defaultSprite;
                //_renderers[i, j] = mapPart;
                _renderers.Add(mapPart);

                CurrentBounds = CurrentBounds.Expand(mapPart.Bounds);
            }
        }
        Debug.Log("current bounds = " + CurrentBounds);

    }

    public void UpdateMap() {
        Vector2Int center = MapUtils.GetTile(_latitude, _longitude, _zoom);
        foreach(var mr in _renderers) {
            mr.UpdateMap(_fetcher, center.x + mr.IndexI - 1, center.y - mr.IndexJ + 1, _zoom);
		}
    }

    public void UpdateGridVisibility(Bounds camera) {
        // Cacher les tiles qui ne sont lus nécessaires
        foreach(var mr in _renderers) {
            if(!mr.Bounds.Intersects2D(camera)) {
                mr.gameObject.SetActive(false);
            } else {
                mr.gameObject.SetActive(true);
            }
        }
        // créer les tiles manquantes.

    }
}