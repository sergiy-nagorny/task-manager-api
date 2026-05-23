# Next Projects

Standalone projects and learning exercises outside the main Task Manager API roadmap.

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
