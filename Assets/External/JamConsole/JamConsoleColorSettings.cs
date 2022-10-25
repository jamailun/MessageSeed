using UnityEngine;

namespace JamUtils2D.JamConsole {
	[System.Serializable]
	public class JamConsoleColorSettings {

		[SerializeField] Color _colorLog = Color.white;
		[SerializeField] Color _colorWarn = Color.yellow;
		[SerializeField] Color _colorError = Color.red;

		public Color ColorLog => _colorLog;
		public Color ColorWarning => _colorWarn;
		public Color ColorError => _colorError;

		public Color FromLogType(LogType type) {
			return type switch {
				LogType.Warning => ColorWarning,
				LogType.Error => ColorError,
				LogType.Exception => ColorError,
				_ => ColorLog,
			};
		}

	}
}