# C# SDL2 + IMGUI 
<img src="https://github.com/witcherofthorns/csharp-sdl-imgui/blob/master/prview.gif" width=90% />
</br>
This is a project for game creation, OpenGL window context creation in SDL2 and input control, implemented in C# </br>
All ImGui draw calls are called from the classic Nuget ImGui.Net package and have linked with intern SDL and SDL_GL calls </br>

## Getting Started
It is assumed that you are using SDL2 in your project.</br>
for create an SDL window with an OpenGL context:
```csharp
(_window, _glContext) = ImGuiGL.CreateWindowAndGLContext("SDL Window", 800, 600);
_renderer = new ImGuiGLRenderer(_window, _glContext);
```

For rendering (drawing) a new frame</br>
usually at the end of a while loop:
```csharp
_renderer.ClearColor(0.05f, 0.05f, 0.05f, 1.00f);
_renderer.NewFrame();
ImGui.ShowDemoWindow();
_renderer.Render();
SDL_GL_SwapWindow(_window);
```

## Repo Structure
- **Example**: contains a simple example project that displays the ImGui demo in an SDL window and shows the basic structure on how you can setup your own game loop.
- **General**: contains the csproj file and `ImGui`,`SDL2` and `OpenGL` folders.

## License
Whatever

### Acknowledgements
This code began as a fork of the [opengl4sharp](https://github.com/giawa/opengl4csharp/tree/dotnetcore) lib. Unfortunately, that didn't work out so this ended up being a near total rewrite besides keeping the OpenGL enums.
