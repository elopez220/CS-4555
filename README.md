# CS-4555
3D Game
Clash Royal and Balloon Tower Defense-inspired game.

# 1. Clone the Repository
git clone https://github.com/<your-username>/<repo-name>.git
cd <repo-name>

# 2. Install Git LFS in the command prompt
git lfs install

# 3. Pull LFS Files
git lfs pull

# Adding large Assets 
git lfs track "*.psd"
git lfs track "*.wav"
git lfs track "*.fbx"

# Commit changes
git add .gitattributes
git commit -m "Track large assets with Git LFS"
