﻿//
//  GamePathStorage.cs
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

using System;
using System.Collections.Generic;
using System.IO;
using log4net;
using Warcraft.Core;
using Warcraft.Core.Extensions;

namespace Everlook.Configuration
{
	/// <summary>
	/// Game path storage. This class handles storing a set of game paths in a binary file.
	/// A binary file format is used for maximal cross-platform compatibility - a null character
	/// is, unfortunately, the only safe separator for multiple paths in current and future systems.
	/// Yes, Linux allows tabs, newlines and carriage returns in paths.
	///
	/// Read <a href="http://stackoverflow.com/a/1976172"/> for a good time.
	/// </summary>
	public sealed class GamePathStorage
	{
		/// <summary>
		/// The current format version of the path store.
		/// </summary>
		private const int FormatVersion = 3;

		/// <summary>
		/// Logger instance for this class.
		/// </summary>
		private static readonly ILog Log = LogManager.GetLogger(typeof(GamePathStorage));

		private readonly object StorageLock = new object();

		/// <summary>
		/// A static instance of the path storage class.
		/// </summary>
		public static readonly GamePathStorage Instance = new GamePathStorage();

		/// <summary>
		/// Gets the stored game paths.
		/// </summary>
		/// <value>The game paths.</value>
		public List<(string Alias, WarcraftVersion Version, string Path)> GamePaths => ReadStoredPaths();

		private GamePathStorage()
		{
			string storagePath = GetPathStoragePath();
			if (File.Exists(storagePath))
			{
				// Check the format version
				int formatVersion;
				using (FileStream fs = File.OpenRead(GetPathStoragePath()))
				{
					using (BinaryReader br = new BinaryReader(fs))
					{
						formatVersion = br.ReadInt32();
					}
				}

				if (formatVersion != FormatVersion)
				{
					File.Delete(GetPathStoragePath());
				}
				else
				{
					return;
				}
			}

			string storageDirectory = Directory.GetParent(storagePath).FullName;
			if (!Directory.Exists(storageDirectory))
			{
				Directory.CreateDirectory(storageDirectory);
			}

			ClearPaths();
		}

		/// <summary>
		/// Stores a provided path in the path storage.
		/// </summary>
		/// <param name="alias">The alias of the path which is displayed in the UI.</param>
		/// <param name="version">The version that the game at the path conforms to.</param>
		/// <param name="pathToStore">Path to store.</param>
		public void StorePath(string alias, WarcraftVersion version, string pathToStore)
		{
			if (!this.GamePaths.Contains((alias, version, pathToStore)))
			{
				lock (this.StorageLock)
				{
					using (FileStream fs = File.Open(GetPathStoragePath(), FileMode.Append))
					{
						using (BinaryWriter bw = new BinaryWriter(fs))
						{
							bw.WriteNullTerminatedString(alias);
							bw.Write((uint)version);
							bw.WriteNullTerminatedString(pathToStore);
							bw.Flush();
						}
					}
				}
			}
		}

		/// <summary>
		/// Removes a path that's been stored.
		/// </summary>
		/// <param name="alias">The alias of the path.</param>
		/// <param name="version">The game version of the path.</param>
		/// <param name="pathToRemove">Path to remove.</param>
		public void RemoveStoredPath(string alias, WarcraftVersion version, string pathToRemove)
		{
			List<(string Alias, WarcraftVersion Version, string Path)> storedPaths = this.GamePaths;
			if (storedPaths.Contains((alias, version, pathToRemove)))
			{
				ClearPaths();
				lock (this.StorageLock)
				{
					storedPaths.Remove((alias, version, pathToRemove));

					using (FileStream fs = File.Open(GetPathStoragePath(), FileMode.Append))
					{
						using (BinaryWriter bw = new BinaryWriter(fs))
						{
							foreach ((string remainingAlias, WarcraftVersion remainingVersion, string remainingPath) in storedPaths)
							{
								bw.WriteNullTerminatedString(remainingAlias);
								bw.Write((uint)remainingVersion);
								bw.WriteNullTerminatedString(remainingPath);
								bw.Flush();
							}
						}
					}
				}
			}
		}

		private List<(string Alias, WarcraftVersion Version, string Path)> ReadStoredPaths()
		{
			List<(string, WarcraftVersion, string)> storedPaths = new List<(string, WarcraftVersion, string)>();
			lock (this.StorageLock)
			{
				try
				{
					using (FileStream fs = File.OpenRead(GetPathStoragePath()))
					{
						using (BinaryReader br = new BinaryReader(fs))
						{
							int formatVersion = br.ReadInt32();
							if (formatVersion != FormatVersion)
							{
								Log.Warn("Read an unsupported path store version. Aborting.");
							}
							else
							{
								while (br.BaseStream.Position != br.BaseStream.Length)
								{
									storedPaths.Add
									(
										(br.ReadNullTerminatedString(), (WarcraftVersion)br.ReadUInt32(), br.ReadNullTerminatedString())
									);
								}
							}
						}
					}
				}
				catch (EndOfStreamException e)
				{
					File.Delete(GetPathStoragePath());
					Log.Warn("Failed to read the stored paths with a fatal error. Deleting path store.", e);
				}
			}

			return storedPaths;
		}

		private void ClearPaths()
		{
			lock (this.StorageLock)
			{
				File.Delete(GetPathStoragePath());

				using (FileStream fs = File.Create(GetPathStoragePath()))
				{
					using (BinaryWriter bw = new BinaryWriter(fs))
					{
						bw.Write(FormatVersion);
					}
				}
			}
		}

		/// <summary>
		/// Gets the path to the game path storage.
		/// </summary>
		/// <returns>The path storage path.</returns>
		private static string GetPathStoragePath()
		{
			return
				$"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}{Path.DirectorySeparatorChar}" +
				$"Everlook{Path.DirectorySeparatorChar}gamepaths.store";
		}
	}
}
