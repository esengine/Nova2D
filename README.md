# Nova2D

Nova2D is a lightweight 2D graphics engine written in C# (.NET 9) using OpenGL via Silk.NET.  
It focuses on providing a minimal, low-level rendering foundation without the overhead of a full game engine.

The project is in early development and currently includes core rendering features such as texture loading, shader support, and sprite drawing.

---

## Features

- 2D rendering via OpenGL (Silk.NET 2.22.0)
- Texture loading with stb_image
- Sprite rendering pipeline
- Custom shader loader and uniform system
- Clean C# codebase with minimal dependencies
- Unsafe support enabled for performance-critical operations

---

## Requirements

- [.NET 9.0 SDK (preview)](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)

---

## Getting Started

```bash
git clone https://github.com/esengine/Nova2D.git
cd Nova2D
dotnet run --project Nova2D.Demo
