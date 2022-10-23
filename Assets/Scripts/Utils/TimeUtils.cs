using System;

public class TimeUtils {
	private TimeUtils() {/* Forbid constructor */}

	private static readonly DateTime epoch = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

	public static long CurrentTimeMs => (long) (DateTime.UtcNow - epoch).TotalMilliseconds;
}