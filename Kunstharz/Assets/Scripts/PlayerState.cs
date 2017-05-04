using System;

namespace Kunstharz
{
	public enum PlayerState
	{
		AwaitingRoundStart,
		SelectingMotion,
		SelectedMotion,
		ExecutingMotion,
		ExecutedMotion,
		SelectingShot,
		ExecutingShot,
		Dead,
		Victorious
	}
}

