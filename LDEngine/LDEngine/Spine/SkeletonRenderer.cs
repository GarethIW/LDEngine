/*******************************************************************************
 * Copyright (c) 2013, Esoteric Software
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 * 
 * 1. Redistributions of source code must retain the above copyright notice, this
 *    list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright notice,
 *    this list of conditions and the following disclaimer in the documentation
 *    and/or other materials provided with the distribution.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
 * ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 ******************************************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Spine {
	public class SkeletonRenderer {
		GraphicsDevice device;
		SpriteBatcher batcher;
		BasicEffect effect;
		RasterizerState rasterizerState;

		public SkeletonRenderer (GraphicsDevice device) {
			this.device = device;

			batcher = new SpriteBatcher();

			effect = new BasicEffect(device);
			effect.World = Matrix.Identity;
			effect.View = Matrix.CreateLookAt(new Vector3(0.0f, 0.0f, 1.0f), Vector3.Zero, Vector3.Up);
			effect.TextureEnabled = true;
            effect.VertexColorEnabled = true;

			rasterizerState = new RasterizerState();
			rasterizerState.CullMode = CullMode.None;

			Bone.yDown = true;
		}

		public void Begin (GraphicsDevice gd, Matrix cameraMatrix) {
			device.RasterizerState = rasterizerState;
            //device.BlendState = BlendState.AlphaBlend;

			effect.Projection = Matrix.CreateOrthographicOffCenter(0, gd.Viewport.Width, gd.Viewport.Height, 0, 1, 0);
            effect.View = Matrix.CreateLookAt(new Vector3(0.0f, 0.0f, 1.0f), Vector3.Zero, Vector3.Up) * cameraMatrix;
		}

		public void End () {
           
			foreach (EffectPass pass in effect.CurrentTechnique.Passes) {
				pass.Apply();
				batcher.Draw(device);
			}
		}

		public void Draw (Skeleton skeleton) {
			List<Slot> drawOrder = skeleton.DrawOrder;
			for (int i = 0, n = drawOrder.Count; i < n; i++) {
				Slot slot = drawOrder[i];
				Attachment attachment = slot.Attachment;
				if (attachment == null)
					continue;
				if (attachment is RegionAttachment) {
					RegionAttachment regionAttachment = (RegionAttachment)attachment;

					SpriteBatchItem item = batcher.CreateBatchItem();
					item.Texture = ((XnaAtlasPage)regionAttachment.Region.Page).Texture;


                    item.vertexTL.Color = new Color(slot.R, slot.G, slot.B, slot.A);

                    item.vertexBL.Color = new Color(slot.R, slot.G, slot.B, slot.A);

                    item.vertexBR.Color = new Color(slot.R, slot.G, slot.B, slot.A);

                    item.vertexTR.Color = new Color(slot.R, slot.G, slot.B, slot.A);


					regionAttachment.UpdateVertices(slot.Bone);
					float[] vertices = regionAttachment.Vertices;
					item.vertexTL.Position.X = vertices[RegionAttachment.X1];
					item.vertexTL.Position.Y = vertices[RegionAttachment.Y1];
					item.vertexTL.Position.Z = 0;
					item.vertexBL.Position.X = vertices[RegionAttachment.X2];
					item.vertexBL.Position.Y = vertices[RegionAttachment.Y2];
					item.vertexBL.Position.Z = 0;
					item.vertexBR.Position.X = vertices[RegionAttachment.X3];
					item.vertexBR.Position.Y = vertices[RegionAttachment.Y3];
					item.vertexBR.Position.Z = 0;
					item.vertexTR.Position.X = vertices[RegionAttachment.X4];
					item.vertexTR.Position.Y = vertices[RegionAttachment.Y4];
					item.vertexTR.Position.Z = 0;

					float[] uvs = regionAttachment.UVs;
					item.vertexTL.TextureCoordinate.X = uvs[RegionAttachment.X1];
					item.vertexTL.TextureCoordinate.Y = uvs[RegionAttachment.Y1];
					item.vertexBL.TextureCoordinate.X = uvs[RegionAttachment.X2];
					item.vertexBL.TextureCoordinate.Y = uvs[RegionAttachment.Y2];
					item.vertexBR.TextureCoordinate.X = uvs[RegionAttachment.X3];
					item.vertexBR.TextureCoordinate.Y = uvs[RegionAttachment.Y3];
					item.vertexTR.TextureCoordinate.X = uvs[RegionAttachment.X4];
					item.vertexTR.TextureCoordinate.Y = uvs[RegionAttachment.Y4];
				}
			}
		}
	}
}
