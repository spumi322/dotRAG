
HTML defines the **structure and meaning** of web content. It is not a programming language — it's a markup language. HTML tells the browser _what_ the content is, CSS tells it _how it looks_, JavaScript tells it _how it behaves_. That separation of concerns is the first thing interviewers test.

---

## 1. Semantic HTML — the most asked HTML topic

Every HTML element has a default meaning. Semantic elements describe _what_ the content is. Non-semantic elements describe nothing.

```
<nav>        → this is navigation
<main>       → this is the primary content
<article>    → this is a self-contained piece of content
<section>    → this is a thematic group
<aside>      → this is supplementary content (sidebar)
<header>     → this is a header
<footer>     → this is a footer

<div>        → this is... nothing. A generic box.
<span>       → this is... nothing. A generic inline wrapper.
```

**Why it matters — three real reasons:**

**1. Accessibility.** Screen readers don't see your CSS. They navigate by landmarks. A `<nav>` element announces itself as navigation — users can jump straight to it. A `<div class="nav">` is invisible to them. More importantly: a `<button>` is focusable by keyboard, responds to Enter and Space, and announces itself as interactive. A `<div onclick="...">` does none of that — you'd have to manually add `tabindex`, keyboard listeners, and ARIA roles, and you'd probably still miss something. Use the native element.

**2. SEO.** Search engines weight content inside semantic elements (`<h1>`, `<article>`, `<main>`) more than content buried inside anonymous `<div>`s.

**3. Maintainability.** `<nav>` is self-documenting. `<div class="nav-wrapper-outer-container">` is not.

**Interview answer:** "Semantic HTML uses elements that describe the meaning of the content, not just its appearance. It matters for accessibility — screen readers navigate by landmarks and native elements like `<button>` have built-in keyboard support. It also helps SEO and makes code easier to read."

---

## 2. The DOM — what JavaScript actually talks to

When the browser receives HTML, it doesn't display it directly. It parses the HTML and builds a tree of objects in memory — one object per element, attribute, and text node. That tree is the **DOM (Document Object Model)**.

JavaScript never touches the raw HTML file. It reads and modifies the page through the DOM:

```js
document.getElementById("title").textContent = "Hello"; // finds the node, changes it
```

When React, Angular, or any framework "updates the UI", what they're actually doing is modifying DOM nodes. The browser then re-renders whatever changed.

**Why DOM size matters:** every DOM node costs memory. Deep nesting (dozens of `<div>` wrappers) makes traversal slower. This is why frameworks encourage component-based thinking — keep the DOM flat and meaningful.

**Interview answer:** "The DOM is the browser's in-memory tree representation of the HTML document. JavaScript interacts with the page through the DOM — it never modifies the HTML file directly. Frameworks like Angular manipulate the DOM to update the UI."

---

## 3. `defer` vs `async` — script loading

By default, when the browser hits a `<script>` tag it stops parsing HTML, downloads the script, executes it, then continues. This blocks rendering.

```
Normal:  HTML parsing → STOP → download → execute → resume parsing
async:   HTML parsing continues while downloading → execute immediately when ready (pauses parsing)
defer:   HTML parsing continues while downloading → execute AFTER parsing is fully done
```

**When to use which:**

- `defer` — for your app scripts. They run after the DOM is ready and in document order. This is what Angular's build output uses.
- `async` — for independent third-party scripts (analytics, ads) that don't need the DOM and don't depend on each other.
- Neither — almost never. Only for tiny inline scripts that must block.

**Interview answer:** "Both `async` and `defer` download scripts without blocking HTML parsing. The difference is when they execute — `async` runs immediately when downloaded (no order guarantee), `defer` runs after the full HTML is parsed in document order. I use `defer` for app scripts, `async` for independent third-party scripts."

---

## 4. SSR vs CSR — how HTML reaches the browser

This is asked in every fullstack interview because it's an architectural decision, not just a frontend detail.

**Client-Side Rendering (CSR)**

The server sends a nearly empty HTML file — just a `<div id="app"></div>` and a JavaScript bundle. The browser downloads the JS, executes it, and the JS builds the entire DOM from scratch in the browser.

```
Server sends:   <html><body><div id="app"></div><script src="app.js"></script></body></html>
Browser runs:   app.js → builds the whole page in memory → injects into #app → user sees content
```

This is how your TO2 Angular app works. The server (your .NET API) only serves JSON data. Angular running in the browser builds all the HTML.

**Pros:** fast navigation after initial load (no full page reloads), rich interactivity, backend is just an API. **Cons:** slow initial load (user sees blank page until JS downloads and runs), poor SEO by default (search engines may not wait for JS to execute).

---

**Server-Side Rendering (SSR)**

The server generates the complete, fully populated HTML for every request and sends it to the browser. The browser displays it immediately — no JavaScript needed to see the content.

```
Browser requests /tournaments
Server:   queries DB → builds HTML with real data → sends complete page
Browser:  displays content immediately, then loads JS to add interactivity
```

This is how traditional web apps worked (PHP, Razor Pages, classic ASP.NET MVC with views). Your .NET backend _can_ do SSR with Razor Pages — it renders the HTML server-side before sending it.

**Pros:** fast initial load (content visible immediately), great SEO (search engines get real HTML), no blank page flash. **Cons:** every navigation is a full page reload, more server load, less interactive by default.

---

**The real world: it's not either/or**

Modern apps often combine both. Next.js (React), Nuxt (Vue), and Angular Universal can SSR the first page load for SEO and performance, then hand off to CSR for subsequent navigation. This is called **hydration** — the server sends real HTML, then JS "wakes it up" and takes over.

For your TO2 app: pure CSR is fine because it's a management tool, not a public-facing SEO-dependent site.

**Interview answer:** "CSR sends a minimal HTML shell and builds the page in the browser with JavaScript. SSR generates complete HTML on the server for each request. CSR gives you rich interactivity but slow initial load and poor SEO. SSR gives fast first load and better SEO but more server work and full page reloads on navigation. Modern frameworks can combine both — SSR the first load, CSR for everything after."

---

## 5. Accessibility — two rules that actually matter

You don't need to memorize WCAG guidelines. Two rules cover 80% of what gets asked:

**Rule 1: Use native interactive elements.** Always use `<button>` for clickable actions, `<a>` for navigation links, `<input>` for form fields. Never use `<div>` or `<span>` with click handlers to fake interactive behavior. Native elements are keyboard-focusable, respond to Enter/Space, and announce themselves to screen readers for free.

**Rule 2: Every image needs an `alt` attribute.** Descriptive images: `alt="Tournament bracket showing semifinals"`. Decorative images that add no information: `alt=""` (empty string — screen readers skip it entirely). Missing `alt` entirely is wrong — the screen reader reads out the filename instead.

**Interview answer:** "The most important accessibility practices are using native HTML elements — `<button>` instead of `<div onclick>` — because they come with keyboard support and screen reader semantics built in, and always providing `alt` text on images so screen readers can describe them."

[[CSS]] [[JavaScript & TypeScript]]