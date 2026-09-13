# GDL emit MSBuild sample (gdlproj)

Demonstrates `Platform.Gdl.Emit` targets wired through a planet `*.gdlproj` declare entry (GUIDERS-ADR-0065).

```xml
<Import Project="path/to/build/Platform.Gdl.Emit.props" />

<PropertyGroup>
  <GdlEmitProject>authoring/planet.gdlproj</GdlEmitProject>
  <GdlEmitOutputDir>$(MSBuildProjectDirectory)\Generated</GdlEmitOutputDir>
  <GdlEmitNamespace>Planet.Generated</GdlEmitNamespace>
</PropertyGroup>
```

`BeforeCompile` runs `gdlc emit --lang=cs --project … --out …` for every `document` listed in the gdlproj.
CI stale check: `dotnet msbuild -t:GdlEmitVerify` (compares all `Generated/*.g.cs` outputs).
