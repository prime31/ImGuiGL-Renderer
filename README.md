# ImGuiGL Renderer
### C# + SDL + OpenGL + Dear ImGui

ImGuiGL# is a C# OpenGL loader and Dear ImGui renderer. It uses [SDL2-CS](https://github.com/flibitijibibo/SDL2-CS) as a base and has it's own OpenGL function loader that loads only the methods required to render Dear ImGui. Under the hood, it uses the excellent [ImGui.Net](https://github.com/mellinoe/ImGui.NET) to access Dear ImGui from C#.


### Getting Started

It is assumed that you are using SDL2 in your project.

- if you don't have [SDL2-CS](https://github.com/flibitijibibo/SDL2-CS) in your project add the `SDL2.cs` file into your project.
- add the `ImGuiGL` folder to your project

```csharp
// create an SDL window with an OpenGL context
var (window, glContext) = ImGuiGL.CreateWindowAndGLContext("SDL GL ImGui Renderer", 800, 600);

// create an ImGuiGLRenderer which will handle rendering and input
var renderer = new ImGuiGLRenderer(window);

...

// in your main game loop, send all the SDL events to the ImGuiGLRenderer. Alternatively, you can manually set the ImGui.IO data
renderer.ProcessEvent(sdlEvent);

// at the beginning of your frame, let the ImGuiGLRenderer know you are starting a frame
renderer.NewFrame();

// at this point, run your normal game loop and do any ImGui calls that you need

// finally, make the glContext active (only required when you are using multiple SDL windows), perform your rendering then render the ImGui data
SDL_GL_MakeCurrent(window, glContext);
renderer.Render();

SDL_GL_SwapWindow(window);

```

## License
Whatever.


### Acknowledgements
This code began as a fork of the [opengl4sharp](https://github.com/giawa/opengl4csharp/tree/dotnetcore) lib. Unfortunately, that didn't work out so this ended up being a near total rewrite besides keeping the OpenGL enums.