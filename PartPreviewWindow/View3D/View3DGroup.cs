﻿/*
Copyright (c) 2014, Lars Brubaker
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

using MatterHackers.Localizations;
using MatterHackers.MeshVisualizer;
using MatterHackers.PolygonMesh;
using System.ComponentModel;
using System.Globalization;
using System.Threading;

namespace MatterHackers.MatterControl.PartPreviewWindow
{
	public partial class View3DWidget
	{
		private void GroupSelectedMeshs()
		{
			if (MeshGroups.Count > 0)
			{
				processingProgressControl.PercentComplete = 0;
				processingProgressControl.Visible = true;
				LockEditControls();
				viewIsInEditModePreLock = true;

				BackgroundWorker createDiscreteMeshesBackgroundWorker = null;
				createDiscreteMeshesBackgroundWorker = new BackgroundWorker();

				createDiscreteMeshesBackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(groupSelectedBackgroundWorker_RunWorkerCompleted);
				createDiscreteMeshesBackgroundWorker.DoWork += new DoWorkEventHandler(groupSelectedBackgroundWorker_DoWork);

				createDiscreteMeshesBackgroundWorker.RunWorkerAsync();
			}
		}

		private void groupSelectedBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			string makingCopyLabel = LocalizedString.Get("Grouping");
			string makingCopyLabelFull = string.Format("{0}:", makingCopyLabel);
			processingProgressControl.ProcessType = makingCopyLabelFull;

			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			BackgroundWorker backgroundWorker = (BackgroundWorker)sender;

			PushMeshGroupDataToAsynchLists(TraceInfoOpperation.DO_COPY);

			for (int i = 0; i < asynchMeshGroups.Count; i++)
			{
				asynchMeshGroups[i].Transform(asynchMeshGroupTransforms[i].TotalTransform);

				bool continueProcessing;
				BackgroundWorker_ProgressChanged((i + 1) * .4 / asynchMeshGroups.Count, "", out continueProcessing);
			}

			DoGroup(backgroundWorker);
		}

		private void DoGroup(BackgroundWorker backgroundWorker)
		{
			if (SelectedMeshGroupIndex == -1)
			{
				SelectedMeshGroupIndex = 0;
			}
			MeshGroup meshGroupWeAreKeeping = asynchMeshGroups[SelectedMeshGroupIndex];
			for (int meshGroupToMoveIndex = asynchMeshGroups.Count - 1; meshGroupToMoveIndex >= 0; meshGroupToMoveIndex--)
			{
				MeshGroup meshGroupToMove = asynchMeshGroups[meshGroupToMoveIndex];
				if (meshGroupToMove != meshGroupWeAreKeeping)
				{
					for (int moveIndex = 0; moveIndex < meshGroupToMove.Meshes.Count; moveIndex++)
					{
						Mesh mesh = meshGroupToMove.Meshes[moveIndex];
						meshGroupWeAreKeeping.Meshes.Add(mesh);
					}

					asynchMeshGroups.RemoveAt(meshGroupToMoveIndex);
					asynchMeshGroupTransforms.RemoveAt(meshGroupToMoveIndex);
				}
				else
				{
					asynchMeshGroupTransforms[meshGroupToMoveIndex] = ScaleRotateTranslate.Identity();
				}
			}

			asynchPlatingDatas.Clear();
			double ratioPerMeshGroup = 1.0 / asynchMeshGroups.Count;
			double currentRatioDone = 0;
			for (int i = 0; i < asynchMeshGroups.Count; i++)
			{
				PlatingMeshGroupData newInfo = new PlatingMeshGroupData();
				asynchPlatingDatas.Add(newInfo);

				MeshGroup meshGroup = asynchMeshGroups[i];

				// create the selection info
				PlatingHelper.CreateITraceableForMeshGroup(asynchPlatingDatas, asynchMeshGroups, i, (double progress0To1, string processingState, out bool continueProcessing) =>
				{
					BackgroundWorker_ProgressChanged(progress0To1, processingState, out continueProcessing);
				});

				currentRatioDone += ratioPerMeshGroup;
			}
		}

		private void groupSelectedBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (WidgetHasBeenClosed)
			{
				return;
			}

			// remove the original mesh and replace it with these new meshes
			PullMeshGroupDataFromAsynchLists();

			// our selection changed to the mesh we just added which is at the end
			SelectedMeshGroupIndex = MeshGroups.Count - 1;

			UnlockEditControls();

			PartHasBeenChanged();

			Invalidate();
		}
	}
}