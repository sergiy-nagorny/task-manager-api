# Next Projects

Standalone projects and learning exercises outside the main Task Manager API roadmap.

---

## Task Manager UI — React + Vite (alternative to Blazor WASM)

Rebuild the existing Blazor WASM task manager frontend using React + Vite — same API, same features, different stack. Goal is to compare the two approaches and learn the most widely-used web front-end ecosystem.

### Why React + Vite

| Criterion | Why React + Vite wins |
|---|---|
| **Most common** | React is the #1 front-end library by usage, job postings, and ecosystem size — the highest learning ROI |
| **Cross-device** | Runs in any browser on any device; no native toolchain needed |
| **Beginner-friendly** | Vite provides zero-config scaffolding; JSX component model maps directly to how Blazor components work |
| **UI parity** | MUI (Material UI) is the React equivalent of MudBlazor — same Material Design system, nearly identical component API |

**Alternatives considered and why skipped:**
- *Vue 3 + Vite* — marginally easier syntax but smaller ecosystem than React; fewer tutorials and job postings
- *React + Next.js* — adds SSR and file-based routing complexity not needed for a simple SPA
- *React Native* — mobile only (iOS/Android); no HTML/CSS; completely different paradigm from the current web app

### Stack
- **React 18+** — UI library
- **Vite** — dev server and bundler (replaces Webpack; instant HMR)
- **TypeScript** — type safety (mirrors `<Nullable>enable</Nullable>` discipline in the .NET project)
- **MUI (Material UI)** — component library equivalent of MudBlazor
- **Axios or fetch** — HTTP client to call the existing Task Manager API

### Setup
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

### Features to implement (matching Blazor app)
- [ ] Load and display task list from `GET /tasks`
- [ ] Create task (form + Enter key support)
- [ ] Toggle complete/undo via `PUT /tasks/{id}`
- [ ] Delete task via `DELETE /tasks/{id}`
- [ ] Edit task dialog via `PUT /tasks/{id}`
- [ ] Snackbar notifications for mutations
- [ ] Empty state message

### Key concepts to learn
| Concept | React equivalent of Blazor |
|---|---|
| Component | Function component returning JSX |
| `@code` block | Component body (hooks, handlers) |
| `@bind` / two-way binding | `useState` + `onChange` |
| `OnInitializedAsync` | `useEffect(() => {}, [])` |
| `IDialogService.ShowAsync` | Conditional render or MUI `<Dialog>` |
| Snackbar | `useState` + MUI `<Snackbar>` |

### Progression
1. Scaffold with Vite, get dev server running
2. Create `TaskList` component — fetch and display tasks
3. Add `CreateTask` form with controlled input and Enter key
4. Add complete/delete actions per row
5. Add `EditTaskDialog` component
6. Wire up snackbar notifications
7. Set `VITE_API_URL` in `.env.local` and test end-to-end against the running .NET API

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
