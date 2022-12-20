using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class MapRendererGrid : MonoBehaviour {

	#region Configuration

	[Header("Config")]
    [SerializeField] private UrlProvider _provider;
    [SerializeField] private string _providerToken;
    [SerializeField] private Sprite _defaultSprite;
    [SerializeField] private MessageRenderer _messageRendererPrefab;

    [Header("Size")]
    [SerializeField] private uint _amountX = 3;
    [SerializeField] private uint _amountY = 3;
    [SerializeField] private Vector2 _fragmentSize = new(256, 256);
    private Vector2 _worldDeltas;

    [Header("Position")]
    [SerializeField] private int _zoom = 10;
    [SerializeField] private double _latitude;
    [SerializeField] private double _longitude;

	#endregion

	#region Specific access

	public int Zoom => _zoom;
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
    // indexes used by the recalculation.
    private Vector2Int indexMin, indexMax;

    // current user position in Unity
    [SerializeField] private Vector3 _unityPosition;

    // Visible messages
    private readonly List<Message> visibleMessages = new();

	#endregion

	private void Start() {
        _fragDx = _fragmentSize.x / 100f;
        _fragDy = _fragmentSize.y / 100f;

        ReloadUrlFetcher();
        ResetChildren();
    }

    private void OnDrawGizmos() {
        //positions
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_unityPosition, 0.1f);
        // current bounds
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(CurrentBounds.center, CurrentBounds.size);
        // center
        if(PerspectivePan.Instance) {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(PerspectivePan.Instance.CameraBounds.min, PerspectivePan.Instance.CameraBounds.max);
            Gizmos.DrawLine(new(PerspectivePan.Instance.CameraBounds.min.x, PerspectivePan.Instance.CameraBounds.max.y, PerspectivePan.Instance.CameraBounds.max.z), new(PerspectivePan.Instance.CameraBounds.max.x, PerspectivePan.Instance.CameraBounds.min.y, PerspectivePan.Instance.CameraBounds.max.z));
            Gizmos.DrawSphere(PerspectivePan.Instance.CameraBounds.center, 0.02f);
        }
    }

    /// <summary>
    /// Set the position on the world.
    /// </summary>
    /// <param name="position">The vector containing latitude and longitude.</param>
    public void SetPosition(Vector2 position) {
        _longitude = position.x;
        _latitude = position.y;
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

    #region Messages management

    public void UpdateMessages(IEnumerable<Message> messages) {
        //XXX To improve. For now, just replace the list.
        visibleMessages.Clear();
        visibleMessages.AddRange(messages);
        foreach(var m in messages) {
            CreateMessageRenderer(m);
		}
	}

    private void CreateMessageRenderer(Message message) {
        var renderer = Instantiate(_messageRendererPrefab);
        renderer.SetMessage(message);
        var pos2d = GetUnityPositionFromWorld(message.RealWorldPosition);
        renderer.transform.position = new(pos2d.x, pos2d.y, -1f);
	}

	#endregion

	#region Global bounds and grid management

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
            // we also keep tiles that are loading
            mr.gameObject.SetActive(mr.IsLoading || mr.Bounds.Intersects2D(camera));
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

	#endregion

	#region Map fragment addition

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

	#endregion

	#region Layers management

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
        // before everything, we compute the WORLD POSITIOn center of the screen.
        var unityScreenCenter = PerspectivePan.Instance.CameraBounds.center;
        var oldWorldScreenCenter = GetWorldPositionFromUnity(unityScreenCenter);

        // Hide old layer
        RenderersLayerContainer.gameObject.SetActive(false);
        // Update zoom value.
        _zoom += (zoomIn ? 1 : -1);
        // Get new layer and show it
        RenderersLayerContainer.gameObject.SetActive(true);
        // Update center
        MapRendererFragment.CENTER = null; // reset center.
        var cameraBounds = PerspectivePan.Instance.RawCameraBounds;
        int ix = (int) (cameraBounds.center.x / _fragDx);
        int iy = (int) (cameraBounds.center.y / _fragDy);

        // If empty, create a first elem
        if(RenderersLayerContainer.childCount == 0) {
            // no child ? the CENTER will be the first created.
            CreateMapElement(ix, iy);
        } else {
            // Already child. If we found one at the center of the screen, we use it. if not, we get the first created (order is preserved, so first one previously created).
            var c = RenderersLayer.Find(m => m.IndexI == ix && m.IndexJ == iy);
            if(c)
                MapRendererFragment.CENTER = c;
            else
                MapRendererFragment.CENTER = RenderersLayerContainer.GetChild(0).GetComponent<MapRendererFragment>();
        }
        // update grid.
        UpdateGridVisibility();
        _worldDeltas = GetWorldDeltas(); // we update the worlddeltas.

        // then we recompute current screen center
        unityScreenCenter = PerspectivePan.Instance.CameraBounds.center;
        var newWorldScreenCenter = GetWorldPositionFromUnity(unityScreenCenter);

        var deltaOldAndNewCenters = (newWorldScreenCenter - oldWorldScreenCenter);
        Debug.Log("Zoom layer change. oldWorldCenter="+oldWorldScreenCenter+", newWorlCenter="+newWorldScreenCenter);
        Debug.Log("deltaWorldCenter="+ deltaOldAndNewCenters);
        PerspectivePan.Instance.ForceMove(deltaOldAndNewCenters);
    }

    #endregion

    #region World and Unity coordinates

    private int lZ = -1;
	private Vector2 GetWorldDeltas() {
        if(lZ == Zoom)
            return _worldDeltas;
        lZ = Zoom;
        // a priori, j'aurais au moins le (0,0), le (1,0) et le (0,1).
        var t00 = MapRendererFragment.CENTER;
        var t10 = RenderersLayer.Find(m => m.IndexI == t00.IndexI + 1 && m.IndexJ == t00.IndexJ);
        var t01 = RenderersLayer.Find(m => m.IndexI == t00.IndexI && m.IndexJ == t00.IndexJ + 1);
        if(!(t00 && t10 && t01)) {
            string s = "[";
            int n = 0;
            foreach(var t in RenderersLayerContainer.GetComponentsInChildren<MapRendererFragment>()) {
                if(n++>0)
                    s += ", ";
                s += "(" + t.IndexI + "," + t.IndexJ + ")";
            }
            Debug.LogWarning("[GetWorldDeltas] Could NOT get all elements [MISSING==" + (t00 ? "" : "(00), ") + (t01 ? "" : "(01), ") + (t10 ? "" : "(10)") + "].\n" +
            "List of current tiles :" + s + "]");
            return new();
        }
        //Debug.Log("test des tiles. t00=" + t00.name + t00.WorldPosition+ ", t01=" + t01.name+t01.WorldPosition + ", t10=" + t10.name+t10.WorldPosition);
        float worldTileDx = t10.WorldPosition.x - t00.WorldPosition.x;
        float worldTileDy = t01.WorldPosition.y - t00.WorldPosition.y;
        Debug.Log("World Delta=(" + worldTileDx + ", " + worldTileDy + ").");
        return new(worldTileDx, worldTileDy);
    }

    public Vector2 GetUnityPositionFromWorld(Vector2 worldCoordinates) {
        var center = MapRendererFragment.CENTER;
        //Debug.Log("------------------------------------------------");

        float distanceWorldX = worldCoordinates.x - center.WorldPosition.x;
        float distanceWorldY = worldCoordinates.y - center.WorldPosition.y;
        //Debug.Log("worldCoos = " + worldCoordinates + ". centerTileWorldCoos="+center.WorldPosition+", distancesInWorld = (" + distanceWorldX + ", " + distanceWorldY + ").");

        float distanceUnityX = _worldDeltas.x * distanceWorldX * _fragDx * 10f;
        float distanceUnityY = _worldDeltas.y * distanceWorldY * _fragDy * 10f;

        Vector2 originalUnityPosition = center.TopLeft;
        Vector2 destination = originalUnityPosition + new Vector2(distanceUnityX, distanceUnityY);
        //Debug.Log("distancesInUnity = (" + distanceUnityX + ", " + distanceUnityY + "). destination = " + destination);

        return destination;
	}

    public Vector2 GetWorldPositionFromUnity(Vector2 unityCoordinates) {
        var center = MapRendererFragment.CENTER;
        float distanceUnityX = unityCoordinates.x - center.TopLeft.x;
        float distanceUnityY = unityCoordinates.y - center.TopLeft.y;

        float distanceWorldX = _worldDeltas.x * distanceUnityX * _fragDx * 10f;
        float distanceWorldY = _worldDeltas.y * distanceUnityY * _fragDy * 10f;

        Vector2 originalWorldPosition = center.WorldPosition;
        Vector2 destination = originalWorldPosition + new Vector2(distanceWorldX, distanceWorldY);
        return destination;
    }

	#endregion
}