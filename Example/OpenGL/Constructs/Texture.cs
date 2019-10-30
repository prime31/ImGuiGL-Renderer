using System;

namespace OpenGLLLLLLLL.Slim
{
	public class Texture : IDisposable
	{
		public uint TextureID;
		public int Width, Height;
		public GL.TextureTarget TextureTarget;


		public Texture(IntPtr pixelData, int width, int height)
			: this(pixelData, width, height, GL.PixelFormat.Bgra, GL.PixelInternalFormat.Rgba8)
		{ }

		public Texture(IntPtr pixelData, int width, int height, GL.PixelFormat format, GL.PixelInternalFormat internalFormat)
		{
			Width = width;
			Height = height;

			// set the texture target and then generate the texture ID
			TextureTarget = GL.TextureTarget.Texture2D;
			TextureID = GL.GenTexture();

			GL.glPixelStorei(GL.PixelStoreParameter.UnpackAlignment, 1); // set pixel alignment
			GL.glBindTexture(TextureTarget, TextureID);     // bind the texture to memory in OpenGL

			//Gl.TexParameteri(TextureTarget, TextureParameterName.GenerateMipmap, 0);
			GL.glTexImage2D(TextureTarget, 0, internalFormat, width, height, 0, format, GL.PixelType.UnsignedByte, pixelData);
			GL.glTexParameteri(TextureTarget, GL.TextureParameterName.TextureMagFilter, GL.TextureParameter.Linear);
			GL.glTexParameteri(TextureTarget, GL.TextureParameterName.TextureMinFilter, GL.TextureParameter.Linear);

			GL.glBindTexture(TextureTarget, 0);
		}

		~Texture()
		{
			Dispose(false);
		}


		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (TextureID != 0)
			{
				GL.DeleteTexture(TextureID);
				TextureID = 0;
			}
		}
	}

}
