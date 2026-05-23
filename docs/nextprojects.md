# Next Projects

Standalone projects and learning exercises outside the main Task Manager API roadmap.

---

## Task Manager Frontend — Cross-platform paths (sorted by ROI)

For a .NET team building enterprise software, the highest ROI path is **extending what already exists** — not learning a new ecosystem from scratch. The biggest enterprise cost is people; the cheapest line of code is the one your team can already write.

### ROI summary

| Order | Path | ROI for this team | When to do it |
|---|---|---|---|
| **1 (recommended)** | **MAUI Blazor Hybrid** | **Highest** — reuses existing Razor + MudBlazor components, single codebase, single language, Microsoft long-term support | When the team needs to ship mobile/desktop versions of the existing app |
| 2 (learning) | React + Vite (web) | Educational — learn the dominant frontend ecosystem and broaden hireability | When the team wants cross-stack fluency or is exploring non-.NET hires |
| 3 (learning) | React Native + Expo (mobile) | Educational — learn the dominant cross-platform mobile pattern | After Phase A, for completeness in the React ecosystem |
| 4 (learning) | Vanilla HTML5 + Web Components | Educational — strip the framework away to understand what it does | When the team wants to demystify framework abstractions |

The phases below are documented in this priority order.

---

## Recommended — MAUI Blazor Hybrid

Highest-ROI path. Extends the existing Blazor WASM app to native Android, iOS, Windows, and macOS apps **without rewriting the UI**.

### Why this wins on ROI

- **Reuses the entire `TaskManager.Web.Blazor` project** — all `.razor` components, MudBlazor styles, services
- **Single codebase, single language** (C#) — no JavaScript context switch
- **Microsoft long-term support** — not subject to JS framework churn (React Native major version upgrades are notoriously painful)
- **Same EF Core, same DI, same hosting model** — your existing knowledge transfers 1:1
- **MudBlazor already chosen** — the Material Design investment from Phase 6 pays off on mobile too
- **No dual-team risk** — same team ships web and mobile

### What you give up vs React Native
- **Mobile rendering is via BlazorWebView (a native WebView)** — ~95% native feel, but scroll inertia, keyboard handling, and platform gestures are slightly less polished than React Native or Swift/Kotlin
- **Animations are CSS-based**, not platform-native animation APIs
- **Cold-start is slightly slower** than a fully native app
- For CRUD apps (like this one): differences are imperceptible. For high-touch consumer apps with custom animations: native or Flutter would be better

### Stack
- **.NET MAUI** — cross-platform host shell (Android, iOS, Windows, macOS)
- **MAUI `BlazorWebView`** — hosts existing Razor components inside the native shell
- **MudBlazor** — reused unchanged from the existing Blazor WASM project
- **Existing Task Manager API** — called over HTTPS from the hybrid app

### Setup
```bash
# Install MAUI workload (one-time)
dotnet workload install maui

# Create the hybrid project
dotnet new maui-blazor -n TaskManager.MauiBlazor
cd TaskManager.MauiBlazor

# Reference the existing Blazor component library so all .razor files are shared
dotnet add reference ../TaskManager.Web.Blazor/TaskManager.Web.Blazor.csproj
```

Then wire up DI in `MauiProgram.cs` to register the same services the web app uses (HTTP client, etc.).

### What carries over from the current Blazor WASM app
- All `.razor` components — `Home.razor`, `EditTaskDialog.razor`, layout components
- All MudBlazor providers and theming
- Services, DI registration, models, DTOs
- bUnit tests (still work — they test components, not hosts)

What you write new:
- The MAUI shell (`App.xaml`, `MainPage.xaml` with a `<BlazorWebView>`)
- Touch-friendly tweaks (button sizing, safe-area insets on iOS)
- Platform-specific extras as needed: push notifications, offline cache, biometric auth, deep links

### Common gotchas (same family as React Native)
1. **`localhost` doesn't reach the dev machine from a phone.** Same fix as Phase B — use the dev machine's LAN IP and bind the API to `0.0.0.0`
2. **The .NET HTTPS dev cert is not trusted on Android/iOS.** Same fix — use HTTP in dev (port 5000)
3. **Touch targets** — MudBlazor sizing is desktop-first; check tap target sizes on mobile

### Progression
1. Install the MAUI workload, scaffold `TaskManager.MauiBlazor`
2. Reference the existing `TaskManager.Web.Blazor` project so all Razor components are shared
3. Configure `HttpClient` in `MauiProgram.cs` pointing at the API (LAN IP for dev)
4. Run on Android (emulator or device via USB debugging) — verify CRUD works end-to-end
5. Run on Windows (instant — same machine)
6. Test touch targets, scrolling, keyboard behaviour; tweak MudBlazor sizing if needed
7. (Optional, needs a Mac or MacInCloud) Build for iOS
8. (Optional) Add platform features — push notifications via Plugin.LocalNotification, offline storage via `SecureStorage`

---

## Learning phases (later) — for ecosystem breadth, not enterprise ROI

The three phases below are valuable as **learning exercises** that build cross-stack fluency, but they are **not** the right enterprise choice for a .NET team. Documented here because:
- They teach the dominant frontend ecosystems (React is #1 by job market)
- Cross-stack fluency makes you a better .NET developer (you see what other ecosystems get right)
- They serve as escape hatches if the team's stack ever shifts away from .NET

If shipping enterprise software, do the MAUI Blazor path above instead.

### Frameworks considered as enterprise recommendations (and why we picked MAUI Blazor)

| Alternative | What it offers | Why MAUI Blazor wins for this team |
|---|---|---|
| **React + React Native** | Largest ecosystem, easiest non-.NET hiring | Forces team to learn a new language (TypeScript) and two new frameworks (React, RN); two codebases double the maintenance |
| **Flutter** | True write-once: Dart → Android, iOS, web, desktop | Different language with zero .NET carry-over; web target renders to canvas (poor accessibility); harder hiring in most .NET markets |
| **PWA on top of Blazor WASM** | Smallest delta — add manifest + service worker | Works, but limited access to native APIs; iOS PWA support has gaps; doesn't give true app-store distribution |
| **Capacitor / Ionic** | Wraps a web app in a native container | Hybrid feel; performance is web-tier; MAUI Blazor wraps in a more capable native shell |
| **Vue 3 + NativeScript** | Vue ecosystem equivalent of React + React Native | Smaller community than React; NativeScript has very small community vs RN |

---

### Phase A (learning) — Web (React + Vite)

The dominant web front-end stack. Material Design UI via MUI. Learning exercise — for enterprise web work in a .NET team, the MAUI Blazor path above is higher ROI.

#### Stack
- **React 18+** — UI library (the de facto web standard)
- **Vite** — dev server and bundler with instant HMR
- **TypeScript** — type safety (mirrors `<Nullable>enable</Nullable>` discipline in the .NET project)
- **MUI (Material UI)** — component library; the React equivalent of MudBlazor
- **fetch / Axios** — HTTP client to call the existing Task Manager API

#### Setup
```bash
npm create vite@latest taskmanager-react -- --template react-ts
cd taskmanager-react
# @emotion/* are required peer dependencies of MUI, not optional extras
npm install @mui/material @emotion/react @emotion/styled @mui/icons-material
npm run dev
```

Configure the API URL in `.env.local` (equivalent to Blazor's `wwwroot/appsettings.json`):
```
VITE_API_URL=https://localhost:7001
```
Access it in code via `import.meta.env.VITE_API_URL`. Vite only exposes variables prefixed `VITE_` to the browser bundle.

#### Features to implement (matching Blazor app)
- [ ] Load and display task list from `GET /tasks`
- [ ] Create task (form + Enter key support)
- [ ] Toggle complete/undo via `PUT /tasks/{id}`
- [ ] Delete task via `DELETE /tasks/{id}`
- [ ] Edit task dialog via `PUT /tasks/{id}`
- [ ] Snackbar notifications for mutations
- [ ] Empty state message

#### Key concepts to learn
| Concept | React equivalent of Blazor |
|---|---|
| Component | Function component returning JSX |
| `@code` block | Component body (hooks, handlers) |
| `@bind` / two-way binding | `useState` + `onChange` |
| `OnInitializedAsync` | `useEffect(() => {}, [])` |
| `IDialogService.ShowAsync` | Conditional render or MUI `<Dialog>` |
| Snackbar | `useState` + MUI `<Snackbar>` |

#### Progression
1. Scaffold with Vite, get dev server running
2. Create `TaskList` component — fetch and display tasks
3. Add `CreateTask` form with controlled input and Enter key
4. Add complete/delete actions per row
5. Add `EditTaskDialog` component
6. Wire up snackbar notifications
7. Set `VITE_API_URL` in `.env.local` and test end-to-end against the running .NET API

---

### Phase B (learning) — Mobile (React Native, target: Android first)

Native Android and iOS app built with React Native. Primary test device: Android (e.g. Pixel 9). iOS comes free *in code* but **building/running iOS apps requires a Mac** — on Windows you can either use Expo EAS Build (cloud-based iOS builds, requires an Apple Developer account) or defer iOS until you have a Mac available.

Learning exercise — for enterprise mobile work in a .NET team, the MAUI Blazor path above is higher ROI.

#### Stack
- **React Native** — same component model as React, but renders to native iOS/Android views instead of HTML
- **Expo** — managed toolchain on top of React Native; eliminates Android Studio / Xcode setup pain for most cases
- **TypeScript** — shared with the web app
- **React Native Paper** — Material Design 3 component library; the native equivalent of MUI
- **fetch** — built-in; same as web

#### Why React Native Paper (and not the alternatives)

| Library | Why we didn't pick it |
|---|---|
| **NativeBase** | Maintenance has slowed; many components are wrappers around RN core with less polish than Paper |
| **React Native Elements** | More iOS-style flavour, not Material Design — breaks parity with MUI/MudBlazor |
| **Tamagui** | Excellent performance and theming, but steeper learning curve and a custom compiler step |
| **Gluestack UI** | Newer and growing fast, but smaller community than Paper today |

React Native Paper aligns with the MUI/MudBlazor Material Design choice, has the largest community of the Material-first RN libraries, and is officially recommended in the Expo docs.

#### Why Expo (and not bare React Native)?
Bare React Native requires Android Studio + Xcode + native build chain debugging. Expo wraps all that and gives you:
- One CLI (`npx expo start`) — no Android Studio required for most development
- Hot reload on a real Android/iOS device by scanning a QR code from the Expo Go app
- Built-in modules for camera, notifications, secure storage
- Optional "eject" to bare React Native later if you need a custom native module

#### Setup
```bash
# When prompted, pick the TypeScript template — exact name varies by Expo version
npx create-expo-app@latest taskmanager-mobile
cd taskmanager-mobile
npm install react-native-paper react-native-safe-area-context
npx expo start
```

> `@expo/vector-icons` ships with Expo by default, so React Native Paper's icon requirement is already satisfied — no extra install needed.

Then on your Android device: install **Expo Go** from the Play Store, scan the QR code from the terminal, and the app loads with live reload.

#### Common gotchas (read before you start)

**1. `localhost` won't reach your dev machine from the phone.**
On the web, `VITE_API_URL=https://localhost:7001` works because the browser and API run on the same machine. On a phone over Wi-Fi, `localhost` resolves to the *phone itself*. You need the dev machine's LAN IP:
```
# In .env or app config
EXPO_PUBLIC_API_URL=http://192.168.1.42:5000
```
Find your dev machine's IP with `ipconfig` (Windows) and ensure the API listens on `0.0.0.0` (not just `localhost`). In `Properties/launchSettings.json`, set `applicationUrl` to `http://0.0.0.0:5000` for the dev profile.

**2. The .NET HTTPS dev cert is not trusted on Android/iOS.**
Self-signed certs cause TLS errors on mobile. Easiest fix for local dev: serve the API over plain HTTP (port 5000, not 7001) and point the mobile app at that. Re-enable HTTPS before any deploy.

**3. CORS must allow your phone's origin.**
The existing dev CORS policy in `Program.cs` already uses `AllowAnyOrigin()`, so this is covered for now — but worth knowing when you tighten it in Phase 16.

#### Features to implement (same as web)
- [ ] Load and display task list
- [ ] Create task (input + button)
- [ ] Toggle complete via tap
- [ ] Delete task via swipe or button
- [ ] Edit task in a modal screen
- [ ] Snackbar notifications
- [ ] Empty state

#### Key concepts to learn (vs Phase A web)
| Web (React) | Mobile (React Native) |
|---|---|
| `<div>`, `<span>`, `<button>` | `<View>`, `<Text>`, `<Pressable>` |
| CSS / `style={{ ... }}` | StyleSheet objects (subset of CSS) |
| `onClick` | `onPress` |
| Router (`react-router-dom`) | Stack/tab navigators (`@react-navigation/native`) |
| `fetch('/tasks')` | Same — `fetch` works identically |
| MUI `<Dialog>` | RN Paper `<Portal>` + `<Dialog>` |

#### What carries over from Phase A
- TypeScript task interfaces (just copy the `.ts` file)
- API client logic (`fetch` calls work identically)
- Component patterns (props, state, effects)
- Material Design vocabulary (MUI → RN Paper is a 1:1 mental mapping)

#### Progression
1. Scaffold with Expo, run on the Android device via Expo Go
2. Resolve the gotchas above: bind the API to `0.0.0.0`, switch to HTTP for dev, set `EXPO_PUBLIC_API_URL` to your LAN IP
3. Copy TypeScript types and API client from the web app verbatim
4. Build the task list screen (`FlatList` of `<Card>` components)
5. Add the create-task input bar
6. Add tap-to-toggle and swipe-to-delete
7. Add an `EditTaskScreen` via React Navigation
8. (Stretch) Use Expo EAS Build to produce an iOS build from Windows, if you have an Apple Developer account

---

### Phase C (learning) — Vanilla Web (HTML5 + Web Components)

Build the same task manager a *third* time, this time using **no framework at all** — only what ships in the browser. The goal isn't a production app; it's to understand what React and Blazor are actually doing for you by removing them.

#### Why this is worth doing after A and B
Phases A (React) and B (React Native) teach you frameworks. Phase C teaches you the **platform underneath** the frameworks. You'll implement — by hand — the things `useState`, `useEffect`, and Blazor's `StateHasChanged()` do automatically. After this exercise, you'll never have to wonder *why* a framework exists.

#### Stack
- **HTML5** — `<template>` element, Custom Elements, Shadow DOM
- **Vanilla JavaScript** (ES2022+) — native ES modules, no transpilation, no build step required
- **Material Web Components** (`@material/web` by Google) — Material Design 3 as plain custom elements like `<md-filled-button>`, `<md-outlined-text-field>`, `<md-dialog>`. Smaller scope than MUI (~25 components vs MUI's 70+) but enough for a CRUD app
- **fetch** — same as Phase A and B

#### Setup (minimal — no `npm` required for the core)
```bash
mkdir taskmanager-vanilla
cd taskmanager-vanilla
# Optional: only needed if you want @material/web from npm
npm init -y
npm install @material/web
# Or skip npm entirely and load Material Web from a CDN via <script type="module">

# Serve the static files (any static server works)
npx serve .
```

A single `index.html` with `<script type="module" src="./app.js">` is enough to start. No bundler, no transpiler, no `node_modules` for the app itself if you use CDN imports.

#### The trade-off (this *is* the learning)
| What you give up vs React | What you gain |
|---|---|
| Built-in reactive state (`useState`) | You implement a 20-line reactive store and finally understand it |
| JSX templating | DOM API fluency — `document.createElement`, `<template>` cloning, `element.replaceChildren()` |
| Hot Module Replacement | Browser refresh — slower but no toolchain to maintain |
| Pre-built component library at MUI scale | A real, but smaller, Material library: `@material/web` |
| Routing via `react-router-dom` | The History API in ~30 lines |

#### Features to implement (same CRUD app, framework-free)
- [ ] Define `<task-list>` as a Custom Element that fetches and renders tasks
- [ ] Define `<task-row>` as a Custom Element with complete/edit/delete actions
- [ ] Define `<task-create-form>` with Enter-key submit
- [ ] Define `<edit-task-dialog>` using `<dialog>` (native HTML5 modal — no library needed)
- [ ] Roll a tiny reactive store (Pub/Sub or `EventTarget`) to keep components in sync
- [ ] Show snackbar messages with a small custom `<task-snackbar>` element (Material Web doesn't ship a snackbar — build your own with CSS transitions, or pull a third-party one)

#### Key concepts to learn
| Framework concept (React/Blazor) | Vanilla equivalent |
|---|---|
| Component | `class MyEl extends HTMLElement { connectedCallback() { ... } }` |
| `useState` / Blazor `[Parameter]` | Custom getters/setters on the element + `this.render()` |
| `useEffect(fn, [])` / `OnInitializedAsync` | `connectedCallback()` lifecycle |
| Cleanup (`useEffect` return) | `disconnectedCallback()` lifecycle |
| Two-way binding | Explicit `addEventListener('input', ...)` + state update |
| JSX | `<template>` + `cloneNode(true)` or tagged template literals |
| CSS-in-JS / scoped styles | Shadow DOM scopes CSS for free |
| Dialog | Native `<dialog>` element (no library needed) or `<md-dialog>` |
| Snackbar | Roll your own — Material Web doesn't include one |

#### Progression
1. Static `index.html` that fetches `/tasks` with `fetch` and dumps JSON into a `<pre>` — confirm the API talks to the page
2. Build `<task-row>` as a Custom Element, render one hardcoded task
3. Build `<task-list>` that fetches and renders many `<task-row>` instances
4. Add the create form — `<task-create-form>` dispatching a `CustomEvent` upward
5. Add complete/delete buttons that `PUT`/`DELETE` and refresh the list
6. Add `<edit-task-dialog>` using the native `<dialog>` element
7. Add a tiny reactive store so siblings update without prop drilling

#### Optional: graduate to Lit
If pure vanilla starts feeling verbose mid-project, **[Lit](https://lit.dev)** (5 KB, by Google) sits on top of Web Components and adds reactive properties + template literals — same platform underneath, less boilerplate. Used in Chrome's settings UI and YouTube TV. Strict subset of "still basically the platform."

#### Optional alternative paradigm: HTMX
**[HTMX](https://htmx.org)** flips the model entirely: the server returns HTML fragments and the browser swaps them in via `hx-get` / `hx-post` attributes. Powerful and minimal, but requires the .NET API to return HTML instead of JSON — a much larger architectural change. Flagged here for awareness, not as the main Phase C path.

---

## Build a Simple GPT from Scratch (nanoGPT)

Implement a minimal GPT transformer in Python following Andrej Karpathy's nanoGPT (github.com/karpathy/nanoGPT).

**Goal:** understand the transformer architecture hands-on — self-attention, multi-head attention, feed-forward layers, and the training loop — before integrating LLM capabilities into the API in a future phase.

### Setup
```bash
git clone https://github.com/karpathy/nanoGPT
cd nanoGPT
pip install torch numpy transformers datasets tiktoken wandb tqdm
```

### Entry point — Shakespeare character-level model
The recommended starting point. Small dataset (1 MB), trains on CPU in minutes.

```bash
# Prepare the dataset
python data/shakespeare_char/prepare.py

# Train (CPU / Mac — reduced model size)
python train.py config/train_shakespeare_char.py \
    --device=cpu --compile=False \
    --eval_iters=20 --log_interval=1 \
    --block_size=64 --batch_size=12 \
    --n_layer=4 --n_head=4 --n_embd=128 \
    --max_iters=2000 --lr_decay_iters=2000 \
    --dropout=0.0

# Train (GPU)
python train.py config/train_shakespeare_char.py

# Sample from the trained model
python sample.py --out_dir=out-shakespeare-char
```

### Key files to study (in order)
| File | What it teaches |
|---|---|
| `model.py` (~300 lines) | Full GPT architecture: embeddings, attention, transformer blocks |
| `train.py` (~300 lines) | Training loop, gradient accumulation, learning rate schedule |
| `sample.py` | Autoregressive inference — how tokens are generated one at a time |
| `data/shakespeare_char/prepare.py` | Tokenisation and dataset preparation |

### Progression
1. Train the Shakespeare char model and sample text
2. Read `model.py` top-to-bottom — trace one forward pass manually
3. Modify a hyperparameter (`n_layer`, `n_head`) and observe the effect on loss
4. Try the word-level Shakespeare config (`config/train_shakespeare.py`)
5. (Stretch) Fine-tune a pretrained GPT-2 checkpoint: `python train.py config/finetune_shakespeare.py`
