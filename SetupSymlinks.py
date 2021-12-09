import os

if os.path.exists("./Testing/"):
    print("The 'Testing/' directory already exists, delete it manually and run again to recreate it.")
    exit()

# Setup (from scratch)
os.mkdir("./Testing/")
for i in range(0, 3):
    os.mkdir(f"./Testing/Symlink{i}")
    os.symlink(os.path.abspath("./Assets"), os.path.abspath(f"./Testing/Symlink{i}/Assets"))
    os.symlink(os.path.abspath("./ProjectSettings"), os.path.abspath(f"./Testing/Symlink{i}/ProjectSettings"))
    os.symlink(os.path.abspath("./Packages"), os.path.abspath(f"./Testing/Symlink{i}/Packages"))