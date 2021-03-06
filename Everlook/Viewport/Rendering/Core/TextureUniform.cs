﻿//
//  TextureUniform.cs
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

#pragma warning disable SA1025 // Code must not contain multiple whitespace in a row, used for alignment

namespace Everlook.Viewport.Rendering.Core
{
	/// <summary>
	/// Uniform slots for textures, used for identifying the corresponding variables and slots in the shader.
	/// </summary>
	public enum TextureUniform
	{
		/// <summary>
		/// The first diffuse texture.
		/// </summary>
		Diffuse0  = 0,

		/// <summary>
		/// The first specular texture.
		/// </summary>
		Specular0 = 1,

		/// <summary>
		/// The second diffuse texture.
		/// </summary>
		Diffuse1  = 2,

		/// <summary>
		/// The second specular texture.
		/// </summary>
		Specular1 = 3,

		/// <summary>
		/// An environment map.
		/// </summary>
		EnvMap    = 4
	}
}
