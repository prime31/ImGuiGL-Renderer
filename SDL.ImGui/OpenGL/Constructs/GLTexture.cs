using System;
using static SDL.ImGuiRenderer.GL;

namespace SDL.ImGuiRenderer
{
	public sealed class GLTexture : IDisposable
	{
		public uint TextureID;
		public int Width, Height;
		TextureTarget TextureTarget;


		public GLTexture(IntPtr pixelData, int width, int height)
			: this(pixelData, width, height, PixelFormat.Bgra, PixelInternalFormat.Rgba8)
		{ }

		public GLTexture(IntPtr pixelData, int width, int height, PixelFormat format, PixelInternalFormat internalFormat)
		{
			Width = width;
			Height = height;

			// set the texture target and then generate the texture ID
			TextureTarget = TextureTarget.Texture2D;
			TextureID = GenTexture();

			glPixelStorei(PixelStoreParameter.UnpackAlignment, 1);
			glBindTexture(TextureTarget, TextureID);

			glTexImage2D(TextureTarget, 0, internalFormat, width, height, 0, format, PixelType.UnsignedByte, pixelData);
			glTexParameteri(TextureTarget, TextureParameterName.TextureMagFilter, TextureParameter.Linear);
			glTexParameteri(TextureTarget, TextureParameterName.TextureMinFilter, TextureParameter.Linear);

			glBindTexture(TextureTarget, 0);
		}

		~GLTexture() => Dispose(false);


		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		void Dispose(bool disposing)
		{
			if (TextureID != 0)
			{
				DeleteTexture(TextureID);
				TextureID = 0;
			}
		}
	}

}
