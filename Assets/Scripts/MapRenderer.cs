using System.Collections;
using UnityEngine;

public class MapRenderer : MonoBehaviour {

    public enum UrlFecther {
        GoogleApi,
        OpenStreetTiles,
        GeoApiFy,
    }

    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Config")]

    [SerializeField] private UrlFecther provider;
    [SerializeField] private string providerToken;
    [SerializeField] private int zoom = 10;
    [SerializeField] private Vector2Int size;
    [SerializeField] private Vector2 position = new(48.8485295f, 2.2609919f);

    private UrlFetcher urlProvider;

    private void Start() {
        if(!spriteRenderer)
            spriteRenderer = GetComponent<SpriteRenderer>();
        if(!spriteRenderer)
            Debug.LogError("Could not find any sprite renderer for MapRenderer " + name + ".");

        ReloadProvider();

        UpdateMap();
    }

    public void ReloadProvider() {
        urlProvider = provider switch {
            UrlFecther.GoogleApi => new GoogleMapUrlFetcher(providerToken, (uint) size.x, (uint) size.y),
            UrlFecther.OpenStreetTiles => new OpenStreetMapURL(),
            UrlFecther.GeoApiFy => new GeoApiFyUrlFetcher(providerToken),
            _ => throw new System.Exception("Unknown provider : " + provider + ".")
        };
    }

    private IEnumerator LoadSprite() {
        //WWW www = new WWW(exampleUrl + key + apiKey);
        WWW www = new WWW(urlProvider.CreateUrl(position, zoom));
        yield return www;
        Debug.Log("Image successfully fetched.");
        spriteRenderer.sprite = Sprite.Create(www.texture, new Rect(0, 0, size.x, size.y), new Vector2());
    }

    public void UpdateMap() {
        StartCoroutine(LoadSprite());
    }
}