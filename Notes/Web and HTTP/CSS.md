

CSS controls how HTML looks. HTML = structure, CSS = appearance. That separation of concerns is the first thing interviewers test — don't mix them.

---

## 1. Why My Style Isn't Applying — Specificity

The most common CSS bug. When two rules target the same element, the browser has to decide which one wins. It uses **specificity** — a score based on what kind of selector you used.

The scoring order from strongest to weakest:

- Inline style (`style="..."`) — always wins
- ID selector (`#id`) — very strong
- Class selector (`.class`), pseudo-class (`:hover`) — medium
- Element selector (`p`, `div`) — weakest

**Practical example:** You style `p { color: red }` but the text stays black. Why? Somewhere else there's `.content p { color: black }`. The class+element combo beats just an element. Add a class to your rule and it wins.

The nuclear option is `!important` — it overrides everything. Avoid it. It breaks the predictable resolution chain and makes debugging a nightmare. If you're reaching for `!important`, your specificity structure is already broken.

**Interview answer:** "Specificity determines which CSS rule wins when multiple rules target the same element. ID beats class, class beats element. If a style isn't applying, I check whether a more specific selector is overriding it."

---

## 2. The Box Model — Why Width Behaves Weirdly

Every HTML element is a rectangular box made of four layers, from inside out:

```
+-------------------------------+  <- margin (transparent, pushes others away)
|  +-------------------------+  |
|  |  border                 |  |
|  |  +-------------------+  |  |
|  |  |  padding          |  |  |
|  |  |  +-------------+  |  |  |
|  |  |  |   content   |  |  |  |
|  |  |  +-------------+  |  |  |
|  |  +-------------------+  |  |
|  +-------------------------+  |
+-------------------------------+
```

**Content** — the actual text or image. **Padding** — space between content and border, inherits background color. **Border** — the visible edge. **Margin** — transparent space outside, pushes other elements away.

**The problem:** By default (`box-sizing: content-box`), when you set `width: 200px`, that's only the content. Padding and border get added on top. So a box with `width: 200px`, `padding: 20px`, `border: 1px` is actually 242px wide. This breaks layouts constantly.

**The fix every modern project uses:**

```css
*, *::before, *::after {
  box-sizing: border-box;
}
```

Now `width: 200px` means the total width including padding and border. The content shrinks to fit. This is predictable. Your TO2 Angular app uses this by default.

**Interview answer:** "The box model is content + padding + border + margin. The important thing is `box-sizing: border-box` — without it, padding and border add to your declared width, which breaks layouts. Every modern CSS reset applies border-box globally."

---

## 3. Flexbox vs Grid — Layout Tools

These are the two modern layout systems. Interviewers ask you to choose between them.

**Flexbox** is for laying things out in **one direction** — either a row or a column. You use it when you want to align or space items inside a container and let the content decide how much space each item takes.

Real examples: a navigation bar with links, a row of buttons, centering one element inside another.

```css
.navbar {
  display: flex;
  justify-content: space-between; /* spread items across the row */
  align-items: center;            /* vertically center them */
}
```

**Grid** is for laying things out in **two directions** — rows AND columns simultaneously. You use it when you have a layout structure you want to impose regardless of content.

Real example: a card grid where you want 3 columns no matter what's inside the cards.

```css
.card-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr); /* 3 equal columns */
  gap: 20px;
}
```

**The mental model:** Flex = the content decides the layout. Grid = you decide the layout, content fills it.

They also compose — your TO2 UI uses grid for the page structure and flex inside each card to align the content within it.

**Interview answer:** "Flexbox is 1D — for aligning items in a row or column. Grid is 2D — for defining a layout in both rows and columns simultaneously. I use flex for component internals like navbars and button groups, grid for page-level structure like card layouts."

---

## 4. Positioning — Why z-index Doesn't Work

By default, elements flow one after another in the document. Positioning lets you break out of that flow.

- **`relative`** — element stays in the normal flow but you can nudge it with `top/left`. More importantly, it becomes the reference point for any `absolute` children inside it.
- **`absolute`** — element is removed from the flow (other elements act like it doesn't exist). It positions itself relative to the nearest ancestor that has `position: relative` (or `absolute`/`fixed`). If none exists, it goes all the way up to the page.
- **`fixed`** — same as absolute but always relative to the browser viewport. It doesn't move when you scroll. Used for sticky headers and modals.
- **`sticky`** — acts like `relative` until you scroll past a threshold, then sticks like `fixed`. Used for table headers. Breaks silently if any ancestor has `overflow: hidden`.

**Why `z-index: 9999` sometimes doesn't work:**

`z-index` only controls stacking order within the same **stacking context**. A stacking context is an isolated layer — elements inside it can't stack above or below elements in a different context, no matter how high their `z-index` is.

Things that create a new stacking context: `position` + any `z-index`, `opacity < 1`, `transform`, `filter`.

So if your modal has `z-index: 9999` but its parent has `opacity: 0.99` (common for fade animations), that parent created a stacking context. Your modal can't escape it.

**Debug approach:** find the element, check if it's positioned. Then walk up the DOM — check if any ancestor has `opacity`, `transform`, or `position + z-index`. That's your culprit.

**Interview answer:** "z-index controls stacking order within a stacking context. The common trap is that certain properties like `opacity` or `transform` on a parent create a new isolated context — elements inside it can't stack above elements outside it regardless of z-index value."

---

## 5. Responsive Design — Media Queries

Responsive design means the layout adapts to different screen sizes. The tool for this is **media queries** — CSS rules that only apply when a condition is true.

```css
/* base styles — applied always (mobile) */
.card { width: 100%; }

/* only applies on screens wider than 768px */
@media (min-width: 768px) {
  .card { width: 50%; }
}
```

**Mobile-first** means you write base styles for small screens, then use `min-width` media queries to add complexity for larger screens. This is the current standard. The opposite approach (desktop-first with `max-width`) is harder to maintain.

**Interview answer:** "Media queries apply CSS conditionally based on screen properties like width. Mobile-first means the base CSS targets small screens and you progressively enhance with min-width breakpoints for larger screens."

---

## 6. CSS Variables — How Your TO2 Theme Works

CSS variables (custom properties) let you define a value once and reuse it everywhere. You've seen this in your TO2 Angular app — `var(--primary-color)`, `var(--bg-card)`, etc.

```css
/* Define on :root — available everywhere */
:root {
  --primary: #4caf50;
  --bg-dark: #1a1a2e;
}

/* Use anywhere */
.card {
  background: var(--bg-dark);
  border-color: var(--primary);
}
```

**Why this matters:** To change your entire theme, you change one value in `:root`. Every element that uses that variable updates automatically. This is how dark mode works — override the variables on a `.dark-theme` class and the whole UI responds.

Unlike Sass variables (which are compiled away), CSS variables exist at runtime — JavaScript can read and write them. This is why frameworks can toggle themes without reloading.

**Interview answer:** "CSS custom properties are runtime variables defined with `--name` and used with `var(--name)`. They cascade like normal CSS — you can override them on any parent and all children update. This is the standard approach for theming."

---

## Common Interview Questions

**"What is the difference between padding and margin?"** Padding is inside the border — it's part of the element, inherits background color. Margin is outside — transparent, pushes other elements away. Vertical margins between elements collapse (the larger one wins, they don't add up).

**"How do you center a div?"** Modern answer: `display: grid; place-items: center;` on the parent. Or flex: `display: flex; justify-content: center; align-items: center;`. Never use old hacks like negative margins.

**"What's the difference between `display: none` and `visibility: hidden`?"** `display: none` removes the element from layout entirely — it takes up no space. `visibility: hidden` makes it invisible but it still occupies space. Use `display: none` to hide things completely, `opacity: 0` for fade animations (element stays in layout, still receives clicks).

**"What is `em` vs `rem`?"** Both are relative units for font sizes. `rem` is relative to the root element (`<html>`) — always predictable. `em` is relative to the current element's font size — compounds through nesting and causes bugs. Default to `rem` for font sizes and spacing.

[[HTML]] [[JavaScript & TypeScript]]