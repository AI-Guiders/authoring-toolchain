# Lab 02 — bindings, melodies, MCP

Prerequisites: [01-catalog-skeleton](01-catalog-skeleton.md).

## Keyboard defaults

Declare the alphabet before writing wire:

```catalog
defaults
  notation.keyboard.binding  = keyboard-key-gesture
  notation.keyboard.melody   = keyboard-key-gesture
  binding.chord-root         = Ctrl+K
end defaults
```

`Ctrl+K` in `bindings` must parse as **key-gesture** wire. A Vim-style `fd` slug belongs in `melodies`, not bindings.

## MCP projection

`mcp` is a **projection**, not a command surface:

```catalog
mcp table
  | command     | expose |
  | filter.date | yes    |
end mcp
```

Validate and emit:

```bash
authoring validate samples/catalog/dash.catalog
authoring emit samples/catalog/dash.catalog --namespace DashSpec.Generated --class DashCatalog
```

## Compile errors to expect

- `notation-wire-mismatch` — gesture does not match `notation.keyboard.*`
- Missing `command-notation` on a channel → `missing-notation-declaration`
