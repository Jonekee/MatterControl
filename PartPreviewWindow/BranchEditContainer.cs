﻿/*
Copyright (c) 2018, John Lewin
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

The views and conclusions contained in the software and documentation are those
of the authors and should not be interpreted as representing official policies,
either expressed or implied, of the FreeBSD Project.
*/

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MatterHackers.Agg.Image;
using MatterHackers.DataConverters3D;
using MatterHackers.MatterControl.Library;

namespace MatterHackers.MatterControl.PartPreviewWindow
{
	public class BranchEditContainer : ILibraryWritableContainer
	{
		private Action<ILibraryItem, IObject3D> saveAction;

		public string ID => "";

		public string Name => "";

		public string StatusMessage => "";

		public string KeywordFilter { get; set; }

		public bool IsProtected => false;

		public Type DefaultView => null;

		public BranchEditContainer(Action<ILibraryItem, IObject3D> saveAction)
		{
			this.saveAction = saveAction;
		}

		public List<ILibraryContainerLink> ChildContainers { get; } = new List<ILibraryContainerLink>();

		public List<ILibraryItem> Items { get; } = new List<ILibraryItem>();

		public ILibraryContainer Parent { get; set; } = null;

		public event EventHandler<ItemChangedEventArgs> ItemContentChanged;
		public event EventHandler ContentChanged;

		public void Activate()
		{
		}

		public void Add(IEnumerable<ILibraryItem> items)
		{
		}

		public bool AllowAction(ContainerActions containerActions)
		{
			return true;
		}

		public void Deactivate()
		{
		}

		public void Dispose()
		{
		}

		public Task<ImageBuffer> GetThumbnail(ILibraryItem item, int width, int height) => null;

		public void Load()
		{
		}

		public void Move(IEnumerable<ILibraryItem> items, ILibraryWritableContainer sourceContainer)
		{
		}

		public void Remove(IEnumerable<ILibraryItem> items)
		{
		}

		public void Rename(ILibraryItem item, string revisedName)
		{
		}

		public void Save(ILibraryItem item, IObject3D content)
		{
			saveAction?.Invoke(item, content);
		}

		public void SetThumbnail(ILibraryItem item, int width, int height, ImageBuffer imageBuffer)
		{
		}
	}
}