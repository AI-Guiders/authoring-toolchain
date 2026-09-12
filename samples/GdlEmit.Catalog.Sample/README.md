# GDL emit MSBuild sample (catalog)

Demonstrates `Platform.Gdl.Emit` targets wired into a planet `*.csproj`.

```xml
<Import Project="path/to/build/Platform.Gdl.Emit.props" />

<PropertyGroup>
  <GdlEmitInput>authoring/dash.catalog.gdl</GdlEmitInput>
  <GdlEmitKind>catalog</GdlEmitKind>
  <GdlEmitNamespace>DashSpec.Generated</GdlEmitNamespace>
  <GdlEmitClass>DashCatalog</GdlEmitClass>
  <GdlEmitOutputDir>$(MSBuildProjectDirectory)\Generated</GdlEmitOutputDir>
</PropertyGroup>
```

Build regenerates `Generated/DashCatalog.g.cs` when the `.catalog.gdl` input is newer than the output.
CI stale check: `dotnet msbuild -t:GdlEmitVerify`.
