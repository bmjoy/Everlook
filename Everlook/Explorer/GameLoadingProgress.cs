﻿//
//  GameLoadingProgress.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) 2016 Jarl Gullberg
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

namespace Everlook.Explorer
{
	/// <summary>
	/// Container for progress reporting of a load operation for a game.
	/// </summary>
	public struct GameLoadingProgress
	{
		/// <summary>
		/// The overall completion percentage.
		/// </summary>
		public double CompletionPercentage;

		/// <summary>
		/// The state of the load operation at the time of reporting.
		/// </summary>
		public GameLoadingState State;

		/// <summary>
		/// The alias of the game which is being loaded.
		/// </summary>
		public string Alias;
	}
}
