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
    private Vector2 _worldDeltas;

    [Header("Position")]
    [SerializeField] private int _zoom = 10;
    [SerializeField] private double _latitude;
    [SerializeField] private double _longitude;

    // URL fetcher
    private UrlFetcher _fetcher;
    // Layers of renderer
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

    // Get the current bounds of the visible tiles
    public Bounds CurrentBounds { get; private set; }
    // used by tile creation
    private float _fragDx, _fragDy;
    // TODO
    // indexes used by the recalculation.
    private Vector2Int indexMin, indexMax;
    [SerializeField] private Vector3 _unityPosition;

    private void Start() {
        _fragDx = _fragmentSize.x / 100f;
        _fragDy = _fragmentSize.y / 100f;

        ReloadUrlFetcher();
        ResetChildren();
    }

    /// <summary>
    /// Set the position on the world.
    /// </summary>
    /// <param name="position">The vector containing latitude and longitude.</param>
    public void SetPosition(Vector2 position) {
        _longitude = position.x;
        _latitude = position.y;
    }

	private void OnDrawGizmos() {
        //positions
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_unityPosition, 0.1f);
        // current bounds
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(CurrentBounds.center, CurrentBounds.size);
	}

    /// <summary>
    /// Reload the URL fetcher from the serializedField attributes.
    /// </summary>
	public void ReloadUrlFetcher() {
        _fetcher = _provider switch {
            UrlProvider.GoogleApi => new GoogleMapUrlFetcher(_providerToken, (uint) _fragmentSize.x, (uint) _fragmentSize.y),
            UrlProvider.OpenStreetTiles => new OpenStreetMapURL(),
            UrlProvider.GeoApiFy => new GeoApiFyUrlFetcher(_providerToken),
            _ => throw new System.Exception("Unknown provider : " + _provider + ".")
        };
    }

    /// <summary>
    /// Reset all grid fragments. Remove old ones, and recreate them all.
    /// </summary>
    public void ResetChildren() {
        // Remove immediate all children
        var tempList = transform.Cast<Transform>().ToList();
        foreach(var child in tempList) {
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

    /// <summary>
    /// Recalculate bounds after children updates
    /// </summary>
    /// <returns>a list of all visible fragments.</returns>
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

    /// <summary>
    /// Create a new grid fragment.
    /// </summary>
    /// <param name="i">The horizontal unique index.</param>
    /// <param name="j">The vertical unique index.</param>
    private void CreateMapElement(int i, int j) {
        // we want to be sure to not recretate an already-existing tile.
        MapRendererFragment f = RenderersLayer.Find(m => m.IndexI == i && m.IndexJ == j);
        if(f) {
            f.gameObject.SetActive(true);
            return;
        }
        // Create the fragment.
        GameObject go = new("map_" + i + "_" + j);
        var mapPart = go.GetOrAddComponent<MapRendererFragment>();
        mapPart.Init(_fragmentSize.x, _fragmentSize.y, i, j);
        go.transform.SetParent(RenderersLayerContainer);
        go.transform.localPosition = new(i * _fragDx, j * _fragDy, 0);
        go.GetOrAddComponent<SpriteRenderer>().sprite = _defaultSprite;
        RenderersLayer.Add(mapPart);
    }

    /// <summary>
    /// Update the image of all the visibles fragments.
    /// </summary>
    public void UpdateMap() {
        // Get the tiles-to-worldmap-tile indexes
        Vector2Int center = MapUtils.GetTile(_longitude, _latitude, _zoom);

        var visibles = RenderersLayer.FindAll(mr => mr.gameObject.activeSelf);
        foreach(var mr in visibles) {
            mr.UpdateMap(_fetcher, center.x + mr.IndexI - 1, center.y - mr.IndexJ + 1, _zoom);
		}

        _worldDeltas = GetWorldDeltas();
        _unityPosition = GetUnityPositionFromWorld(new Vector2((float) _longitude, (float) _latitude));
    }

    public void UpdateGridVisibility() {
        Bounds camera = PerspectivePan.Instance.CameraBounds;
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

        // TODO, finally,update elements.
        UpdateMap();
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

    public void ZoomLayerChange(bool zoomIn) {
        // Hide old layer
        RenderersLayerContainer.gameObject.SetActive(false);
        // Update zoom value.
        _zoom += (zoomIn ? 1 : -1);
        Debug.LogWarning("zoom changed. new zoom = " + _zoom);
        // Get new layer and show it
        RenderersLayerContainer.gameObject.SetActive(true);
        // If empty, create a first elem
        if(RenderersLayerContainer.childCount == 0) {
            var cameraBounds = PerspectivePan.Instance.RawCameraBounds;
            CreateMapElement((int) (cameraBounds.center.y / _fragDy), (int) (cameraBounds.center.x / _fragDy));
        }
        // update grid.
        UpdateGridVisibility();
    }

    private Vector2 GetWorldDeltas() {
        // a priori, j'aurais au moins le (0,0), le (1,0) et le (0,1).
        var t00 = MapRendererFragment.CENTER;
        var t10 = RenderersLayer.Find(m => m.IndexI == 1 && m.IndexJ == 0);
        var t01 = RenderersLayer.Find(m => m.IndexI == 0 && m.IndexJ == 1);
        if(!(t00 && t10 && t01)) {
            Debug.LogWarning("[GetUnityPositionFromWorld] Could NOT get all elements (" + t00 + ", " + t10 + "," + t01 + ")");
            return new();
        }
        Debug.Log("test des tiles. t00=" + t00.name + t00.WorldPosition+ ", t01=" + t01.name+t01.WorldPosition + ", t10=" + t10.name+t10.WorldPosition);
        float worldTileDx = t10.WorldPosition.x - t00.WorldPosition.x;
        float worldTileDy = t01.WorldPosition.y - t00.WorldPosition.y;
        Debug.Log("World D=(" + worldTileDx + ", " + worldTileDy + ").");
        return new(worldTileDx, worldTileDy);
    }

    public Vector2 GetUnityPositionFromWorld(Vector2 worldCoordinates) {
        var center = MapRendererFragment.CENTER;
        Debug.Log("------------------------------------------------");

        float distanceWorldX = worldCoordinates.x - center.WorldPosition.x;
        float distanceWorldY = worldCoordinates.y - center.WorldPosition.y;
        Debug.Log("worldCoos = " + worldCoordinates + ". centerTileWorldCoos="+center.WorldPosition+", distancesInWorld = (" + distanceWorldX + ", " + distanceWorldY + ").");

        float distanceUnityX = _worldDeltas.x * distanceWorldX * _fragDx * 10f;
        float distanceUnityY = _worldDeltas.y * distanceWorldY * _fragDy * 10f;

        Vector2 originalUnityPosition = center.TopLeft;
        Vector2 destination = originalUnityPosition + new Vector2(distanceUnityX, distanceUnityY);
        Debug.Log("distancesInUnity = (" + distanceUnityX + ", " + distanceUnityY + "). destination = " + destination);

        return destination;
	}
}