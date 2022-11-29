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
    private readonly Dictionary<int, FragmentsLayer> _renderersMap = new();
    private List<MapRendererFragment> RenderersLayer {
        get {
            GarantyLayerExists();
            return _renderersMap[_zoom].list;
        }
    }
    private Transform RenderersLayerContainer {
        get {
            GarantyLayerExists();
            return _renderersMap[_zoom].container;
        }
    }


    private Vector3 _unityPosition;
    public Bounds CurrentBounds { get; private set; }

    private float _fragDx, _fragDy;

    private void Start() {
        _fragDx = _fragmentSize.x / 100f;
        _fragDy = _fragmentSize.y / 100f;

        ReloadUrlFetcher();
        UpdateChildren();
    }

    public void SetPosition(Vector2 position) {
        _latitude  = position.x;
        _longitude = position.y;
        _unityPosition = new((float) _latitude, (float) _longitude, transform.position.z);
    }

	private void OnDrawGizmos() {
        //positions
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_unityPosition - transform.position, 0.1f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.15f);
        // current bounds
        Gizmos.DrawWireCube(CurrentBounds.center, CurrentBounds.size);
        // index bounds
        Gizmos.color = Color.white;
        var ib = new Vector3(_fragDx * (indexMax.x - indexMin.x), _fragDy * (indexMax.y - indexMin.y));
        Gizmos.DrawWireCube(ib / 2f, ib);
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

        // Add new children
        for(int i = 0; i < _amountX; i++) {
            for(int j = 0; j < _amountY; j++) {
                CreateMapElement(i, j);
            }
        }

        // Recalculate bounds
        RecalculateBounds();
        Debug.Log("current bounds = " + CurrentBounds);
    }

    private Vector2Int indexMin, indexMax;
    // Recalculate bounds after children updates
    private List<MapRendererFragment> RecalculateBounds() {
        indexMin = new(int.MaxValue, int.MaxValue);
        indexMax = new(int.MinValue, int.MinValue);
        var visible = RenderersLayer.Find(mr => mr.gameObject.activeInHierarchy);
        // Reset the bounds
        CurrentBounds = new Bounds(
            // center
            visible == null ? transform.position : visible.Bounds.center,
             // size
             new Vector3()
        );
        List<MapRendererFragment> visibleFragments = new();
        // Expand with all visibles trucs
        foreach(var mr in RenderersLayer) {
            if(mr.gameObject.activeInHierarchy) {
                // expand the bound
                CurrentBounds = CurrentBounds.Expand(mr.Bounds);
                // try to expand the indexes
                if(mr.IndexI < indexMin.x)
                    indexMin.x = mr.IndexI;
                if(mr.IndexJ < indexMin.y)
                    indexMin.y = mr.IndexJ;
                if(mr.IndexI > indexMax.x)
                    indexMax.x = mr.IndexI;
                if(mr.IndexJ > indexMax.y)
                    indexMax.y = mr.IndexJ;
                // Add the fragment to he visible list
            }
        }
        // Return all visible tiles
        return visibleFragments;
    }

    private void CreateMapElement(int i, int j) {
        GameObject go = new("map_" + i + "_" + j);
        var mapPart = go.GetOrAddComponent<MapRendererFragment>();
        mapPart.Init(_fragmentSize.x, _fragmentSize.y, i, j);
        go.transform.SetParent(RenderersLayerContainer);
        go.transform.localPosition = new(i * _fragDx, j * _fragDy, 0);
        go.GetOrAddComponent<SpriteRenderer>().sprite = _defaultSprite;
        RenderersLayer.Add(mapPart);
    }

    public void UpdateMap() {
        Vector2Int center = MapUtils.GetTile(_latitude, _longitude, _zoom);
        foreach(var mr in RenderersLayer) {
            mr.UpdateMap(_fetcher, center.x + mr.IndexI - 1, center.y - mr.IndexJ + 1, _zoom);
		}
    }

    public void UpdateGridVisibility(Bounds camera) {
        // Hide non-visible tiles.
        foreach(var mr in RenderersLayer) {
            mr.gameObject.SetActive(mr.Bounds.Intersects2D(camera));
        }

        // Recalculate with hidden elements
        var visibles = RecalculateBounds();

        // Create missing tiles
        // For each direction (left, right, up, down) we add a row/column if the camera overflows from the bounds

        // LEFT : add a column on the left
        if(camera.min.x < CurrentBounds.min.x) {
            visibles = AddMapElementsColumn(indexMin.x - 1, indexMin.y, indexMax.y);
        }

        // RIGHT : add a column on the right
        if(camera.max.x > CurrentBounds.max.x) {
            visibles = AddMapElementsColumn(indexMax.x + 1, indexMin.y, indexMax.y);
        }

        // BOTTOM : add a column at the bottom
        if(camera.min.y < CurrentBounds.min.y) {
            visibles = AddMapElementsRow(indexMin.y - 1, indexMin.x, indexMax.x);
        }

        // TOP : add a column on the top
        if(camera.max.y > CurrentBounds.max.y) {
            visibles = AddMapElementsRow(indexMax.y + 1, indexMin.x, indexMax.x);
        }

        // check if no fragment is missing INSIDE the indexes
        for(int i = indexMin.x; i <= indexMax.x; i++) {
            for(int j = indexMin.y; j <= indexMax.y; j++) {
                var mr = visibles.Find(r => r.IndexI == i && r.IndexJ == j);
                if(mr == null) {
                    CreateMapElement(i, j);
                }
            }
        }

    }

    private List<MapRendererFragment> AddMapElementsColumn(int x, int yMin, int yMax) {
        for(int y = yMin; y <= yMax; y++) {
            CreateMapElement(x, y);
        }
        return RecalculateBounds();
    }
    private List<MapRendererFragment> AddMapElementsRow(int y, int xMin, int xMax) {
        for(int x = xMin; x <= xMax; x++) {
            CreateMapElement(x, y);
        }
        return RecalculateBounds();
    }
    private class FragmentsLayer {
        public readonly List<MapRendererFragment> list = new();
        public readonly Transform container;
        public FragmentsLayer(Transform container) {
            this.container = container;
        }
    }
    private void GarantyLayerExists() {
        if(!_renderersMap.ContainsKey(_zoom)) {
            var obj = new GameObject("layer_" + _zoom);
            obj.transform.SetParent(transform);
            _renderersMap[_zoom] = new(obj.transform);
        }
    }
}