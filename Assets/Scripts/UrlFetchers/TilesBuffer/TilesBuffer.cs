using System.Collections.Generic;
using UnityEngine;

public struct TextureQuery {
	public readonly bool found;
	public readonly Sprite texture;
	public TextureQuery(Sprite texture) {
		found = texture != null;
		this.texture = texture;
	}
}

public class TilesBuffer {
	private static readonly object Lock = new();

	private readonly TextureQuery EMPTY_QUERY = new(null);

	private static TilesBuffer _instance;
	public static TilesBuffer Instance {
		get {
			if(_instance == null)
				_instance = new();
			return _instance;
		}
	}

	private readonly Dictionary<int, Dictionary<Vector2Int, Sprite>> buffer = new();

	public TextureQuery TryGet(int zoom, int x, int y) {
		lock(Lock) {
			if(!buffer.ContainsKey(zoom))
				return EMPTY_QUERY;
			var sub = buffer[zoom];
			Vector2Int point = new(x, y);
			if(!sub.ContainsKey(point))
				return EMPTY_QUERY;
			return new(sub[point]);
		}
	}

	public void Clear() {
		lock(Lock) {
			buffer.Clear();
		}
	}

	public void Put(int zoom, int x, int y, Sprite texture) {
		lock(Lock) {
			if(!buffer.ContainsKey(zoom))
				buffer.Add(zoom, new());
			buffer[zoom][new(x, y)] = texture;
		}
	}
	
}