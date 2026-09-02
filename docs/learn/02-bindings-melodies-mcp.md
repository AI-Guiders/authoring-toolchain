# Lab 02 — bindings, melodies, MCP

Prerequisites: [01-catalog-skeleton](01-catalog-skeleton.md).

## Keyboard grammars

Declare string grammars before writing wire:

```catalog
defaults
  grammar.keyboard.binding  = keyboard-key-gesture
  grammar.keyboard.melody   = keyboard-key-gesture
  binding.chord-root        = Ctrl+K
end defaults
```

`Ctrl+K` in `bindings` must parse as **keyboard-key-gesture** wire. Vim slugs like `fd` belong in `melodies` when `grammar.keyboard.melody = keyboard-vim`.

## Line grammars per channel

```catalog
channels
  console
    filter = filter-bar
    grammar
      command = command-console
      argument = argument-kv
    end grammar
end channels
```

Grammar ids are federation SSOT — see `docs/grammar/notation/` in guiders-platform.

## MCP projection

`mcp` is a **projection**, not a surface:

```catalog
mcp table
  | command     | expose |
  | filter.date | yes    |
end mcp
```

```bash
authoring validate samples/catalog/dash.catalog.gdl
authoring emit samples/catalog/dash.catalog.gdl --namespace DashSpec.Generated --class DashCatalog
```

## Compile errors

- `grammar-wire-mismatch` — cell does not match declared `grammar.*` id
- `missing-grammar-declaration` — line channel without `grammar` block
- `unknown-grammar-id` — id not in `NotationGrammarRegistry`
