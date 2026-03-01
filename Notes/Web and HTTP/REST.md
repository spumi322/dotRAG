### *REST (Representational State Transfer) principles.*

REST is an architectural style for designing networked APIs. It uses HTTP as transport and treats everything as a **resource** identified by a URL.

### *Core constraints*

1. **Client-Server** — separation of concerns.
2. **Stateless** — each request contains all info needed; server stores no session.
3. **Cacheable** — responses must define if they are cacheable.
4. **Uniform Interface** — consistent URL structure, standard HTTP methods.
5. **Layered System** — client doesn't know if it talks to the server directly or through a proxy/load balancer.

### *Resource naming conventions*

- Use **nouns**, not verbs: `/api/products` not `/api/getProducts`
- Use **plural**: `/api/orders` not `/api/order`
- Use **hierarchy** for relations: `/api/orders/42/items`
- Use **query parameters** for filtering: `/api/products?category=electronics&sort=price`

| Action         | Method | Endpoint          | Response              |
| -------------- | ------ | ----------------- | --------------------- |
| List all       | GET    | `/api/products`   | 200 + array           |
| Get one        | GET    | `/api/products/5` | 200 or 404            |
| Create         | POST   | `/api/products`   | 201 + Location header |
| Replace        | PUT    | `/api/products/5` | 200 or 204            |
| Partial update | PATCH  | `/api/products/5` | 200 or 204            |
| Delete         | DELETE | `/api/products/5` | 204 or 404            |

### *Idempotency*

GET, PUT, DELETE are idempotent — calling them N times produces the same result as 1 call. POST is **not** idempotent — each call may create a new resource. This is a common interview question.

### *Versioning strategies*

1. **URL path** (most common): `/api/v1/products` — simple, explicit.
2. **Query string**: `/api/products?api-version=1.0`
3. **Header**: `X-Api-Version: 1` or `Accept: application/vnd.myapi.v1+json`

ASP.NET Core supports all three via `Asp.Versioning.Http` NuGet package.

### *Common interview questions*

- "What makes an API RESTful?" → Stateless, resource-based URLs, standard HTTP methods, proper status codes.
- "PUT vs PATCH?" → PUT replaces the entire resource, PATCH modifies specific fields.
- "POST vs PUT?" → POST creates (server assigns ID), PUT replaces at a known ID.

### *REST vs SOAP*

REST is an architectural style — lightweight, stateless, HTTP + JSON.
SOAP is a protocol — strict XML format, verbose, legacy enterprise systems.
Junior answer: "REST is simpler and dominant in modern APIs. SOAP exists in older enterprise integrations like banking."


[[HTTP Fundamentals]]
[[ASP.NET Core]]