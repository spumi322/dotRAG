### *HTTP protocol basics for web developers.*

HTTP (Hypertext Transfer Protocol) is a stateless, request-response protocol. The client sends a request, the server returns a response. Each request is independent — the server does not remember previous requests.

### *HTTP Methods*

| Method  | Purpose                                           | Idempotent        | Safe | Has Body |
| ------- | ------------------------------------------------- | ----------------- | ---- | -------- |
| GET     | Retrieve a resource                               | ✅                 | ✅    | ❌        |
| POST    | Create a resource                                 | ❌                 | ❌    | ✅        |
| PUT     | Replace a resource entirely                       | ✅                 | ❌    | ✅        |
| PATCH   | Partially update a resource                       | ⚠️ Not guaranteed | ❌    | ✅        |
| DELETE  | Remove a resource                                 | ✅                 | ❌    | ❌        |
| OPTIONS | Discover allowed methods (used by CORS preflight) | ✅                 | ✅    | ❌        |
**Idempotent** = calling it multiple times produces the same result as calling it once.
**Safe** = does not modify server state.

### *Status Codes (must know)*

**2xx — Success:**
- `200 OK` — general success
- `201 Created` — resource created (POST), include `Location` header
- `204 No Content` — success, no body (common for DELETE/PUT)

**3xx — Redirection:**
- `301 Moved Permanently`, `304 Not Modified`

**4xx — Client Error:**
- `400 Bad Request` — invalid input / validation failure
- `401 Unauthorized` — not authenticated (misleading name)
- `403 Forbidden` — authenticated but not authorized
- `404 Not Found` — resource doesn't exist
- `409 Conflict` — state conflict (e.g., duplicate create)
- `422 Unprocessable Entity` — semantically invalid request
- `429 Too Many Requests` — client exceeded rate limit

**5xx — Server Error:**
- `500 Internal Server Error` — unhandled exception
- `502 Bad Gateway`, `503 Service Unavailable`

### *Important Headers*

- `Content-Type`: format of the body (`application/json`, `text/html`)
- `Accept`: client's preferred response format
- `Authorization`: credentials (`Bearer <token>`)
- `Cache-Control`: caching directives
- `X-Correlation-Id`: request tracing (custom)

### *HTTPS*

HTTPS = HTTP + TLS encryption. All production APIs must use HTTPS. ASP.NET Core enforces this via `app.UseHttpsRedirection()`.

### *CORS (Cross-Origin Resource Sharing)*

When a browser makes a request to a different domain, CORS policies apply. The server must include `Access-Control-Allow-Origin` headers.

```csharp
// ASP.NET Core CORS configuration
builder.Services.AddCors(o => o.AddPolicy("AllowFrontend", p =>
    p.WithOrigins("https://myapp.com")
     .AllowAnyMethod()
     .AllowAnyHeader()));

app.UseCors("AllowFrontend");
```

The browser sends a **preflight** `OPTIONS` request for non-simple requests (e.g., with custom headers or PUT/DELETE). The server responds with allowed origins/methods.
### *ProblemDetails (RFC 7807)*
Standardized error response format. ASP.NET Core returns this automatically when using `[ApiController]`.

{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Not Found",
  "status": 404,
  "detail": "Tournament with id 42 was not found."
}

Fields: `type` (URI), `title` (short), `status` (HTTP code), `detail` (specific message).

[[ASP.NET Core]]
[[REST]]