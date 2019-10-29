using System;
using System.Numerics;
using OpenGL;

namespace Example
{
	public class CubeDemo : IDemo
	{
		private static int width = 800, height = 600;
		ShaderProgram program;
		VAO cube;

		public CubeDemo()
		{
			program = new ShaderProgram(new Shader(vertexShader2Source, ShaderType.VertexShader), new Shader(fragmentShader2Source, ShaderType.FragmentShader));
			program["projection_matrix"].SetValue(Matrix4.CreatePerspectiveFieldOfView(0.45f, (float)width / height, 0.1f, 1000f));
			program["modelview_matrix"].SetValue(Matrix4.CreateTranslation(new Vector3(2, 2, -10)) * Matrix4.CreateRotation(new Vector3(1, -1, 0), 0.2f));
			program["color"].SetValue(new Vector3(0, 0, 1));

			Console.WriteLine(program.ProgramLog);

			cube = Geometry.CreateCube(program, new Vector3(-1, -1, -1), new Vector3(1, 1, 1));
		}

		public void Render()
		{
			Gl.Viewport(0, 0, width, height);
			Gl.ClearColor(0.3f, 0.4f, 0.6f, 1);
			Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			cube.Program.Use();
			cube.Draw();
		}


		static string vertexShader2Source = @"#version 330

layout(location = 0)in vec4 in_position;

uniform mat4 projection_matrix;
uniform mat4 modelview_matrix;

void main()
{
    gl_Position = projection_matrix * modelview_matrix * in_position;
}";

		static string fragmentShader2Source = @"#version 330
out vec4 fragColor;
uniform vec3 color;

void main()
{
    fragColor = vec4(color, 1.0);
}";
	}
}
