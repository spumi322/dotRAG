# Task

Implement `OpenRouterLlmService` as a second `ILlmService` implementation that calls OpenRouter's API instead of Anthropic directly. Then swap the DI registration in `Program.cs` so the app uses OpenRouter at runtime.

## Context

- Read `CLAUDE.md` for project conventions before doing anything.
- Read the existing `ILlmService` interface and `AnthropicLlmService` to understand the contract you must satisfy — method signatures, return types, cancellation token forwarding. Ignore any streaming-related methods for now — streaming is deferred to Phase 3.
- Read `VoyageEmbeddingService` for the raw `HttpClient` + `IHttpClientFactory` pattern already established in this project.
- OpenRouter exposes an OpenAI-compatible REST API at `https://openrouter.ai/api/v1/chat/completions`. Non-streamed responses use `choices[0].message.content` for the completion text.
- The API key will come from user secrets under `ApiKeys:OpenRouterApiKey`.
- The model name must be configurable via `appsettings.json` under `OpenRouter:Model` with NO default value — it is a required config entry.

## Constraints

- **Do NOT modify** `ILlmService`, `ChatService`, `PromptBuilder`, `QueryRewriter`, or any other existing service. The new class must satisfy the existing interface contract exactly.
- **Do NOT modify** `AnthropicLlmService` — it stays in the codebase, just unused for now.
- If the interface has streaming methods, provide a `throw new NotSupportedException("Streaming is not implemented. Deferred to Phase 3.")` stub.
- The class must be `internal sealed`, consistent with all other service classes in the project.
- Use raw `HttpClient` via `IHttpClientFactory` — no new NuGet packages. Follow the same HTTP pattern used in `VoyageEmbeddingService`.
- Register the `HttpClient` as a named/typed client in `Program.cs`.
- In `Program.cs`, replace the `ILlmService` singleton registration from `AnthropicLlmService` to `OpenRouterLlmService`. Comment out the old registration, do not delete it.
- Log requests and errors with Serilog (inject `ILogger<OpenRouterLlmService>`), consistent with existing logging patterns.
- Map OpenRouter error responses to exceptions the same way `AnthropicLlmService` does — inspect its error handling and replicate the pattern.
- Create any DTOs needed for OpenRouter request/response serialization in the same folder. Use records. Keep them `internal`.
- Verify the solution builds and all existing tests pass after implementation.
