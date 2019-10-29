using System;
using System.Numerics;
using OpenGL;

namespace Example
{
	public class Primitives : IDemo
	{
		int width = 800, height = 600;
		ShaderProgram program;
		VBO<Vector3> triangle;
		VBO<Vector3> triangleColor;
		VBO<uint> triangleElements;
		uint ID;

		VAO _tri;

		public Primitives()
		{
			Gl.Enable(EnableCap.DepthTest);
			Gl.Disable(EnableCap.CullFace);

			// compile the shader program
			program = new ShaderProgram(VertexShader, FragmentShader);

			// set the view and projection matrix, which are static throughout this tutorial
			program.Use();
			program["projection_matrix"].SetValue(Matrix4.CreatePerspectiveFieldOfView(0.45f, (float)width / height, 0.1f, 1000f));
			program["view_matrix"].SetValue(Matrix4.LookAt(new Vector3(0, 0, 10), Vector3.Zero, new Vector3(0, 1, 0)));

			// create a triangle with vertices and colors
			triangle = new VBO<Vector3>(new Vector3[] { new Vector3(0, 1, 0), new Vector3(-1, -1, 0), new Vector3(1, -1, 0) });
			triangleColor = new VBO<Vector3>(new Vector3[] { new Vector3(1, 0, 0), new Vector3(0, 1, 0), new Vector3(0, 0, 1) });
			triangleElements = new VBO<uint>(new uint[] { 0, 1, 2 }, BufferTarget.ElementArrayBuffer);

			ID = Gl.GenVertexArray();
			Gl.BindVertexArray(ID);

			var loc = program["in_position"].Location;
			Gl.EnableVertexAttribArray(loc);
			Gl.BindBuffer(triangle.BufferTarget, triangle.ID);
			Gl.VertexAttribPointer(loc, triangle.Size, triangle.PointerType, triangle.Normalize, 0, IntPtr.Zero);

			loc = program["in_color"].Location;
			Gl.EnableVertexAttribArray(loc);
			Gl.BindBuffer(triangleColor.BufferTarget, triangleColor.ID);
			Gl.VertexAttribPointer(loc, triangleColor.Size, triangleColor.PointerType, triangleColor.Normalize, 0, IntPtr.Zero);

			Gl.BindBuffer(BufferTarget.ElementArrayBuffer, triangleElements.ID);

			Gl.BindVertexArray(0);


			var vbos = new IGenericVBO[3];
			vbos[0] = new GenericVAO.GenericVBO<Vector3>(triangle, "in_position");
			vbos[1] = new GenericVAO.GenericVBO<Vector3>(triangleColor, "in_color");
			vbos[2] = new GenericVAO.GenericVBO<uint>(triangleElements);
			_tri = new VAO(program, vbos);
		}

		public void Render()
		{
			// set up the OpenGL viewport and clear both the color and depth bits
			Gl.Viewport(0, 0, width, height);
			Gl.ClearColor(0.7f, 0.4f, 0.9f, 1);
			Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			_tri.Program.Use();
			_tri.Program["model_matrix"].SetValue(Matrix4.CreateTranslation(new Vector3(1.5f, 0, 0)));
			_tri.Draw();


			// use our shader program
			//Gl.UseProgram(program);

			// bind the vertex positions, colors and elements of the triangle
			program["model_matrix"].SetValue(Matrix4.CreateTranslation(new Vector3(-1.5f, 0, 0)));

			// draw the triangle
			Gl.BindVertexArray(ID);
			Gl.DrawElements(BeginMode.Triangles, triangleElements.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);
			Gl.BindVertexArray(0);
		}


		public static string VertexShader = @"
#version 330

layout(location = 0)in vec4 in_position;
layout(location = 1)in vec3 in_color;
//in vec3 vertexColor;

out vec3 color;

uniform mat4 projection_matrix;
uniform mat4 view_matrix;
uniform mat4 model_matrix;

void main(void)
{
    color = in_color;
    gl_Position = projection_matrix * view_matrix * model_matrix * vec4(in_position.xyz, 1);
}
";

		public static string FragmentShader = @"
#version 330

in vec3 color;
out vec4 fragment;

void main(void)
{
    fragment = vec4(color, 1);
	//fragment = vec4(0, 1, 1, 1);
}";

	}
}
