# Next Projects

Standalone projects and learning exercises outside the main Task Manager API roadmap.

---

## Task Manager Frontend — Web + Mobile (React ecosystem)

Rebuild the Blazor WASM frontend using the most widely-used UI ecosystem, **starting with web (React + Vite) and extending to mobile (React Native)**. Same API, same features, modern stack with the highest learning ROI on the job market.

### Strategy: two frameworks, one ecosystem

No single framework is simultaneously *most common*, *best-looking*, and *truly cross-device*. The pragmatic choice is to use React for the web (Phase A) and React Native for mobile (Phase B) — same language (TypeScript), same component model, same mental model. Shared knowledge transfers; the only thing that changes between targets is the rendering layer.

### Why this split over "one framework for everything"

| Alternative | What it offers | Why we didn't pick it |
|---|---|---|
| **Flutter** | True write-once: single Dart codebase → Android, iOS, web, desktop. Pixel-perfect identical UI on every platform. | Different language (Dart) with no carry-over to .NET work. Web target renders to canvas — poor accessibility and SEO. Smaller job market than React. |
| **.NET MAUI Blazor Hybrid** | Reuses existing Razor components on Android, iOS, Windows, Mac, web. Highest reuse of existing skill. | Defeats the purpose of this exercise — the goal is to *learn a new ecosystem*, not stay in .NET. Smaller community than React Native. |
| **React Native + React Native Web** | Single React codebase for iOS, Android, *and* web. Used by Meta, Microsoft Outlook, Shopify. | Web target is a second-class citizen — feels less native on web than a real web app. Heavier toolchain. The "RNW" library has fewer maintainers than React or React Native individually. |
| **PWA on top of Blazor WASM** | Smallest delta — add a manifest + service worker, installs on Pixel 9 from Chrome. | Still a web app underneath; limited access to native APIs. Doesn't teach anything new. iOS support has gaps. |
| **Capacitor / Ionic** | Wraps a web app in a native container for the app stores. | Hybrid feel — not truly native. Performance is web-tier, not native-tier. |
| **Vue 3 + NativeScript** | Vue ecosystem equivalent of React + React Native. | Vue has a smaller ecosystem than React; NativeScript has a much smaller community than React Native. |

**The trade-off being accepted:** writing the UI twice (once in React, once in React Native). The web and mobile codebases share TypeScript types and API client logic but not UI components. In exchange you get a *real* web app and a *real* native mobile app — neither compromised.

---

### Phase A — Web (React + Vite)

The dominant web front-end stack. Material Design UI via MUI.

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

### Phase B — Mobile (React Native, target: Android first)

Native Android and iOS app built with React Native. Primary test device: Android (e.g. Pixel 9). iOS comes free *in code* but **building/running iOS apps requires a Mac** — on Windows you can either use Expo EAS Build (cloud-based iOS builds, requires an Apple Developer account) or defer iOS until you have a Mac available.

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
