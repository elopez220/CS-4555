# CS-4555
3D Game
Clash Royal and Balloon Tower Defense-inspired game.

---

## 1. Clone the Repository
```bash
git clone https://github.com/<your-username>/<repo-name>.git
cd <repo>
```
## 2. Install Git LFS in the command prompt
```bash
git lfs install
```
## 3. Pull LFS Files
```bash
git lfs pull
```
## Adding large Assets 
```bash
git lfs track "*.psd"
git lfs track "*.wav"
git lfs track "*.fbx"
```
## Commit changes
```bash
git add .gitattributes
git commit -m "Track large assets with Git LFS"
```
