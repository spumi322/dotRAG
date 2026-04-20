# TODO: Embedding Persistence — Content-Hash Cache

## Problem

`NotesIngestionService` re-embeds every chunk on every app restart via Voyage AI, burning tokens on unchanged content. Free tier absorbs this at current scale, but the approach does not scale and is architecturally wasteful.

## Solution

Persist embeddings to disk. On startup, hash each chunk's content and compare against stored hashes. Only call Voyage AI for chunks whose content actually changed. Serve unchanged chunks from the local store.

## Design

- Hash function: SHA-256 of the raw chunk text (post-chunking, post-WikiLink stripping)
- Storage: SQLite + `sqlite-vec` (or flat JSON file as a simpler first pass)
- Schema concept: `ChunkHash (TEXT PK) | SourceFile (TEXT) | HeadingPath (TEXT) | Vector (BLOB) | CreatedAt (TEXT)`
- On startup: chunk all markdown files → compute hashes → diff against stored hashes → embed only new/changed chunks → delete orphaned rows (removed/renamed chunks)
- New `IVectorStore` implementation backed by SQLite, replacing `InMemoryVectorStore` via DI swap

## Considerations

- Chunk identity is the hash, not the heading or file path — renaming a heading without changing content should not trigger re-embedding
- Deleting a note file should remove its chunks from the store (orphan cleanup)
- The Voyage model name should be stored alongside vectors — switching models invalidates all cached embeddings
- Keep `InMemoryVectorStore` in the codebase for testing / fallback, same as `AnthropicLlmService`

## Depends On

- Phase 2 fully operational (OpenRouter swap done, RAG pipeline validated end-to-end)
